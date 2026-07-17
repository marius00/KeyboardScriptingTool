using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KST.Config;
using KST.ScriptWizard;

namespace KST.UI {
    /// <summary>
    /// Generates a lua script from a few checkboxes and dropdowns, so that the common
    /// "hold LMB" / "spam a key" scripts don't have to be written by hand.
    /// </summary>
    public partial class ScriptWizardForm : Form {
        /// <summary>
        /// The configuration being edited. Pre-populate it to re-open an existing generated script.
        /// </summary>
        public WizardConfig Config { get; set; } = new WizardConfig();

        /// <summary>
        /// Filename of the generated script, relative to the settings folder, ex "satisfactory.lua".
        /// Only set once the wizard has been saved.
        /// </summary>
        public string ScriptFileName { get; private set; } = string.Empty;

        private readonly List<WizardBinding> _bindings = new List<WizardBinding>();

        public ScriptWizardForm() {
            InitializeComponent();
        }

        private void ScriptWizardForm_Load(object sender, EventArgs e) {
            tbName.Text = Config.ScriptName;
            tbPrefix.Text = Config.OutputPrefix;
            chkColorMovement.Checked = Config.ColorMovementKeys;

            _bindings.AddRange(Config.Bindings ?? new List<WizardBinding>());

            listBindings.DoubleClick += (s, args) => btnModifyBinding_Click(s, args);
            listBindings.ItemSelectionChanged += (s, args) => UpdateButtonState();

            UpdateListView();
        }

        /// <summary>
        /// A human readable description of what a binding does, for the list view.
        /// </summary>
        private static string Describe(WizardBinding binding) {
            switch (binding.Action) {
                case WizardAction.HoldButton:
                    return $"Hold down {binding.Target}";

                case WizardAction.SpamMouseButton:
                    return binding.HoldToSpam
                        ? $"Spam {binding.Target} while held"
                        : $"Spam {binding.Target}";

                case WizardAction.SpamKey:
                    return binding.HoldToSpam
                        ? $"Spam the \"{binding.Target}\" key while held"
                        : $"Spam the \"{binding.Target}\" key";

                case WizardAction.Autorun:
                    var cancels = binding.CancelKeys != null && binding.CancelKeys.Count > 0
                        ? string.Join("/", binding.CancelKeys)
                        : null;
                    return cancels == null
                        ? $"Autorun ({binding.Target})"
                        : $"Autorun ({binding.Target}, cancelled by {cancels})";

                case WizardAction.CancelAll:
                    return "Cancel all scripts";

                default:
                    return binding.Action.ToString();
            }
        }

        private void UpdateListView() {
            listBindings.BeginUpdate();
            listBindings.Items.Clear();

            foreach (var binding in _bindings) {
                var lvi = new ListViewItem(binding.TriggerKey);
                lvi.SubItems.Add(Describe(binding));
                lvi.SubItems.Add(binding.ColorKey);
                lvi.Tag = binding;
                listBindings.Items.Add(lvi);
            }

            listBindings.EndUpdate();
            UpdateButtonState();
        }

        private void UpdateButtonState() {
            var hasSelection = listBindings.SelectedItems.Count > 0;
            btnModifyBinding.Enabled = hasSelection;
            btnRemoveBinding.Enabled = hasSelection;
            btnSave.Enabled = _bindings.Count > 0 && tbName.Text.Trim().Length > 0;
            btnPreview.Enabled = _bindings.Count > 0;
        }

        private void Input_Changed(object sender, EventArgs e) {
            UpdateButtonState();
        }

        private WizardBinding SelectedBinding() {
            return listBindings.SelectedItems.Count > 0
                ? (WizardBinding)listBindings.SelectedItems[0].Tag
                : null;
        }

        private void btnAddBinding_Click(object sender, EventArgs e) {
            var dialog = new AddBindingDialog {
                UsedTriggers = _bindings.Select(m => m.TriggerKey).ToList()
            };

            if (dialog.ShowDialog(this) == DialogResult.OK) {
                _bindings.Add(dialog.Binding);
                UpdateListView();
            }
        }

        private void btnModifyBinding_Click(object sender, EventArgs e) {
            var selected = SelectedBinding();
            if (selected == null) {
                return;
            }

            var dialog = new AddBindingDialog {
                Binding = selected,
                // Every trigger except the one being edited, otherwise it collides with itself.
                UsedTriggers = _bindings.Where(m => m != selected).Select(m => m.TriggerKey).ToList()
            };

            if (dialog.ShowDialog(this) == DialogResult.OK) {
                _bindings[_bindings.IndexOf(selected)] = dialog.Binding;
                UpdateListView();
            }
        }

        private void btnRemoveBinding_Click(object sender, EventArgs e) {
            var selected = SelectedBinding();
            if (selected == null) {
                return;
            }

            _bindings.Remove(selected);
            UpdateListView();
        }

        /// <summary>
        /// Builds the config from the current input.
        /// </summary>
        private WizardConfig BuildConfig() {
            return new WizardConfig {
                ScriptName = FileName(),
                OutputPrefix = tbPrefix.Text.Trim(),
                ColorMovementKeys = chkColorMovement.Checked,
                Bindings = _bindings.ToList()
            };
        }

        private string FileName() {
            var name = tbName.Text.Trim();
            if (!name.EndsWith(".lua", StringComparison.OrdinalIgnoreCase)) {
                name += ".lua";
            }

            return name;
        }

        private void btnPreview_Click(object sender, EventArgs e) {
            var script = ScriptGenerator.Generate(BuildConfig());

            using (var preview = new Form {
                Text = $"Preview of {FileName()}",
                ClientSize = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent
            }) {
                preview.Controls.Add(new TextBox {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both,
                    WordWrap = false,
                    Dock = DockStyle.Fill,
                    Font = new Font(FontFamily.GenericMonospace, 9),
                    Text = script
                });

                preview.ShowDialog(this);
            }
        }

        /// <summary>
        /// Validates the script name and warns about bindings that are configured but unreachable.
        /// </summary>
        private bool ValidateInput(out string error) {
            error = null;
            var name = tbName.Text.Trim();

            if (name.Length == 0) {
                error = "Give the script a name";
            }
            else if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                error = "The script name contains characters that are not allowed in a filename";
            }
            else if (_bindings.Count == 0) {
                error = "Add at least one key binding";
            }
            else if (_bindings.Count(m => m.Action == WizardAction.Autorun) > 1) {
                error = "Only one autorun binding is supported per script";
            }

            return error == null;
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (!ValidateInput(out var error)) {
                MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var config = BuildConfig();
            var target = Path.Combine(AppPaths.SettingsFolder, config.ScriptName);

            if (File.Exists(target) && ScriptGenerator.TryParseConfig(File.ReadAllText(target)) == null) {
                // Not a wizard generated script, so it may well be hand written. Don't silently eat it.
                var confirm = MessageBox.Show(
                    $"\"{config.ScriptName}\" already exists and was not created by the wizard.\r\n\r\nOverwrite it?",
                    "Overwrite?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (confirm != DialogResult.Yes) {
                    return;
                }
            }

            try {
                File.WriteAllText(target, ScriptGenerator.Generate(config));
            }
            catch (IOException ex) {
                MessageBox.Show($"Could not write {target}:\r\n\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Config = config;
            ScriptFileName = config.ScriptName;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
