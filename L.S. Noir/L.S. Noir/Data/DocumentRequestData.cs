using LSNoir.Settings;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;
using System;
using System.Linq;

namespace LSNoir.Data
{
    public class DocumentRequestData : IIdentifiable
    {
        public string ID { get; set; }
        public DateTime TimeRequested; //remove as it is unused?
        public DateTime TimeDecision; //NOTE: set on request by adding rnd minutes
        public bool DecisionSeenByPlayer;

        public DocumentRequestData()
        {
        }

        public DocumentRequestData(DocumentData data)
        {
            ID = data.ID;
            TimeRequested = DateTime.Now;

            var minutesDocConsidered = MathHelper.GetRandomInteger(Consts.MIN_TIME_DOCUMENT_CONSIDERED, Consts.MAX_TIME_DOCUMENT_CONSIDERED);
            TimeDecision = DateTime.Now.AddMinutes(minutesDocConsidered);
        }

        public bool IsConsidered() => DateTime.Now > TimeDecision;

        public bool CanDocumentRequestBeAccepted(CaseData caseData)
        {
            var caseProgress = caseData.Progress.GetCaseProgress();
            var docuData = caseData.GetDocumentDataById(ID);

            if (!docuData.DialogIDRequiredToAccept.All(d => caseProgress.DialogsPassed.Contains(d)))
            {
                return false;
            }

            if (!docuData.EvidenceIDRequiredToAccept.All(e => caseProgress.CollectedEvidence.FirstOrDefault(l => l.ID == e) != null))
            {
                return false;
            }

            if (!docuData.ReportIDRequiredToAccept.All(r => caseProgress.ReportsReceived.Contains(r)))
            {
                return false;
            }

            if (!docuData.StageIDRequiredToAccept.All(s => caseProgress.StagesPassed.Contains(s)))
            {
                return false;
            }

            return true;
        }
    }
}
