using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using LSNoir.Extensions;
using Rage;

// Modified from Albo's base RPH Check sclass
namespace LSNoir.Startup
{
    internal class RageCheck
    {
        internal static bool RPHCheck(float minVers)
        {
            try
            {
                var vInfo = FileVersionInfo.GetVersionInfo("RAGEPluginHook.exe");
                float rVers = float.Parse(vInfo.ProductVersion.Substring(0, 4), CultureInfo.InvariantCulture);
                $"RPH v {rVers.ToString()} detected; minimum version {minVers.ToString()}".AddLog(true);

                if (rVers < minVers)
                {
                    GameFiber.StartNew(delegate
                    {
                        while (Game.IsLoading)
                            GameFiber.Yield();

                        "Invalid RPH Version".DisplayNotification($"Your Rage is not up-to-date\nMinimum version is: {minVers}\nYour version is {rVers}", "");
                        "RPH version does not meet minimum requirements.".AddLog(true);

                        "Invalid RPH Version".DisplayNotification("You are being directed to ragepluginhook.net to download the latest version\nPress ~y~enter~w~ to cancel", "");

                        var count = 0;
                        while (count <= 300)
                        {
                            if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Enter))
                            {
                                break;
                            }
                            GameFiber.Sleep(10);
                            count++;
                        }
                        System.Diagnostics.Process.Start("http://bit.ly/RPHUpdate");
                    });
                    return false;
                }
                else
                    return true;
            }
            catch (Exception e)
            {
                (e.ToString()).AddLog();
                Game.LogTrivial(File.Exists("RAGEPluginHook.exe")
                    ? "RAGEPluginHook.exe exists"
                    : "RAGEPluginHook doesn't exist.");
                "Unable to detect RPH".DisplayNotification("Unable to find RagePluginHook. Please send Fiskey111 your log.", "");
                return false;
            }
        }
    }
}
