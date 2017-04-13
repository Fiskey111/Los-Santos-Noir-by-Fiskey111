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
            if (File.Exists(_location))
            {
                var version = File.ReadLines(_location).First();
                ("Existing .ini found; " + version).AddLog();
                if (version != "Version: 4/7/17")
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
        
        private static void UpdateIni(string version)
        {
            var lines = File.ReadAllLines(_location);
            var lineList = new List<string>();

            switch (version)
            {
                case "Version: 1/10/17":
                    lineList.AddRange(lines);
                    lineList[0] = "Version: 4/7/17";
                    lineList.Add("");
                    lineList.Add("The below value changes the coroner van model -- DEFAULT=burrito3");
                    lineList.Add("CoronerVan=burrito3");
                    lineList.Add("");
                    lineList.Add("The below value will set your detective name -- DEFAULT=Daniel Reagan");
                    lineList.Add("DetectiveName=Daniel Reagan");
                    lineList.Add("");
                    lineList.Add("The below value changes the vehicle list to pull for cars -- DEFAULT=police police2 police3 police4");
                    lineList.Add("Please format it in the following manner (spaces inbetween cars): vehiclename vehiclename vehiclename");
                    lineList.Add("CarList=police police2 police3 police4");
                    lineList.Add("");
                    lineList.Add("The below value changes the minimum and maximum times inbetween stages");
                    lineList.Add("DEAFULT_MIN=5000");
                    lineList.Add("DEAFULT_MAX=10000");
                    lineList.Add("MIN=5000");
                    lineList.Add("MAX=10000");
                    break;
                case "Version: 2/10/17":
                    lineList.AddRange(lines);
                    lineList[0] = "Version: 4/7/17";
                    lineList.Add("");
                    lineList.Add("The below value will set your detective name -- DEFAULT=Daniel Reagan");
                    lineList.Add("DetectiveName=Daniel Reagan");
                    lineList.Add("");
                    lineList.Add("The below value changes the vehicle list to pull for cars -- DEFAULT=police police2 police3 police4");
                    lineList.Add("Please format it in the following manner (spaces inbetween cars): vehiclename vehiclename vehiclename");
                    lineList.Add("CarList=police police2 police3 police4");
                    lineList.Add("");
                    lineList.Add("The below value changes the minimum and maximum times inbetween stages");
                    lineList.Add("DEAFULT_MIN=5000");
                    lineList.Add("DEAFULT_MAX=10000");
                    lineList.Add("MIN=5000");
                    lineList.Add("MAX=10000");
                    break;
            }

            File.Delete(_location);

            using (var ini = File.AppendText(_location))
            {
                foreach (var line in lineList)
                    ini.WriteLine(line);
            }
        }

        private static void CreateIni()
        {
            "Creating .ini".AddLog();
            using (var ini = File.AppendText(_location))
            {
                ini.WriteLine("Version: 4/7/17");
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
                ini.WriteLine("");
                ini.WriteLine("The below value changes the vehicle list to pull for cars -- DEFAULT=police police2 police3 police4");
                ini.WriteLine("Please format it in the following manner (spaces inbetween cars): vehiclename vehiclename vehiclename");
                ini.WriteLine("CarList=police police2 police3 police4");
                ini.WriteLine("");
                ini.WriteLine("The below value changes the minimum and maximum times inbetween stages");
                ini.WriteLine("DEAFULT_MIN=5000");
                ini.WriteLine("DEAFULT_MAX=10000");
                ini.WriteLine("MIN=5000");
                ini.WriteLine("MAX=10000");
            }
        }

        public static InitializationFile InitializeIni()
        {
            var ini = new InitializationFile(_location);
            ini.Create();
            return ini;
        }

        public static bool AiAudio()
        {
            var ini = InitializeIni();
            return ini.ReadBoolean("Options", "AIAudio", true);
        }

        public static Keys ComputerKey()
        {
            var ini = InitializeIni();

            var converter = new KeysConverter();
            var value = ini.ReadString("KeyBindings", "ComputerKey", "D1");
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
            var ini = InitializeIni();
            return ini.ReadString("Options", "CoronerVan", "burrito3");
        }

        public static string OfficerName()
        {
            var ini = InitializeIni();
            return ini.ReadString("Options", "DetectiveName", "Daniel Reagan");
        }

        public static List<string> VehicleList()
        {
            var ini = InitializeIni();

            return ini.ReadString("Options", "CarList", "police police2 police3 police4").Split(' ').ToList();
        }

        public static bool LicenseViewed()
        {
            var ini = InitializeIni();
            return ini.ReadBoolean("Options", "LicenseViewed", false);
        }

        public static bool ReadmeViewed()
        {
            var ini = InitializeIni();
            return ini.ReadBoolean("Options", "ReadmeViewed", false);
        }

        public static double MinValue()
        {
            var ini = InitializeIni();
            return ini.ReadDouble("Options", "MIN", 5000);
        }

        public static double MaxValue()
        {
            var ini = InitializeIni();
            return ini.ReadDouble("Options", "MAX", 10000);
        }
    }
}
