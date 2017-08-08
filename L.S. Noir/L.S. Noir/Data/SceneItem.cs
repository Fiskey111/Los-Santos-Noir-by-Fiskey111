using LtFlash.Common;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;

namespace LSNoir.Data
{
    public class SceneItem : IIdentifiable
    {
        public string ID { get; set; }
        public string Type; //Vehicle, Object, Ped
        public SpawnPoint Spawn;
        public Rotator Rotation;
        public bool ShouldSerializeRotation() => !Rotation.IsZero();

        public string Model;

        public SpawnPoint[] AccessoryPositions;

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

        public WeaponHash Weapon;
        public bool EquipWeapon;

        public SceneItem()
        {
        }
    }
}
