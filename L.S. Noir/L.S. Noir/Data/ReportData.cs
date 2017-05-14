using LtFlash.Common.EvidenceLibrary.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Data
{
    public class ReportData : IIdentifiable
    {
        public string ID { get; set; }
        public string Title;
        public string Text;
    }
}
