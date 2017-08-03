using LtFlash.Common.EvidenceLibrary.Serialization;
using System.Collections.Generic;

namespace LSNoir.Data
{
    public class CaseProgress : IIdentifiable
    {
        public string ID { get; set; }
        public int CaseNo;
        public string CaseID;

        public string LastStageID;
        public List<string> StagesPassed = new List<string>();

        public List<string> NextScripts = new List<string>();

        public bool Finished;

        public List<string> Victims = new List<string>();
        public List<string> Officers = new List<string>();
        public List<string> WitnessesInterviewed = new List<string>();

        public List<string> DialogsPassed = new List<string>();
        public List<string> InterrogationsPassed = new List<string>();

        public List<string> ReportsReceived = new List<string>();
        public List<string> NotesMade = new List<string>();

        public List<string> SuspectsArrested = new List<string>();
        public List<string> SuspectsKilled = new List<string>();

        public List<string> PersonsTalkedTo = new List<string>();

        public List<CollectedEvidenceData> CollectedEvidence = new List<CollectedEvidenceData>();
        public List<DocumentRequestData> RequestedDocuments = new List<DocumentRequestData>();

        public CaseProgress()
        {
        }
    }
}
