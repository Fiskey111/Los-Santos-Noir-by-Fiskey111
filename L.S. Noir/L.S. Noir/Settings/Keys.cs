using Rage;
using System.Windows.Forms;

namespace LSNoir.Settings
{
    class Controls
    {
        //TODO:
        // - return vals from ini or default
        // - use ControlSets?

        static Controls()
        {
            //load vals from ini
        }

        //always assign default vals
        public static Keys KeyActivateComputer = Keys.C;
        public static Keys ModifierActivateComputer = Keys.LShiftKey;

        public static ControllerButtons CtrlButtonActivateComputer = Rage.ControllerButtons.None;

        public static Keys KeyTalkToPed = Keys.Y;

        public static Keys KeyOpenSceneCamera = Keys.Q;

        //public static Keys KeyCallCoroner = Keys
    }
}
