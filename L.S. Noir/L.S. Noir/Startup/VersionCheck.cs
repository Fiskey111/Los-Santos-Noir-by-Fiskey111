using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using LSNoir.Extensions;
using Rage;

namespace LSNoir.Startup
{
    internal class VersionCheck
    {
        internal static void CheckVersion()
        {
            if (CompareVersions())
            {
                "L.S. Noir Update Check".DisplayNotification("You have the current version of L.S. Noir!", 0);
            }
            else
            {
                "Newer version available".DisplayNotification($"A newer version of L.S. Noir is available", 0);

                "Newer version available".DisplayNotification("You are being directed to the download page to download the latest version\nPress ~y~enter~w~ to cancel", 0);

                var count = 0;
                while (count <= 300)
                {
                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Enter))
                    {
                        break;
                    }
                    GameFiber.Sleep(20);
                    count++;
                }
                System.Diagnostics.Process.Start("http://bit.ly/LSNDownload");
            }
        }

        private static bool CompareVersions()
        {
            try
            {
                var client = new WebClient();
                var webVersion = client.DownloadString("http://www.fiskey111mods.com/updated-version.html");

                var versInfo = FileVersionInfo.GetVersionInfo(@"Plugins\LSPDFR\L.S. Noir.dll");
                var fileVersion = versInfo.FileVersion;

                ($"Web version retrieved: {webVersion}; Current version: {fileVersion}").AddLog(true);

                if (webVersion == fileVersion)
                {
                    "Client version is updated".AddLog(true);
                    return true;
                }
                else
                {
                    "Client version is outdated".AddLog(true);
                    return false;
                }
            }
            catch (Exception e)
            {
                e.ToString().AddLog();
                return false;
            }
        }

        internal static bool OldLSNCheck()
        {
            try
            {
                if (File.Exists(@"Plugins/LSPDFR/L.S. Noire.exe"))
                {
                    "Old LSN version found".AddLog(true);
                    Game.DisplayNotification(
                        "Old version of L.S. Noir found \nPlease remove all LSN versions and reinstall");
                    return false;
                }
                else
                {
                    "Old LSN check passed".AddLog(true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                $"Error with file check: {ex.ToString()}".AddLog(true);
                return true;
            }
        }
    }
}
