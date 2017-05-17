using LtFlash.Common.EvidenceLibrary.Serialization;

namespace LSNoir.Data
{
    public class NoteData : IIdentifiable
    {
        public string ID { get; set; }
        public string Title;
        public string Text;
    }
}
