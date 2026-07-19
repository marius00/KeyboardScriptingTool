using System;
using System.Linq;
using System.Threading;
using HidSharp;
using InputSimulatorStandard.Native;
using log4net;
using KST.InputProviders.Args;
using System.Runtime.InteropServices;

namespace KST.InputProviders {
    /// <summary>
    /// Native-HID input provider for Logitech G-keys. Talks to the keyboard
    /// directly and does NOT require G-Hub / LGS or the Logitech SDK DLL.
    ///
    /// The G910 keeps its G-keys in "onboard" mode by default (they either do
    /// nothing or masquerade as F-keys). Sending a single activation report puts
    /// the keyboard into software mode, after which G/M/MR presses arrive as raw
    /// 20-byte vendor reports on usage page 0xFF43 / usage 0x0602. Unlike the SDK
    /// path this yields BOTH key-down and key-up, so real hold behaviour works.
    ///
    /// Report layout (report id 0x11):
    ///   11 FF 08 00 &lt;lo&gt; &lt;hi&gt;   G-keys: lo bit0..7 = G1..G8, hi bit0 = G9, all-zero = release
    ///   11 FF 09 00 &lt;m&gt;         M-keys: 0x01=M1 0x02=M2 0x04=M3 (mode select)
    ///   11 FF 0A 00 &lt;r&gt;         MR:     0x01 pressed
    /// </summary>
    internal class LogitechHidInputProvider : IGKeyInputProvider {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogitechHidInputProvider));

        private const int LogitechVid = 0x046D;
        private const int G910Pid = 0xC335;

        // Switch G-keys from onboard/F-key mode into software (macro) mode.
        private static readonly byte[] G910Activate = {
            0x11, 0xFF, 0x08, 0x2E, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private volatile bool _isRunning = true;
        private HidStream _stream;

        // Currently-held G-key bitmask (bit0 = G1 .. bit8 = G9) and current M mode.
        private int _lastGMask;
        private InputModifierState _currentMState = InputModifierState.M1;

        public event InputEventHandler OnInput;

        /// <summary>
        /// True when a supported keyboard (currently only the G910) exposing the
        /// 20-byte vendor interface is connected, so the caller can prefer this
        /// provider over the SDK-based one.
        /// </summary>
        public static bool IsSupportedDevicePresent() {
            try {
                return FindTargetInterface() != null;
            }
            catch (Exception ex) {
                Logger.Warn("Failed to probe for HID G-key devices", ex);
                return false;
            }
        }

        private static HidDevice FindTargetInterface() {
            // The vendor interface we want is the one with 20-byte in/out reports
            // (usage page 0xFF43 / usage 0x0602). Selecting on report length keeps
            // us off the standard keyboard/media collections without needing to
            // parse the usage page (which some collections refuse without elevation).
            return DeviceList.Local.GetHidDevices(LogitechVid, G910Pid)
                .FirstOrDefault(d => d.GetMaxInputReportLength() == 20
                                     && d.GetMaxOutputReportLength() >= 20);
        }

        public void Start() {
            new Thread(() => {
                while (_isRunning) {
                    if (!RunOnce()) {
                        // Device missing or dropped; back off before retrying so a
                        // permanently-absent keyboard doesn't spin the CPU.
                        Thread.Sleep(2000);
                    }
                }
            }) { IsBackground = true, Name = "LogitechHidInputProvider" }.Start();
        }

        /// <summary>Opens the device, activates software mode, and pumps reports
        /// until the device disappears or we're disposed. Returns false if it
        /// couldn't get started (so the caller backs off before retrying).</summary>
        private bool RunOnce() {
            var target = FindTargetInterface();
            if (target == null)
                return false;

            HidStream stream;
            try {
                stream = target.Open();
            }
            catch (Exception ex) {
                Logger.Warn("Could not open Logitech HID interface", ex);
                return false;
            }

            _stream = stream;
            try {
                stream.Write(G910Activate);
                Logger.Info("Logitech HID G-key provider active (software mode enabled)");

                var buf = new byte[target.GetMaxInputReportLength()];
                // Block until a report arrives rather than polling with a short
                // timeout (which would make HidSharp throw a TimeoutException every
                // interval). Dispose() closes the stream, which unblocks Read and
                // surfaces as an exception we treat as a clean shutdown/disconnect.
                stream.ReadTimeout = Timeout.Infinite;
                _lastGMask = 0;

                while (_isRunning) {
                    int n = stream.Read(buf, 0, buf.Length);
                    if (n >= 5)
                        HandleReport(buf, n);
                }
                return true;
            }
            catch (Exception ex) {
                if (_isRunning)
                    Logger.Warn("Logitech HID read loop stopped (device disconnected?)", ex);
                return true; // treat as a transient disconnect; retry via the outer loop
            }
            finally {
                try { stream.Dispose(); } catch { /* ignore */ }
                _stream = null;
            }
        }

        private void HandleReport(byte[] b, int n) {
            if (b[0] != 0x11 || b[1] != 0xFF)
                return;

            switch (b[2]) {
                case 0x08: // G-keys
                    int mask = b[4] | ((n >= 6 ? b[5] & 0x01 : 0) << 8);
                    EmitGKeyChanges(mask);
                    break;
                case 0x09: // M-keys act as a sticky mode selector
                    if (b[4] == 0x01) _currentMState = InputModifierState.M1;
                    else if (b[4] == 0x02) _currentMState = InputModifierState.M2;
                    else if (b[4] == 0x04) _currentMState = InputModifierState.M3;
                    break;
                // 0x0A (MR) intentionally ignored — no script event today.
            }
        }

        private void EmitGKeyChanges(int mask) {
            int changed = mask ^ _lastGMask;
            if (changed == 0)
                return;

            ushort modifiers = CurrentModifiers();
            for (int bit = 0; bit < 9; bit++) {
                int flag = 1 << bit;
                if ((changed & flag) == 0)
                    continue;

                bool down = (mask & flag) != 0;
                OnInput?.Invoke(this, new InputEventArg(
                    $"G{bit + 1}",
                    modifiers,
                    down ? InputEventType.Down : InputEventType.Up));
            }

            _lastGMask = mask;
        }

        private ushort CurrentModifiers() {
            ushort modifiers = (ushort)_currentMState;
            if ((GetAsyncKeyState((ushort)VirtualKeyCode.LSHIFT) & 0x8000) != 0)
                modifiers += (ushort)InputModifierState.Shift;
            if ((GetAsyncKeyState((ushort)VirtualKeyCode.LCONTROL) & 0x8000) != 0)
                modifiers += (ushort)InputModifierState.Ctrl;
            return modifiers;
        }

        public void Dispose() {
            _isRunning = false;
            try { _stream?.Dispose(); } catch { /* ignore */ }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern short GetAsyncKeyState(ushort virtualKeyCode);
    }
}
