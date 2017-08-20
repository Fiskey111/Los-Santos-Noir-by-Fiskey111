using LSNoir.Data;
using LtFlash.Common.Processes;
using Rage;
using Rage.Native;
using System.Linq;

namespace LSNoir.Scenes
{
    class SceneLeaveWithHeli : SceneBase, ISceneActiveWithVehicle
    {
        public bool HasFinished { get; private set; }

        private SceneData data;

        public Vehicle Veh { get; private set; }

        private Ped driver;
        private ProcessHost proc = new ProcessHost();

        public SceneLeaveWithHeli(SceneData sceneData)
        {
            data = sceneData;
        }

        public void Create()
        {
            Veh = SpawnVehicle();

            driver = SpawnDriver(Veh);

            proc[SpawnIfNotExists] = true;
            proc.Start();
        }

        private Vehicle SpawnVehicle()
        {

            var vehData = data.Items.FirstOrDefault(i => i.ID == "vehicle");
            World.GetEntities(vehData.Spawn.Position, 5f, GetEntitiesFlags.ConsiderAllVehicles | GetEntitiesFlags.ConsiderAllPeds).ToList().ForEach(e => e.Delete());
            var v = CreateItem(vehData, (m, p, h) => new Vehicle(m, p, h));
            v.MakePersistent();
            return v;
        }

        private Ped SpawnDriver(Vehicle v)
        {
            var driverData = data.Items.FirstOrDefault(i => i.ID == "driver");
            var d = CreateItem(driverData, (m, p, h) => new Ped(m, p, h));
            d.WarpIntoVehicle(v, -1);
            d.MakePersistent();
            d.BlockPermanentEvents = true;
            d.KeepTasks = true;
            return d;
        }

        public void EnterVehicle(Ped p)
        {
            var seatID = data.Items.FirstOrDefault(i => i.ID == "player").VehicleSeatID;
            p.Tasks.EnterVehicle(Veh, seatID);
        }

        public void Start()
        {
            var pos = driver.GetOffsetPosition(new Vector3(500, 0, 100));
            //driver.Tasks.DriveToPosition(pos, 14f, VehicleDrivingFlags.Normal);

            //TASK_HELI_MISSION(Ped pilot, Vehicle vehicle, Vehicle vehicleToFollow, Ped pedToFollow, float posX, float posY, float posZ,
            //int mode, float speed, float radius, float angle, int p11,
            //int height, float p13, int p14)

            NativeFunction.Natives.TASK_HELI_MISSION(driver, Veh, 0, 0, pos.X, pos.Y, pos.Z, 4, 10f, -1.0f, -1.0f, 10, 10, 5.0f, 0);
            proc[SpawnIfNotExists] = false;
            proc[IsDone] = true;
        }

        private void SpawnIfNotExists()
        {
            if(!Veh)
            {
                Veh = SpawnVehicle();
                driver = SpawnDriver(Veh);
            }
        }

        private void IsDone()
        {
            if (Stages.Base.StageCalloutScript.DistToPlayer(Veh) > 150)
            {
                HasFinished = true;
                proc[IsDone] = false;
            }
        }

        public void Dispose()
        {
            if (Veh) Veh.Delete();
            if (driver) driver.Delete();
            proc.Stop();
        }
    }
}
