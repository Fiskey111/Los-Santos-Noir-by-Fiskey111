using LtFlash.Common.EvidenceLibrary.Serialization;
using System.Linq;

namespace LSNoir.Data
{
    public class DocumentData : IIdentifiable
    {
        public string ID { get; set; }
        public string To;
        public string Title;
        public string Text;

        public string[] EvidenceIDRequiredToRequest;
        public string[] DialogIDRequiredToRequest;
        public string[] ReportIDRequiredToRequest;
        public string[] StageIDRequiredToRequest;

        public string[] EvidenceIDRequiredToAccept;
        public string[] DialogIDRequiredToAccept;
        public string[] ReportIDRequiredToAccept;
        public string[] StageIDRequiredToAccept;

        public bool CanDocumentBeRequested(CaseData caseData)
        {
            var caseProgress = caseData.Progress.GetCaseProgress();

            if (!DialogIDRequiredToRequest.All(d => caseProgress.DialogsPassed.Contains(d)))
            {
                return false;
            }

            if (!EvidenceIDRequiredToRequest.All(e => caseProgress.CollectedEvidence.FirstOrDefault(l => l.ID == e) != null))
            {
                return false;
            }

            if (!ReportIDRequiredToRequest.All(r => caseProgress.ReportsReceived.Contains(r)))
            {
                return false;
            }

            if (!StageIDRequiredToRequest.All(s => caseProgress.StagesPassed.Contains(s)))
            {
                return false;
            }

            return true;
        }
    }
}
