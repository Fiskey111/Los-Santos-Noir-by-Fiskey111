using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSNoir.Resources;
using LtFlash.Common;

namespace LSNoir.Data.NewData
{
    public class EntityData : IData
    {
        // IData
        public string ID { get; set; }
        public string Name { get; set; }

        // String
        public string Description { get; set; }
        public string Scenario { get; set; }
        public string Model { get; set; }
        public string WeaponName { get; set; } // Suspect
        public string ReportID { get; set; } // EMS + Coroner

        // Spawnpoint
        public SpawnPoint Spawn { get; set; }
        public SpawnPoint WitnessPickupPosition { get; set; } // Witness

        // Dialogue
        public List<DialogueLine> Dialogue { get; set; }

        // Float
        public float SuspectChanceResisting { get; set; } // Suspect

        // Boolean
        public bool WitnessIsCompliant { get; set; } // Wit
        public bool FirstOfficerIsImportant { get; set; } // FO
        public bool FirstOfficerCanBeInspected { get; set; } // FO
        public bool ServicesSpawnAtScene { get; set; } // EMS + Coroner
        public bool ParamedicsTransportToHospital { get; set; } // EMS
        public bool IsCollected { get; set; } // EMS + Coroner
        
        //Enums
        public enum EntityDataType { FirstOfficer, Witness, Suspect, Interrogation, EMS, Coroner }
        public EntityDataType Type { get; set; }
    }
}
