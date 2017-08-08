using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Computer.GwenForms
{
    class FindPersonForm : Rage.Forms.GwenForm
    {
        private Label name_lb;
        private TextBox find;
        private MultilineTextBox description;
        private ListBox found;

        private Button search;
        private Button close;

        public FindPersonForm() : base(typeof(WinForms.FindPerson_Form))
        {
        }

        public override void InitializeLayout()
        {
            SharedMethods.SetFormPositionCenter(this);
            base.InitializeLayout();
        }
    }
}
