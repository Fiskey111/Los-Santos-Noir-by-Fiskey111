using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Media;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System.Drawing;
using Font = RAGENativeUI.Common.EFont;

namespace LSNoir.Resources
{
    public delegate void EmptyArgs();

    public class MissionPassedScreen
    {
        public event EmptyArgs OnContinueHit;
        public string Title { get; set; }

        private List<MissionPassedItem> _items = new List<MissionPassedItem>();
        private int _completeValue;
        private Medal _medal;

        public bool Visible { get; set; }

        public MissionPassedScreen(string title, int completionRate, Medal medal)
        {
            Title = title;
            _completeValue = completionRate;
            _medal = medal;

            Visible = false;
        }

        public void AddItem(MissionPassedItem item) => _items.Add(item);

        public void Show()
        {
            GameFiber.StartNew(delegate
            {
                //var m = new MediaPlayer();
                //m.Open(new Uri(Path.GetFullPath(@"Plugins/LSPDFR/LSNoir/Audio/Complete.wav")));
                //m.HasAudio.ToString().AddLog();
                //while (!m.HasAudio || m.IsBuffering)
                //    GameFiber.Yield();
                //m.Volume = m.Volume / 2;
                //"Playing audio".AddLog();
                //m.Position = TimeSpan.Zero;
                //m.NaturalDuration.ToString().AddLog();
                //m.Play();
            });
            Visible = true;
        }

        public void Draw()
        {
            if (!Visible) return;

            var res = UIMenu.GetScreenResolutionMantainRatio();
            var middle = Convert.ToInt32(res.Width / 2);

            new Sprite("mpentry", "mp_modenotselected_gradient", new Point(0, 10), new Size(Convert.ToInt32(res.Width), 450 + (_items.Count * 40)),
                0f, Color.FromArgb(200, 255, 255, 255)).Draw();

            new ResText("mission passed", new Point(middle, 100), 2.5f, Color.FromArgb(255, 199, 168, 87), Font.Pricedown, ResText.Alignment.Centered).Draw();

            new ResText(Title, new Point(middle, 230), 0.5f, Color.White, Font.ChaletLondon, ResText.Alignment.Centered).Draw();
            new ResRectangle(new Point(middle - 300, 290), new Size(600, 2), Color.White).Draw();

            for (var i = 0; i < _items.Count; i++)
            {
                new ResText(_items[i].Label, new Point(middle - 230, 300 + (40 * i)), 0.35f, Color.White, Font.ChaletLondon, ResText.Alignment.Left).Draw();
                new ResText(_items[i].Status, new Point(_items[i].TickState == TickboxState.None ? middle + 265 : middle + 230, 300 + (40 * i)), 0.35f, Color.White, Font.ChaletLondon, ResText.Alignment.Right).Draw();
                if (_items[i].TickState == TickboxState.None) continue;
                var spriteName = "shop_box_blank";
                if (_items[i].TickState == TickboxState.Tick)
                    spriteName = "shop_box_tick";
                else if (_items[i].TickState == TickboxState.Cross)
                    spriteName = "shop_box_cross";

                new Sprite("commonmenu", spriteName, new Point(middle + 230, 290 + (40 * i)), new Size(48, 48)).Draw();
            }
            new ResRectangle(new Point(middle - 300, 300 + (40 * _items.Count)), new Size(600, 2), Color.White).Draw();

            new ResText("Completion", new Point(middle - 150, 320 + (40 * _items.Count)), 0.4f).Draw();
            new ResText(_completeValue + "%", new Point(middle + 150, 320 + (40 * _items.Count)), 0.4f, Color.White, Font.ChaletLondon, ResText.Alignment.Right).Draw();

            var medalSprite = "bronzemedal";
            if (_medal == Medal.Silver)
                medalSprite = "silvermedal";
            else if (_medal == Medal.Gold)
                medalSprite = "goldmedal";

            new Sprite("mpmissionend", medalSprite, new Point(middle + 150, 320 + (40 * _items.Count)), new Size(32, 32)).Draw();

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
            //Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET"); -- todo figure this out
            ContinueHit();
        }

        public enum Medal { Bronze, Silver, Gold }

        public enum TickboxState { None, Empty, Tick, Cross, }

        protected virtual void ContinueHit() { OnContinueHit?.Invoke(); }
    }

    public class MissionPassedHandler
    {
        private MissionPassedScreen Screen { get; }
        private GameFiber _fiber;

        public MissionPassedHandler() { }

        public MissionPassedHandler(string title, int completion, MissionPassedScreen.Medal medal)
        {
            Screen = new MissionPassedScreen(title, completion, medal);
        }

        public void AddItem(MissionPassedItem item) => Screen.AddItem(item);

        public void AddItem(string label, string status, MissionPassedScreen.TickboxState tickState) => Screen.AddItem(new MissionPassedItem(label, status, tickState));

        public void Show()
        {
            _fiber = new GameFiber(ShowFiber);
            _fiber.Start();
        }

        private void ShowFiber()
        {
            Screen.Show();
            while (!Game.IsKeyDown(Keys.Enter))
            {
                Screen.Draw();
                GameFiber.Yield();
            }
            _fiber.Abort();
        }
    }

    public class MissionPassedItem
    {
        internal string Label { get; set; }
        internal string Status { get; set; }
        internal MissionPassedScreen.TickboxState TickState { get; set; }

        public MissionPassedItem(string label, string status, MissionPassedScreen.TickboxState state)
        {
            Label = label;
            Status = status;
            TickState = state;
        }
    }
}
