namespace LSNoir.Callouts.SA.Services
{/*
    public class Ems : ServiceBase
    {
        private Ped _patient;
        private bool _takeToHospital;
        private Blip _blipEmt;

        public Ems(
            Ped patient,
            SpawnPoint dispTo,
            Dialog dialog,
            bool transportToHospital,
            bool spawnAtScene = false,
            EHospitals dispatchFrom = EHospitals.Closest)
            : base(
                  "AMBULANCE", "s_m_m_paramedic_01", "s_m_m_paramedic_01",
                  GetSpawn(spawnAtScene, dispatchFrom, dispTo),
                  dispTo, dialog)
        {
            this._patient = patient;
            _takeToHospital = transportToHospital;
        }

        public Ems(
            Ped patient,
            SpawnPoint dispTo,
            Dialog dialog,
            bool transportToHospital,
            bool spawnAtScene,
            SpawnPoint dispatchFrom)
            : base(
                  "AMBULANCE", "s_m_m_paramedic_01", "s_m_m_paramedic_01",
                  dispatchFrom,
                  dispTo, dialog)
        {
            this._patient = patient;
            _takeToHospital = transportToHospital;
        }

        private static SpawnPoint GetSpawn(
            bool spawnAtScene, EHospitals dispatchFrom, SpawnPoint dispatchTo)
        {
            return spawnAtScene ? dispatchTo :
                (dispatchFrom == EHospitals.Closest ?
                Hospitals.GetClosestHospitalSpawn(dispatchTo.Position) :
                Hospitals.GetHospitalSpawn(dispatchFrom));
        }

        protected override void PostSpawn()
        {
            Vehicle.IsSirenOn = true;
        }

        protected override void PostArrival()
        {
            Vehicle.IsSirenSilent = true;
            Proc.SwapProcesses(PostArrival, GoToPatient);
        }

        private void GoToPatient()
        {
            AttachNotepadToPedDriver();

            PedWorker.IsInvincible = true;
            PedDriver.IsInvincible = true;
            PedWorker.Tasks.GoToOffsetFromEntity(_patient, 1f, 0f, 5f);
            PedDriver.Tasks.GoToOffsetFromEntity(_patient, 4f, 8f, 5f);

            Proc.SwapProcesses(GoToPatient, CheckIfWithPatient);
        }

        private void CheckIfWithPatient()
        {
            if (PedWorker.Position.DistanceTo(_patient) < 2f &&
                PedDriver.Position.DistanceTo(_patient) < 5f)
            {
                Proc.SwapProcesses(CheckIfWithPatient, PerformProcedures);
            }
        }

        private void PerformProcedures()
        {
            Procedures();

            if (_takeToHospital) MovePatientToAmbulance(_patient);

            _blipEmt = new Blip(PedWorker)
            {
                Color = System.Drawing.Color.Green,
                Sprite = BlipSprite.Health,
                Scale = 0.25f
            };

            Game.DisplayHelp("Talk to EMS to receive a medical report.");
            Proc.SwapProcesses(PerformProcedures, WaitForDialogueActivation);
        }

        private void WaitForDialogueActivation()
        {
            if (!(Vector3.Distance(Game.LocalPlayer.Character.Position, PedWorker.Position) <= 3f)) return;
            Game.DisplayHelp($"Press ~y~{KeyStartDialogue}~s~ to talk to the paramedic.");

            if (!Game.IsKeyDown(KeyStartDialogue)) return;
            if (_blipEmt) _blipEmt.Delete();

            Dialogue.StartDialog();
            Proc.SwapProcesses(WaitForDialogueActivation, CheckForDialogueFinished);
        }

        private void CheckForDialogueFinished()
        {
            if (Dialogue.HasEnded)
                Proc.SwapProcesses(CheckForDialogueFinished, BackToVehicle);
        }

        private void MovePatientToAmbulance(Ped patient)
        {
            for (int i = 1; i < 51; i++)
            {
                if (patient) NativeFunction.Natives.SetEntityAlpha(patient, 255 - i * 5, false);
                GameFiber.Wait(10);
            }

            if (patient) patient.Delete();
        }

        public void Procedures()
        {
            bool emt = false, emtd = false;
            GameFiber.StartNew(delegate
            {
                PedWorker.Position = _patient.LeftPosition;
                GameFiber.Sleep(3000);

                NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(PedDriver, _patient, -1);
                GameFiber.Sleep(0500);

                PedDriver.Tasks.PlayAnimation("amb@medic@standing@timeofdeath@enter", "enter",
                    4, AnimationFlags.StayInEndFrame);
                GameFiber.Sleep(9000);

                PedDriver.Tasks.PlayAnimation("amb@medic@standing@timeofdeath@idle_a", "idle_b",
                    4, AnimationFlags.StayInEndFrame);
                GameFiber.Sleep(6000);

                PedDriver.Tasks.PlayAnimation("amb@medic@standing@timeofdeath@exit", "exit",
                    4, AnimationFlags.StayInEndFrame);
                GameFiber.Sleep(7000);

                emtd = true;
            });
            //====================
            GameFiber.StartNew(delegate
            {

                NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(PedWorker, _patient, -1);
                GameFiber.Sleep(0500);

                PedWorker.Tasks.PlayAnimation("amb@medic@standing@tendtodead@enter", "enter",
                    4, AnimationFlags.StayInEndFrame);
                GameFiber.Sleep(2000);

                PedWorker.Tasks.PlayAnimation("amb@medic@standing@tendtodead@idle_a", "idle_b",
                    4, AnimationFlags.StayInEndFrame);
                GameFiber.Sleep(4000);

                PedWorker.Tasks.PlayAnimation("amb@medic@standing@tendtodead@exit", "exit",
                    4, AnimationFlags.StayInEndFrame);
                GameFiber.Sleep(2000);

                PedWorker.Tasks.PlayAnimation("amb@code_human_police_investigate@idle_intro", "idle_intro",
                    4, AnimationFlags.StayInEndFrame);
                GameFiber.Sleep(1500);

                PedWorker.Tasks.PlayAnimation("amb@code_human_police_investigate@idle_b", "idle_d",
                    3, AnimationFlags.None);
                GameFiber.Sleep(9000);

                emt = true;
            });

            while (!emt && !emtd)
            {
                GameFiber.Yield();
            }
        }

        public override void Dispose()
        {
        }
    }

    public enum EHospitals
    {
        MountZonahMc,
        PillbohHillMc,
        CentralLsmc,
        //add St.Fiacre
        Closest,
    }*/
}
