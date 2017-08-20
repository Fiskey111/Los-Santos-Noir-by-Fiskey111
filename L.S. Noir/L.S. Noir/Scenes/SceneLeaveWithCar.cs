using LSNoir.Data;
using Rage;
using System.Linq;

namespace LSNoir.Scenes
{
    class SceneLeaveWithCar : SceneBase, ISceneActiveWithVehicle
    {
        public Vehicle Veh { get; private set; }

        public bool HasFinished { get; private set; }

        private SceneData data;
        private Ped psgr;
        private Ped external;

        public SceneLeaveWithCar(SceneData sceneData)
        {
            data = sceneData;
        }

        public void Create()
        {
            var vehData = data.Items.FirstOrDefault(i => i.ID == "vehicle");
            Veh = CreateItem(vehData, (m, p, h) => new Vehicle(m, p, h));
            Veh.MakePersistent();

            var psgrData = data.Items.FirstOrDefault(i => i.ID == "ped");
            psgr = CreateItem(psgrData, (m, p, h) => new Ped(m, p, h));
            psgr.WarpIntoVehicle(Veh, psgrData.VehicleSeatID);
            psgr.MakePersistent();
            psgr.BlockPermanentEvents = true;
        }

        public void EnterVehicle(Ped p)
        {
            var seatID = data.Items.FirstOrDefault(i => i.ID == "externalPed").VehicleSeatID;
            p.Tasks.EnterVehicle(Veh, seatID);
            external = p;
        }

        public void Start()
        {
            var driver = Veh.GetPedOnSeat(-1);
            driver.Tasks.CruiseWithVehicle(15f);
        }

        public void Dispose()
        {
            if (psgr) psgr.IsPersistent = false;
            if (Veh) Veh.IsPersistent = false;
            if (external) external.IsPersistent = false;
        }
    }
}
