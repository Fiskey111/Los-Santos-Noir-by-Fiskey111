using System.Drawing;
using LSNoir.Extensions;
using Rage;

namespace LSNoir.Callouts.SA.Computer
{
    class Background
    {


        private static Texture _background;

        internal static void EnableBackground(string filename)
        {
            _background = Game.CreateTextureFromFile(@"Plugins\LSPDFR\LSNoir\Textures\" + filename);
            if (_background == null)
            {
                @"Failed to load L.S. Noir background.".AddLog();
                Game.DisplayNotification("L.S. Noir background not found -- please check your installation");
            }
            else if (filename == "s_m_m_lsmetro.jpg")
            {
                Game.RawFrameRender += FrameRender;
            }
            else
            {
                "Enabling background - computer".AddLog();
                Game.RawFrameRender += OnRawFrameRender;
            }
        }
        private static void OnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            e.Graphics.DrawTexture(_background, 0f, 0f, Game.Resolution.Width, Game.Resolution.Height);
        }
        private static void FrameRender(object sender, GraphicsEventArgs e)
        {
            e.Graphics.DrawTexture(_background, Game.Resolution.Width, (Game.Resolution.Height - 75), 150, 307);
        }
        internal static void DisableBackground(Type type)
        {
            switch (type)
            {
                case Type.Computer:
                    "Disabling background - computer".AddLog();
                    Game.RawFrameRender -= OnRawFrameRender;
                    break;
                case Type.Suspect:
                    Game.RawFrameRender -= FrameRender;
                    break;
            }
        }

        internal enum Type { Computer, Suspect }
    }
}
