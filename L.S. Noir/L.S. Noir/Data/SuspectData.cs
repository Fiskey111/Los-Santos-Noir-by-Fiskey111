using LtFlash.Common.EvidenceLibrary.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Data
{
    class SuspectData : IIdentifiable
    {
        public string ID { get; set; }
        public string Name;
        public string ReportID; //NOTE: how we know about his identity
        public string WarrantID;
        public string Description; //?
    }
}
