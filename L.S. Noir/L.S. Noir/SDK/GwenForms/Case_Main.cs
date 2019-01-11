using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSNoir.Data;
using Rage;
using Rage.Forms;

namespace LSNoir.SDK.GwenForms
{
    class Case_Main : GwenForm
    {

        private Gwen.Control.Label lb_ID;
        private Gwen.Control.Label lb_Name;
        private Gwen.Control.Label lb_City;
        private Gwen.Control.Label lb_Address;
        private Gwen.Control.TextBox tb_ID;
        private Gwen.Control.TextBox tb_Name;
        private Gwen.Control.TextBox tb_City;
        private Gwen.Control.TextBox tb_Address;

        private Gwen.Control.Button btn_Save;

        private SDK_Host host;

        public Case_Main(SDK_Host h) : base(typeof(WinForms.Case_Main))
        {
            host = h;
        }

        public override void InitializeLayout()
        {
            //Window.MakeModal();

            Position =
                new System.Drawing.Point(
                    1,
                    Game.Resolution.Height / 2 -
                    this.Size.Height / 2);

            base.InitializeLayout();
        }
    }
}
