using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using InputSimulatorStandard.Native;
using log4net;
using KST.InputProviders.Args;

namespace KST.InputProviders {
    /// <summary>
    /// Input provider for G-keys
    /// </summary>
    internal class LogitechInputProvider : IGKeyInputProvider {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogitechInputProvider));
        private const int MaxGKeys = 29; // G1 G2 G...
        private const int MaxGStates = 3; // M1 M2 M3

        private volatile bool _isRunning = true;
        private readonly Dictionary<string, bool> _state = new Dictionary<string, bool>(MaxGKeys);

        public event InputEventHandler OnInput;


        /// <summary>
        /// Translate 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static ushort MStateToUshort(int state) {
            if (state == 1)
                return (ushort)InputModifierState.M1;
            else if (state == 2)
                return (ushort)InputModifierState.M2;
            else if (state == 3)
                return (ushort)InputModifierState.M3;

            return 0;
        }

        public void Start() {
            new Thread(() => {
                if (LogitechGKeys.LogiGkeyInitWithoutCallback() == 0) {
                    Logger.Error("Could not initialize logitech input provider, G-keys are disabled");
                    return;
                }

                while (_isRunning) {
                    Thread.Sleep(1);

                    for (int key = 1; key <= MaxGKeys; key++) {
                        for (int state = 1; state <= MaxGStates; state++) {
                            var uniqueKey = $"G{key} M{state}";
                            if (LogitechGKeys.LogiGkeyIsKeyboardGkeyPressed(key, state) != 0) {
                                // Limit it to 1 event per click (No support for holding down G keys for the moment)
                                if (!GetLastState(uniqueKey)) {
                                    ushort modifiers = MStateToUshort(state);
                                    if ((GetAsyncKeyState((ushort)VirtualKeyCode.LSHIFT) & 0x8000) != 0) {
                                        modifiers += (ushort)InputModifierState.Shift;
                                    }
                                    if ((GetAsyncKeyState((ushort)VirtualKeyCode.LCONTROL) & 0x8000) != 0) {
                                        modifiers += (ushort)InputModifierState.Ctrl;
                                    }

                                    OnInput?.Invoke(this, new InputEventArg($"G{key}", modifiers, InputEventType.Down));
                                }

                                _state[uniqueKey] = true;
                            }
                            else {
                                _state[uniqueKey] = false;
                            }
                        }
                    }
                }

                LogitechGKeys.LogiGkeyShutdown();
            }).Start();
        }

        private bool GetLastState(string key) {
            if (_state.ContainsKey(key))
                return _state[key];

            return false;
        }

        public void Dispose() {
            _isRunning = false;
        }


        [DllImport("user32.dll", SetLastError = true)]
        public static extern short GetAsyncKeyState(ushort virtualKeyCode);
    }
}