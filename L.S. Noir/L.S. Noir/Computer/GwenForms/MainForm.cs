using LSNoir.Data;
using Rage;
using Rage.Forms;
using System.Collections.Generic;
using System.Linq;

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

            Window.DisableResizing();

            //Row format: #313: name
            data.ForEach(c => listCases.AddRow(c.Name, c.ID, c));

            listCases.RowSelected += ListCases_RowSelected;

            reports.Clicked += Reports_Clicked;
            warrants.Clicked += Warrants_Clicked;
            notes.Clicked += Notes_Clicked;
            evidence.Clicked += Evidence_Clicked;
            close.Clicked += (s, e) => Window.Close();

            SetButtonsState(false);

            base.InitializeLayout();
        }

        private void SetButtonsState(bool enabled)
        {
            reports.IsDisabled = !enabled;
            reports.KeyboardInputEnabled = enabled;

            warrants.IsDisabled = !enabled;
            warrants.KeyboardInputEnabled = enabled;

            notes.IsDisabled = !enabled;
            notes.KeyboardInputEnabled = enabled;

            evidence.IsDisabled = !enabled;
            evidence.KeyboardInputEnabled = enabled;
        }

        private void Evidence_Clicked(Gwen.Control.Base sender, Gwen.Control.ClickedEventArgs arguments)
        {
            var selectedCase = GetSelectedCaseData();
            if (selectedCase == null) return;

            GameFiber.StartNew(() =>
            {
                if (selectedCase == null)
                {
                    var mb = new Gwen.Control.MessageBox(this, "No case was selected!", "WARNING");
                    return;
                }

                var evidenceWnd = new GwenForms.EvidenceListForm(host, selectedCase);

                host.AddWnd(evidenceWnd);

                evidenceWnd.Show();
            });
        }

        private CaseData GetSelectedCaseData()
        {
            var d = listCases.SelectedRow?.UserData;
            return d != null ? d as CaseData : null;
        }

        private void Notes_Clicked(Gwen.Control.Base sender, Gwen.Control.ClickedEventArgs arguments)
        {
            var selectedCase = GetSelectedCaseData();
            if (selectedCase == null) return;

            GameFiber.StartNew(() =>
            {
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
            var selectedCase = GetSelectedCaseData();
            if (selectedCase == null) return;

            GameFiber.StartNew(() =>
            {
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
            var cp = cd.Progress.GetCaseProgress();
            caseNo.Text = cp.CaseNo.ToString();

            if (!string.IsNullOrEmpty(cd.City))
            {
                city.Text = cd.City;
            }

            if(!string.IsNullOrEmpty(cd.Address))
            {
                address.Text = cd.Address;
            }

            SetButtonsState(true);

            //TODO: add proper case data entry
            var vicsId = cd.Progress.GetCaseProgress().Victims;
            if (vicsId.Count > 0)
            {
                List<string> witnessesNames = new List<string>();
                foreach (var w in vicsId)
                {
                    var n = cd.GetVictimData(w);
                    witnessesNames.Add(n.Name);
                }
                if (witnessesNames.Count < 3)
                {
                    var names = string.Join("; ", witnessesNames);
                    victim.Text = names;
                }
                else
                {
                    victim.Text = "multiple";
                }
            }

            var officers = cd.Progress.GetCaseProgress().Officers;
            if (officers.Count > 0)
            {
                List<string> officersNames = new List<string>();
                foreach (var o in officers)
                {
                    var on = cd.GetOfficerData(o);
                    officersNames.Add(on.Name);
                }
                if (officersNames.Count < 3)
                {
                    var names = string.Join("; ", officersNames);
                    firstOfficer.Text = names;
                }
                else
                {
                    firstOfficer.Text = "multiple";
                }
            }
        }

        private void Reports_Clicked(Gwen.Control.Base sender, Gwen.Control.ClickedEventArgs arguments)
        {
            var selectedCase = GetSelectedCaseData();
            if (selectedCase == null) return;

            GameFiber.StartNew(() =>
            {
                if (selectedCase == null)
                {
                    var mb = new Gwen.Control.MessageBox(this, "No case was selected!", "WARNING");
                    return;
                }

                var r = selectedCase.Progress.GetCaseProgress().ReportsReceived ?? new List<string>();

                List<ReportData> rd = new List<ReportData>();
                r.ForEach(k => rd.Add(selectedCase.GetReportData(k)));
                var reportsWnd = new ReportsListForm(rd.ToArray());
                host.AddWnd(reportsWnd);
                reportsWnd.Show();
            });
        }
    }
}
