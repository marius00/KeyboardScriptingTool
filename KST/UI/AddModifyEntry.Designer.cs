namespace KST.UI {
    partial class AddModifyEntry {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddModifyEntry));
            this.tbProcess = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.lbScript = new System.Windows.Forms.Label();
            this.tbScript = new System.Windows.Forms.TextBox();
            this.btnBrowseLua = new System.Windows.Forms.Button();
            this.btnPickProcess = new System.Windows.Forms.Button();
            this.btnBrowseProcess = new System.Windows.Forms.Button();
            this.btnWizard = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkOpenFolder = new System.Windows.Forms.LinkLabel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbProcess
            // 
            this.tbProcess.Location = new System.Drawing.Point(89, 45);
            this.tbProcess.Name = "tbProcess";
            this.tbProcess.Size = new System.Drawing.Size(304, 20);
            this.tbProcess.TabIndex = 3;
            this.tbProcess.TextChanged += new System.EventHandler(this.tbProcess_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Process/Exe";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Description";
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(89, 19);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(304, 20);
            this.tbDescription.TabIndex = 1;
            // 
            // lbScript
            // 
            this.lbScript.AutoSize = true;
            this.lbScript.Location = new System.Drawing.Point(12, 74);
            this.lbScript.Name = "lbScript";
            this.lbScript.Size = new System.Drawing.Size(34, 13);
            this.lbScript.TabIndex = 4;
            this.lbScript.Text = "Script";
            // 
            // tbScript
            // 
            this.tbScript.Location = new System.Drawing.Point(89, 71);
            this.tbScript.Name = "tbScript";
            this.tbScript.Size = new System.Drawing.Size(304, 20);
            this.tbScript.TabIndex = 5;
            // 
            // btnBrowseLua
            // 
            this.btnBrowseLua.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBrowseLua.Location = new System.Drawing.Point(399, 69);
            this.btnBrowseLua.Name = "btnBrowseLua";
            this.btnBrowseLua.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseLua.TabIndex = 6;
            this.btnBrowseLua.Text = "Browse..";
            this.btnBrowseLua.UseVisualStyleBackColor = true;
            this.btnBrowseLua.Click += new System.EventHandler(this.btnBrowseLua_Click);
            // 
            // btnPickProcess
            // 
            this.btnPickProcess.Location = new System.Drawing.Point(399, 43);
            this.btnPickProcess.Name = "btnPickProcess";
            this.btnPickProcess.Size = new System.Drawing.Size(75, 23);
            this.btnPickProcess.TabIndex = 7;
            this.btnPickProcess.Text = "Find..";
            this.btnPickProcess.UseVisualStyleBackColor = true;
            this.btnPickProcess.Click += new System.EventHandler(this.btnPickProcess_Click);
            // 
            // btnBrowseProcess
            // 
            this.btnBrowseProcess.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBrowseProcess.Location = new System.Drawing.Point(480, 43);
            this.btnBrowseProcess.Name = "btnBrowseProcess";
            this.btnBrowseProcess.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseProcess.TabIndex = 8;
            this.btnBrowseProcess.Text = "Browse..";
            this.btnBrowseProcess.UseVisualStyleBackColor = true;
            this.btnBrowseProcess.Click += new System.EventHandler(this.btnBrowseProcess_Click);
            //
            // btnWizard
            //
            this.btnWizard.Location = new System.Drawing.Point(480, 69);
            this.btnWizard.Name = "btnWizard";
            this.btnWizard.Size = new System.Drawing.Size(75, 23);
            this.btnWizard.TabIndex = 12;
            this.btnWizard.Text = "Wizard..";
            this.btnWizard.UseVisualStyleBackColor = true;
            this.btnWizard.Click += new System.EventHandler(this.btnWizard_Click);
            //
            // groupBox1
            //
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.linkOpenFolder);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.tbDescription);
            this.groupBox1.Controls.Add(this.btnBrowseProcess);
            this.groupBox1.Controls.Add(this.tbProcess);
            this.groupBox1.Controls.Add(this.btnPickProcess);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnBrowseLua);
            this.groupBox1.Controls.Add(this.btnWizard);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lbScript);
            this.groupBox1.Controls.Add(this.tbScript);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(596, 166);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Script configuration";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // linkOpenFolder
            // 
            this.linkOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkOpenFolder.AutoSize = true;
            this.linkOpenFolder.Location = new System.Drawing.Point(500, 147);
            this.linkOpenFolder.Name = "linkOpenFolder";
            this.linkOpenFolder.Size = new System.Drawing.Size(90, 13);
            this.linkOpenFolder.TabIndex = 11;
            this.linkOpenFolder.TabStop = true;
            this.linkOpenFolder.Text = "Open script folder";
            this.linkOpenFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkOpenFolder_LinkClicked);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(89, 135);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(6, 135);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // AddModifyEntry
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(618, 186);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddModifyEntry";
            this.Text = "LUA Script Config";
            this.Load += new System.EventHandler(this.AddModifyEntry_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbProcess;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Label lbScript;
        private System.Windows.Forms.TextBox tbScript;
        private System.Windows.Forms.Button btnBrowseLua;
        private System.Windows.Forms.Button btnPickProcess;
        private System.Windows.Forms.Button btnBrowseProcess;
        private System.Windows.Forms.Button btnWizard;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.LinkLabel linkOpenFolder;
    }
}