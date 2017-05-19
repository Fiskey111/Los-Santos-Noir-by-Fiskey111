using LSNoir.Data;
using LSNoir.Resources;
using Rage;
using System;

namespace LSNoir.Scenes
{
    class SceneBase
    {
        protected static Entity GenerateItem(SceneItem data)
        {
            Entity result = default(Entity);
            switch (data.Type)
            {
                case "Ped":
                    result = CreateItem(data, (m, p, h) => new Ped(m, p, h));
                    (result as Ped).RandomizeVariation();

                    if (!string.IsNullOrEmpty(data.Scenario))
                    {
                        var sh = new PedScenarioLoop(result as Ped, data.Scenario);
                        sh.IsActive = true;
                    }

                    if(!string.IsNullOrEmpty(data.AnimDictionary) && !string.IsNullOrEmpty(data.AnimName))
                    {
                        var ah = new PedAnimationLoop(result as Ped, data.AnimDictionary, data.AnimName);
                        ah.IsActive = true;
                    }

                    if(data.EquipWeapon)
                    {
                        (result as Ped).Inventory.GiveNewWeapon(data.Weapon, 100, false);
                    }
                        
                    break;

                case "Vehicle":
                    result = CreateItem<Vehicle>(data, (m, p, h) => new Vehicle(m, p, h));
                    break;

                case "Object":
                    result = CreateItem<Rage.Object>(data, (m, p, h) => new Rage.Object(m, p, h));
                    break;

                default:
                    var msg = $"{nameof(Scene)}.{nameof(GenerateItem)}: SceneItem type could not be recognized: {data.Type}, id: {data.ID}";
                    throw new ArgumentException(msg);
            }

            result.MakePersistent();
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
