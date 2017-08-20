﻿using LSNoir.Data;
using LSNoir.Resources;
using LSNoir.Scenes;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.EvidenceLibrary;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;
using System.Windows.Forms;

namespace LSNoir.Stages
{
    //TECHNICAL REQUIREMENTS:
    // - StageData: x,
    //              CallPos defines the call area,
    // - WitnessData: defines ped; Model, Spawn, DialogID?

    public class DialogWithPedLeavingWithVehicle : BasicScript
    {
        private readonly StageData data;
        private Ped Player => Game.LocalPlayer.Character;
        private float DistToPlayer(Vector3 p) => Vector3.Distance(Player.Position, p);
        private Dialog dialog;
        private string dialogID;

        private const string MSG_TALK = "Go closer to talk.";
        private const string MSG_LEAVE = "Leave the area.";
        private const string MSG_PRESS_TO_TALK = "Press ~y~{0}~s~ to start talking.";

        private Keys KEY_START_INTERROGATION = Settings.Controls.KeyTalkToPed;

        private Ped ped;
        private string personID;
        private Blip blipCallArea;

        private ISceneActiveWithVehicle scene;

        public DialogWithPedLeavingWithVehicle(StageData stageData)
        {
            data = stageData;
        }

        protected override bool Initialize()
        {
            blipCallArea = Base.SharedStageMethods.CreateBlip(data);

            scene = Base.SharedStageMethods.GetScene(data) as ISceneActiveWithVehicle;

            Base.SharedStageMethods.DisplayNotification(data);

            NativeFunction.Natives.FlashMinimapDisplay();

            ActivateStage(Away);

            return true;
        }

        private void Away()
        {
            if (DistToPlayer(data.CallPosition) < 150)
            {
                scene?.Create();

                CreatePed();

                SwapStages(Away, NotifyToTalk);
            }
        }

        private void CreatePed()
        {
            var personData = data.ParentCase.GetPersonData(data.PersonsID[0]);

            personID = personData.ID;

            ped = new Ped(personData.Model, personData.Spawn.Position, personData.Spawn.Heading);
            ped.MakePersistent();

            if(!string.IsNullOrEmpty(personData.Scenario))
            {
                NativeFunction.Natives.TASK_START_SCENARIO_IN_PLACE(ped, personData.Scenario, 0, true);
            }

            var dialogData = data.ParentCase.GetDialogData(personData.DialogID);
            dialogID = dialogData.ID;
            dialog = new Dialog(dialogData.Dialog);
        }

        private void NotifyToTalk()
        {
            if (!ped) CreatePed();

            if (DistToPlayer(ped.Position) < 15)
            {
                Game.DisplayHelp(MSG_TALK, 3000);

                SwapStages(NotifyToTalk, NotifyPressToStartTalking);
            }
        }

        private void NotifyPressToStartTalking()
        {
            if (DistToPlayer(ped.Position) < 6)
            {
                Game.DisplayHelp(string.Format(MSG_PRESS_TO_TALK, KEY_START_INTERROGATION), 3000);

                if (blipCallArea) blipCallArea.Delete();

                SwapStages(NotifyPressToStartTalking, CanStartTalking);
            }
        }

        private void CanStartTalking()
        {
            if (Game.IsKeyDown(KEY_START_INTERROGATION))
            {
                dialog.StartDialog();

                NativeFunction.Natives.TaskTurnPedToFaceEntity(Player, ped, 3000);
                NativeFunction.Natives.TaskTurnPedToFaceEntity(ped, Player, 3000);

                SwapStages(CanStartTalking, IsFinished);
            }
        }

        private void IsFinished()
        {
            if (dialog.HasEnded)
            {
                DeactivateStage(IsFinished);

                ActivateStage(HasLeft);
                ActivateStage(PedEntersVeh);
            }
        }

        private void HasLeft()
        {
            Game.DisplaySubtitle(MSG_LEAVE, 100);

            if (DistToPlayer(data.CallPosition) > data.CallAreaRadius)
            {
                SetScriptFinishedSuccessfulyAndSave();
            }
        }

        private void PedEntersVeh()
        {
            scene.EnterVehicle(ped);

            SwapStages(PedEntersVeh, IsInVehicle);
        }

        private void IsInVehicle()
        {
            if(ped.IsInAnyVehicle(false))
            {
                scene.Start();

                DeactivateStage(IsInVehicle);
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
            if (ped) ped.Delete();
            if (blipCallArea) blipCallArea.Delete();
            scene?.Dispose();
        }
    }
}