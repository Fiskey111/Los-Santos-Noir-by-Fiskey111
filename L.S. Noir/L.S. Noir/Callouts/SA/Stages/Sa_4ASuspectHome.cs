using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Fiskey111Common;
using LSNoir.Callouts.SA.Creators;
using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using LtFlash.Common.ScriptManager.Scripts;
using LtFlash.Common.Serialization;
using Rage;
using Rage.Native;

namespace LSNoir.Callouts.SA.Stages
{
    public class Sa_4ASuspectHome : BasicScript
    {
        // System
        private bool _beginDialogue;

        // Positions
        private SpawnPt _oneSpawn;

        // Entities
        private Ped _one;
        private Blip _areaBlip;

        // Conversations
        private Interrogation _interrogation;
        
        // Data
        private CaseData _cData;
        private PedData _sData;
        private ReportData _sReportData;

        // Misc
        private string _scenario;

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 4a [Suspect Home]".AddLog();

            var spawn_scene = LoadDict();

            var random = Rand.RandomNumber(spawn_scene.Count);
            _scenario = spawn_scene.Values.ToArray()[random];
            _oneSpawn = spawn_scene.Keys.ToArray()[random];
            _areaBlip = new Blip(_oneSpawn.Spawn)
            {
                Sprite = BlipSprite.GangAttackPackage,
                Color = Color.DarkOrange,
                Name = "Interrogate Suspect"
            };

            _cData = Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);

            _sData = Serializer.GetSelectedListElementFromXml<PedData>(Main.SDataPath,
                s => s.FirstOrDefault<PedData>(c => String.Equals(c.Name, _cData.CurrentSuspect, StringComparison.CurrentCultureIgnoreCase)));

