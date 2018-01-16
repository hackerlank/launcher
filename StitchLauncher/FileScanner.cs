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
    public partial class FileScanner : Form
    {
        delegate void VoidDelegate();
        delegate void StringArgReturningVoidDelegate(string file);

        bool doneSearching = false;
        Thread fileThread;

        public FileScanner()
        {
            InitializeComponent();

            fileThread = new Thread(FindEQ2Path);
            fileThread.IsBackground = true;
            fileThread.Start();
        }

        private void FileScanner_Closed(object sender, EventArgs e)
        {
            if (Visible)
                Environment.Exit(0);
        }

        private void CloseForm()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new VoidDelegate(CloseForm));
            }
            else
            {
                this.Close();
            }
        }

        private void FindEQ2Path()
        {
            foreach (DriveInfo d in DriveInfo.GetDrives().Where(x => x.IsReady == true))
            {
                Stack<String> searchStack = new Stack<string>();
                string[] tmp;

                searchStack.Push(d.RootDirectory.FullName);

                while (searchStack.Count > 0)
                {
                    String path = searchStack.Pop();

                    try
                    {
                        tmp = Directory.GetFiles(path, "EverQuest2.exe");
                    }
                    catch
                    {
                        continue;
                    }

                    for (int i = 0; i < tmp.Length; i++)
                    {
                        UpdateFiles(tmp[i]);
                    }

                    tmp = Directory.GetDirectories(path);
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        searchStack.Push(tmp[i]);
                    }

                    if (doneSearching)
                        break;
                }

                if (doneSearching)
                    break;
            }

            FinishUpdate();
        }

        private void FinishUpdate()
        {
            HideLoader();

            if (this.Visible)
            {
                if (this.listBox1.Items.Count == 0)
                {
                    DialogResult r = MessageBox.Show("Uh oh! Doesn't look like you have EverQuest 2 installed.\n\nWould you like to download a copy from the official EQ2 site?", "", MessageBoxButtons.YesNo);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            MessageBox.Show("Great! The download will begin shortly after this message.\n\nBe sure to install _and_ fully patch the game. You will need to check \"Download Full Game\" in the \"Select Game Version\" section of the options.\n\nOnce you've installed the game, re-run this file.");
                            System.Diagnostics.Process.Start("https://launch.daybreakgames.com/installer/EQ2_setup.exe");
                            break;

                        case DialogResult.No:
                            MessageBox.Show("No worries.\n\nDownlaod a copy on your own, and re-run this file when you've installed and fully downloaded the game.");
                            break;
                    }

                    CloseForm();
                }
            }
            else
            {
                CloseForm();
            }
        }


        private void HideLoader()
        {
            if (this.loader.InvokeRequired)
            {
                this.Invoke(new VoidDelegate(HideLoader));
            }
            else
            {
                this.loader.Hide();
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            nextButton.Enabled = true;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            Hide();

            doneSearching = true;

            Backup form2 = new Backup(Path.GetDirectoryName(this.listBox1.SelectedItem.ToString()));
            form2.Show();
        }

        private void SetResultText(string text)
        {
            if (this.status.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(SetResultText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.status.Text = text;
                this.status.Show();
            }
        }

        private void UpdateFiles(string file)
        {
            if (this.listBox1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(UpdateFiles);
                this.Invoke(d, new object[] { file });
            }
            else
            {
                this.listBox1.Items.Add(file);

                if (this.listBox1.Items.Count > 1)
                {
                    SetResultText("Looks like you have a few copies, please select one\nfrom the list.");
                }

                nextButton.Show();
            }
        }
    }
}
