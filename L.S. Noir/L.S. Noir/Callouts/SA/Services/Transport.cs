using LtFlash.Common.EvidenceLibrary;
using Rage;

namespace LSNoir.Callouts.SA.Services
{
    public class Transport
    {
        private Vehicle policeCar;
        private Blip carBlip;
        private readonly string[] policeCarModels = new string[]
        {
            "POLICE",
            "POLICE2",
            "POLICE3",
        };
        private readonly SpawnPoint[] policeCarInitPositions = new SpawnPoint[]
        {
            new SpawnPoint(230.815689f, new Vector3(399.9877f,-1598.8855f,28.7242641f)),
        };
        private SpawnPoint policeCarSpawn;

        private Ped copDriver;
        private readonly string[] copModels = new string[]
        {
            "s_m_y_cop_01",
        };

        private Ped ped;
        private Vector3 pickupPos;

        private GameFiber process;
        private bool canRun = true;

        public Transport(
            Ped ped, Vector3 pickupPos,
            EPoliceStations dispatchFrom = EPoliceStations.Closest) :
            this(
                ped, pickupPos,

                dispatchFrom == EPoliceStations.Closest ?
                PoliceStations.GetClosestPoliceStationSpawn(pickupPos) :
                PoliceStations.GetPoliceStationSpawn(dispatchFrom))
        {
        }

        public Transport(Ped ped, Vector3 pickupPos, SpawnPoint dispatchFrom)
        {
            this.ped = ped;
            policeCarSpawn = dispatchFrom;
            this.pickupPos = pickupPos;
            process = new GameFiber(Process);
            process.Start();
        }

        private enum EState
        {
            CreateEntities,
            SetTaskDrive,
            WaitForArrival,
            SetPedTaskEnter, //if dist > x -> task goto pos,then enter
            WaitForPed,
            DriveAway,
            CheckIfCanBeDisposed,
            Dispose,
        }
        private EState _state = EState.CreateEntities;

        //TODO: cop get out the rmp -> goes to ped, talks to him -> they get back to veh and drive away
        private void Process()
        {
            while (canRun)
            {
                switch (_state)
                {
                    case EState.CreateEntities:

                        string _policeCarModel = policeCarModels[MathHelper.GetRandomInteger(policeCarModels.Length)];

                        policeCar = new Vehicle(_policeCarModel, policeCarSpawn.Position);
                        policeCar.Heading = policeCarSpawn.Heading;
                        carBlip = new Blip(policeCar);
                        carBlip.Scale = 0.5f;
                        carBlip.Color = System.Drawing.Color.Blue;
                        policeCar.IsInvincible = true;
                        policeCar.MakePersistent();

                        string _copModel = copModels[MathHelper.GetRandomInteger(copModels.Length)];
                        copDriver = new Ped(_copModel, policeCar.Position.Around(2f), 0f);
                        copDriver.WarpIntoVehicle(policeCar, -1);
                        copDriver.KeepTasks = true;
                        copDriver.BlockPermanentEvents = true;

                        _state = EState.SetTaskDrive;

                        break;
                    case EState.SetTaskDrive:

                        copDriver.Tasks.DriveToPosition(pickupPos, 10f, VehicleDrivingFlags.StopAtDestination | VehicleDrivingFlags.Emergency);

                        _state = EState.WaitForArrival;

                        break;
                    case EState.WaitForArrival:

                        if (policeCar.Position.DistanceTo(pickupPos) <= 5.5f)
                        {
                            _state = EState.SetPedTaskEnter;
                        }

                        break;
                    case EState.SetPedTaskEnter:

                        Game.LogVerbose("Transport.SetPedTaskEnter.Beginning");

                        ped.Tasks.ClearImmediately();

                        ped.Tasks.GoToOffsetFromEntity(policeCar, 3f, 90f, 1f);

                        while (ped.Position.DistanceTo(policeCar.Position) > 7f)
                        {
                            GameFiber.Yield();
                        }

                        ped.Tasks.ClearImmediately();
                        ped.Tasks.EnterVehicle(policeCar, 2);

                        Game.LogVerbose("Transport.SetPedTaskEnter.End");

                        _state = EState.WaitForPed;

                        break;
                    case EState.WaitForPed:

                        if (ped.IsInVehicle(policeCar, false))
                        {
                            Game.LogVerbose("Transport.WitnessInVehicle");
                            if (carBlip) carBlip.Delete();
                            _state = EState.DriveAway;
                        }

                        break;
                    case EState.DriveAway:

                        copDriver.Tasks.DriveToPosition(policeCarSpawn.Position, 10f, VehicleDrivingFlags.Normal);

                        _state = EState.CheckIfCanBeDisposed;
                        break;

                    case EState.CheckIfCanBeDisposed:

                        if (policeCar.Position.DistanceTo(Game.LocalPlayer.Character.Position) >= 200f)
                        {
                            _state = EState.Dispose;
                        }

                        break;

                    case EState.Dispose:

                        if (ped.Exists()) ped.Dismiss();
                        if (copDriver.Exists()) copDriver.Dismiss();
                        if (policeCar.Exists()) policeCar.Dismiss();
                        if (carBlip) carBlip.Delete();
                        canRun = false;

                        break;

                    default:
                        break;
                }

                GameFiber.Yield();
            }
        }
    }
}
