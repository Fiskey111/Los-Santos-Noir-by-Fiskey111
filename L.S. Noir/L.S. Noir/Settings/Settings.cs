using Rage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LSNoir.Callouts.SA.Commons
{
    class Settings
    {
        private static string _location = "Plugins/LSPDFR/LSNoir/Settings.ini";
        public static void IniUpdateCheck()
        {
            if (File.Exists(_location)) return;
            "No .ini found".AddLog();
            CreateIni();
        }

        private static void CreateIni()
        {
            "Creating .ini".AddLog();
            using (var ini = File.AppendText(_location))
            {
                ini.WriteLine("Version: 4/18/17");
                ini.WriteLine("DO NOT CHANGE THE ABOVE VALUE");
                ini.WriteLine("");
                ini.WriteLine("");
                ini.WriteLine("[KeyBindings]");
                ini.WriteLine("");
                ini.WriteLine("This must be a value found here: https://msdn.microsoft.com/en-us/library/system.windows.forms.keys(v=vs.110).aspx");
                ini.WriteLine("");
                ini.WriteLine("The below value will change the key to open the SAJRS computer -- DEFAULT=D1");
                ini.WriteLine("ComputerKey=D1");
                ini.WriteLine("");
                ini.WriteLine("The below value will change the key to collect a piece of evidence -- DEFAULT=D1");
                ini.WriteLine("CollectKey=D1");
                ini.WriteLine("");
                ini.WriteLine("The below value will change the key to interact with various objects (e.g. evidence, witnesses, etc) -- DEFAULT=D2");
                ini.WriteLine("InteractKey=D2");
                ini.WriteLine("");
                ini.WriteLine("The below value will change the key to leave an interacted object (e.g. evidence, etc) -- DEFAULT=D3");
                ini.WriteLine("LeaveKey=D3");
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
                ini.WriteLine("");
                ini.WriteLine("Change the following three values in order to edit the maximum time taken for evidence/warrants be tested/approved -- DEFAULTS=[Days=2] [Hours=0] [Minutes=0]");
                ini.WriteLine("E.g.: Want the maximum to be 1 day, 4 hours, and 17 minutes from the instant you click \"test evidence / request warrant\"? You'd set the values to:");
                ini.WriteLine("Days=1 / Hours=4 / Minutes=17");
                ini.WriteLine("MUST BE A POSITIVE INTEGER/ZERO -- e.g. 0 - 9999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999...");
                ini.WriteLine("Days=2");
                ini.WriteLine("Hours=0");
                ini.WriteLine("Minutes=0");
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
                Game.DisplayNotification("[LSN] It looks like an ~r~incorrect key~w~ was selected for ComputerKey. Please check that next time you restart. Defaults being used now...");
            }
            return cKey;
        }

        public static Keys CollectKey()
        {
            var ini = InitializeIni();

            var converter = new KeysConverter();
            var value = ini.ReadString("KeyBindings", "CollectKey", "D1");
            Keys cKey;

            try
            {
                cKey = (Keys)converter.ConvertFromString(value);
            }
            catch
            {
                cKey = Keys.D1;
                Game.DisplayNotification("[LSN] It looks like an ~r~incorrect key~w~ was selected for CollectKey. Please check that next time you restart. Defaults being used now...");
            }
            return cKey;
        }
        
        public static Keys InteractKey()
        {
            var ini = InitializeIni();

            var converter = new KeysConverter();
            var value = ini.ReadString("KeyBindings", "InteractKey", "D2");
            Keys cKey;

            try
            {
                cKey = (Keys)converter.ConvertFromString(value);
            }
            catch
            {
                cKey = Keys.D2;
                Game.DisplayNotification("[LSN] It looks like an ~r~incorrect key~w~ was selected for InteractKey. Please check that next time you restart. Defaults being used now...");
            }
            return cKey;
        }

        public static Keys LeaveKey()
        {
            var ini = InitializeIni();

            var converter = new KeysConverter();
            var value = ini.ReadString("KeyBindings", "LeaveKey", "D3");
            Keys cKey;

            try
            {
                cKey = (Keys)converter.ConvertFromString(value);
            }
            catch
            {
                cKey = Keys.D3;
                Game.DisplayNotification("[LSN] It looks like an ~r~incorrect key~w~ was selected for LeaveKey. Please check that next time you restart. Defaults being used now...");
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

        public static TestTime TestTimes()
        {
            var ini = InitializeIni();
            return new TestTime(ini.ReadInt32("Options", "Days", 2), ini.ReadInt32("Options", "Hours", 0), ini.ReadInt32("Options", "Minutes", 0));
        }
    }

    public class TestTime
    {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public TestTime(int d, int h, int m)
        {
            Days = d;
            Hours = h;
            Minutes = m;
        }
    }
}
