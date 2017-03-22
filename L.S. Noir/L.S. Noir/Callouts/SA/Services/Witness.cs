using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.Universal;

namespace LSNoir.Callouts.SA.Services
{
    using Rage;
    using LSPD_First_Response.Mod.API;

    namespace LtFlash.Common.EvidenceLibrary.Evidence
    {
        public class Witness : EvidencePed
        {
            //PUBLIC
            public bool IsArrested { get; protected set; }
            public bool IsCompliant { get; set; }
            public DialogLine[] DialogRefuseTransportToStation { get; set; }

            public override string TextInteractWithEvidence
                => $"Press ~y~{KeyInteract} ~s~to talk to the witness.";

            protected override string TextWhileInspecting
                => $"Press ~y~{KeyInteract} ~s~to release the witness.~n~Press ~y~{KeyLeave} ~s~to tell the witness to stay at scene.~n~Press ~y~{KeyCollect} ~s~to transport the witness to the station.";

            public Dialog Dialog { get; set; }

            //PRIVATE
            private Services.Transport transport;
            private Vector3 pickupPos;
            private DialogLine[] dialogRefuseTransport = new DialogLine[]
            {
                new DialogLine(0, "I'm not going to deal with this; I'll be dead within 24 hours!") 
            };

            public Witness(
                string id, string description,
                SpawnPoint spawn, Model model,
                Dialog dialog, Vector3 pickupPos)
                : base(id, description, spawn, model)
            {
                this.Dialog = dialog;
                this.pickupPos = pickupPos;

                DialogRefuseTransportToStation = dialogRefuseTransport;
            }

            private enum EState
            {
                InitDialog,
                CheckIfDialogFinished,
                WaitForFurtherInstructions, // stay at this stage; player can go away and get back to see the same set of instructions
            }
            private EState _state = EState.InitDialog;

            protected override void Process()
            {
                if (!Ped) Dismiss();

                if (Functions.IsPedArrested(Ped))
                {
                    IsArrested = true;
                    //Dismiss(); //TODO: test if doesn't 'cancel' the arrest state
                }

                switch (_state)
                {
                    case EState.InitDialog:

                        if (IsCollected) return;

                        Dialog.StartDialog();
                        Checked = true;
                        _state = EState.CheckIfDialogFinished;

                        break;
                    case EState.CheckIfDialogFinished:
                        if (Dialog.HasEnded)
                        {
                            IsCollected = true;
                            _state = EState.WaitForFurtherInstructions;
                        }
                        break;
                    case EState.WaitForFurtherInstructions:

                        WaitForFurtherInstruction();

                        break;
                    default:
                        break;
                }
            }

            protected virtual void WaitForFurtherInstruction()
            {
                if (!IsPlayerClose) return;

                Game.DisplayHelp(GetTextWhileInspectingWithAdditional());

                //release -> done
                //tell to stay -> set Checked to true and set state to WaitFor... in the next contact?

                if (Game.IsKeyDown(KeyInteract))
                {
                    SetEvidenceCollected();

                    Ped.Tasks.Wander();
                }
                else if (Game.IsKeyDown(KeyLeave))
                {
                    _state = EState.CheckIfDialogFinished; //prevent from reading _keyInteract 2x -> releasing the suspect
                    SwapStages(Process, AwayOrClose);
                }
                else if (Game.IsKeyDown(KeyCollect))
                {
                    if (IsCompliant)
                    {
                        SetEvidenceCollected();
                        transport = new Services.Transport(Ped, pickupPos);
                    }
                    else
                    {
                        Dialog refuseBeingTransported = new Dialog(DialogRefuseTransportToStation, Ped.Position);
                        refuseBeingTransported.StartDialog();
                        while (!refuseBeingTransported.HasEnded)
                        {
                            GameFiber.Yield();
                        }
                    }
                }
            }

            protected override void DisplayInfoEvidenceCollected()
            {

            }

            protected override void End()
            {
                //if(Ped.Exists()) Ped.Dismiss();
            }
        }
    }
}
