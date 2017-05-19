using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.Serialization;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Data
{
    public class CaseProgress : IIdentifiable
    {
        public string ID { get; set; }
        public int CaseNo;
        public string CaseID;

        public string LastStageID;
        public string[] NextScripts;
        public bool Finished;

        public List<string> WitnessesInterviewed = new List<string>();
        public List<string> DialogsPassed = new List<string>();
        public List<string> ReportsReceived = new List<string>();
        public List<string> NotesMade = new List<string>();
        public List<string> StagesPassed = new List<string>();
        public List<CollectedEvidenceData> CollectedEvidence = new List<CollectedEvidenceData>();
        public List<DocumentRequestData> RequestedDocuments = new List<DocumentRequestData>();

        public CaseProgress()
        {
        }
    }
}
