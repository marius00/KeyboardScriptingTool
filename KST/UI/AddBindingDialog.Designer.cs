namespace KST.UI {
    partial class AddBindingDialog {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent() {
            this.lblAction = new System.Windows.Forms.Label();
            this.cbAction = new System.Windows.Forms.ComboBox();
            this.lblTrigger = new System.Windows.Forms.Label();
            this.cbTrigger = new System.Windows.Forms.ComboBox();
            this.lblColor = new System.Windows.Forms.Label();
            this.cbColor = new System.Windows.Forms.ComboBox();
            this.lblColorHint = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.cbTarget = new System.Windows.Forms.ComboBox();
            this.chkHold = new System.Windows.Forms.CheckBox();
            this.lblCancelKeys = new System.Windows.Forms.Label();
            this.tbCancelKeys = new System.Windows.Forms.TextBox();
            this.lblError = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblAction
            //
            this.lblAction.AutoSize = true;
            this.lblAction.Location = new System.Drawing.Point(12, 15);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(37, 13);
            this.lblAction.TabIndex = 0;
            this.lblAction.Text = "Action";
            //
            // cbAction
            //
            this.cbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAction.FormattingEnabled = true;
            this.cbAction.Location = new System.Drawing.Point(110, 12);
            this.cbAction.Name = "cbAction";
            this.cbAction.Size = new System.Drawing.Size(280, 21);
            this.cbAction.TabIndex = 1;
            this.cbAction.SelectedIndexChanged += new System.EventHandler(this.cbAction_SelectedIndexChanged);
            //
            // lblTrigger
            //
            this.lblTrigger.AutoSize = true;
            this.lblTrigger.Location = new System.Drawing.Point(12, 45);
            this.lblTrigger.Name = "lblTrigger";
            this.lblTrigger.Size = new System.Drawing.Size(61, 13);
            this.lblTrigger.TabIndex = 2;
            this.lblTrigger.Text = "Trigger key";
            //
            // cbTrigger
            //
            this.cbTrigger.FormattingEnabled = true;
            this.cbTrigger.Location = new System.Drawing.Point(110, 42);
            this.cbTrigger.Name = "cbTrigger";
            this.cbTrigger.Size = new System.Drawing.Size(120, 21);
            this.cbTrigger.TabIndex = 3;
            this.cbTrigger.TextChanged += new System.EventHandler(this.Input_Changed);
            //
            // lblColor
            //
            this.lblColor.AutoSize = true;
            this.lblColor.Location = new System.Drawing.Point(12, 75);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(66, 13);
            this.lblColor.TabIndex = 4;
            this.lblColor.Text = "Key to light";
            //
            // cbColor
            //
            this.cbColor.FormattingEnabled = true;
            this.cbColor.Location = new System.Drawing.Point(110, 72);
            this.cbColor.Name = "cbColor";
            this.cbColor.Size = new System.Drawing.Size(120, 21);
            this.cbColor.TabIndex = 5;
            this.cbColor.TextChanged += new System.EventHandler(this.Input_Changed);
            //
            // lblColorHint
            //
            this.lblColorHint.AutoSize = true;
            this.lblColorHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblColorHint.Location = new System.Drawing.Point(240, 75);
            this.lblColorHint.Name = "lblColorHint";
            this.lblColorHint.Size = new System.Drawing.Size(150, 13);
            this.lblColorHint.TabIndex = 6;
            this.lblColorHint.Text = "Only differs with G-Hub";
            //
            // lblTarget
            //
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(12, 105);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(70, 13);
            this.lblTarget.TabIndex = 7;
            this.lblTarget.Text = "Key to spam";
            //
            // cbTarget
            //
            this.cbTarget.FormattingEnabled = true;
            this.cbTarget.Location = new System.Drawing.Point(110, 102);
            this.cbTarget.Name = "cbTarget";
            this.cbTarget.Size = new System.Drawing.Size(120, 21);
            this.cbTarget.TabIndex = 8;
            this.cbTarget.TextChanged += new System.EventHandler(this.Input_Changed);
            //
            // chkHold
            //
            this.chkHold.AutoSize = true;
            this.chkHold.Location = new System.Drawing.Point(110, 132);
            this.chkHold.Name = "chkHold";
            this.chkHold.Size = new System.Drawing.Size(220, 17);
            this.chkHold.TabIndex = 9;
            this.chkHold.Text = "Only while the trigger key is held down";
            this.chkHold.UseVisualStyleBackColor = true;
            this.chkHold.CheckedChanged += new System.EventHandler(this.Input_Changed);
            //
            // lblCancelKeys
            //
            this.lblCancelKeys.AutoSize = true;
            this.lblCancelKeys.Location = new System.Drawing.Point(12, 161);
            this.lblCancelKeys.Name = "lblCancelKeys";
            this.lblCancelKeys.Size = new System.Drawing.Size(66, 13);
            this.lblCancelKeys.TabIndex = 10;
            this.lblCancelKeys.Text = "Cancelled by";
            //
            // tbCancelKeys
            //
            this.tbCancelKeys.Location = new System.Drawing.Point(110, 158);
            this.tbCancelKeys.Name = "tbCancelKeys";
            this.tbCancelKeys.Size = new System.Drawing.Size(120, 20);
            this.tbCancelKeys.TabIndex = 11;
            this.tbCancelKeys.TextChanged += new System.EventHandler(this.Input_Changed);
            //
            // lblError
            //
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Firebrick;
            this.lblError.Location = new System.Drawing.Point(12, 190);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 12;
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(234, 215);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(315, 215);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // AddBindingDialog
            //
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(402, 250);
            this.Controls.Add(this.lblAction);
            this.Controls.Add(this.cbAction);
            this.Controls.Add(this.lblTrigger);
            this.Controls.Add(this.cbTrigger);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.cbColor);
            this.Controls.Add(this.lblColorHint);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.cbTarget);
            this.Controls.Add(this.chkHold);
            this.Controls.Add(this.lblCancelKeys);
            this.Controls.Add(this.tbCancelKeys);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddBindingDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Key binding";
            this.Load += new System.EventHandler(this.AddBindingDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.ComboBox cbAction;
        private System.Windows.Forms.Label lblTrigger;
        private System.Windows.Forms.ComboBox cbTrigger;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.ComboBox cbColor;
        private System.Windows.Forms.Label lblColorHint;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.ComboBox cbTarget;
        private System.Windows.Forms.CheckBox chkHold;
        private System.Windows.Forms.Label lblCancelKeys;
        private System.Windows.Forms.TextBox tbCancelKeys;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
