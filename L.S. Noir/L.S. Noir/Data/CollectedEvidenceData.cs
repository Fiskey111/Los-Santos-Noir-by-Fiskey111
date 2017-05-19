using LtFlash.Common.EvidenceLibrary.Serialization;
using System;

namespace LSNoir.Data
{
    public class CollectedEvidenceData : IIdentifiable
    {
        public string ID { get; set; }
        public DateTime TimeCollected;
        public DateTime TimeAnalysisDone;
        public bool ReportSeenByPlayer;

        public CollectedEvidenceData()
        {
        }

        public CollectedEvidenceData(string id, DateTime collected)
        {
            ID = id;
            TimeCollected = collected;
        }

        public bool CanEvidenceBeAnalyzed()
        {
            return DateTime.Now > TimeAnalysisDone;
        }

        public bool WasAnalysisRequested()
        {
            return TimeAnalysisDone != default(DateTime);
        }
    }
}
