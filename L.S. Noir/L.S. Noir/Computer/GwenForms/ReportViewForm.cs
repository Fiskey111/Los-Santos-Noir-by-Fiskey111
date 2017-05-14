using Gwen.Control;
using LSNoir.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Computer
{
    class ReportViewForm : Rage.Forms.GwenForm
    {
        private readonly ReportData data;

        TextBox title;
        MultilineTextBox text;
        Label titleLb, textLb;
        Button close;

        public ReportViewForm(ReportData reportData) : base(typeof(ReportView_Form))
        {
            data = reportData;
        }

        public override void InitializeLayout()
        {
            title.SetText(data.Title);
            title.Disable();
            text.SetText(data.Text);
            text.Disable();

            close.Clicked += (s, e) => Window.Close(); 

            base.InitializeLayout();
        }
    }
}
