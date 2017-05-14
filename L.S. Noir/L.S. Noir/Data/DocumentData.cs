using LtFlash.Common.EvidenceLibrary.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class DocumentRequestData : IIdentifiable
    {
        //http://stackoverflow.com/questions/2821040/how-do-i-get-the-time-difference-between-two-datetime-objects-using-c
        public string ID { get; set; }
        public DateTime TimeRequested;
        public DateTime TimeDecision; //NOTE: set on request by adding rnd minutes
        public bool Considered;
        public bool Accepted;
        public bool DecisionSeenByPlayer;

        public DocumentRequestData()
        {
        }

        public DocumentRequestData(DocumentData data)
        {
            ID = data.ID;
            TimeRequested = DateTime.Now;
            //TODO: rnd time
            TimeDecision = DateTime.Now.AddMinutes(5);
        }
    }
}
