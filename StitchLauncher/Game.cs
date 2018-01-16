using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StitchLauncher
{
    public static class Game
    {
        public static void Launch()
        {
            var startInfo = new ProcessStartInfo
            {
                WorkingDirectory = GetDirectory(),
                FileName = "EverQuest2.exe"
            };
            System.Diagnostics.Process.Start(startInfo);
        }

        public static string GetDirectory()
        {
            return Properties.Settings.Default.EQ2Directory;
        }

        public static bool IsValidSetup()
        {
            string pathToFile = $"{GetDirectory()}\\eq2_default.ini";

            return File.ReadAllText(pathToFile).Contains("cl_ls_address eq2emulator.net");
        }
        
        public static void SetDirectory(string directory)
        {
            Properties.Settings.Default.EQ2Directory = directory;
            Properties.Settings.Default.Save();
        }

        public static void SetLoginAddress()
        {
            string pathToFile = $"{GetDirectory()}\\eq2_default.ini";
            string text = File.ReadAllText(pathToFile);

            text = Regex.Replace(text, @"^cl_ls_address\s.*?$", "cl_ls_address eq2emulator.net\r\n", RegexOptions.Multiline);

            File.WriteAllText(pathToFile, text);
        }
    }
}
