using LSNoir.Data;
using LSNoir.Resources;
using Rage;
using System;

namespace LSNoir.Scenes
{
    class SceneBase
    {
        public static Entity GenerateItem(SceneItem data)
        {
            Entity result = default(Entity);

            switch (data.Type)
            {
                case "Ped":
                    result = GeneratePed(data);
                    break;

                case "Vehicle":
                    result = GenerateVehicle(data);
                    break;

                case "Object":
                    result = CreateItem(data, (m, p, h) => new Rage.Object(m, p, h));
                    break;

                case "Camera":
                    result = GenerateCamera(data);
                    break;

                default:
                    var msg = $"{nameof(Scene)}.{nameof(GenerateItem)}: SceneItem type could not be recognized: {data.Type}, id: {data.ID}";
                    throw new ArgumentException(msg);
            }

            if(!(result is Camera)) result.MakePersistent();

            return result;
        }

        private static Entity GenerateCamera(SceneItem data)
        {
            Camera result;
            result = new Camera(false);
            result.Position = data.Spawn.Position;
            if(!data.Rotation.IsZero()) result.Rotation = data.Rotation;
            //result.MakePersistent();
            return result;
        }

        private static Entity GeneratePed(SceneItem data)
        {
            Ped result = CreateItem(data, (m, p, h) => new Ped(m, p, h));

            result.RandomizeVariation();

            if (!string.IsNullOrEmpty(data.Scenario))
            {
                var sh = new PedScenarioLoop(result, data.Scenario);
                sh.IsActive = true;
            }

            if (!string.IsNullOrEmpty(data.AnimDictionary) && !string.IsNullOrEmpty(data.AnimName))
            {
                var ah = new PedAnimationLoop(result, data.AnimDictionary, data.AnimName);
                ah.IsActive = true;
            }

            if (data.EquipWeapon)
            {
                result.Inventory.GiveNewWeapon(data.Weapon, 100, false);
            }

            return result;
        }

        private static Entity GenerateVehicle(SceneItem data)
        {
            Vehicle result = CreateItem(data, (m, p, h) => new Vehicle(m, p, h));

            if (data.IsSirenOn.GetValueOrDefault(false) && result.Model.IsEmergencyVehicle)
            {
                result.IsEngineOn = true;
                result.IsSirenOn = true;

                if (data.IsSirenSilent.GetValueOrDefault(false))
                {
                    result.IsSirenSilent = true;
                }
            }

            return result;
        }

        protected static T CreateItem<T>(SceneItem d, Func<string, Vector3, float, T> ctor) where T : Entity
        {
            var e = ctor(d.Model, d.Spawn.Position, d.Spawn.Heading);
            e.MakePersistent();
            return e;
        }
    }
}
