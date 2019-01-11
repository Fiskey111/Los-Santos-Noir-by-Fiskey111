using LSNoir.Data;
using LSNoir.Resources;
using LSNoir.Scenes;
using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LSNoir.Stages
{
    //https://github.com/Fiskey111/Los-Santos-Noir-by-Fiskey111/blob/master/L.S.%20Noir/L.S.%20Noir/Callouts/SA/Stages/Sa_4ASuspectHome.cs

    public class InterrogatePedWithTimeout : BasicScript
    {
        //TODO:
        // - add RNUI TimerBar and display time left while leaving?

        //TECHNICAL REQUIREMENTS:
        // - StageData: 1 WitnessID, CallPosition, CallBlipSprite, CallBlipName, Notification data,
        // - WitnessData: Model, Spawn, DialogID (interrogation), Scenario, accessory: NoteID
        // - InterrogationData

        private readonly StageData data;

        private Ped Player => Game.LocalPlayer.Character;
        private float DistToPlayer(Vector3 p) => Vector3.Distance(Player.Position, p);

        private Stopwatch evacTime = new Stopwatch();
        private int EVAC_TIME_SEC = 30;
        private float EVAC_MIN_DIST = 7f;

        //TODO: replace suspect with suspect's name
        private const string MSG_TALK_TO_PED = "Talk to ~y~person~s~ and collect as much information as possible.";
        private const string MSG_PRESS_TO_TALK = "Press ~y~{0}~w~ to ask the ~y~person~w~ some questions.";
        private const string MSG_LEAVE = "It looks like the ~y~person~w~ is done talking, leave the scene before you lose the case.";

        private const string INTERROGEE = "inter_interrogee";

        private const Keys KEY_START_INTERROGATION = Keys.Y;
         
        private Blip areaBlip;

        private readonly PersonData pedsData;
        private Ped ped;
        private PedScenarioLoop pedScenario;
        private Interrogation interrogation;

        IScene scene;

        public InterrogatePedWithTimeout(StageData stageData)
        {
            data = stageData;
            pedsData = data.GetResourceByName<PersonData>(INTERROGEE);
        }

        protected override bool Initialize()
        {
            areaBlip = Base.SharedStageMethods.CreateBlip(data);

            scene = Base.SharedStageMethods.GetScene(data);

            data.CallNotification.DisplayNotification();

            NativeFunction.Natives.FlashMinimapDisplay();

            ActivateStage(Away);

            return true;
        }

        private void Away()
        {
            if (DistToPlayer(data.CallPosition) < 150f)
            {
                Spawn();

                SwapStages(Away, NotifyTalk);
            }
        }

        private void Spawn()
        {
            scene?.Create();

            ped = new Ped(pedsData.Model, pedsData.Spawn.Position, pedsData.Spawn.Heading);
            ped.BlockPermanentEvents = true;
            ped.IsPersistent = true;
            ped.ResetVariation();

            pedScenario = new PedScenarioLoop(ped, pedsData.Scenario);
            pedScenario.IsActive = true;

            var interrogationData = data.ParentCase.GetResourceByID<InterrogationData>(pedsData.InterrogationID);
            interrogation = new Interrogation(interrogationData.Lines, ped);
        }

        private void NotifyTalk()
        {
            if (DistToPlayer(ped.Position) < 10)
            {
                Game.DisplayHelp(MSG_TALK_TO_PED);

                SwapStages(NotifyTalk, NotifyPressToTalk);
            }
        }
        private void NotifyPressToTalk()
        {
            if (DistToPlayer(ped.Position) < 3)
            {
                if (areaBlip) areaBlip.Delete();

                pedScenario.IsActive = false;

                Game.DisplayHelp(string.Format(MSG_PRESS_TO_TALK, KEY_START_INTERROGATION));

                SwapStages(NotifyPressToTalk, WaitForConversationStart);
            }
        }

        private void WaitForConversationStart()
        {
            if (DistToPlayer(ped.Position) < 3 && Game.IsKeyDown(KEY_START_INTERROGATION))
            {
                Game.HideHelp();

                ped.Tasks.ClearImmediately();

                GameFiber.Sleep(0500);

                ped.Face(Player);

                GameFiber.Sleep(1000);

                interrogation.StartDialog();

                SwapStages(WaitForConversationStart, IsConversationFinished);
            }
        }
        private void IsConversationFinished()
        {
            if (interrogation.HasEnded)
            {
                Game.DisplayHelp(MSG_LEAVE);

                evacTime.Start();

                SwapStages(IsConversationFinished, Evac);
            }
        }

        private void Evac()
        {
            if(evacTime.Elapsed.Seconds > EVAC_TIME_SEC &&
               DistToPlayer(ped.Position) < EVAC_MIN_DIST)
            {
                CaseLost();

                DeactivateStage(Evac);
            }
            else if(DistToPlayer(ped.Position) > data.CallAreaRadius)
            {
                Success();

                DeactivateStage(Evac);
            }
        }

        protected override void Process()
        {
        }

        protected override void End()
        {
            if (areaBlip) areaBlip.Delete();
            if (ped) ped.Dismiss();
            scene?.Dispose();
        }

        private void SaveProgress()
        {
            data.ParentCase.Progress.AddNotesToProgress(pedsData.NotesID);
            data.ParentCase.Progress.AddNotesToProgress(data.GetAllIDsOfType<NoteData>().ToArray());
            data.ParentCase.Progress.AddReportsToProgress(data.GetAllIDsOfType<ReportData>().ToArray());
            data.ParentCase.Progress.AddInterrogations(pedsData.InterrogationID);

            data.ParentCase.Progress.SetNextScripts(data.NextScripts[0]);
            data.ParentCase.Progress.SetLastStage(data.ID);
        }

        private void Success()
        {
            SaveProgress();

            DisplayMissionPassedScreen();

            SetScriptFinished(true);
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

        private void CaseLost()
        {
            SaveProgress();

            data.ParentCase.Progress.ModifyCaseProgress(c => c.Finished = true);
            Attributes.NextScripts.Clear();

            DisplayMissionFailedScreen();

            SetScriptFinished(true);
        }

        private void DisplayMissionFailedScreen()
        {
            //MissionFailedScreen failed = new MissionFailedScreen("Violated suspect rights");

            //failed.Show();

            //while (!Game.IsKeyDown(Keys.Enter))
            //{
            //    failed.Draw();
            //    GameFiber.Yield();
            //}

            //this.Attributes.NextScripts.Clear();
            //set case finished
        }
    }
}
