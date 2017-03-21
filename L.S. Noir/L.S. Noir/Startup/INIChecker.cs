using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Extensions;
using Rage;

namespace LSNoir
{
    class INIChecker
    {
        private static bool _isRunning = true, _accepted = false;
        private static bool _readmeViewed = Settings.ReadmeViewed();
        private static bool _licenseViewed = Settings.LicenseViewed();

        internal static bool INICheck()
        {
            if (!_readmeViewed) LoadReadme();

            if (!_licenseViewed) DisplayLicense();

            return _accepted;
        }

        private static void LoadReadme()
        {
            "Loading readme".AddLog();
            if (File.Exists(@"Plugins/LSPDFR/LSNoir/Readme.txt"))
            {
                Process.Start("explorer.exe", @"Plugins/LSPDFR/LSNoir/Readme.txt");
                _readmeViewed = true;
            }
            else
            {
                "Readme not found".AddLog();
            }
        }

        private static void DisplayLicense()
        {
            "Displaying License".AddLog();
            _licenseViewed = true;
            GameFiber.StartNew(delegate
            {
                _isRunning = true;
                while (_isRunning)
                {
                    Game.RawFrameRender += LicenseDisplay;
                }
            });
        }

        private static void LicenseDisplay(object sender, GraphicsEventArgs graphicsEventArgs)
        {
            RectangleF rectangle = new RectangleF(new PointF(Game.Resolution.Width / 4, 100), new SizeF(300, 500));
            graphicsEventArgs.Graphics.DrawRectangle(rectangle, Color.White);
            graphicsEventArgs.Graphics.DrawText(
                "This is a license, please review it. \n This is an example of a super long line that needs a ton of room because of its insane amount of important information and it continues for a long time" +
                "and is continues down here as well with a break \n here to test the breaks again \n\n\n\n Press 1 to accept the license; press 2 to not accept the license",
                "Arial", 12f, rectangle.Location, Color.Black, rectangle);

            if (Game.IsKeyDown(Keys.D1))
            {
                "License accepted".AddLog();
                _isRunning = false;
                _accepted = true;
                Game.RawFrameRender -= LicenseDisplay;
            }
            else if (Game.IsKeyDown(Keys.D2))
            {
                "License not accepted".AddLog();
                _isRunning = false;
                _accepted = false;
                Game.RawFrameRender -= LicenseDisplay;
            }
        }
    }
}
