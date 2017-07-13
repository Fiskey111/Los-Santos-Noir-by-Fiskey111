using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;

namespace LSNoir
{
    internal class DetectiveNotebook
    {
        private GameFiber _addFiber, _displayFiber;
        internal string Text { get; set; }
        private bool _display;
        private Texture _texture = Game.CreateTextureFromFile(@"Plugins\LSPDFR\LSNoir\Textures\notebook.png");

        internal DetectiveNotebook() { }

        internal void AddEntry()
        {
            _addFiber = new GameFiber(GetEntry);
            _addFiber.Start();
        }

        internal void DisplayNotebook()
        {
            Game.RawFrameRender += Game_RawFrameRender;
        }

        private void GetEntry()
        {
            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(6, "FMMC_KEY_TIP8", "", "", "", "", "", 5000);

            while (NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() == 0)
                GameFiber.Yield();

            var entry = NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
            Game.DisplayNotification(entry);
        }

        private void Game_RawFrameRender(object sender, GraphicsEventArgs e)
        {
            e.Graphics.DrawTexture(_texture, new Vector2(100f, 100f), new Vector2(640f, 504f), 0f, 0f, 0f, 0f, 0f);
            e.Graphics.DrawText(Text, "Arial", 12f, new PointF(100f, 100f), Color.Black);
        }
    }
}
