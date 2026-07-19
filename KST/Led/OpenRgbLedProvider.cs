using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net;
using OpenRGB.NET;

namespace KST.Led {
    /// <summary>
    /// Per-key color integration through an OpenRGB SDK server (default 127.0.0.1:6742).
    /// Used when Logitech software is not installed, or as a preferred backend when OpenRGB is running.
    ///
    /// OpenRGB addresses LEDs by index within a device and expects byte [0, 255] colors, so this
    /// provider resolves KST key names to LED indices on the connected keyboard and scales the
    /// percentage-based colors used elsewhere in KST. Writes are batched into a local buffer and
    /// pushed to the device on <see cref="Flush"/> to avoid a network round-trip per key.
    ///
    /// Setting individual keys requires the device to be in "Direct" mode, which makes KST own every
    /// LED (unscripted keys go black). To avoid clobbering the user's normal lighting, Direct mode is
    /// entered lazily on the first <see cref="SetColor"/> and handed back to the device's original
    /// mode (e.g. a Spectrum effect) on <see cref="RestoreState"/> (alt-tab) and <see cref="Dispose"/>.
    /// </summary>
    internal class OpenRgbLedProvider : ILedProvider {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OpenRgbLedProvider));

        private readonly string _ip;
        private readonly int _port;
        private readonly object _lock = new object();

        private OpenRgbClient _client;
        private int _deviceId = -1;
        private int _originalModeIndex = -1;
        private Color[] _buffer;
        private readonly Dictionary<string, int> _keyToLedIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private bool _isInitialized;
        private bool _directModeActive;
        private bool _dirty;

        public OpenRgbLedProvider(string ip = "127.0.0.1", int port = 6742) {
            _ip = ip;
            _port = port;
        }

        public bool Start() {
            try {
                // Some Logitech keyboards (e.g. G910) occasionally end up in a state where OpenRGB has
                // them detected but won't render until a device rescan. OpenRGB.NET doesn't expose the
                // rescan command (packet id 140), so we send it over a throwaway raw socket first, then
                // give the server a moment to re-detect before connecting for real.
                if (RescanDevices()) {
                    Thread.Sleep(RescanSettleMs);
                }

                _client = new OpenRgbClient(_ip, _port, "KST", autoConnect: false, timeoutMs: 1000);
                _client.Connect();

                var devices = _client.GetAllControllerData();
                Logger.Info($"OpenRGB reported {devices.Length} device(s)");

                Device keyboard = null;
                foreach (var device in devices) {
                    Logger.Info($"OpenRGB device #{device.Index}: {device.Type} - {device.Name} ({device.Leds.Length} LEDs)");
                    if (keyboard == null && device.Type == OpenRGB.NET.DeviceType.Keyboard) {
                        keyboard = device;
                    }
                }

                if (keyboard == null) {
                    Logger.Warn("OpenRGB is running but no keyboard device was found. Colors will not be set.");
                    return false;
                }

                _deviceId = keyboard.Index;
                // Remember the mode the keyboard was in (e.g. a Spectrum effect) so we can hand it
                // back when KST is not actively painting keys. We do NOT enter Direct mode here.
                _originalModeIndex = keyboard.ActiveModeIndex;

                ResolveKeyIndices(keyboard);

                _buffer = new Color[keyboard.Leds.Length];

                _isInitialized = true;
                Logger.Info($"OpenRGB initialized on \"{keyboard.Name}\", mapped {_keyToLedIndex.Count} keys. " +
                            $"Original mode index {_originalModeIndex}.");
                return true;
            }
            catch (Exception ex) {
                Logger.Warn($"Could not connect to OpenRGB on {_ip}:{_port}. Is the OpenRGB SDK server running?");
                Logger.Warn(ex.Message, ex);
                _isInitialized = false;
                return false;
            }
        }

        // OpenRGB SDK network protocol constants (see OpenRGB NetworkProtocol.h).
        private const uint CmdSetClientName = 50;
        private const uint CmdRequestRescanDevices = 140;
        private const int RescanSettleMs = 1500;

        /// <summary>
        /// Asks the OpenRGB server to re-detect devices (equivalent to the GUI's "Rescan Devices").
        /// Sent as a raw header-only packet because OpenRGB.NET does not wrap this command.
        /// Returns true if the request was delivered (i.e. the server is reachable).
        /// </summary>
        private bool RescanDevices() {
            try {
                using (var tcp = new TcpClient()) {
                    var connect = tcp.BeginConnect(_ip, _port, null, null);
                    if (!connect.AsyncWaitHandle.WaitOne(1000)) {
                        // OpenRGB not running / not reachable; the normal connect below will report it.
                        return false;
                    }
                    tcp.EndConnect(connect);

                    using (var stream = tcp.GetStream()) {
                        // Register a client name first so the server logs a sensible source, then rescan.
                        SendPacket(stream, 0, CmdSetClientName, Encoding.ASCII.GetBytes("KST\0"));
                        SendPacket(stream, 0, CmdRequestRescanDevices, Array.Empty<byte>());
                        stream.Flush();
                    }
                }

                Logger.Info("Requested OpenRGB device rescan");
                return true;
            }
            catch (Exception ex) {
                Logger.Warn("Error requesting OpenRGB device rescan (continuing without it)", ex);
                return false;
            }
        }

        /// <summary>
        /// Writes a single OpenRGB SDK packet: the 16-byte "ORGB" header followed by the payload.
        /// All multi-byte fields are little-endian, matching the protocol and x64 host order.
        /// </summary>
        private static void SendPacket(NetworkStream stream, uint deviceIndex, uint commandId, byte[] data) {
            var header = new byte[16];
            header[0] = (byte)'O';
            header[1] = (byte)'R';
            header[2] = (byte)'G';
            header[3] = (byte)'B';
            BitConverter.GetBytes(deviceIndex).CopyTo(header, 4);
            BitConverter.GetBytes(commandId).CopyTo(header, 8);
            BitConverter.GetBytes((uint)data.Length).CopyTo(header, 12);

            stream.Write(header, 0, header.Length);
            if (data.Length > 0) {
                stream.Write(data, 0, data.Length);
            }
        }

        private void ResolveKeyIndices(Device keyboard) {
            // Build a name -> index lookup for this keyboard's LEDs, then match our expected names.
            var ledIndexByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < keyboard.Leds.Length; i++) {
                var name = keyboard.Leds[i].Name;
                if (!string.IsNullOrEmpty(name) && !ledIndexByName.ContainsKey(name)) {
                    ledIndexByName[name] = i;
                }
            }

            foreach (var key in KeyMapper.LogitechMappingKeys) {
                var ledName = OpenRgbKeyMapping.TryGetLedName(key);
                if (ledName != null && ledIndexByName.TryGetValue(ledName, out var index)) {
                    _keyToLedIndex[key] = index;
                }
            }

            if (_keyToLedIndex.Count == 0) {
                Logger.Warn("None of the expected key names matched this keyboard's OpenRGB LED names. " +
                            "SetColor() will have no effect. Check the device's LED naming in OpenRGB.");
            }
        }

        public void SetColor(string key, int r, int g, int b) {
            if (r < 0 || r > 100) {
                Logger.Warn($"Argument red \"{r}\" is outside range [0, 100]");
            } else if (g < 0 || g > 100) {
                Logger.Warn($"Argument green \"{g}\" is outside range [0, 100]");
            } else if (b < 0 || b > 100) {
                Logger.Warn($"Argument blue \"{b}\" is outside range [0, 100]");
            } else if (string.IsNullOrEmpty(key)) {
                Logger.Warn("Attempting to set color for key, but argument is NULL");
            } else if (!_isInitialized) {
                Logger.Warn("Attempting to set keyboard colors, but OpenRGB is not initialized");
            } else if (!_keyToLedIndex.TryGetValue(key, out var ledIndex)) {
                Logger.Warn($"Key \"{key}\" is not available on this OpenRGB keyboard");
            } else {
                Logger.Info($"SetColor {key} -> LED {ledIndex} = ({r}, {g}, {b})");
                lock (_lock) {
                    EnsureDirectMode();
                    _buffer[ledIndex] = new Color(ToByte(r), ToByte(g), ToByte(b));
                    _dirty = true;
                }
            }
        }

        /// <summary>
        /// Switches the keyboard into Direct mode so per-key writes take effect. Called lazily so the
        /// user's normal lighting keeps running until a script actually paints a key. Must hold _lock.
        /// </summary>
        private void EnsureDirectMode() {
            if (_directModeActive) {
                return;
            }

            try {
                _client.SetCustomMode(_deviceId);
                Array.Clear(_buffer, 0, _buffer.Length); // start from a clean (black) canvas
                _directModeActive = true;
                _dirty = true;
                Logger.Info("Entered OpenRGB Direct mode");
            }
            catch (Exception ex) {
                Logger.Warn("Error switching OpenRGB device to Direct mode", ex);
            }
        }

        public void Flush() {
            if (!_isInitialized) {
                return;
            }

            lock (_lock) {
                if (!_directModeActive || !_dirty) {
                    return;
                }

                try {
                    _client.UpdateLeds(_deviceId, _buffer);
                    _dirty = false;
                    Logger.Info($"Pushed {_buffer.Length} LEDs to OpenRGB device {_deviceId}");
                }
                catch (Exception ex) {
                    Logger.Warn("Error pushing colors to OpenRGB", ex);
                }
            }
        }

        public void SaveState() {
            // The state we restore to is the device's original mode, captured in Start(). Nothing to do here.
        }

        /// <summary>
        /// Hands the keyboard back to the mode it was in when KST started (e.g. a Spectrum effect).
        /// Direct mode is re-entered automatically on the next <see cref="SetColor"/>.
        /// </summary>
        public void RestoreState() {
            if (!_isInitialized) {
                return;
            }

            lock (_lock) {
                if (!_directModeActive) {
                    return;
                }

                try {
                    if (_originalModeIndex >= 0) {
                        _client.UpdateMode(_deviceId, _originalModeIndex);
                    }
                    _directModeActive = false;
                    _dirty = false;
                }
                catch (Exception ex) {
                    Logger.Warn("Error restoring OpenRGB device to its original mode", ex);
                }
            }
        }

        private static byte ToByte(int percentage) {
            int value = (int)Math.Round(percentage * 255.0 / 100.0);
            if (value < 0) value = 0;
            if (value > 255) value = 255;
            return (byte)value;
        }

        public void Dispose() {
            if (_isInitialized) {
                RestoreState();
            }
            _client?.Dispose();
            _isInitialized = false;
        }
    }
}
