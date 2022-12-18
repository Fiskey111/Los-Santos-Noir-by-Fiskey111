using System;
using CaseManager.NewData;
using CaseManager.Resources;
using LSPD_First_Response.Mod;
using Rage;
using Object = Rage.Object;

namespace LSNoir.Callouts.Stages
{
    public static class CaseManagerExtensions
    {
        public static bool SpawnItem(this SceneItem item, out Entity entity)
        {
            entity = null;

            var model = new Model(item.Model);
            var position = new Vector3(item.SpawnPosition.Position.X, item.SpawnPosition.Position.Y, item.SpawnPosition.Position.Z);
            var heading = item.SpawnPosition.Heading;
            var rotation = new Rotator(item.SpawnPosition.Rotation.Pitch, item.SpawnPosition.Rotation.Roll, item.SpawnPosition.Rotation.Yaw);
            
            switch (item.Type)
            {
                case SceneItem.EItemType.Ped:
                {
                    var ped = new Ped(model, position, heading);
                    if (rotation != Rotator.Zero) ped.Rotation = rotation;
                    if (!string.IsNullOrWhiteSpace(item.WeaponName)) ped.Inventory.GiveNewWeapon(new WeaponAsset(item.WeaponName), 100, true);
                    entity = ped;
                    break;
                }
                case SceneItem.EItemType.Vehicle:
                {
                    var veh = new Vehicle(model, position, heading);
                    if (rotation != Rotator.Zero) veh.Rotation = rotation;
                    veh.IsSirenOn = item.VehicleLightsOn;
                    veh.IsSirenSilent = item.VehicleLightsOn;
                    entity = veh;
                    break;
                }
                case SceneItem.EItemType.Object:
                {
                    var obj = new Object(model, position, heading);
                    if (rotation != Rotator.Zero) obj.Rotation = rotation;
                    obj.IsPositionFrozen = item.ObjectPositionFrozen;
                    entity = obj;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return entity;
        }
    }
}