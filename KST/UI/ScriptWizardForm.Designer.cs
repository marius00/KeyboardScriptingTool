namespace KST.UI {
    partial class ScriptWizardForm {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent() {
            this.gbScript = new System.Windows.Forms.GroupBox();
            this.lblName = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.lblLuaSuffix = new System.Windows.Forms.Label();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.tbPrefix = new System.Windows.Forms.TextBox();
            this.lblPrefixHint = new System.Windows.Forms.Label();
            this.gbBindings = new System.Windows.Forms.GroupBox();
            this.listBindings = new System.Windows.Forms.ListView();
            this.colKey = new System.Windows.Forms.ColumnHeader();
            this.colAction = new System.Windows.Forms.ColumnHeader();
            this.colLights = new System.Windows.Forms.ColumnHeader();
            this.btnAddBinding = new System.Windows.Forms.Button();
            this.btnModifyBinding = new System.Windows.Forms.Button();
            this.btnRemoveBinding = new System.Windows.Forms.Button();
            this.chkColorMovement = new System.Windows.Forms.CheckBox();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbScript.SuspendLayout();
            this.gbBindings.SuspendLayout();
            this.SuspendLayout();
            //
            // gbScript
            //
            this.gbScript.Controls.Add(this.lblName);
            this.gbScript.Controls.Add(this.tbName);
            this.gbScript.Controls.Add(this.lblLuaSuffix);
            this.gbScript.Controls.Add(this.lblPrefix);
            this.gbScript.Controls.Add(this.tbPrefix);
            this.gbScript.Controls.Add(this.lblPrefixHint);
            this.gbScript.Location = new System.Drawing.Point(12, 12);
            this.gbScript.Name = "gbScript";
            this.gbScript.Size = new System.Drawing.Size(520, 90);
            this.gbScript.TabIndex = 0;
            this.gbScript.TabStop = false;
            this.gbScript.Text = "Script";
            //
            // lblName
            //
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 28);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(66, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Script name";
            //
            // tbName
            //
            this.tbName.Location = new System.Drawing.Point(100, 25);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(200, 20);
            this.tbName.TabIndex = 1;
            this.tbName.TextChanged += new System.EventHandler(this.Input_Changed);
            //
            // lblLuaSuffix
            //
            this.lblLuaSuffix.AutoSize = true;
            this.lblLuaSuffix.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblLuaSuffix.Location = new System.Drawing.Point(306, 28);
            this.lblLuaSuffix.Name = "lblLuaSuffix";
            this.lblLuaSuffix.Size = new System.Drawing.Size(24, 13);
            this.lblLuaSuffix.TabIndex = 2;
            this.lblLuaSuffix.Text = ".lua";
            //
            // lblPrefix
            //
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Location = new System.Drawing.Point(12, 58);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(56, 13);
            this.lblPrefix.TabIndex = 3;
            this.lblPrefix.Text = "Log prefix";
            //
            // tbPrefix
            //
            this.tbPrefix.Location = new System.Drawing.Point(100, 55);
            this.tbPrefix.Name = "tbPrefix";
            this.tbPrefix.Size = new System.Drawing.Size(200, 20);
            this.tbPrefix.TabIndex = 4;
            //
            // lblPrefixHint
            //
            this.lblPrefixHint.AutoSize = true;
            this.lblPrefixHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPrefixHint.Location = new System.Drawing.Point(306, 58);
            this.lblPrefixHint.Name = "lblPrefixHint";
            this.lblPrefixHint.Size = new System.Drawing.Size(190, 13);
            this.lblPrefixHint.TabIndex = 5;
            this.lblPrefixHint.Text = "Optional, prefixes this script\'s log output";
            //
            // gbBindings
            //
            this.gbBindings.Controls.Add(this.listBindings);
            this.gbBindings.Controls.Add(this.btnAddBinding);
            this.gbBindings.Controls.Add(this.btnModifyBinding);
            this.gbBindings.Controls.Add(this.btnRemoveBinding);
            this.gbBindings.Location = new System.Drawing.Point(12, 108);
            this.gbBindings.Name = "gbBindings";
            this.gbBindings.Size = new System.Drawing.Size(520, 250);
            this.gbBindings.TabIndex = 1;
            this.gbBindings.TabStop = false;
            this.gbBindings.Text = "Key bindings";
            //
            // listBindings
            //
            this.listBindings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colKey,
            this.colAction,
            this.colLights});
            this.listBindings.FullRowSelect = true;
            this.listBindings.HideSelection = false;
            this.listBindings.Location = new System.Drawing.Point(10, 22);
            this.listBindings.MultiSelect = false;
            this.listBindings.Name = "listBindings";
            this.listBindings.Size = new System.Drawing.Size(390, 215);
            this.listBindings.TabIndex = 0;
            this.listBindings.UseCompatibleStateImageBehavior = false;
            this.listBindings.View = System.Windows.Forms.View.Details;
            //
            // colKey
            //
            this.colKey.Text = "Key";
            this.colKey.Width = 60;
            //
            // colAction
            //
            this.colAction.Text = "Does";
            this.colAction.Width = 250;
            //
            // colLights
            //
            this.colLights.Text = "Lights up";
            this.colLights.Width = 70;
            //
            // btnAddBinding
            //
            this.btnAddBinding.Location = new System.Drawing.Point(410, 22);
            this.btnAddBinding.Name = "btnAddBinding";
            this.btnAddBinding.Size = new System.Drawing.Size(100, 23);
            this.btnAddBinding.TabIndex = 1;
            this.btnAddBinding.Text = "Add..";
            this.btnAddBinding.UseVisualStyleBackColor = true;
            this.btnAddBinding.Click += new System.EventHandler(this.btnAddBinding_Click);
            //
            // btnModifyBinding
            //
            this.btnModifyBinding.Location = new System.Drawing.Point(410, 51);
            this.btnModifyBinding.Name = "btnModifyBinding";
            this.btnModifyBinding.Size = new System.Drawing.Size(100, 23);
            this.btnModifyBinding.TabIndex = 2;
            this.btnModifyBinding.Text = "Modify..";
            this.btnModifyBinding.UseVisualStyleBackColor = true;
            this.btnModifyBinding.Click += new System.EventHandler(this.btnModifyBinding_Click);
            //
            // btnRemoveBinding
            //
            this.btnRemoveBinding.Location = new System.Drawing.Point(410, 80);
            this.btnRemoveBinding.Name = "btnRemoveBinding";
            this.btnRemoveBinding.Size = new System.Drawing.Size(100, 23);
            this.btnRemoveBinding.TabIndex = 3;
            this.btnRemoveBinding.Text = "Remove";
            this.btnRemoveBinding.UseVisualStyleBackColor = true;
            this.btnRemoveBinding.Click += new System.EventHandler(this.btnRemoveBinding_Click);
            //
            // chkColorMovement
            //
            this.chkColorMovement.AutoSize = true;
            this.chkColorMovement.Checked = true;
            this.chkColorMovement.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkColorMovement.Location = new System.Drawing.Point(12, 366);
            this.chkColorMovement.Name = "chkColorMovement";
            this.chkColorMovement.Size = new System.Drawing.Size(200, 17);
            this.chkColorMovement.TabIndex = 2;
            this.chkColorMovement.Text = "Light up the WASD movement keys";
            this.chkColorMovement.UseVisualStyleBackColor = true;
            //
            // btnPreview
            //
            this.btnPreview.Location = new System.Drawing.Point(376, 396);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 3;
            this.btnPreview.Text = "Preview..";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(12, 396);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(457, 396);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // ScriptWizardForm
            //
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(544, 431);
            this.Controls.Add(this.gbScript);
            this.Controls.Add(this.gbBindings);
            this.Controls.Add(this.chkColorMovement);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScriptWizardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Script wizard";
            this.Load += new System.EventHandler(this.ScriptWizardForm_Load);
            this.gbScript.ResumeLayout(false);
            this.gbScript.PerformLayout();
            this.gbBindings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbScript;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label lblLuaSuffix;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.TextBox tbPrefix;
        private System.Windows.Forms.Label lblPrefixHint;
        private System.Windows.Forms.GroupBox gbBindings;
        private System.Windows.Forms.ListView listBindings;
        private System.Windows.Forms.ColumnHeader colKey;
        private System.Windows.Forms.ColumnHeader colAction;
        private System.Windows.Forms.ColumnHeader colLights;
        private System.Windows.Forms.Button btnAddBinding;
        private System.Windows.Forms.Button btnModifyBinding;
        private System.Windows.Forms.Button btnRemoveBinding;
        private System.Windows.Forms.CheckBox chkColorMovement;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
