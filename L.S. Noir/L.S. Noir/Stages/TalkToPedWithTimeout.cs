﻿using LSNoir.Data;
using LSNoir.Resources;
using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LSNoir.Stages
{
    //https://github.com/Fiskey111/Los-Santos-Noir-by-Fiskey111/blob/master/L.S.%20Noir/L.S.%20Noir/Callouts/SA/Stages/Sa_4ASuspectHome.cs

    //{new SpawnPt(208.22f, 1059.81f, -446.85f, 66.02f), "WORLD_HUMAN_GARDENER_PLANT"},
    //{new SpawnPt(210.04f, -288.29f, 15.24f, 54.75f), "WORLD_HUMAN_DRINKING"},
    //{new SpawnPt(48.47f, 1280.95f, -1602.24f, 54.23f), "WORLD_HUMAN_JOG_STANDING"},
    //{new SpawnPt(315.62f, -1372.31f, -903.95f, 12.47f), "WORLD_HUMAN_HAMMERING"},
    //{new SpawnPt(159.69f, 788.48f, 2178.03f, 52.65f), "WORLD_HUMAN_MAID_CLEAN"}

    public class TalkToPedWithTimeout : BasicScript
    {
        //TODO:
        // - add RNUI TimerBar and display time left while leaving

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
        private float FINISH_DIST = 20f;

        //TODO: replace suspect with suspect's name
        private const string MSG_TALK_TO_PED = "While you don't have enough information to arrest the ~r~suspect~w~ nothing is stopping you from have a conversation!";
        private const string MSG_PRESS_TO_TALK = "Press ~y~{0}~w~ to ask the ~r~suspect~w~ some questions.";
        private const string MSG_LEAVE = "It looks like the ~r~suspect~w~ is done talking, leave the scene before you lose the case";

        private const Keys KEY_START_INTERROGATION = Keys.Y;
         
        private Blip areaBlip;

        private readonly WitnessData pedsData;
        private Ped ped;
        private PedScenarioLoop pedScenario;
        private Interrogation interrogation;

        public TalkToPedWithTimeout(StageData stageData)
        {
            data = stageData;
            var id = data.WitnessID.FirstOrDefault();
            pedsData = data.ParentCase.GetWitnessData(id);
        }

        protected override bool Initialize()
        {
            areaBlip = new Blip(data.CallPosition)
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
            if (DistToPlayer(data.CallPosition) < 150f)
            {
                Spawn();

                SwapStages(Away, NotifyTalk);
            }
        }

        private void Spawn()
        {
            ped = new Ped(pedsData.Model, pedsData.Spawn.Position, pedsData.Spawn.Heading);
            ped.BlockPermanentEvents = true;
            ped.IsPersistent = true;
            ped.ResetVariation();

            pedScenario = new PedScenarioLoop(ped, pedsData.Scenario);
            pedScenario.IsActive = true;

            var idata = data.ParentCase.GetInterrogationData(pedsData.DialogID);
            interrogation = new Interrogation(idata.Lines, ped);
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
                var notesPed = pedsData.NotesID;
                if (notesPed != null && notesPed.Length > 0)
                {
                    data.ParentCase.ModifyCaseProgress(c => c.NotesMade.AddRange(notesPed));
                }

                var notesStage = data.NotesID;
                if(notesStage != null && notesStage.Length > 0)
                {
                    data.ParentCase.ModifyCaseProgress(c => c.NotesMade.AddRange(notesStage));
                }

                data.ParentCase.ModifyCaseProgress(c => c.LastStageID = data.ID);

                Game.DisplayHelp(MSG_LEAVE);

                evacTime.Start();

                SwapStages(IsConversationFinished, Evac);
            }
        }

        private void Evac()
        {
            if(evacTime.Elapsed.Seconds > EVAC_TIME_SEC &&
               DistToPlayer(ped.Position) < 7)
            {
                CaseLost();

                DeactivateStage(Evac);
            }
            else if(DistToPlayer(ped.Position) > FINISH_DIST)
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
        }

        private void Success()
        {
            SetScriptFinished(true);

            //var value = _interrogation.QuestionList.Where(q => q.Value == false)
            //        .Aggregate(100, (current, q) => current - 15);

            //var medal = MissionPassedScreen.Medal.Gold;
            //if (value >= 80 && value < 100) medal = MissionPassedScreen.Medal.Silver;
            //else if (value < 80) medal = MissionPassedScreen.Medal.Bronze;

            //var handler = new MissionPassedHandler("Suspect Interrogation", value, medal);

            //handler.AddItem("Spoke to Suspect", "", MissionPassedScreen.TickboxState.Tick);
            //var num = 0;
            //foreach (var q in _interrogation.QuestionList)
            //{
            //    num++;
            //    var correct = q.Value ? "Correct" : "Incorrect";
            //    handler.AddItem($"Question {num}", correct, MissionPassedScreen.TickboxState.None);
            //}

            //handler.Show();
        }

        private void CaseLost()
        {
            data.ParentCase.ModifyCaseProgress(c => c.Finished = true);
            Attributes.NextScripts.Clear();

            SetScriptFinished(true);

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