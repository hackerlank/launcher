using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StitchLauncher
{
    public partial class Launcher : Form
    {

        private bool firstRun = false;

        public Launcher()
        {
            InitializeComponent();

            string eq2Path = Properties.Settings.Default.EQ2Directory;

            if (eq2Path == "")
                firstRun = true;

            if (firstRun)
            {
                FileScanner fileScanner = new FileScanner();
                fileScanner.Show();
            }
            else
            {
                if (!Game.IsValidSetup())
                    Game.SetLoginAddress();

                Game.Launch();

                Environment.Exit(0);
            }
        }
    }
}
