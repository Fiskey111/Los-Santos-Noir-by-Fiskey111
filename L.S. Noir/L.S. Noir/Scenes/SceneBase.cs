using LSNoir.Resources;
using Rage;
using System;

namespace LSNoir.Scenes
{
    class SceneBase
    {
        protected static Entity GenerateItem(SceneItem t)
        {
            Entity result = default(Entity);
            switch (t.Type)
            {
                case "Ped":
                    result = CreateItem(t, (m, p, h) => new Ped(m, p, h));
                    (result as Ped).RandomizeVariation();

                    if (!string.IsNullOrEmpty(t.Scenario))
                    {
                        var sh = new PedScenarioLoop(result as Ped, t.Scenario);
                        sh.IsActive = true;
                    }

                    if(!string.IsNullOrEmpty(t.AnimDictionary) && !string.IsNullOrEmpty(t.AnimName))
                    {
                        var ah = new PedAnimationLoop(result as Ped, t.AnimDictionary, t.AnimName);
                        ah.IsActive = true;
                    }
                        
                    break;
                case "Vehicle":
                    result = CreateItem<Vehicle>(t, (m, p, h) => new Vehicle(m, p, h));
                    break;
                case "Object":
                    result = CreateItem<Rage.Object>(t, (m, p, h) => new Rage.Object(m, p, h));
                    break;
                default:
                    var msg = $"{nameof(Scene)}.{nameof(GenerateItem)}: SceneItem type could not be recognized: {t.Type}, id: {t.ID}";
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
