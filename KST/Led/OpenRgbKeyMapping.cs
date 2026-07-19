using System;
using System.Collections.Generic;

namespace KST.Led {
    /// <summary>
    /// Maps KST key names (the same identifiers used by <see cref="KeyMapper"/>) to the
    /// LED names OpenRGB reports for keyboard devices (the common "Key: X" convention).
    ///
    /// OpenRGB addresses LEDs by their index within a device, so at connect time
    /// <see cref="OpenRgbLedProvider"/> resolves each of these names against the concrete
    /// keyboard's reported LED names to build a key -> index lookup. Names that a given
    /// keyboard does not expose are simply skipped (and logged).
    /// </summary>
    internal static class OpenRgbKeyMapping {
        private static readonly Dictionary<string, string> _ledNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "ESC", "Key: Escape" },
            { "F1", "Key: F1" },
            { "F2", "Key: F2" },
            { "F3", "Key: F3" },
            { "F4", "Key: F4" },
            { "F5", "Key: F5" },
            { "F6", "Key: F6" },
            { "F7", "Key: F7" },
            { "F8", "Key: F8" },
            { "F9", "Key: F9" },
            { "F10", "Key: F10" },
            { "F11", "Key: F11" },
            { "F12", "Key: F12" },
            { "PRINTSCREEN", "Key: Print Screen" },
            { "SCROLL", "Key: Scroll Lock" },
            { "PAUSE,.-", "Key: Pause/Break" },
            { "TILDE", "Key: `" },
            { "D1", "Key: 1" },
            { "D2", "Key: 2" },
            { "D3", "Key: 3" },
            { "D4", "Key: 4" },
            { "D5", "Key: 5" },
            { "D6", "Key: 6" },
            { "D7", "Key: 7" },
            { "D8", "Key: 8" },
            { "D9", "Key: 9" },
            { "D0", "Key: 0" },
            { "1", "Key: 1" },
            { "2", "Key: 2" },
            { "3", "Key: 3" },
            { "4", "Key: 4" },
            { "5", "Key: 5" },
            { "6", "Key: 6" },
            { "7", "Key: 7" },
            { "8", "Key: 8" },
            { "9", "Key: 9" },
            { "0", "Key: 0" },
            { "MINUS", "Key: -" },
            { "EQUALS", "Key: =" },
            { "BACKSPACE", "Key: Backspace" },
            { "INSERT", "Key: Insert" },
            { "HOME", "Key: Home" },
            { "PAGE_UP", "Key: Page Up" },
            { "NUMLOCK", "Key: Num Lock" },
            { "DIVIDE", "Key: Number Pad /" },
            { "MULTIPLY", "Key: Number Pad *" },
            { "SUBTRACT", "Key: Number Pad -" },
            { "TAB", "Key: Tab" },
            { "Q", "Key: Q" },
            { "W", "Key: W" },
            { "E", "Key: E" },
            { "R", "Key: R" },
            { "T", "Key: T" },
            { "Y", "Key: Y" },
            { "U", "Key: U" },
            { "I", "Key: I" },
            { "O", "Key: O" },
            { "P", "Key: P" },
            { "OPEN_BRACKET", "Key: [" },
            { "CLOSE_BRACKET", "Key: ]" },
            { "BACKSLASH", "Key: \\" },
            { "KEYBOARD_DELETE", "Key: Delete" },
            { "END", "Key: End" },
            { "PAGE_DOWN", "Key: Page Down" },
            { "NUMPAD7", "Key: Number Pad 7" },
            { "NUMPAD8", "Key: Number Pad 8" },
            { "NUMPAD9", "Key: Number Pad 9" },
            { "NUMPADPLUS", "Key: Number Pad +" },
            { "CAPS_LOCK", "Key: Caps Lock" },
            { "A", "Key: A" },
            { "S", "Key: S" },
            { "D", "Key: D" },
            { "F", "Key: F" },
            { "G", "Key: G" },
            { "H", "Key: H" },
            { "J", "Key: J" },
            { "K", "Key: K" },
            { "L", "Key: L" },
            { "SEMICOLON", "Key: ;" },
            { "APOSTROPHE", "Key: '" },
            { "ENTER", "Key: Enter" },
            { "NUMPAD4", "Key: Number Pad 4" },
            { "NUMPAD5", "Key: Number Pad 5" },
            { "NUMPAD6", "Key: Number Pad 6" },
            { "LEFT_SHIFT", "Key: Left Shift" },
            { "Z", "Key: Z" },
            { "X", "Key: X" },
            { "C", "Key: C" },
            { "V", "Key: V" },
            { "B", "Key: B" },
            { "N", "Key: N" },
            { "M", "Key: M" },
            { "COMMA", "Key: ," },
            { "PERIOD", "Key: ." },
            { "FORWARD_SLASH", "Key: /" },
            { "RIGHT_SHIFT", "Key: Right Shift" },
            { "ARROW_UP", "Key: Up Arrow" },
            { "NUMPAD1", "Key: Number Pad 1" },
            { "NUMPAD2", "Key: Number Pad 2" },
            { "NUMPAD3", "Key: Number Pad 3" },
            { "NUMPADENTER", "Key: Number Pad Enter" },
            { "LEFT_CONTROL", "Key: Left Control" },
            { "LEFT_WINDOWS", "Key: Left Windows" },
            { "LEFT_ALT", "Key: Left Alt" },
            { "SPACE", "Key: Space" },
            { "RIGHT_ALT", "Key: Right Alt" },
            { "RIGHT_WINDOWS", "Key: Right Windows" },
            { "APPLICATION_SELECT", "Key: Menu" },
            { "RIGHT_CONTROL", "Key: Right Control" },
            { "ARROW_LEFT", "Key: Left Arrow" },
            { "ARROW_DOWN", "Key: Down Arrow" },
            { "ARROW_RIGHT", "Key: Right Arrow" },
            { "NUM_ZERO", "Key: Number Pad 0" },
            { "NUM_PERIOD", "Key: Number Pad ." },
            { "G1", "Key: G1" },
            { "G2", "Key: G2" },
            { "G3", "Key: G3" },
            { "G4", "Key: G4" },
            { "G5", "Key: G5" },
            { "G6", "Key: G6" },
            { "G7", "Key: G7" },
            { "G8", "Key: G8" },
            { "G9", "Key: G9" },
            { "G_LOGO", "Key: Logo" },
            { "G_BADGE", "Key: Badge" },
        };

        /// <summary>
        /// Returns the OpenRGB LED name expected for the given KST key, or null if the key is unknown.
        /// </summary>
        public static string TryGetLedName(string key) {
            if (string.IsNullOrEmpty(key)) {
                return null;
            }

            return _ledNames.TryGetValue(key, out var name) ? name : null;
        }

        public static bool IsValidMapping(string key) {
            return !string.IsNullOrEmpty(key) && _ledNames.ContainsKey(key);
        }
    }
}
