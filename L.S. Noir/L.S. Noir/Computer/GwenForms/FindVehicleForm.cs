using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Computer.GwenForms
{
    class FindVehicleForm : Rage.Forms.GwenForm
    {
        private Label plate_lb;
        private TextBox find;
        private ListBox found;
        private MultilineTextBox description;

        private Button search;
        private Button close;

        public FindVehicleForm() : base(typeof(WinForms.FindVehicle_Form))
        {
        }

        public override void InitializeLayout()
        {
            SharedMethods.SetFormPositionCenter(this);
            base.InitializeLayout();
        }
    }
}
