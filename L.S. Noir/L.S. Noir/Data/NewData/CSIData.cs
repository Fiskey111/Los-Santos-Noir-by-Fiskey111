using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LtFlash.Common;

namespace LSNoir.Data.NewData
{
    public class CSIData : IData
    {
        // IData
        public string ID { get; set; }
        public string Name { get; set; }

        // String
        public string Description { get; set; }
        public string Model { get; set; }
        public string Traces { get; set; }
        public string DeadBodyObjectAdditionalTextWhileInspecting { get; set; } // DeadBody + Object

        // Int
        public int[] WitnessIDs { get; set; }
        public int[] ServiceIDs { get; set; }

        // Spawnpoint
        public SpawnPoint Spawn { get; set; }
        public SpawnPoint WitnessPickupPosition { get; set; } // Witness

        // Boolean
        public bool IsImportant { get; set; }
        public bool CanBeInspected { get; set; }
        public bool PlaySoundImportantNearby { get; set; }
        public bool PlaySoundImportantCollected { get; set; }
        
        // Enum
        public enum CSIDataType { DeadBody, Witness, Object }
        public CSIDataType Type { get; set; }
    }
}
