using Fiskey111Common;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Callouts.SA.Creators;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LSNoir.Extensions;
using System.Drawing;
using LSNoir.Callouts.Universal;
using static LtFlash.Common.Serialization.Serializer;

namespace LSNoir.Callouts
{
    public class Sa_4ASuspectHome : BasicScript
    {
        // System
        private bool _beginDialogue;

        // Positions
        private SpawnPoint _oneSpawn;

        // Entities
        private Ped _one;
        private Blip _areaBlip;

        // Conversations
        private Interrogation _interrogation;
        
        // Data
        private CaseData _cData;
        private PedData _sData;
        private ReportData _sReportData;

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 4a [Suspect Home]".AddLog();
            _oneSpawn = GetRandomSpawn().Keys.FirstOrDefault();
            _areaBlip = new Blip(_oneSpawn.Spawn)
            {
                Sprite = BlipSprite.GangAttackPackage,
                Color = Color.DarkOrange,
                Name = "Interrogate Suspect"
            };

            _cData = LoadItemFromXML<CaseData>(Main.CDataPath);

            _sData = GetSelectedListElementFromXml<PedData>(Main.SDataPath,
                s => s.FirstOrDefault(c => c.Name == _cData.CurrentSuspect));
            
            _one = new Ped(_sData.Model, _oneSpawn.Spawn, _oneSpawn.Heading);
            _one.MakeMissionPed();
            _one.ResetVariation();
            _one.Heading = _oneSpawn.Heading;

            "Sexual Assault Case Update".DisplayNotification("Speak to suspect");
            GameFiber.StartNew(delegate
            {
                "_one.Task.Start".AddLog();
                while (Game.LocalPlayer.Character.Position.DistanceTo(_one) > 5f)
                {
                    _one.Task_Scenario(GetRandomSpawn().Values.FirstOrDefault());
                    while (NativeFunction.Natives.IS_PED_USING_ANY_SCENARIO<bool>(_one))
                        GameFiber.Yield();
                    GameFiber.Yield();
                }
                NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(_one, Game.LocalPlayer.Character, -1);
            });
            
            return true;
        }

        // todo -- get positions and put them in xml
        private static Dictionary<SpawnPoint, string> GetRandomSpawn()
        {
            var spawnlist = new Dictionary<SpawnPoint, string>();

            spawnlist.Add(new SpawnPoint(208.22f, 1059.81f, -446.85f, 66.02f), "WORLD_HUMAN_GARDENER_PLANT");
            
            return spawnlist;
        }
        bool _notified, _interrStarted, _leaveNotified;
        protected override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(_oneSpawn.Spawn) < 10f && !_notified)
            {
                _notified = true;
                Game.DisplayHelp("While you don't have enough information to arrest the ~r~suspect~w~ nothing is stopping you from have a conversation!");
            }

            if (Game.LocalPlayer.Character.Position.DistanceTo(_oneSpawn.Spawn) < 3f && !_beginDialogue)
            {
                _beginDialogue = true;
                if (_areaBlip.Exists()) _areaBlip.Delete();
                Game.DisplayHelp("Press ~y~Y~w~ to ask the ~r~suspect~w~ some questions.");
            }

            if (Game.IsKeyDown(Keys.Y) && !_interrStarted)
            {
                _interrogation = new Interrogation(InterrogationCreator.InterrogationLineCreator(InterrogationCreator.Type.Suspect, _one), _one);
                _interrogation.StartDialog();
                _interrStarted = true;
            }

            if (_interrStarted && _interrogation.HasEnded && !_leaveNotified)
            {
                _sReportData = new ReportData(ReportData.Service.SusFamily, _one, _interrogation.InterrgoationText);
                _leaveNotified = true;
                Game.DisplayHelp("It looks like the ~r~suspect~w~ is done talking, leave the scene before you lose the case");
            }

            if (_interrStarted && Game.LocalPlayer.Character.DistanceTo(_one) > 20f)
            {
                if (!_interrogation.HasEnded) return;

                var value = _interrogation.QuestionList.Where(q => q.Value == false)
                    .Aggregate(100, (current, q) => current - 10);

                var medal = MissionPassedScreen.Medal.Gold;
                if (value >= 80 && value < 100) medal = MissionPassedScreen.Medal.Silver;
                else if (value < 80) medal = MissionPassedScreen.Medal.Bronze;

                var handler = new MissionPassedHandler("Suspect Interrogation", value, medal);

                handler.AddItem("Spoke to Suspect", "", MissionPassedScreen.TickboxState.Tick);
                foreach (var q in _interrogation.QuestionList)
                {
                    var correct = q.Value ? "Correct" : "Incorrect";
                    var tick = q.Value ? MissionPassedScreen.TickboxState.Tick : MissionPassedScreen.TickboxState.None;
                    handler.AddItem($"Question {_interrogation.QuestionList[q.Key]}", correct, tick);
                }

                handler.Show();
                SetScriptFinished();
            }
        }

        protected override void End() { }

        protected void SetScriptFinished()
        {
            "Sexual Assault Case Update".DisplayNotification("Suspect Conversation \nAdded to ~b~SAJRS");
            _cData.CurrentStage = CaseData.LastStage.SuspectHome;
            _cData.LastCompletedStage = CaseData.LastStage.SuspectHome;
            _cData.CompletedStages.Add(CaseData.LastStage.SuspectHome);
            _cData.SajrsUpdates.Add("Interrogated Suspect");
            _cData.WarrantAccess = true;
            _cData.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            SaveItemToXML<CaseData>(_cData, Main.CDataPath);

            var l = LoadItemFromXML<List<ReportData>>(Main.RDataPath);
            l.Add(_sReportData);
            SaveItemToXML(l, Main.RDataPath);

            if (_areaBlip.Exists()) _areaBlip.Delete();
            if (_one) _one.Dismiss();
            SetScriptFinished(true);
        }
    }
}
