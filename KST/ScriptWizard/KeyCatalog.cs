using System.Collections.Generic;
using System.Linq;

namespace KST.ScriptWizard {
    /// <summary>
    /// The key/button choices offered by the wizard.
    /// These are suggestions for the dropdowns, KeyMapper remains the authority on what is valid.
    /// </summary>
    internal static class KeyCatalog {
        public static readonly string[] MouseButtons = { "LMB", "RMB", "MMB" };

        /// <summary>
        /// Keys that make sense as a trigger. G-keys first, since that is the typical setup,
        /// followed by F-keys for G-Hub users whose G-keys arrive as F-keys.
        /// </summary>
        public static IEnumerable<string> TriggerKeys {
            get {
                foreach (var key in Enumerable.Range(1, 9)) {
                    yield return $"G{key}";
                }

                foreach (var key in Enumerable.Range(1, 12)) {
                    yield return $"F{key}";
                }
            }
        }

        /// <summary>
        /// Keys that can be lit up. Same as the triggers, KST can only color keyboard keys.
        /// </summary>
        public static IEnumerable<string> ColorKeys => TriggerKeys;

        /// <summary>
        /// Keys that can be sent to the game.
        /// </summary>
        public static IEnumerable<string> TargetKeys {
            get {
                foreach (var key in "ABCDEFGHIJKLMNOPQRSTUVWXYZ") {
                    yield return key.ToString();
                }

                foreach (var key in Enumerable.Range(0, 10)) {
                    yield return key.ToString();
                }

                foreach (var key in Enumerable.Range(1, 12)) {
                    yield return $"F{key}";
                }

                yield return "SPACE";
                yield return "RETURN";
                yield return "TAB";
                yield return "SHIFT";
                yield return "CONTROL";
                yield return "MENU";
                yield return "ESCAPE";
            }
        }

        /// <summary>
        /// Keys that typically cancel autorun.
        /// </summary>
        public static readonly string[] MovementKeys = { "W", "A", "S", "D" };
    }
}
