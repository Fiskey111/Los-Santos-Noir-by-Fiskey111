using Rage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LSNoir.Extensions;

namespace LSNoir.Callouts.SA.Commons
{
    class Settings
    {
        private static string _location = "Plugins/LSPDFR/LSNoir/Settings.ini";
        public static void IniUpdateCheck()
        {
            OfficerData_Darkmyre.PlayerData.LoadOfficer();
            if (File.Exists(_location))
            {
                string version = File.ReadLines(_location).First();
                ("Existing .ini found; " + version).AddLog();
                if (version != "Version: 2/10/17")
                {
                    UpdateIni(version);
                }
            }
            else
            {
                "No .ini found".AddLog();
                CreateIni();
            }
        }

        // FIX TO UPDATE
        private static void UpdateIni(string version)
        {
            string[] lines = File.ReadAllLines(_location);
            List<string> lineList = new List<string>();

            if (version == "Version: 1/10/17")
            {
                lineList.AddRange(lines);
                lineList[0] = "Version: 2/10/17";
                lineList.Add("");
                lineList.Add("The below value changes the coroner van model -- DEFAULT=burrito3");
                lineList.Add("CoronerVan=burrito3");
            }

            File.Delete(_location);

            using (StreamWriter ini = File.AppendText(_location))
            {
                foreach (string line in lineList)
                {
                    ini.WriteLine(line);
                }
            }
        }

        private static void CreateIni()
        {
            "Creating .ini".AddLog();
            using (StreamWriter ini = File.AppendText(_location))
            {
                ini.WriteLine("Version: 2/10/17");
                ini.WriteLine("DO NOT CHANGE THE ABOVE VALUE");
                ini.WriteLine("");
                ini.WriteLine("");
                ini.WriteLine("[KeyBindings]");
                ini.WriteLine("");
                ini.WriteLine("The below value will change the key to open the SAJRS computer -- DEFAULT=D1");
                ini.WriteLine("This must be a value found here: https://msdn.microsoft.com/en-us/library/system.windows.forms.keys(v=vs.110).aspx");
                ini.WriteLine("ComputerKey=D1");
                ini.WriteLine("");
                ini.WriteLine("");
                ini.WriteLine("[Options]");
                ini.WriteLine("");
                ini.WriteLine("The below value will allow for AI audio features -- DEFAULT=true");
                ini.WriteLine("AIAudio=true");
                ini.WriteLine("");
                ini.WriteLine("The below value changes the coroner van model -- DEFAULT=burrito3");
                ini.WriteLine("CoronerVan=burrito3");
            }
        }

        public static InitializationFile InitializeIni()
        {
            InitializationFile ini = new InitializationFile(_location);
            ini.Create();
            return ini;
        }

        public static bool AiAudio()
        {
            InitializationFile ini = InitializeIni();

            return ini.ReadBoolean("Options", "AIAudio", true);
        }

        public static string OfficerName()
        {
            return OfficerData_Darkmyre.PlayerData.OfficerName;
        }

        public static string UnitName()
        {
            return OfficerData_Darkmyre.PlayerData.CallSign;
        }

        public static Keys ComputerKey()
        {
            InitializationFile ini = InitializeIni();

            KeysConverter converter = new KeysConverter();
            string value = ini.ReadString("KeyBindings", "ComputerKey", "D1");
            Keys cKey;

            try
            {
                cKey = (Keys)converter.ConvertFromString(value);
            }
            catch
            {
                cKey = Keys.D1;
                Game.DisplayNotification("[LSN] It looks like an ~r~incorrect key~w~ was selected for ComputerKey(). Please check that next time you restart. Defaults being used now...");
            }
            return cKey;
        }

        public static string CoronerModel()
        {
            InitializationFile ini = InitializeIni();

            return ini.ReadString("Options", "CoronerVan", "burrito3");
        }

        public static bool LicenseViewed()
        {
            InitializationFile ini = InitializeIni();

            return ini.ReadBoolean("Options", "LicenseViewed", false);
        }

        public static bool ReadmeViewed()
        {
            InitializationFile ini = InitializeIni();

            return ini.ReadBoolean("Options", "ReadmeViewed", false);
        }
    }
}
