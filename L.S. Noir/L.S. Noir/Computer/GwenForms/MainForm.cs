using LSNoir.Data;
using Rage;
using Rage.Forms;
using System.Collections.Generic;

namespace LSNoir.Computer
{
    class MainForm : GwenForm
    {
        public bool IsClosed { get; private set; }
        private List<CaseData> data;

#pragma warning disable CS0169 // Unused
#pragma warning disable CS0649 // Unused

        private Gwen.Control.Label caseNo, caseNoLb, victimLb, victim, casesLb, city, cityLb, address, addressLb, firstOfficerLb, firstOfficer;
        private Gwen.Control.Button reports, warrants, evidence, notes;
        Gwen.Control.ListBox listCases;

#pragma warning restore CS0169 // Unused
#pragma warning restore CS0649 // Unusedo

        public MainForm(List<CaseData> caseData) : base(typeof(Main_Form))
        {
            data = caseData;
        }

        public override void InitializeLayout()
        {
            int x = (Game.Resolution.Width / 2) - (Size.Width / 2);
            int y = (Game.Resolution.Height / 2) - (Size.Height / 2);

            Position = new System.Drawing.Point(x, y);

            //Row format: #313: name
            data.ForEach(c => listCases.AddRow(c.Name, c.ID, c));

            listCases.RowSelected += ListCases_RowSelected;
            reports.Clicked += Reports_Clicked;
            warrants.Clicked += Warrants_Clicked;

            base.InitializeLayout();
        }

        private void Warrants_Clicked(Gwen.Control.Base sender, Gwen.Control.ClickedEventArgs arguments)
        {
            GameFiber.StartNew(() =>
            {
                var cd = listCases?.SelectedRow?.UserData as CaseData;

                if (cd == null)
                {
                    var mb = new Gwen.Control.MessageBox(this, "No case was selected!", "WARNING");
                    return;
                }

                var d = new DocumentsListForm(cd);
                d.Show();
            });
        }

        private void ListCases_RowSelected(Gwen.Control.Base sender, Gwen.Control.ItemSelectedEventArgs arguments)
        {
            var cd = (listCases.SelectedRow.UserData as CaseData);
            var cp = cd.GetCaseProgress();
            caseNo.Text = cp.CaseNo.ToString();
            victim.Text = "Test Joanne Doe";
        }

        private void Reports_Clicked(Gwen.Control.Base sender, Gwen.Control.ClickedEventArgs arguments)
        {
            GameFiber.StartNew(() =>
            {
                var cd = listCases?.SelectedRow?.UserData as CaseData;

                if (cd == null)
                {
                    var mb = new Gwen.Control.MessageBox(this, "No case was selected!", "WARNING");
                    return;
                }

                var r = cd.GetCaseProgress().ReportsReceived ?? new List<string>();

                List<ReportData> rd = new List<ReportData>();
                r.ForEach(k => rd.Add(cd.GetReportData(k)));
                var rl = new ReportsListForm(rd.ToArray());
                rl.Show();
            });
        }
    }
}
