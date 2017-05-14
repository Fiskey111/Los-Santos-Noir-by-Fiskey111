using LtFlash.Common;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Scenes
{
    public class SceneItem : IIdentifiable
    {
        public string ID { get; set; }
        public string Type; //Vehicle, Object, Ped
        public SpawnPoint Spawn;
        public string Model;

        public string AnimDictionary = null;
        public bool ShouldSerializeAnimDictionary() => !string.IsNullOrEmpty(AnimDictionary);

        public string AnimName = null;
        public bool ShouldSerializeAnimName() => !string.IsNullOrEmpty(AnimName);

        public string Scenario;

        public bool? IsSirenOn;
        public bool ShouldSerializeIsSirenOn() => IsSirenOn.HasValue;

        public bool? IsSirenSilent;
        public bool ShouldSerializeIsSirenSilent() => IsSirenSilent.HasValue;

        public string VehicleID;
        public int VehicleSeatID;

        public string Weapon;

        public SceneItem()
        {
        }
    }
}