            "Sexual Assault Case Update".DisplayNotification("Speak to suspect", _cData.Number);

            
            return true;
        }

        // todo -- get positions and put them in xml
        private static Dictionary<SpawnPt, string> LoadDict()
        {
            var spawnlist = new Dictionary<SpawnPt, string>
            {
                {new SpawnPt(208.22f, 1059.81f, -446.85f, 66.02f), "WORLD_HUMAN_GARDENER_PLANT"},
                {new SpawnPt(210.04f, -288.29f, 15.24f, 54.75f), "WORLD_HUMAN_DRINKING"},
                {new SpawnPt(48.47f, 1280.95f, -1602.24f, 54.23f), "WORLD_HUMAN_JOG_STANDING"},
                {new SpawnPt(315.62f, -1372.31f, -903.95f, 12.47f), "WORLD_HUMAN_HAMMERING"},
                {new SpawnPt(159.69f, 788.48f, 2178.03f, 52.65f), "WORLD_HUMAN_MAID_CLEAN"}
            };

            return spawnlist;
        }
        bool _notified, _interrStarted, _leaveNotified;
        protected override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(_oneSpawn.Spawn) > 150f) return;

            if (!_one)
            {
                _one = new Ped(_sData.Model, _oneSpawn.Spawn, _oneSpawn.Heading);
                _one.MakeMissionPed();
                _one.ResetVariation();
                _one.Heading = _oneSpawn.Heading;

                GameFiber.StartNew(delegate
                {
                    "_one.Task.Start".AddLog();
                    while (Game.LocalPlayer.Character.Position.DistanceTo(_one) > 5f)
                    {
                        _one.Task_Scenario(_scenario);
                        while (NativeFunction.Natives.IS_PED_USING_ANY_SCENARIO<bool>(_one))
                            GameFiber.Yield();
                        GameFiber.Yield();
                    }
                    NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(_one, Game.LocalPlayer.Character, -1);
                });
            }

            if (Game.LocalPlayer.Character.Position.DistanceTo(_oneSpawn.Spawn) < 10f && !_notified)
            {
                _notified = true;
                Game.DisplayHelp("While you don't have enough information to arrest the ~r~suspect~w~ nothing is stopping you from have a conversation!");
                _one.Tasks.Clear();
                GameFiber.Sleep(1000);
                _one.Face(Game.LocalPlayer.Character);
            }

            if (Game.LocalPlayer.Character.Position.DistanceTo(_oneSpawn.Spawn) < 3f && !_beginDialogue)
            {
                _beginDialogue = true;
                if (_areaBlip.Exists()) _areaBlip.Delete();
                Game.DisplayHelp("Press ~y~Y~w~ to ask the ~r~suspect~w~ some questions.");
            }

            if (Game.IsKeyDown(Keys.Y) && !_interrStarted)
            {
                _one.Tasks.ClearImmediately();
                GameFiber.Sleep(0500);
                _one.Face(Game.LocalPlayer.Character);

                GameFiber.Sleep(1000);
                _interrogation = new Interrogation(InterrogationCreator.InterrogationLineCreator(InterrogationCreator.Type.Suspect, _one), _one);
                _interrogation.StartDialog();
                _interrStarted = true;
            }

            if (_interrStarted && _interrogation.HasEnded && !_leaveNotified)
            {
                _sReportData = new ReportData(ReportData.Service.SusFamily, _one, _interrogation.InterrgoationText);
                _leaveNotified = true;
                Game.DisplayHelp("It looks like the ~r~suspect~w~ is done talking, leave the scene before you lose the case");
                StartTimer();
            }

            if (_interrStarted && Game.LocalPlayer.Character.DistanceTo(_one) > 20f)
            {
                if (!_interrogation.HasEnded) return;

                var value = _interrogation.QuestionList.Where(q => q.Value == false)
                    .Aggregate(100, (current, q) => current - 15);

                var medal = MissionPassedScreen.Medal.Gold;
                if (value >= 80 && value < 100) medal = MissionPassedScreen.Medal.Silver;
                else if (value < 80) medal = MissionPassedScreen.Medal.Bronze;

                var handler = new MissionPassedHandler("Suspect Interrogation", value, medal);

                handler.AddItem("Spoke to Suspect", "", MissionPassedScreen.TickboxState.Tick);
                var num = 0;
                foreach (var q in _interrogation.QuestionList)
                {
                    num++;
                    var correct = q.Value ? "Correct" : "Incorrect";
                    handler.AddItem($"Question {num}", correct, MissionPassedScreen.TickboxState.None);
                }

                handler.Show();
                SetScriptFinished();
            }
        }

        protected override void End() { }

        protected void SetScriptFinished()
        {
            "Sexual Assault Case Update".DisplayNotification("Suspect Conversation \nAdded to ~b~SAJRS", _cData.Number);
            _cData.CurrentStage = CaseData.LastStage.SuspectHome;
            _cData.LastCompletedStage = CaseData.LastStage.SuspectHome;
            _cData.CompletedStages.Add(CaseData.LastStage.SuspectHome);
            _cData.SajrsUpdates.Add("Interrogated Suspect");
            _cData.WarrantAccess = true;
            _cData.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            Serializer.SaveItemToXML<CaseData>(_cData, Main.CDataPath);

            var l = Serializer.LoadItemFromXML<List<ReportData>>(Main.RDataPath);
            l.Add(_sReportData);
            Serializer.SaveItemToXML(l, Main.RDataPath);

            if (_areaBlip.Exists()) _areaBlip.Delete();
            if (_one) _one.Dismiss();
            SetScriptFinished(true);
        }

        private void StartTimer()
        {
            GameFiber.StartNew(delegate
            {
                var sw = new Stopwatch();
                sw.Start();
                while (Game.LocalPlayer.Character.Position.DistanceTo(_one) < 20f)
                {
                    if (sw.Elapsed.Seconds > 30)
                    {
                        CaseLost();
                        sw.Stop();
                    }
                    GameFiber.Yield();
                }
                sw.Stop();
            });
        }

        private void CaseLost()
        {
            MissionFailedScreen failed = new MissionFailedScreen("Violated suspect rights");
            failed.Show();
            while (!Game.IsKeyDown(Keys.Enter))
            {
                failed.Draw();
                GameFiber.Yield();
            }
            this.Attributes.NextScripts.Clear();
        }
    }
}
