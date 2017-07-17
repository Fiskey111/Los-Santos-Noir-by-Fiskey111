using LSNoir.Data;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using System;
using System.Linq;

namespace LSNoir.Stages
{
    class WaitForEvent : BasicScript
    {
        //NOTES:
        // - check if WarrantRequestData.TimeDecision >= Now and 
        //   finish the case or continue with an arrest stage
        // - can be also used to check if given evidence was analyzed by a lab

        // - define StageFailure?

        //TODO:
        // - notify of a doc being accepted and to view a report!
        // - handle document rejected

        private string[] documentsToAccept = { };
        private string[] evidenceToAnalyze = { };

        private readonly StageData data;

        public WaitForEvent(StageData caseData)
        {
            data = caseData;
            //reports id => when player checks a warrant in a computer!!!
            documentsToAccept = data.DocumentsID ?? new string[]{ };
            evidenceToAnalyze = data.EvidenceID ?? new string[]{ };
        }

        protected override bool Initialize()
        {
            Base.SharedStageMethods.DisplayNotification(data);

            return true;
        }

        protected override void Process()
        {
            if (AllDocumentsAccepted() && AllEvidenceAnalyzedAndReportSeen())
            {
                SetFinishedSuccessfullyAndSave();
            }
        }

        private bool AllDocumentsAccepted()
        {
            if (documentsToAccept.Length < 1)
            {
                return true;
            }
            if (!documentsToAccept.All(x => data.ParentCase.GetCaseProgress().RequestedDocuments.Select(d => d.ID).Contains(x)))
            {
                return false;
            }

            //TODO: optimize this BS
            return 
                documentsToAccept.
                    Select(d => data.ParentCase.GetDocuRequestData(d)).
                    All(r => data.ParentCase.CanDocumentRequestBeAccepted(r.ID) && r.IsConsidered() && r.DecisionSeenByPlayer);
        }

        private bool AllEvidenceAnalyzedAndReportSeen()
        {
            if (evidenceToAnalyze.Length < 1) return true;
            var collectedIDs = data.ParentCase.GetCaseProgress().CollectedEvidence;
            if (collectedIDs == null || collectedIDs.Count < 1) return true;
            var evidToAnalyze = collectedIDs.Where(s => evidenceToAnalyze.Contains(s.ID));
            if (evidToAnalyze == null || evidToAnalyze.Count() < 1) return false;
            return evidToAnalyze.All(e => e.CanEvidenceBeAnalyzed() && e.ReportSeenByPlayer);
        }

        private bool AllEvidenceSeen()
        {
            return false;
        }

        private void SetFinishedSuccessfullyAndSave()
        {
            Game.LogTrivial("WaitForEvent.End()");
            data.SaveNextScriptsToProgress(data.NextScripts[0]);
            data.SetThisAsLastStage();
            SetScriptFinished(true);
        }

        protected override void End()
        {
        }
    }
}
