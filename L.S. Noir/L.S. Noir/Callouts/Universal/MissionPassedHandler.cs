using System.Windows.Forms;
using Rage;

namespace LSNoir.Callouts.Universal
{
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
