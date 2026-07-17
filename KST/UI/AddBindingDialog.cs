using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KST.ScriptWizard;

namespace KST.UI {
    /// <summary>
    /// Add/modify a single "key does X" binding in the script wizard.
    /// </summary>
    public partial class AddBindingDialog : Form {
        /// <summary>
        /// The binding being edited. Replaced with the result when the dialog is accepted.
        /// </summary>
        public WizardBinding Binding { get; set; } = new WizardBinding();

        /// <summary>
        /// Triggers already in use by other bindings, used to prevent duplicates.
        /// </summary>
        public List<string> UsedTriggers { get; set; } = new List<string>();

        /// <summary>
        /// Combobox entry mapping a friendly name to a WizardAction.
        /// </summary>
        private class ActionItem {
            public WizardAction Action { get; set; }
            public string Text { get; set; }
            public override string ToString() => Text;
        }

        private static readonly ActionItem[] Actions = {
            new ActionItem { Action = WizardAction.HoldButton, Text = "Hold down a mouse button or key" },
            new ActionItem { Action = WizardAction.SpamMouseButton, Text = "Spam a mouse button" },
            new ActionItem { Action = WizardAction.SpamKey, Text = "Spam a key" },
            new ActionItem { Action = WizardAction.Autorun, Text = "Autorun" },
            new ActionItem { Action = WizardAction.CancelAll, Text = "Cancel all scripts" },
        };

        public AddBindingDialog() {
            InitializeComponent();
        }

        private WizardAction SelectedAction => ((ActionItem)cbAction.SelectedItem).Action;

        private void AddBindingDialog_Load(object sender, EventArgs e) {
            cbAction.Items.AddRange(Actions);
            cbTrigger.Items.AddRange(KeyCatalog.TriggerKeys.Cast<object>().ToArray());
            cbColor.Items.AddRange(KeyCatalog.ColorKeys.Cast<object>().ToArray());

            cbAction.SelectedItem = Actions.First(m => m.Action == Binding.Action);
            cbTrigger.Text = Binding.TriggerKey;
            cbColor.Text = string.IsNullOrEmpty(Binding.ColorKey) ? Binding.TriggerKey : Binding.ColorKey;
            chkHold.Checked = Binding.HoldToSpam;
            tbCancelKeys.Text = string.Join(", ", Binding.CancelKeys ?? new List<string>());

            // Populated by cbAction_SelectedIndexChanged, so it must be set afterwards.
            // Leave the default it picked when adding a new binding, there is nothing to restore.
            if (!string.IsNullOrEmpty(Binding.Target)) {
                cbTarget.Text = Binding.Target;
            }

            UpdateFieldVisibility();
            ValidateInput(out _);
        }

        private void cbAction_SelectedIndexChanged(object sender, EventArgs e) {
            var previousTarget = cbTarget.Text;

            cbTarget.Items.Clear();
            switch (SelectedAction) {
                case WizardAction.HoldButton:
                    cbTarget.Items.AddRange(KeyCatalog.MouseButtons.Cast<object>().ToArray());
                    cbTarget.Items.AddRange(KeyCatalog.TargetKeys.Cast<object>().ToArray());
                    break;

                case WizardAction.SpamMouseButton:
                    cbTarget.Items.AddRange(KeyCatalog.MouseButtons.Cast<object>().ToArray());
                    break;

                case WizardAction.SpamKey:
                    cbTarget.Items.AddRange(KeyCatalog.TargetKeys.Cast<object>().ToArray());
                    break;

                case WizardAction.Autorun:
                    cbTarget.Items.AddRange(KeyCatalog.MovementKeys.Cast<object>().ToArray());
                    break;
            }

            // Keep the previous choice if it is still offered, otherwise fall back to the first entry.
            if (cbTarget.Items.Contains(previousTarget)) {
                cbTarget.Text = previousTarget;
            }
            else if (cbTarget.Items.Count > 0) {
                cbTarget.SelectedIndex = 0;
            }
            else {
                cbTarget.Text = string.Empty;
            }

            if (SelectedAction == WizardAction.Autorun && string.IsNullOrWhiteSpace(tbCancelKeys.Text)) {
                tbCancelKeys.Text = "S";
            }

            UpdateFieldVisibility();
            ValidateInput(out _);
        }

