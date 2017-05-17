using LtFlash.Common.EvidenceLibrary.Serialization;
using System;

namespace LSNoir.Data
{
    public class DocumentRequestData : IIdentifiable
    {
        //http://stackoverflow.com/questions/2821040/how-do-i-get-the-time-difference-between-two-datetime-objects-using-c
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
            //TODO: rnd time
            TimeDecision = DateTime.Now.AddMinutes(1);
        }

        public bool IsConsidered()
        {
            return DateTime.Now > TimeDecision;
        }
    }
}
