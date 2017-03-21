using LSNoir.Callouts.SA.Data;
using LtFlash.Common.Processes;
using Rage;
using Rage.Native;
using System.Windows.Forms;

namespace LSNoir.Callouts.SA.Services
{
    public abstract class ServiceBase : ICollectable
    {
        //PUBLIC
        public bool IsCollected { get; protected set; }
        public string MsgIsCollected { get; set; }
        public float DisposeDistance { get; set; } = 200.0f;
        public float VehicleDrivingSpeed { get; set; } = 10.0f;
        public VehicleDrivingFlags VehDrivingFlags { get; set; }
            = VehicleDrivingFlags.Emergency;

        public string ModelVehicle { get; set; }
        public string ModelPedDriver { get; set; }
        public string ModelPedWorker { get; set; }

        public Keys KeyStartDialogue { get; set; } = Keys.Y;
        public Ped PedDriver { get; private set; }
        public Ped PedWorker { get; private set; }
        public SpawnPoint SpawnPosition { get; set; }

        //PROTECTED
        protected Vehicle Vehicle { get; private set; }
        protected Vector3 PlayerPos => Game.LocalPlayer.Character.Position;
        public Dialog Dialogue { get; set; }

        //PRIVATE
        protected ProcessHost Proc { get; private set; } = new ProcessHost();
        private Blip _blipVeh;
        private SpawnPoint _destPoint;
        private Rage.Object _notepad;

        public ServiceBase(
            string vehModel, string modelPedDriver, string modelPedWorker,
            SpawnPoint spawnPos, SpawnPoint dest, Dialog dialogue)
        {
            ModelVehicle = vehModel;
            ModelPedWorker = modelPedWorker;
            ModelPedDriver = modelPedDriver;

            SpawnPosition = spawnPos;
            _destPoint = dest;

            Dialogue = dialogue;
        }

        public void Dispatch()
        {
            Proc.ActivateProcess(CreateEntities);
            Proc.Start();
        }

        protected void AttachNotepadToPedDriver()
        {
            _notepad = new Rage.Object("prop_notepad_01", PedDriver.Position);
            int boneId = PedDriver.GetBoneIndex(PedBoneId.LeftPhHand);
            NativeFunction.Natives.AttachEntityToEntity(
                _notepad, PedDriver, boneId,
                0f, 0f, 0f, 0f, 0f, 0f,
                true, false, false, false, 2, 1);
        }

        private void CreateEntities()
        {
            Vehicle = new Vehicle(ModelVehicle, SpawnPosition.Position);
            Vehicle.Heading = SpawnPosition.Heading;
            Vehicle.MakePersistent();
            _blipVeh = new Blip(Vehicle);
            _blipVeh.Scale = 0.5f;
            Vehicle.IsInvincible = true;

            PedDriver = new Ped(ModelPedDriver, Vehicle.Position.Around2D(5f), 0f);
            PedDriver.RandomizeVariation();
            PedDriver.WarpIntoVehicle(Vehicle, -1);
            PedDriver.BlockPermanentEvents = true;
            PedDriver.KeepTasks = true;

            PedWorker = new Ped(ModelPedWorker, Vehicle.Position.Around2D(5f), 0f);
            PedWorker.RandomizeVariation();
            PedWorker.WarpIntoVehicle(Vehicle, 0);
            PedWorker.BlockPermanentEvents = true;
            PedWorker.KeepTasks = true;

            PostSpawn();

            Proc.SwapProcesses(CreateEntities, DispatchFromSpawnPoint);
            Proc.ActivateProcess(AntiRollOver);
        }

        private void AntiRollOver()
        {
            if (!Vehicle.Exists()) Proc.DeactivateProcess(AntiRollOver);

            if (Vehicle.Rotation.Roll > 70f || Vehicle.Rotation.Roll < -70f)
            {
                Vehicle.SetRotationRoll(0f);
            }
        }

        protected abstract void PostSpawn();

        private void DispatchFromSpawnPoint()
        {
            PedDriver.Tasks.DriveToPosition(
                Vehicle, _destPoint.Position,
                VehicleDrivingSpeed, VehDrivingFlags, 5f);

            Proc.SwapProcesses(DispatchFromSpawnPoint, WaitForArrival);
        }

        private void WaitForArrival()
        {
            if (Vector3.Distance(Vehicle.Position, _destPoint.Position) <= 10f &&
                Vehicle.Speed == 0f)
            {
                Proc.SwapProcesses(WaitForArrival, PostArrival);
            }
        }

        protected abstract void PostArrival();

        protected void BackToVehicle()
        {
            IsCollected = true;

            DisplayMsgIsCollected();

            PedWorker.Tasks.GoToOffsetFromEntity(Vehicle, 0.1f, 0f, 1f);
            PedDriver.Tasks.GoToOffsetFromEntity(Vehicle, 0.1f, 0f, 1f);

            Proc.DeactivateProcess(BackToVehicle);
            Proc.ActivateProcess(CheckIfPedDriverCloseToVeh);
            Proc.ActivateProcess(CheckIfPedWorkerCloseToVeh);
        }

        private void DisplayMsgIsCollected()
        {
            if (MsgIsCollected != string.Empty) Game.DisplayHelp(MsgIsCollected);
        }

        private void CheckIfPedDriverCloseToVeh()
        {
            if (Vector3.Distance(PedDriver.Position, Vehicle.Position) <= 5f)
            {
                PedDriver.Tasks.EnterVehicle(Vehicle, -1);
                Proc.SwapProcesses(CheckIfPedDriverCloseToVeh, CheckIfPedsAreInVeh);
            }
        }

        private void CheckIfPedWorkerCloseToVeh()
        {
            if (Vector3.Distance(PedWorker.Position, Vehicle.Position) <= 5f)
            {
                PedWorker.Tasks.EnterVehicle(Vehicle, 0);
                Proc.SwapProcesses(CheckIfPedWorkerCloseToVeh, CheckIfPedsAreInVeh);
            }
        }

        private void CheckIfPedsAreInVeh()
        {
            if (PedDriver.IsInVehicle(Vehicle, false) &&
                PedWorker.IsInVehicle(Vehicle, false))
            {
                Proc.SwapProcesses(CheckIfPedsAreInVeh, DriveBackToSpawn);
            }
        }

        protected void DriveBackToSpawn()
        {
            if (_blipVeh.IsValid()) _blipVeh.Delete();

            PedDriver.Tasks.DriveToPosition(
                SpawnPosition.Position, VehicleDrivingSpeed, VehDrivingFlags);

            Proc.SwapProcesses(DriveBackToSpawn, CheckIfCanBeDisposed);
        }

        private void CheckIfCanBeDisposed()
        {
            if (Vector3.Distance(PlayerPos, Vehicle.Position) >= DisposeDistance ||
                Vector3.Distance(Vehicle.Position, SpawnPosition.Position) <= 10f)
            {
                Proc.DeactivateProcess(CheckIfCanBeDisposed);
                Proc.DeactivateProcess(AntiRollOver);
                InternalDispose();
            }
        }

        private void InternalDispose()
        {
            if (_blipVeh) _blipVeh.Delete();
            if (PedDriver) PedDriver.Delete();
            if (PedWorker) PedWorker.Delete();
            if (Vehicle) Vehicle.Delete();

            Dispose();
        }

        public abstract void Dispose();
    }
}
