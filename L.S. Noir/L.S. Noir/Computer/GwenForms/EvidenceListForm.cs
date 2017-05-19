using Gwen.Control;
using LSNoir.Data;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LSNoir.Computer.GwenForms
{
    class EvidenceListForm : GwenForm
    {
        private CaseData data;

        private ListBox evidence;
        private TextBox status;
        private MultilineTextBox description, analysis;
        private Button close, request;

        class Evidence
        {
            public CollectedEvidenceData collected;
            public EvidenceData data;
        }

        public EvidenceListForm(CaseData caseData) : base(typeof(WinForms.EvidenceList_Form))
        {
            data = caseData;
        }

        public override void InitializeLayout()
        {
            SharedMethods.SetFormPositionCenter(this);

            close.Clicked += (s, e) => Window.Close();

            evidence.RowSelected += Evidence_RowSelected;

            request.Clicked += Request_Clicked;

            DisableTextBoxes();

            var collectedEvidence = data.GetCaseProgress().CollectedEvidence;

            if (collectedEvidence.Count < 1) return;

            for (int i = 0; i < collectedEvidence.Count; i++)
            {
                var e = new Evidence
                {
                    collected = collectedEvidence[i],
                    data = data.GetEvidenceData(collectedEvidence[i].ID),
                };

                evidence.AddRow(e.data.Name, e.data.ID, e);
            }

            base.InitializeLayout();
        }

        private void Request_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var selectedEvidence = GetSelectedEvidence();

            if(selectedEvidence == null)
            {
                var mb = new MessageBox(this, "Select an evidence!", "WARNING");
                return;
            }

            data.ModifyCaseProgress(m => m.CollectedEvidence.Find(e => e.ID == selectedEvidence.collected.ID).TimeAnalysisDone = DateTime.Now.AddMinutes(1));
        }

        private void Evidence_RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            var selectedEvidence = GetSelectedEvidence();

            SharedMethods.AddSplittedTxtToMultilineTextBox(selectedEvidence.data.Description, description);
            description.Padding = Gwen.Padding.One;

            if (selectedEvidence.collected.WasAnalysisRequested())
            {
                request.Disable();
                request.KeyboardInputEnabled = false;

                if(selectedEvidence.collected.CanEvidenceBeAnalyzed())
                {
                    status.Text = "Analysis done. Report is available.";

                    if (selectedEvidence.collected.ReportSeenByPlayer) return;

                    var reports = new List<string>();

                    foreach (var r in selectedEvidence.data.ReportsID)
                    {
                        var rdata = data.GetReportData(r);
                        reports.Add(rdata.Title);
                        reports.Add(rdata.Text);
                        reports.Add("{n}{n}");
                    }

                    SharedMethods.AddSplittedTxtToMultilineTextBox(string.Join("{n}", reports), analysis);

                    analysis.Padding = Gwen.Padding.One;

                    data.ModifyCaseProgress(m => m.CollectedEvidence.Find(e => e.ID == selectedEvidence.collected.ID).ReportSeenByPlayer = true);

                    data.AddReportsToProgress(selectedEvidence.data.ReportsID);
                    data.AddReportsToProgress(selectedEvidence.data.NotesID);
                }
                else
                {
                    status.Text = "Analysis in progress";
                }
            }
            else
            {
                request.Enable();
                request.KeyboardInputEnabled = true;

                status.Text = "Analysis available to request";
            }
        }

        private Evidence GetSelectedEvidence()
        {
            return evidence.SelectedRow.UserData as Evidence;
        }

        private void DisableTextBoxes()
        {
            description.Disable();
            description.KeyboardInputEnabled = false;

            analysis.Disable();
            analysis.KeyboardInputEnabled = false;

            status.Disable();
            status.KeyboardInputEnabled = false;
        }
    }
}
