using System.Collections.Generic;

namespace KST.ScriptWizard {
    /// <summary>
    /// The behaviors the wizard is able to generate.
    /// Each maps to a lua library in the settings folder.
    /// </summary>
    public enum WizardAction {
        HoldButton,
        SpamMouseButton,
        SpamKey,
        Autorun,
        CancelAll
    }

    /// <summary>
    /// A single "key does X" binding in the wizard.
    /// </summary>
    public class WizardBinding {
        /// <summary>
        /// The key KST will receive, ex "G5" or "F5"
        /// </summary>
        public string TriggerKey { get; set; } = string.Empty;

        /// <summary>
        /// The key to light up, ex "G5".
        /// Differs from TriggerKey when G-Hub remaps G-keys to F-keys.
        /// </summary>
        public string ColorKey { get; set; } = string.Empty;

        public WizardAction Action { get; set; }

        /// <summary>
        /// The key or mouse button being held/spammed, ex "E" or "LMB".
        /// Unused for CancelAll.
        /// </summary>
        public string Target { get; set; } = string.Empty;

        /// <summary>
        /// True to only spam while the trigger is held down.
        /// Only valid for SpamKey/SpamMouseButton, and only for real keyboard keys.
        /// </summary>
        public bool HoldToSpam { get; set; }

        /// <summary>
        /// Keys that cancel autorun, ex ["S"]. Only used for Autorun.
        /// </summary>
        public List<string> CancelKeys { get; set; } = new List<string>();
    }

    /// <summary>
    /// The full wizard configuration. Serialized into the generated lua file as a
    /// JSON header comment so the script can be re-opened in the wizard.
    /// </summary>
    public class WizardConfig {
        public string ScriptName { get; set; } = string.Empty;
        public string OutputPrefix { get; set; } = string.Empty;
        public List<WizardBinding> Bindings { get; set; } = new List<WizardBinding>();

        /// <summary>
        /// Movement keys (WASD) to light up, purely cosmetic.
        /// </summary>
        public bool ColorMovementKeys { get; set; } = true;
    }
}
