﻿//copy the interrogation from here:
//https://github.com/Fiskey111/Los-Santos-Noir-by-Fiskey111/blob/26da8e2882a5ae313fc2ef7b3d91e5841a4e5b76/L.S.%20Noir/L.S.%20Noir/Callouts/SA/Creators/Interrogation_Creator.cs

//https://github.com/Fiskey111/Los-Santos-Noir-by-Fiskey111/blob/master/L.S.%20Noir/L.S.%20Noir/Callouts/SA/Computer/main_form.Designer.cs

using LSNoir.Data;
using LSNoir.Resources;
using LSNoir.Scenes;
using LSNoir.Settings;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.EvidenceLibrary;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;
using System.Drawing;
using System.Windows.Forms;

namespace LSNoir.Stages
{
    //TECHNICAL REQUIREMENTS:
    // - StageData: x,
    //              CallPos defines the call area,
    // - WitnessData: defines ped; Model, Spawn, DialogID?

    public class DialogWithPed : BasicScript
    {
        private readonly StageData data;
        private Ped Player => Game.LocalPlayer.Character;
        private float DistToPlayer(Vector3 p) => Vector3.Distance(Player.Position, p);
        private Dialog dialog;
        private string dialogID;

        private const string MSG_TALK = "Go closer to talk.";
        private const string MSG_LEAVE = "Leave the area.";
        private const string MSG_PRESS_TO_TALK = "Press ~y~{0}~s~ to start talking.";

        private const string PED = "dialog_ped";

        //private Keys KEY_START_INTERROGATION = Settings.Controls.KeyTalkToPed;
        private ControlSet CONTROL_START_INTERROGATION = Main.Controls.TalkToPed;

        private Vector3 callPos;
        private Ped ped;
        private string personID;
        private PedScenarioLoop pedScenario;
        private Blip blipCallArea;

        private RouteAdvisor ra;

        private IScene scene;

        public DialogWithPed(StageData stageData)
        {
            data = stageData;
        }

        protected override bool Initialize()
        {
            callPos = data.CallPosition;

            blipCallArea = Base.SharedStageMethods.CreateBlip(data);

            scene = Base.SharedStageMethods.GetScene(data);

            data.CallNotification.DisplayNotification();

            NativeFunction.Natives.FlashMinimapDisplay();

            ra = new RouteAdvisor(data.CallPosition);

            ra.Start(false, true);

            ActivateStage(Away);

            return true;
        }

        private void Away()
        {
            if (DistToPlayer(data.CallPosition) < 80)
            {
                scene?.Create();

                CreatePed();

                SwapStages(Away, NotifyToTalk);
            }

        }

        private void CreatePed()
        {
            var personData = data.GetPersonData(PED);

            personID = personData.ID;

            ped = new Ped(personData.Model, personData.Spawn.Position, personData.Spawn.Heading);
            ped.MakePersistent();

            pedScenario = new PedScenarioLoop(ped, personData.Scenario);

            var dialogData = data.ParentCase.GetDialogData(personData.DialogID);
            dialogID = dialogData.ID;
            dialog = new Dialog(dialogData.Dialog);
        }

        private void NotifyToTalk()
        {
            if (!ped) CreatePed();

            if(DistToPlayer(ped.Position) < 15)
            {
                Game.DisplayHelp(MSG_TALK, 3000);

                ra.Stop();

                SwapStages(NotifyToTalk, NotifyPressToStartTalking);
            }
        }
        
        private void NotifyPressToStartTalking()
        {
            if(DistToPlayer(ped.Position) < 6)
            {
                Game.DisplayHelp(string.Format(MSG_PRESS_TO_TALK, CONTROL_START_INTERROGATION.GetDescription()), 3000);

                pedScenario.IsActive = false;

                if (blipCallArea) blipCallArea.Delete();

                SwapStages(NotifyPressToStartTalking, CanStartTalking);
            }
        }

        private void CanStartTalking()
        {
            if(CONTROL_START_INTERROGATION.IsActive())
            {
                dialog.StartDialog();

                NativeFunction.Natives.TaskTurnPedToFaceEntity(Player, ped, 3000);
                NativeFunction.Natives.TaskTurnPedToFaceEntity(ped, Player, 3000);

                SwapStages(CanStartTalking, IsFinished);
            }
        }

        private void IsFinished()
        {
            if(dialog.HasEnded)
            {
                Game.DisplaySubtitle(MSG_LEAVE, 100);

                if(DistToPlayer(callPos) > data.CallAreaRadius)
                {
                    SetScriptFinishedSuccessfulyAndSave();
                }
            }
        }

        protected override void Process()
        {
        }

        protected void SetScriptFinishedSuccessfulyAndSave()
        {
            Functions.PlayScannerAudio("ATTN_DISPATCH CODE_04_PATROL");

            data.ParentCase.Progress.AddDialogsToProgress(dialogID);

            data.ParentCase.Progress.AddPersonsTalkedTo(personID);

            data.ParentCase.Progress.SetNextScripts(data.NextScripts[0]);

            data.ParentCase.Progress.SetLastStage(data.ID);

            SetScriptFinished(true);

            DisplayMissionPassedScreen();
        }

        private void DisplayMissionPassedScreen()
        {
            var handler = new MissionPassedScreen(data.Name, 100, MissionPassedScreen.MedalType.Gold);

            var item = new MissionPassedScreenItem("Spoke to person", "", MissionPassedScreenItem.TickboxState.Tick);

            handler.Items.Add(item);

            handler.Show();
        }

        protected override void End()
        {
            ra?.Stop();
            if (ped) ped.Delete();
            if (blipCallArea) blipCallArea.Delete();
            scene?.Dispose();
        }
    }
}
