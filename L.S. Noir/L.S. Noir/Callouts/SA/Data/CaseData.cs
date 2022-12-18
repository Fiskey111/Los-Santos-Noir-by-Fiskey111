using System;
using System.Collections.Generic;
using CaseManager.Resources;

namespace LSNoir.Callouts.SA.Data
{
    [Serializable]
    public class CaseData
    {
        public int Number { get;set; }
        public string CurrentStatus { get; set; }
        public int TotalSolvedCases { get; set; }
        public List<int> WitnessIDs { get; set; }
        public List<LastStage> CompletedStages { get; set; }
        public LastStage LastCompletedStage { get; set; }
        public LastStage CurrentStage { get; set; }
        public StageHelper Helper { get; set; }
        public List<string> SajrsUpdates { get; set; }
        public DateTime CrimeTime { get; set; }
        public bool EvidenceTested { get; set; }
        public bool WarrantRequested { get; set; }
        public bool WarrantAccess { get; set; }
        public bool WarrantSubmitted { get; set; }
        public bool WarrantApproved { get; set; }
        public bool WarrantHeard { get; set; }
        public string WarrantReason { get; set; }
        public DateTime WarrantApprovedDate { get; set; }
        public bool ComputerAccess { get; set; }
        public SpawnPoint SecCamSpawn { get; set; }
        public SpawnPoint SusSpawnPoint { get; set; }
        public SpawnPoint SusTarget { get; set; }
        public string StartingStage { get; set; }
        public string CurrentSuspect { get; set; }

        public CaseData() { }

        public enum LastStage { CSI, Hospital, MedicalExaminer, Station, VictimFamily, SuspectHome, Wait, SuspectWork, None }

        public enum StageHelper { Dispatched, Accepted, OnScene, Fo, ExaminedBody, Ems, Coroner, Evidence, CaseDone, None }
    }
}