        private void UpdateFieldVisibility() {
            var action = SelectedAction;
            var hasTarget = action != WizardAction.CancelAll;
            var canHold = action == WizardAction.SpamKey || action == WizardAction.SpamMouseButton;

            lblTarget.Visible = hasTarget;
            cbTarget.Visible = hasTarget;
            chkHold.Visible = canHold;
            lblCancelKeys.Visible = action == WizardAction.Autorun;
            tbCancelKeys.Visible = action == WizardAction.Autorun;

            switch (action) {
                case WizardAction.HoldButton:
                    lblTarget.Text = "Button to hold";
                    break;
                case WizardAction.SpamMouseButton:
                    lblTarget.Text = "Button to spam";
                    break;
                case WizardAction.SpamKey:
                    lblTarget.Text = "Key to spam";
                    break;
                case WizardAction.Autorun:
                    lblTarget.Text = "Movement key";
                    break;
            }
        }

        private void Input_Changed(object sender, EventArgs e) {
            ValidateInput(out _);
        }

        private List<string> ParsedCancelKeys() {
            return tbCancelKeys.Text
                .Split(',')
                .Select(m => m.Trim().ToUpperInvariant())
                .Where(m => m.Length > 0)
                .ToList();
        }

        /// <summary>
        /// Validates the current input, returning false and an error message if it is unusable.
        /// </summary>
        private bool ValidateInput(out string error) {
            error = null;
            var action = SelectedAction;
            var trigger = cbTrigger.Text.Trim();
            var color = cbColor.Text.Trim();
            var target = cbTarget.Text.Trim();

            if (trigger.Length == 0) {
                error = "Pick a trigger key";
            }
            else if (!KeyMapper.IsValidLogitechMapping(trigger)) {
                error = $"KST does not recognize the key \"{trigger}\"";
            }
            else if (UsedTriggers.Any(m => string.Equals(m, trigger, StringComparison.OrdinalIgnoreCase))) {
                error = $"\"{trigger}\" is already bound to something else";
            }
            else if (color.Length == 0) {
                error = "Pick a key to light up";
            }
            else if (!KeyMapper.IsValidLogitechMapping(color)) {
                error = $"KST cannot light up the key \"{color}\"";
            }
            else if (action != WizardAction.CancelAll && target.Length == 0) {
                error = "Pick a key or button";
            }
            else if (action == WizardAction.SpamMouseButton && !KeyCatalog.MouseButtons.Contains(target.ToUpperInvariant())) {
                error = "Pick a mouse button (LMB, RMB or MMB)";
            }
            else if ((action == WizardAction.SpamKey || action == WizardAction.Autorun) && !KeyMapper.IsValidKeyCode(target)) {
                error = $"KST cannot press the key \"{target}\"";
            }
            else if (action == WizardAction.HoldButton
                     && !KeyCatalog.MouseButtons.Contains(target.ToUpperInvariant())
                     && !KeyMapper.IsValidKeyCode(target)) {
                error = $"KST cannot hold down \"{target}\"";
            }
            else if (chkHold.Checked && chkHold.Visible && trigger.ToUpperInvariant().StartsWith("G")) {
                // G-keys never send a KeyUpEvent, so the script would spam forever.
                error = "G-keys cannot be held. Map the G-key to an F-key in G-Hub and trigger on that instead.";
            }
            else if (action == WizardAction.Autorun && ParsedCancelKeys().Any(m => !KeyMapper.IsValidKeyCode(m))) {
                error = "One of the cancel keys is not a key KST recognizes";
            }

            lblError.Text = error ?? string.Empty;
            btnOk.Enabled = error == null;
            return error == null;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            if (!ValidateInput(out var error)) {
                MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var action = SelectedAction;
            Binding = new WizardBinding {
                Action = action,
                TriggerKey = cbTrigger.Text.Trim().ToUpperInvariant(),
                ColorKey = cbColor.Text.Trim().ToUpperInvariant(),
                Target = action == WizardAction.CancelAll ? string.Empty : cbTarget.Text.Trim().ToUpperInvariant(),
                HoldToSpam = chkHold.Visible && chkHold.Checked,
                CancelKeys = action == WizardAction.Autorun ? ParsedCancelKeys() : new List<string>()
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
