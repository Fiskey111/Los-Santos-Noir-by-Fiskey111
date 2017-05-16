using LtFlash.Common.EvidenceLibrary.Serialization;

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
}
