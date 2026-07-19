using System;
using System.Linq;
using System.Threading;
using InputSimulatorStandard;
using KST.Led;
using log4net;

namespace KST.LuaIntegration {
    internal class LuaIntegration {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LuaIntegration));
        private readonly ILedProvider _ledProvider;
        private readonly InputSimulator _simulator = new InputSimulator();
        private string _outputPrefix = string.Empty;

        public LuaIntegration(ILedProvider ledProvider) {
            _ledProvider = ledProvider;
        }
        

        public void SetColor(string key, int r, int g, int b) {
            _ledProvider.SetColor(key, r, g, b);
        }

        public void MouseMove(int x, int y) {
            _simulator.Mouse.MoveMouseBy(x, y);
        }

        public void MouseDown(string key) {
            if (key == "LMB") {
                _simulator.Mouse.LeftButtonDown();
            } else if (key == "RMB") {
                _simulator.Mouse.RightButtonDown();
            } else if (key == "MMB") {
                _simulator.Mouse.MiddleButtonDown();
            } else {
                Logger.Warn($"Unknown mouse key \"{key}\", expected LMB, RMB or MMB");
            }
        }

        public void MouseUp(string key) {
            if (key == "LMB") {
                _simulator.Mouse.LeftButtonUp();
            } else if (key == "RMB") {
                _simulator.Mouse.RightButtonUp();
            } else if (key == "MMB") {
                _simulator.Mouse.MiddleButtonUp();
            } else {
                Logger.Warn($"Unknown mouse key \"{key}\", expected LMB, RMB or MMB");
            }
        }

        public void MouseClick(string key) {
            if (key == "LMB") {
                _simulator.Mouse.LeftButtonClick();
            } else if (key == "RMB") {
                _simulator.Mouse.RightButtonClick();
            } else if (key == "MMB") {
                _simulator.Mouse.MiddleButtonClick();
            } else {
                Logger.Warn($"Unknown mouse key \"{key}\", expected LMB, RMB or MMB");
            }
        }

        public void MouseDoubleClick(string key) {
            if (key == "LMB") {
                _simulator.Mouse.LeftButtonDoubleClick();
            } else if (key == "RMB") {
                _simulator.Mouse.RightButtonDoubleClick();
            } else if (key == "MMB") {
                _simulator.Mouse.MiddleButtonDoubleClick();
            } else {
                Logger.Warn($"Unknown mouse key \"{key}\", expected LMB, RMB or MMB");
            }
        }



        // https://github.com/GregsStack/InputSimulatorStandard
        public void KeyDown(string key) {
            if (KeyMapper.IsValidKeyCode(key)) {
                _simulator.Keyboard.KeyDown(KeyMapper.TranslateToKeyCode(key));
            } else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
        }

        public void KeyUp(string key) {
            if (KeyMapper.IsValidKeyCode(key)) {
                _simulator.Keyboard.KeyUp(KeyMapper.TranslateToKeyCode(key));
            } else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
        }

        public void KeyPress(string key) {
            if (KeyMapper.IsValidKeyCode(key)) {
                _simulator.Keyboard.KeyPress(KeyMapper.TranslateToKeyCode(key));
            } else {
                Logger.Warn($"Invalid key \"{key}\"");
            }
        }

        public void Sleep(int milliseconds) {
            Thread.Sleep(milliseconds);
        }

        public long Time() {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public void OutputLogMessage(string message, params object[] args) {
            try {
                Logger.Info(_outputPrefix + string.Format(message, args));
            } catch (FormatException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(message + "[" + string.Join(", ", args.Select(arg => arg.ToString())) + "]");

            }
        }

        public void OutputLogMessage(string message) {
            Logger.Info(_outputPrefix + message);
        }

        /// <summary>
        /// Sets the output prefix for log messages, ex [SomeGame]
        /// </summary>
        /// <param name="prefix"></param>
        public void SetOutputPrefix(string prefix) {
            _outputPrefix = prefix;
        }

    }
}
