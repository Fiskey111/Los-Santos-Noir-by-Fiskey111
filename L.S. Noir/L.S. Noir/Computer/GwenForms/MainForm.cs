using LSNoir.Data;
using Rage;
using Rage.Forms;
using System.Collections.Generic;

namespace LSNoir.Computer
{
    class MainForm : GwenForm
    {
        //TODO:
        // - common method to get and validate selected caseData

        public bool IsClosed { get; private set; }
        private readonly List<CaseData> data;
        private readonly ComputerController host;

#pragma warning disable CS0169 // Unused
#pragma warning disable CS0649 // Unused

        private Gwen.Control.Label caseNo, caseNoLb, victimLb, victim, casesLb, city, cityLb, address, addressLb, firstOfficerLb, firstOfficer;
        private Gwen.Control.Button reports, warrants, evidence, notes, close;
        Gwen.Control.ListBox listCases;

#pragma warning restore CS0169 // Unused
#pragma warning restore CS0649 // Unusedo

        public MainForm(ComputerController ctrl, List<CaseData> caseData) : base(typeof(Main_Form))
        {
            data = caseData;
            host = ctrl;
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
            notes.Clicked += Notes_Clicked;
            close.Clicked += (s, e) => Window.Close();
            base.InitializeLayout();
        }

        private CaseData GetSelectedCaseData() => listCases.SelectedRow.UserData as CaseData;

        private void Notes_Clicked(Gwen.Control.Base sender, Gwen.Control.ClickedEventArgs arguments)
        {
            GameFiber.StartNew(() =>
            {
                var selectedCase = GetSelectedCaseData();

                if (selectedCase == null)
                {
                    var mb = new Gwen.Control.MessageBox(this, "No case was selected!", "WARNING");
                    return;
                }

                var notesWnd = new GwenForms.NotesMadeForm(GetSelectedCaseData());

                host.AddWnd(notesWnd);

                notesWnd.Show();
            });
        }

        private void Warrants_Clicked(Gwen.Control.Base sender, Gwen.Control.ClickedEventArgs arguments)
        {
            GameFiber.StartNew(() =>
            {
                var selectedCase = GetSelectedCaseData();

                if (selectedCase == null)
                {
                    var mb = new Gwen.Control.MessageBox(this, "No case was selected!", "WARNING");
                    return;
                }

                var docsWnd = new DocumentsListForm(host, selectedCase);
                host.AddWnd(docsWnd);
                docsWnd.Show();
            });
        }

        private void ListCases_RowSelected(Gwen.Control.Base sender, Gwen.Control.ItemSelectedEventArgs arguments)
        {
            var cd = (listCases.SelectedRow.UserData as CaseData);
            var cp = cd.GetCaseProgress();
            caseNo.Text = cp.CaseNo.ToString();

            //TODO: add proper case data entry
            victim.Text = "Test Joanne Doe";
        }

        private void Reports_Clicked(Gwen.Control.Base sender, Gwen.Control.ClickedEventArgs arguments)
        {
            GameFiber.StartNew(() =>
            {
                var selectedCase = GetSelectedCaseData();

                if (selectedCase == null)
                {
                    var mb = new Gwen.Control.MessageBox(this, "No case was selected!", "WARNING");
                    return;
                }

                var r = selectedCase.GetCaseProgress().ReportsReceived ?? new List<string>();

                List<ReportData> rd = new List<ReportData>();
                r.ForEach(k => rd.Add(selectedCase.GetReportData(k)));
                var reportsWnd = new ReportsListForm(rd.ToArray());
                host.AddWnd(reportsWnd);
                reportsWnd.Show();
            });
        }
    }
}
