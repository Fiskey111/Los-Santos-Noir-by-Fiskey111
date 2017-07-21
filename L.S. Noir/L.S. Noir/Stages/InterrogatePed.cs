//copy the interrogation from here:
//https://github.com/Fiskey111/Los-Santos-Noir-by-Fiskey111/blob/26da8e2882a5ae313fc2ef7b3d91e5841a4e5b76/L.S.%20Noir/L.S.%20Noir/Callouts/SA/Creators/Interrogation_Creator.cs

//https://github.com/Fiskey111/Los-Santos-Noir-by-Fiskey111/blob/master/L.S.%20Noir/L.S.%20Noir/Callouts/SA/Computer/main_form.Designer.cs

using LSNoir.Data;
using LSNoir.Resources;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
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

        private const string MSG_TALK = "Go closer to talk.";
        private const string MSG_LEAVE = "Leave the area.";
        private const string MSG_PRESS_TO_TALK = "Press ~y~{0}~s~ to start the interrogation.";
        private Keys KEY_START_INTERROGATION = Settings.Controls.KeyTalkToPed;

        //TODO: use serialized data!
        private static string[] scenarios =
        {
            "WORLD_HUMAN_PICNIC",
            "WORLD_HUMAN_PUSH_UPS",
            "WORLD_HUMAN_SMOKING",
            "WORLD_HUMAN_YOGA",
            "WORLD_HUMAN_DRINKING",
            "WORLD_HUMAN_GARDENER_LEAF_BLOWER",
            "WORLD_HUMAN_PARTYING",
            "WORLD_HUMAN_SIT_UPS",
            "WORLD_HUMAN_STAND_MOBILE"
        };

        private Vector3 callPos;
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

            Game.DisplayNotification(data.NotificationTexDic, data.NotificationTexName,
                                     data.NotificationTitle, data.NotificationSubtitle, data.NotificationText);

            ActivateStage(Away);

            return true;
        }

        private void Away()
        {
            if(DistToPlayer(data.CallPosition) < 150)
            {
                var witnessData = data.ParentCase.GetWitnessData(data.WitnessID[0]);
                ped = new Ped(witnessData.Model, witnessData.Spawn.Position, witnessData.Spawn.Heading);
                ped.MakePersistent();
                pedScenario = new PedScenarioLoop(ped, MathHelper.Choose(scenarios));

                var interrogationData = data.ParentCase.GetInterrogationData(witnessData.DialogID);
                interrogation = new Interrogation(interrogationData.Lines, ped);

                if (!string.IsNullOrEmpty(data.SceneID))
                {
                    scene = new Scenes.Scene(data.ParentCase.GetSceneData(data.SceneID));
                    scene.Create();
                }

                SwapStages(Away, NotifyToTalk);
            }
        }

        private void NotifyToTalk()
        {
            if(DistToPlayer(ped.Position) < 15)
            {
                Game.DisplayHelp(MSG_TALK, 3000);

                SwapStages(NotifyToTalk, NotifyPressToStartTalking);
            }
        }
        
        private void NotifyPressToStartTalking()
        {
            if(DistToPlayer(ped.Position) < 6)
            {
                Game.DisplayHelp(string.Format(MSG_PRESS_TO_TALK, KEY_START_INTERROGATION), 3000);
                pedScenario.IsActive = false;
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

            data.SaveNextScriptsToProgress(data.NextScripts[0]);
            data.SetThisAsLastStage();

            SetScriptFinished(true);

            DisplayMissionPassedScreen();
        }

        private void DisplayMissionPassedScreen()
        {
            //var value = interrogation.GoodAnswers / interrogation.Questions * 100;

            //var medal = RAGENativeUI.Elements.MissionPassedScreen.MedalType.Gold;

            //if (value >= 80 && value < 100) medal = RAGENativeUI.Elements.MissionPassedScreen.MedalType.Silver;
            //else if (value < 80) medal = RAGENativeUI.Elements.MissionPassedScreen.MedalType.Bronze;

            //var handler = new MissionPassedHandler("Victim Family", value, medal);

            //handler.AddItem("Spoke to Family", "", MissionPassedScreen.TickboxState.Tick);
            //var num = 0;

            //handler.Show();
        }

        protected override void End()
        {
            scene?.Dispose();
            if (ped) ped.Delete();
            if (blipCallArea) blipCallArea.Delete();
        }
    }
}
