using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KST.Config;
using KST.Settings;
using KST.Settings.Dto;

namespace KST.UI {
    public partial class ConfigurationView : Form {
        public ConfigurationView() {
            InitializeComponent();
        }

        private void ConfigurationView_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;
            UpdateListView();

            listView1.DoubleClick +=ListView1_DoubleClick;
            listView1.ItemSelectionChanged +=ListView1_ItemSelectionChanged;
            btnModify.Enabled = listView1.SelectedItems.Count > 0;
            btnDelete.Enabled = listView1.SelectedItems.Count > 0;
        }

        private void ListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            btnModify.Enabled = listView1.SelectedItems.Count > 0;
            btnDelete.Enabled = listView1.SelectedItems.Count > 0;
        }

        private void ListView1_DoubleClick(object sender, EventArgs e) {
            btnModify_Click(sender, e);
        }

        private void UpdateListView() {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            var sr = SettingsReader.Load(AppPaths.SettingsFile);
            foreach (var entry in sr.Entries) {
                ListViewItem lvi = new ListViewItem(entry.Description ?? entry.Process);
                lvi.SubItems.Add(entry.Process);
                lvi.SubItems.Add(entry.Path);
                lvi.Tag = entry.Id;
                listView1.Items.Add(lvi);
            }

            listView1.EndUpdate();
        }

        private void btnAdd_Click(object sender, EventArgs e) {
            var dialog = new AddModifyEntry();
            if (dialog.ShowDialog() == DialogResult.OK) {
                var settings = SettingsReader.Load(AppPaths.SettingsFile);

                
                if (settings.Entries.Any(m => m.Process == dialog.Process)) {
                    MessageBox.Show("A script already exists for this process", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                settings.Entries.Add(new LuaScriptEntry {
                    Process = dialog.Process,
                    Description = dialog.Description,
                    Path = dialog.Script,
                    Id = Guid.NewGuid().ToString()
                });



                SettingsReader.Persist(AppPaths.SettingsFile, settings);
                UpdateListView();
            }
        }

        private void btnModify_Click(object sender, EventArgs e) {
            foreach (var entry in listView1.SelectedItems) {
                ListViewItem lvi = (ListViewItem)entry;
                var dialog = new AddModifyEntry() {
                    Description = lvi.Text,
                    Process = lvi.SubItems[1].Text,
                    Script = lvi.SubItems[2].Text
                };

                if (dialog.ShowDialog() == DialogResult.OK) {
                    var settings = SettingsReader.Load(AppPaths.SettingsFile);
                    var tag = lvi.Tag.ToString();

                    var entries = settings.Entries.Where(m => m.Id != tag).ToList();
                    if (entries.Any(m => m.Process == dialog.Process)) {
                        MessageBox.Show("A script already exists for this process", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    entries.Add(new LuaScriptEntry {
                        Process = dialog.Process,
                        Description = dialog.Description,
                        Path = dialog.Script,
                        Id = tag
                    });

                    settings.Entries = entries;
                    SettingsReader.Persist(AppPaths.SettingsFile, settings);

                    UpdateListView();
                }
                return;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e) {

            foreach (var entry in listView1.SelectedItems) {
                if (MessageBox.Show("Are you sure?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

                ListViewItem lvi = (ListViewItem)entry;
                var tag = lvi.Tag.ToString();

                var settings = SettingsReader.Load(AppPaths.SettingsFile);
                settings.Entries = settings.Entries.Where(m => m.Id != tag).ToList();
                SettingsReader.Persist(AppPaths.SettingsFile, settings);
                UpdateListView();
                return;
            }

        }

        private void btnViewFolder_Click(object sender, EventArgs e) {
            Process.Start(new ProcessStartInfo { FileName = "file://" + AppPaths.SettingsFolder, UseShellExecute = true });
        }

        private void linkViewLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(new ProcessStartInfo { FileName = "file://" + Path.Combine(AppPaths.CoreFolder, "log.txt"), UseShellExecute = true });
        }

        private void modifyToolStripMenuItem_Click(object sender, EventArgs e) {
            btnModify_Click(null, null);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            btnDelete_Click(null, null);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {
            var enabled = listView1.SelectedItems.Count > 0;
            foreach (var elem in contextMenuStrip1.Items) {
                ToolStripMenuItem item = elem as ToolStripMenuItem;
                item.Enabled = enabled;
            }
        }
    }
}
