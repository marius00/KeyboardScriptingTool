using System;
using System.Collections.Generic;
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
    /// </summary>
    internal class OpenRgbLedProvider : ILedProvider {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OpenRgbLedProvider));

        private readonly string _ip;
        private readonly int _port;
        private readonly object _lock = new object();

        private OpenRgbClient _client;
        private int _deviceId = -1;
        private Color[] _buffer;
        private Color[] _savedColors;
        private readonly Dictionary<string, int> _keyToLedIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private bool _isInitialized;
        private bool _dirty;

        public OpenRgbLedProvider(string ip = "127.0.0.1", int port = 6742) {
            _ip = ip;
            _port = port;
        }

        public bool Start() {
            try {
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

                // Put the device into direct/custom control so our per-key writes take effect.
                _client.SetCustomMode(_deviceId);

                ResolveKeyIndices(keyboard);

                // Snapshot the current lighting so it can be restored later (mirrors the Logitech Save/Restore).
                _savedColors = CloneOrBlack(keyboard.Colors, keyboard.Leds.Length);
                _buffer = (Color[])_savedColors.Clone();

                _isInitialized = true;
                Logger.Info($"OpenRGB initialized on \"{keyboard.Name}\", mapped {_keyToLedIndex.Count} keys.");
                return true;
            }
            catch (Exception ex) {
                Logger.Warn($"Could not connect to OpenRGB on {_ip}:{_port}. Is the OpenRGB SDK server running?");
                Logger.Warn(ex.Message, ex);
                _isInitialized = false;
                return false;
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

        private static Color[] CloneOrBlack(Color[] source, int length) {
            var result = new Color[length];
            if (source != null && source.Length == length) {
                Array.Copy(source, result, length);
            }
            return result;
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
                Logger.Debug($"Setting color for {key} to ({r}, {g}, {b})");
                lock (_lock) {
                    _buffer[ledIndex] = new Color(ToByte(r), ToByte(g), ToByte(b));
                    _dirty = true;
                }
            }
        }

        public void Flush() {
            if (!_isInitialized) {
                return;
            }

            lock (_lock) {
                if (!_dirty) {
                    return;
                }

                try {
                    _client.UpdateLeds(_deviceId, _buffer);
                    _dirty = false;
                }
                catch (Exception ex) {
                    Logger.Warn("Error pushing colors to OpenRGB", ex);
                }
            }
        }

        public void SaveState() {
            if (!_isInitialized) {
                return;
            }

            lock (_lock) {
                try {
                    var current = _client.GetControllerData(_deviceId);
                    _savedColors = CloneOrBlack(current.Colors, _buffer.Length);
                }
                catch (Exception ex) {
                    Logger.Warn("Error saving OpenRGB lighting state", ex);
                }
            }
        }

        public void RestoreState() {
            if (!_isInitialized) {
                return;
            }

            lock (_lock) {
                try {
                    _client.UpdateLeds(_deviceId, _savedColors);
                    Array.Copy(_savedColors, _buffer, _buffer.Length);
                    _dirty = false;
                }
                catch (Exception ex) {
                    Logger.Warn("Error restoring OpenRGB lighting state", ex);
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
