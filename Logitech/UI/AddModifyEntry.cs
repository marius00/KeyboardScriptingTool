﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using KST.Config;

namespace KST.UI {
    public partial class AddModifyEntry : Form {
        public string Description { get; set; } = string.Empty;
        public string Script { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;

        public AddModifyEntry() {
            InitializeComponent();
        }

        private void btnBrowseLua_Click(object sender, EventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Filter = "Lua scripts (*.lua)|*.lua";
            dialog.InitialDirectory = AppPaths.SettingsFolder;
            if (dialog.ShowDialog() == DialogResult.OK) {
                if (File.Exists(dialog.FileName)) {
                    tbScript.Text = Path.GetFileName(dialog.FileName);
                }


                DialogResult = DialogResult.None;
            }
        }

        private void btnPickProcess_Click(object sender, EventArgs e) {
            var dialog = new BrowseProcess();
            if (dialog.ShowDialog() == DialogResult.OK) {
                tbProcess.Text = dialog.ProcessName;
            }
        }

        private void AddModifyEntry_Load(object sender, EventArgs e) {
            tbScript.Text = Script;
            tbDescription.Text = Description;
            tbProcess.Text = Process;

            tbScript.ModifiedChanged += TbScript_ModifiedChanged;
            tbScript.TextChanged += TbScript_ModifiedChanged;
            tbProcess.ModifiedChanged += TbScript_ModifiedChanged;
            tbProcess.TextChanged += TbScript_ModifiedChanged;


            TbScript_ModifiedChanged(null, null);
        }

        private void TbScript_ModifiedChanged(object sender, EventArgs e) {
            btnSave.Enabled = tbScript.Text.Length > 0 && tbProcess.Text.Length > 0 && File.Exists(Path.Combine(AppPaths.SettingsFolder, tbScript.Text));
        }

        private void btnBrowseProcess_Click(object sender, EventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Filter = "Executeables (*.exe)|*.exe";

            if (dialog.ShowDialog() == DialogResult.OK) {
                if (File.Exists(dialog.FileName)) {
                    tbProcess.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
                }

                DialogResult = DialogResult.None;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Script = tbScript.Text;
            Process = tbProcess.Text;
            Description = tbDescription.Text;
            this.Close();
        }

        private void linkOpenFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(AppPaths.SettingsFolder);
        }

        private void groupBox1_Enter(object sender, EventArgs e) {
        }

        private void tbProcess_TextChanged(object sender, EventArgs e) {

        }
    }
}