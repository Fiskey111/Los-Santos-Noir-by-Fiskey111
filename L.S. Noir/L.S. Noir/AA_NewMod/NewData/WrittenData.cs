using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace LSNoir.Data.NewData
{
    public class WrittenData : IData
    {
        // IData
        public string ID { get; set; }
        public string Name { get; set; }

        // String
        public string To { get; set; }
        public string Text { get; set; }
        
        // String List
        public List<string> EvidenceIDRequiredToRequest { get; set; }
        public List<string> DialogueIDRequiredToRequest { get; set; }
        public List<string> ReportIDRequiredToRequest { get; set; }
        public List<string> StageIDRequiredToRequest { get; set; }
        public List<string> EvidenceIDRequiredToAccept { get; set; }
        public List<string> DialogueIDRequiredToAccept { get; set; }
        public List<string> ReportIDRequiredToAccept { get; set; }
        public List<string> StageIDRequiredToAccept { get; set; }

        // Enum
        public enum WrittenDataType { Document, Notes, Reports }
        public WrittenDataType Type { get; set; }
    }
}
