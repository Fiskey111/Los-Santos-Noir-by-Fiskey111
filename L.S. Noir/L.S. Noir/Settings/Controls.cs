using Rage;
using System.Windows.Forms;

namespace LSNoir.Settings
{
    public class Controls
    {
        //TODO:
        // - return vals from ini or default
        // - use ControlSets?
        public ControlSet ActivateComputer = new ControlSet(Keys.C, Keys.LShiftKey, ControllerButtons.None);
        public ControlSet TalkToPed = new ControlSet(Keys.Y, Keys.None, ControllerButtons.None);
        public ControlSet CallCoroner = new ControlSet(Keys.D8, Keys.None, ControllerButtons.None);
        public ControlSet CallEMS = new ControlSet(Keys.D8, Keys.None, ControllerButtons.None);
    }

    public class ControlSet
    {
        //PUBLIC
        public Keys Key { get; set; }
        public Keys Modifier { get; set; }
        public ControllerButtons ControllerBtn { get; set; }
        public string ColorLetter { get; set; } = "y";

        private const string RTAG = "~s~"; //color reset

        public ControlSet() { }

        public ControlSet(Keys key, Keys modifier, ControllerButtons ctrlBtn)
        {
            Key = key;
            Modifier = modifier;
            ControllerBtn = ctrlBtn;
        }

        public string GetDescription()
        {
            string result = Modifier == Keys.None ?
                $"~{ColorLetter}~{Key.ToString()}{RTAG}" :
                $"~{ColorLetter}~{Modifier.ToString()}{RTAG} + ~{ColorLetter}~{Key.ToString()}{RTAG}";


            if (Game.IsControllerConnected && ControllerBtn != ControllerButtons.None)
            {
                result += $" or ~{ColorLetter}~{ControllerBtn.ToString()}{RTAG}";
            }

            return result;
        }

        public bool IsActive()
            => AreKeyboardControlsActive() || AreControllerControlsActive();

        private bool AreKeyboardControlsActive()
            => Modifier == Keys.None ? Game.IsKeyDown(Key) :
            Game.IsKeyDownRightNow(Modifier) && Game.IsKeyDown(Key);

        private bool AreControllerControlsActive()
        {
            if (!Game.IsControllerConnected ||
                ControllerBtn == ControllerButtons.None) return false;

            return Game.IsControllerButtonDown(ControllerBtn);
        }

        public static implicit operator bool(ControlSet ctrlSet) => ctrlSet.IsActive();
    }
}
