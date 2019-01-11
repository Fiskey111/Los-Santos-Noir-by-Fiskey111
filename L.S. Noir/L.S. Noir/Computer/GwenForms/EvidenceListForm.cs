using Gwen.Control;
using LSNoir.Data;
using LSNoir.DataAccess;
using LSNoir.Settings;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;
using Rage.Forms;
using System;
using System.Collections.Generic;

namespace LSNoir.Computer.GwenForms
{
    class EvidenceListForm : GwenForm
    {
        private CaseData data;
        private ComputerController host;

        private ListBox evidence;
        private TextBox status;
        private MultilineTextBox description, analysis;
        private Button close, request;

        class Evidence
        {
            public CollectedEvidenceData collected;
            public EvidenceData data;
        }

        public EvidenceListForm(ComputerController ctrl, CaseData caseData) : base(typeof(WinForms.EvidenceList_Form))
        {
            host = ctrl;
            data = caseData;
        }

        public override void InitializeLayout()
        {
            SharedMethods.SetFormPositionCenter(this);

            Window.DisableResizing();

            close.Clicked += (s, e) => Window.Close();

            evidence.RowSelected += Evidence_RowSelected;

            request.Disable();
            request.KeyboardInputEnabled = false;

            request.Clicked += Request_Clicked;

            DisableTextBoxes();

            var collectedEvidence = data.Progress.GetCaseProgress().CollectedEvidence;

            if (collectedEvidence.Count < 1) return;

            for (int i = 0; i < collectedEvidence.Count; i++)
            {
                var e = new Evidence
                {
                    collected = collectedEvidence[i],
                    data = data.GetResourceByID<EvidenceData>(collectedEvidence[i].ID),
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
            var minutesToAnalysisDone = MathHelper.GetRandomInteger(Consts.MIN_TIME_EVIDENCE_ANALYSIS, Consts.MAX_TIME_EVIDENCE_ANALYSIS);
            var progress = data.Progress.GetCaseProgress();
            progress.CollectedEvidence.Find(e => e.ID == selectedEvidence.collected.ID).TimeAnalysisDone = DateTime.Now.AddMinutes(minutesToAnalysisDone);
            DataProvider.Instance.Save(data.CaseProgressPath, progress);

            request.Disable();
            request.KeyboardInputEnabled = false;

            status.Text = "Analysis in progress";

            GameFiber.StartNew(() =>
            {
                var progressWnd = new ProgressForm("Sending request");
                host.AddWnd(progressWnd);
                progressWnd.Show();
            });
        }

        private void Evidence_RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            var selectedEvidence = GetSelectedEvidence();

            SharedMethods.AddSplittedTxtToMultilineTextBox(selectedEvidence.data.Description, description);
            description.Padding = Gwen.Padding.One;

            if(selectedEvidence.data.ReportsID.Length < 1)
            {
                request.Disable();
                request.KeyboardInputEnabled = false;

                status.Text = "Piece of evidence cannot be analyzed.";

                return;
            }

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
                        var rdata = data.GetResourceByID<ReportData>(r);
                        reports.Add(rdata.Title);
                        reports.Add(rdata.Text);
                        reports.Add("{n}{n}");
                    }

                    SharedMethods.AddSplittedTxtToMultilineTextBox(string.Join("{n}", reports), analysis);

                    analysis.Padding = Gwen.Padding.One;

                    data.Progress.ModifyCaseProgress(m => m.CollectedEvidence.Find(e => e.ID == selectedEvidence.collected.ID).ReportSeenByPlayer = true);

                    data.Progress.AddReportsToProgress(selectedEvidence.data.ReportsID);
                    data.Progress.AddReportsToProgress(selectedEvidence.data.NotesID);
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
            var d = evidence.SelectedRow?.UserData;
            return d != null ? d as Evidence : null;
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
