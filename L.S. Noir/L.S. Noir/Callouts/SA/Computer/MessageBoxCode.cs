using System.Drawing;
using Gwen.Control;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using Rage;
using Rage.Forms;

namespace LSNoir.Callouts.SA.Computer
{
    public class MessageBoxCode : GwenForm
    {
        private Button return_but;
        private Label Message_Text;

        internal static bool Accepted = false;
        internal static string Message = "";
        internal static GameFiber FormReturn;

        public MessageBoxCode()
            : base(typeof(MessageBoxForm))
        {

        }

        public override void InitializeLayout()
        {
            base.InitializeLayout();
            Window.IsClosable = false;
            Position = new Point(Game.Resolution.Width / 2 - Window.Width / 2, Game.Resolution.Height / 2 - Window.Height / 2);
            "Message Box Created".AddLog();

            CreateMethods();

            FillData();
        }

        private void CreateMethods()
        {
            return_but.Clicked += Return_but_Clicked;
        }

        private void FillData()
        {
            Message_Text.Text = Message;
            Message_Text.MaximumSize = new Point(489, 57);
            Message_Text.Dock = Gwen.Pos.Fill;
        }

        private void Return_but_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Accepted = true;
            Window.Close();
            Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.MessageBoxFiber, ComputerController.Fibers.MainFiber);
        }
    }
}
