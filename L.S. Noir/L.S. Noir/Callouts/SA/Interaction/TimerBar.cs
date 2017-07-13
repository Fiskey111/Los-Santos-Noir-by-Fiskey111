using System.Drawing;
using LSNoir.Extensions;
using Rage;
using RAGENativeUI.Elements;

namespace LSNoir.Callouts.SA.Interaction
{
    public static class TimerBar
    {
        private static TimerBarPool _timerBarPool;

        private static BarTimerBar _barTimerBar;
        private static TextTimerBar _textTimerBar;
        private static TextTimerBar _textTimerBar2;
        private static float _distance;
        private static Vector3 _loc;
        internal static bool Runtimer = false;

        public static void Main(Vector3 loc)
        {
            "Starting TimerBar Main".AddLog();
            _distance = Game.LocalPlayer.Character.TravelDistanceTo(loc);
            _loc = loc;

            _timerBarPool = new TimerBarPool();

            _barTimerBar = new BarTimerBar("Distance Remaining");
            _barTimerBar.ForegroundColor = Color.Yellow;

            _timerBarPool.Add(_barTimerBar);

            "Starting TimerBar Process".AddLog();
            Game.FrameRender += Process;

            while (true)
                GameFiber.Yield();
        }


        public static void Process(object sender, GraphicsEventArgs e)
        {
            _timerBarPool.Draw();

            float distanceleft = Game.LocalPlayer.Character.TravelDistanceTo(_loc);
            float percent = (distanceleft / _distance);

            _barTimerBar.Percentage = percent;

            if (Runtimer)
            {
                Game.FrameRender -= Process;
            }
        }
    }
}
