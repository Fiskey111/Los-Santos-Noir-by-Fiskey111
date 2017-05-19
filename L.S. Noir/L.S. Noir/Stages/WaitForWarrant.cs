using LSNoir.Data;
using LtFlash.Common.ScriptManager.Scripts;
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

        private string[] documentsToAccept = { };
        private string[] evidenceToAnalyze = { };

        private readonly StageData data;

        public WaitForEvent(StageData caseData)
        {
            data = caseData;
        }

        protected override bool Initialize()
        {
            return true;
        }

        protected override void Process()
        {
            if(documentsToAccept.All(d => data.ParentCase.CanDocumentRequestBeAccepted(d))/* &&
                evidenceToAnalyze.All(e => data.ParentCase.IsEvidenceAnalyzed)*/)
            {
                SetScriptFinished(true);
            }
        }

        protected override void End()
        {
        }
    }
}
