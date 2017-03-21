using Rage;

namespace LSNoir.Extensions
{
    internal static class NotificationExtensionMethods
    {
        public static void AddLog(this string text, bool logRelease = false)
        {
            if (!logRelease)
            {
                Game.LogTrivialDebug("[L.S. Noir Log]: " + text);
            }
            else
            {
                Game.LogTrivial("[L.S. Noir Log]: " + text);
            }
        }

        public static void DisplayNotification(this string subtitle, string body)
        {
            CaseData _Case = LtFlash.Common.Serialization.Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Case #: " + _Case.Number, subtitle, body);
        }
    }
}
