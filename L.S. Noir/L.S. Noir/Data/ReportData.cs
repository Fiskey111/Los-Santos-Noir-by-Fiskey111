using LtFlash.Common.EvidenceLibrary.Serialization;

namespace LSNoir.Data
{
    public class ReportData : IIdentifiable
    {
        public string ID { get; set; }
        public string Title;
        public string Text;
    }
}
