using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StitchLauncher
{
    public partial class Backup : Form
    {
        delegate void VoidDelegate();

        private string backupDirectory = "";
        private string eq2Directory = "";
        private int totalFiles = 0;
        private int currentFile = 0;

        public Backup(string path)
        {
            InitializeComponent();
            this.eq2Directory = path;
        }

        private void Backup_Load(object sender, EventArgs e)
        {
            AskBackup();
        }

        private void Backup_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void AskBackup()
        {
            DialogResult d = MessageBox.Show("Would you like to make a backup of your EQ2 installation for emulator play?\n\nThis is _highly recommended_ if you plan to play on both the official servers and emulated servers.", "Stitch Launcher", MessageBoxButtons.YesNo);

            switch (d)
            {
                case DialogResult.Yes:
                    SelectBackupDirectory();
                    break;

                case DialogResult.No:
                    SkipBackupAndLaunch();
                    break;
            }
        }

        private void CopyFiles()
        {
            CopyAll(eq2Directory, backupDirectory);
        }

        private void CopyAll(string SourcePath, string DestinationPath)
        {
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*.*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            string[] files = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories);
            SetTotalFiles(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                SetCurrentFile(i);
                File.Copy(files[i], files[i].Replace(SourcePath, DestinationPath));
            }

            SetCurrentFile(-1);
        }

        private void SelectBackupDirectory()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Where should we copy your EQ2 installation to?\nYou'll need at least 25GB of free space.";
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    backupDirectory = fbd.SelectedPath;

                    Thread t = new Thread(CopyFiles)
                    {
                        IsBackground = true
                    };
                    t.Start();
                } else
                {
                    AskBackup();
                }
            }
        }

        private void SetTotalFiles(int count)
        {
            totalFiles = count;
        }

        private void SetCurrentFile(int index)
        {
            currentFile = index;
            UpdateProgress();
        }

        private void SetBackupAndLaunch()
        {
            Game.SetDirectory(backupDirectory);
            Game.SetLoginAddress();
            Game.Launch();

            Environment.Exit(0);
        }

        private void SkipBackupAndLaunch()
        {
            DialogResult d = MessageBox.Show("Proceeding will modify your EQ2 installation.\nYou will need to re-run the official patcher to connect to live servers.\n\nIs this OK?", "Warning", MessageBoxButtons.YesNo);

            switch (d)
            {
                case DialogResult.Yes:
                    Game.SetDirectory(eq2Directory);
                    Game.SetLoginAddress();
                    Game.Launch();
                    Application.Exit();
                    break;

                case DialogResult.No:
                    break;
            }

            Environment.Exit(0);
        }

        private void UpdateProgress()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new VoidDelegate(UpdateProgress));
            }
            else
            {
                if (currentFile > -1)
                {
                    this.status.Text = $"Copying {currentFile} of {totalFiles} Files";
                    this.progressBar1.Value = Convert.ToInt32((new decimal(currentFile) / new decimal(totalFiles)) * 100m);
                }
                else
                {
                    this.status.Text = "Finished";
                    this.progressBar1.Value = 100;

                    SetBackupAndLaunch();
                }
            }
        }
    }
}
