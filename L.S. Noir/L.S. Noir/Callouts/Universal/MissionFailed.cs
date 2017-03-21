using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using LSNoir.Extensions;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Font = RAGENativeUI.Common.EFont;
using Color = System.Drawing.Color;

namespace LSNoir.Callouts.Universal
{
    public class MissionFailedScreen
    {
        public string Reason { get; set; }
        public bool Visible { get; set; }

        public MissionFailedScreen(string reason)
        {
            Reason = reason;
            Visible = false;
        }

        public void Show()
        {
            GameFiber.StartNew(delegate
            {
                var m = new MediaPlayer();
                m.Open(new Uri(Path.GetFullPath(@"Plugins/LSPDFR/LSNoir/Audio/Failed.wav")));
                m.HasAudio.ToString().AddLog();
                while (!m.HasAudio || m.IsBuffering)
                    GameFiber.Yield();
                m.Volume = m.Volume / 2;
                "Playing audio".AddLog();
                m.Position = TimeSpan.Zero;
                m.NaturalDuration.ToString().AddLog();
                m.Play();
            });
            Visible = true;
        }

        public void Draw()
        {
            if (!Visible) return;

            var res = UIMenu.GetScreenResolutionMantainRatio();
            var middle = Convert.ToInt32(res.Width / 2);

            new Sprite("mpentry", "mp_modenotselected_gradient", new Point(0, 30), new Size(Convert.ToInt32(res.Width), 300),
                0f, Color.FromArgb(230, 255, 255, 255)).Draw();

            new ResText("mission failed", new Point(middle, 100), 2.5f, Color.FromArgb(255, 148, 27, 46), Font.Pricedown, ResText.Alignment.Centered).Draw();

            new ResText(Reason, new Point(middle, 230), 0.5f, Color.White, Font.ChaletLondon, ResText.Alignment.Centered).Draw();

            var scaleform = new Scaleform(0);
            scaleform.Load("instructional_buttons");
            scaleform.CallFunction("CLEAR_ALL");
            scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", 0);
            scaleform.CallFunction("CREATE_CONTAINER");


            scaleform.CallFunction("SET_DATA_SLOT", 0, NativeFunction.Natives.x0499D7B09FC9B407<string>(2, 201, 0), "Continue");
            scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);
            scaleform.Render2D();

            if (!Game.IsKeyDown(Keys.Enter)) return;
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", 0); // Doesn't work
        }
    }
}
