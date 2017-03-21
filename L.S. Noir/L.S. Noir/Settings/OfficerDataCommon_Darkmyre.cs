using Rage;

namespace OfficerData_Darkmyre
{
    internal static class PlayerData
    {
        internal static InitializationFile IniFile = new InitializationFile(@"Plugins\LSPDFR\myOfficer.ini");

        internal static string OfficerName = "Pete Malloy";
        internal static string CallSign = "DIV_01 ADAM BEAT_12";

        internal static void LoadOfficer()
        {
            if (!IniFile.Exists())
            {
                IniFile.Write("Officer", "Name", "Officer");
                IniFile.Write("Radio", "Division", 1);
                IniFile.Write("Radio", "Unit", "ADAM");
                IniFile.Write("Radio", "Beat", 12);
            }

            OfficerName = IniFile.ReadString("Officer", "Name", "Pete Malloy");
            int divnum = IniFile.ReadInt16("Radio", "Division", 1);
            string division = "DIV_" + divnum.ToString("D2");
            string unit = IniFile.ReadString("Radio", "Unit", "ADAM").ToUpper();
            int beatnum = IniFile.ReadInt16("Radio", "Beat", 12);
            string beat = "BEAT_" + beatnum.ToString("D2");
            CallSign = division + " " + unit + " " + beat;
        }
    }
}
