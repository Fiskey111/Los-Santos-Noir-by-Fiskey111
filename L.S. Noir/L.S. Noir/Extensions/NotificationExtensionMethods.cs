using Rage;

namespace LSNoir.Extensions
{
    internal static class NotificationExtensionMethods
    {
        public static void AddLog(this string text, bool logRelease = false)
        {
            if (logRelease)
                Game.LogTrivial("[L.S. Noir Log]: " + text);
            else
            {
#if DEBUG
                Game.LogTrivialDebug("[L.S. Noir Log]: " + text);
#endif
            }
        }

        public static void DisplayNotification(this string subtitle, string body, int number)
        {
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Case #: " + number, subtitle, body);
        }
    }
}
