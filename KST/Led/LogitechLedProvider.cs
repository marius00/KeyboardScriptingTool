using System;
using System.Collections.Generic;
using log4net;

namespace KST.Led {
    /// <summary>
    /// Color integration for Logitech keyboards.
    /// Allows setting colors per-key on keyboards with support for this.
    /// Requires Logitech Gaming Software or G-Hub to be installed and running.
    /// </summary>
    internal class LogitechLedProvider : ILedProvider {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogitechLedProvider));

        private bool _isInitialized;


        public bool Start() {
            _isInitialized = LogitechGSDK.LogiLedInitWithName("LogitechLua");
            if (_isInitialized) {
                LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_ALL);
                LogitechGSDK.LogiLedSaveCurrentLighting();
            }
            else {
                Logger.Warn("Error initializing logitech LED driver");
            }

            return _isInitialized;
        }

        public void SetColor(string key, int r, int g, int b) {
            if (r < 0 || r > 100) {
                Logger.Warn($"Argument red \"{r}\" is outside range [0, 100]");
            } else if (g < 0 || g > 100) {
                Logger.Warn($"Argument green \"{g}\" is outside range [0, 100]");
            }
            else if (b < 0 || b > 100) {
                Logger.Warn($"Argument blue \"{b}\" is outside range [0, 100]");
            }
            else if (string.IsNullOrEmpty(key)) {
                Logger.Warn("Attempting to set color for key, but argument is NULL");
            }
            else if (!KeyMapper.IsValidLogitechMapping(key)) {
                Logger.Warn("Invalid key \"{key}\"");
            } else if (!_isInitialized) {
                Logger.Warn("Attempting to set keyboard colors, but LED api is not initialized");
            }
            else {
                Logger.Debug($"Setting color for {key} to ({r}, {g}, {b})");
                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(KeyMapper.TranslateToLogitechMapping(key), r, g, b);

            }
        }

        public void SaveState() {
            if (_isInitialized) {
                LogitechGSDK.LogiLedSaveCurrentLighting();
            }
        }

        public void RestoreState() {
            if (_isInitialized) {
                LogitechGSDK.LogiLedRestoreLighting();
            }
        }

        public void Flush() {
            // Logitech SDK writes immediately, nothing to batch.
        }

        public void Dispose() {
            if (_isInitialized) {
                LogitechGSDK.LogiLedShutdown();
            }
        }
    }
}
