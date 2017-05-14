using LtFlash.Common.EvidenceLibrary.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Data
{
    public class PieceOfEvidence : IIdentifiable
    {
        public string ID { get; set; }
        public DateTime TimeCollected;
        public DateTime TimeRequestedAnalysis;

        public PieceOfEvidence()
        {
        }

        public PieceOfEvidence(string id, DateTime collected)
        {
            ID = id;
            TimeCollected = collected;
        }
    }
}
