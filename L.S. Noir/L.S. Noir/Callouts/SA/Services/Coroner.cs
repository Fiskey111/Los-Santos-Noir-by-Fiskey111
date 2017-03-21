using LSNoir.Callouts.SA.Data;
using Rage;
using Rage.Native;

namespace LSNoir.Callouts.SA.Services
{
    public class Coroner : ServiceBase
    {
        private Ped _body;
        private Blip _blipEmt;

        private static SpawnPoint _coronersOffice
            = new SpawnPoint(
                270.346252f,
                new Vector3(218.361008f, -1381.16431f, 30.1247978f));

        public Coroner(
            Ped body,
            SpawnPoint dispatchTo,
            Dialog dialogue,
            bool spawnAtScene = false) :
            base(
                GetVehModel(),
                GetPedModel(),
                GetPedModel(),
                spawnAtScene ? dispatchTo : _coronersOffice,
                dispatchTo,
                dialogue)
        {
            this._body = body;
        }

        public Coroner(
            Ped body,
            SpawnPoint dispatchTo,
            SpawnPoint dispatchFrom,
            Dialog dialogue,
            bool spawnAtScene = false) :
            base(
                GetVehModel(),
                GetPedModel(),
                GetPedModel(),
                spawnAtScene ? dispatchTo : dispatchFrom,
                dispatchTo,
                dialogue)
        {
            this._body = body;
        }

        private static string GetPedModel()
        {
            string[] pedModels = new string[]
            {
                "s_m_m_paramedic_01",
            };
            return pedModels[MathHelper.GetRandomInteger(pedModels.Length - 1)];
        }

        private static string GetVehModel()
        {
            string[] meVan = new string[]
            {
                "burrito3",
                "youga",
            };
            return meVan[MathHelper.GetRandomInteger(meVan.Length - 1)];
        }

        protected override void PostSpawn()
        {
        }

        protected override void PostArrival()
        {
            Proc.SwapProcesses(PostArrival, GoToBody);
        }

        private void GoToBody()
        {
            AttachNotepadToPedDriver();

            PedWorker.IsInvincible = true;
            PedDriver.IsInvincible = true;
            PedWorker.Tasks.GoToOffsetFromEntity(_body, 1f, 0f, 1f);
            PedDriver.Tasks.GoToOffsetFromEntity(_body, 4f, 8f, 1f);

            Proc.SwapProcesses(GoToBody, CheckIfWithPatient);
        }

        private void CheckIfWithPatient()
        {
            if (PedWorker.Position.DistanceTo(_body) < 2f &&
                PedDriver.Position.DistanceTo(_body) < 5f)
            {
                Proc.SwapProcesses(CheckIfWithPatient, PerformProcedures);
            }
        }

        private void PerformProcedures()
        {
            Procedures(_body);

            MovePatientToAmbulance(_body);

            _blipEmt = new Blip(PedWorker);
            _blipEmt.Color = System.Drawing.Color.Green;
            _blipEmt.Sprite = BlipSprite.Health;
            _blipEmt.Scale = 0.25f;

            Game.DisplayHelp("Talk to the coroner to receive a report.");

            Proc.SwapProcesses(PerformProcedures, WaitForDialogueActivation);
        }

        private void WaitForDialogueActivation()
        {
            if (Vector3.Distance(Game.LocalPlayer.Character.Position, PedWorker.Position) <= 3f)
            {
                Game.DisplayHelp($"Press ~y~{KeyStartDialogue}~s~ to talk to the coroner.");

                if (Game.IsKeyDown(KeyStartDialogue))
                {
                    if (_blipEmt) _blipEmt.Delete();

                    Dialogue.StartDialog();
                    Proc.SwapProcesses(WaitForDialogueActivation, CheckForDialogueFinished);
                }
            }
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

        public void Procedures(Ped target)
        {
            bool emt = false, emtd = false;
            GameFiber.StartNew(delegate
            {
                PedWorker.Position = target.LeftPosition;
                GameFiber.Sleep(3000);

                NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(PedDriver, target, -1);
                PedDriver.Face(target);
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

                PedWorker.Face(target);
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
}
