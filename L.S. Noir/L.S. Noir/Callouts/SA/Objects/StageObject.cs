using Rage;
using System;

namespace StageObjects
{
    [Serializable]
    public class StageObject
    {
        public bool Exists { get; set; }
        public Vector3 Spawn { get; set; }
        public float Heading { get; set; }
        public Ped Ped { get; set; }
        public Vehicle Vehicle { get; set; }
        public Rage.Object Object { get; set; }
        public gender Gender { get; set; }
        public bool IsImportant { get; set; }
        public string[] Dialogue { get; set; }
        public enum gender { Male, Female, Neither }
        public Type ObjectType { get; set; }

        public StageObject() { }

        public StageObject(bool exists, Vector3 spawn, float heading)
        {
            Exists = exists;
            Spawn = spawn;
            Heading = heading;
        }

        public StageObject(Ped ped)
        {
            Ped = ped;
            if (ped.IsMale == true)
                Gender = gender.Male;
            else
                Gender = gender.Female;
        }

        public StageObject(string pedmodel, Vector3 spawn, float heading)
        {
            Ped = new Ped(pedmodel, spawn, heading);
            if (Ped.IsMale == true)
                Gender = gender.Male;
            else
                Gender = gender.Female;
            Spawn = spawn;
            Heading = heading;
            Exists = true;
        }

        public StageObject(Vehicle veh)
        {
            Vehicle = veh;
        }

        public StageObject(Rage.Object obj)
        {
            Object = obj;
        }

        public enum Type { Ped, Vehicle, Barrier }
    }
}
