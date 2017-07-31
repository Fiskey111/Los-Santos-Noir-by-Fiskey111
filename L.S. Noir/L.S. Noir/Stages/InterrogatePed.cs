﻿//copy the interrogation from here:
//https://github.com/Fiskey111/Los-Santos-Noir-by-Fiskey111/blob/26da8e2882a5ae313fc2ef7b3d91e5841a4e5b76/L.S.%20Noir/L.S.%20Noir/Callouts/SA/Creators/Interrogation_Creator.cs

//https://github.com/Fiskey111/Los-Santos-Noir-by-Fiskey111/blob/master/L.S.%20Noir/L.S.%20Noir/Callouts/SA/Computer/main_form.Designer.cs

using LSNoir.Data;
using LSNoir.Resources;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.EvidenceLibrary.Serialization;
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

    public class InterrogatePed : BasicScript
    {
        private readonly StageData data;
        private Ped Player => Game.LocalPlayer.Character;
        private float DistToPlayer(Vector3 p) => Vector3.Distance(Player.Position, p);
        private Interrogation interrogation;

        private Scenes.IScene scene;

        private Keys KEY_START_INTERROGATION = Settings.Controls.KeyTalkToPed;
        private const string MSG_TALK = "Go closer to talk.";
        private const string MSG_LEAVE = "Leave the area.";
        private const string MSG_PRESS_TO_TALK = "Press ~y~{0}~s~ to start the interrogation.";

        private const float DIST_NOTIFY_TALK = 15;
        private const float DIST_AWAY = 90;
        private const float DIST_CLOSE_START_TALK = 6.5f;

        private Vector3 callPos;
        private WitnessData witnessData;
        private Ped ped;
        private PedScenarioLoop pedScenario;
        private Blip blipCallArea;

        public InterrogatePed(StageData stageData)
        {
            data = stageData;
        }

        protected override bool Initialize()
        {
            callPos = data.CallPosition;

            blipCallArea = new Blip(callPos)
            {
                Sprite = data.CallBlipSprite,
                Color = Color.DarkOrange,
                Name = data.CallBlipName,
            };

            Base.SharedStageMethods.DisplayNotification(data);

            NativeFunction.Natives.FlashMinimapDisplay();

            ActivateStage(Away);

            return true;
        }

        private void Away()
        {
            if(DistToPlayer(data.CallPosition) < DIST_AWAY)
            {
                CreatePedToInterrogate();

                if (!string.IsNullOrEmpty(data.SceneID))
                {
                    scene = new Scenes.Scene(data.ParentCase.GetSceneData(data.SceneID));
                    scene.Create();
                }

                SwapStages(Away, NotifyToTalk);
            }
        }

        private void CreatePedToInterrogate()
        {
            witnessData = data.ParentCase.GetWitnessData(data.WitnessID[0]);

            ped = new Ped(witnessData.Model, witnessData.Spawn.Position, witnessData.Spawn.Heading);
            ped.MakePersistent();

            pedScenario = new PedScenarioLoop(ped, witnessData.Scenario);

            var interrogationData = data.ParentCase.GetInterrogationData(witnessData.DialogID);
            interrogation = new Interrogation(interrogationData.Lines, ped);
        }

        private void NotifyToTalk()
        {
            if (!ped) CreatePedToInterrogate();

            if(DistToPlayer(ped.Position) < DIST_NOTIFY_TALK)
            {
                Game.DisplayHelp(MSG_TALK, 3000);

                SwapStages(NotifyToTalk, NotifyPressToStartTalking);
            }
        }
        
        private void NotifyPressToStartTalking()
        {
            if(DistToPlayer(ped.Position) < DIST_CLOSE_START_TALK)
            {
                Game.DisplayHelp(string.Format(MSG_PRESS_TO_TALK, KEY_START_INTERROGATION), 3000);

                pedScenario.IsActive = false;

                interrogation.DistanceToStart = DIST_CLOSE_START_TALK + 0.5f;

                if (blipCallArea) blipCallArea.Delete();

                SwapStages(NotifyPressToStartTalking, CanStartTalking);
            }
        }

        private void CanStartTalking()
        {
            if(Game.IsKeyDown(KEY_START_INTERROGATION))
            {
                interrogation.StartDialog();

                ped.Face(Player);
                Player.Face(ped);

                SwapStages(CanStartTalking, IsFinished);
            }
        }

        private void IsFinished()
        {
            if(interrogation.HasEnded)
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

            //data.ParentCase.SaveWitnessDataToProgress(witnessData);

            data.SaveNextScriptsToProgress(data.NextScripts[0]);
            data.SetThisAsLastStage();

            SetScriptFinished(true);

            DisplayMissionPassedScreen();
        }

        private void DisplayMissionPassedScreen()
        {
            var percentGoodAnswers = (float)interrogation.GoodAnswers / (float)interrogation.Questions * 100;

            var medal = percentGoodAnswers > 85 ? MissionPassedScreen.MedalType.Gold :
                        percentGoodAnswers > 70 ? MissionPassedScreen.MedalType.Silver :
                                                  MissionPassedScreen.MedalType.Bronze;
            
            var handler = new MissionPassedScreen(data.Name, (int)percentGoodAnswers, medal);

            var item1 = new MissionPassedScreenItem("Person interrogated", "", MissionPassedScreenItem.TickboxState.Tick);

            handler.Items.Add(item1);

            var questions = new MissionPassedScreenItem($"Good answers/questions", $"{interrogation.GoodAnswers}/{interrogation.Questions}");

            handler.Items.Add(questions);

            handler.Show();
        }

        protected override void End()
        {
            scene?.Dispose();
            if (ped) ped.Delete();
            if (blipCallArea) blipCallArea.Delete();
        }
    }
}
