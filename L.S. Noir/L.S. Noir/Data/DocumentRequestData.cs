using LSNoir.Settings;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;
using System;

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
    }
}
