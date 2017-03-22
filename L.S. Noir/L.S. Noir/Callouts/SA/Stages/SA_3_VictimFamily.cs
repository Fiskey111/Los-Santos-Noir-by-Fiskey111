using Fiskey111Common;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Rage.Native;
using LSNoir.Callouts.SA.Commons;
using LSPD_First_Response.Mod.API;
using LSNoir.Callouts.SA.Creators;
using LSNoir.Extensions;
using static LtFlash.Common.Serialization.Serializer;
using System.Drawing;
using LSNoir.Callouts.Universal;

namespace LSNoir.Callouts
{
    public class SA_3_VictimFamily : BasicScript
    {
        // System
        private bool _beginDialogue, _interrStarted;

        // Locations
        private SpawnPoint _oneSpawn;

        // Entities
        private Ped _one;
        private Blip _areaBlip;

        // Data
        private ReportData _vfData;
        private CaseData _cData;
        private PedData _pData;

        // Conversations
        private Interrogation _interrogation;

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 3 [Victim Family]".AddLog();

            _oneSpawn = GetRandomSpawn();

            _areaBlip = new Blip(_oneSpawn.Spawn)
            {
                Sprite = BlipSprite.GangAttackPackage,
                Color = Color.DarkOrange,
                Name = "Victim's Family"
            };

            ExtensionMethods.LogDistanceFromCallout(_oneSpawn.Spawn);
            
            _cData = LoadItemFromXML<CaseData>(Main.CDataPath);
            _pData = GetSelectedListElementFromXml<PedData>(Main.PDataPath,
                p => p.FirstOrDefault(f => f.Type == PedType.VictimFamily));

            _one = new Ped(_pData.Model, _oneSpawn.Spawn, _oneSpawn.Heading);
            
            "Sexual Assault Case Update".DisplayNotification("Speak to family of the victim");

            ScenarioHelper();
           
            return true;
        }

        private void ScenarioHelper()
        {
            GameFiber.StartNew(delegate
            {
                while (Game.LocalPlayer.Character.Position.DistanceTo(_one) > 5f)
                {
                    string[] scenarios =
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

                    _one.Task_Scenario(scenarios[MathHelper.GetRandomInteger(scenarios.Length)]);
                    while (NativeFunction.Natives.IS_PED_ACTIVE_IN_SCENARIO<bool>(_one))
                        GameFiber.Yield();
                    GameFiber.Yield();
                }
                
                NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(_one, Game.LocalPlayer.Character, -1);
            });
        }

        // todo -- get positions and put them in xml
        private static SpawnPoint GetRandomSpawn()
        {
            List<SpawnPoint> spawnlist = new List<SpawnPoint>
            {
                new SpawnPoint(95.29f, -278.10f, 386.18f, 110.83f)
            };

            return spawnlist[MathHelper.GetRandomInteger(spawnlist.Count)];
        }

        protected override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(_oneSpawn.Spawn) < 6f && _beginDialogue == false)
            {
                "Beginning Dialog".AddLog();
                
                _beginDialogue = true;
                _one.Tasks.Clear();
                GameFiber.Sleep(0500);
                var player = Game.LocalPlayer.Character;
                NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(_one, player, -1);
                Game.DisplayHelp("Press ~y~Y~w~ to start the interrogation");  
            }
            if (Game.IsKeyDown(Keys.Y) && !_interrStarted)
            {
                if (_areaBlip.Exists()) _areaBlip.Delete();
                NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(Game.LocalPlayer.Character, _one, -1);
                GameFiber.Sleep(0500);
                if (Vector3.Distance2D(Game.LocalPlayer.Character.Position, _one.Position) < 1.5f) Game.LocalPlayer.Character.Position = Game.LocalPlayer.Character.RearPosition;
                _interrogation = new Interrogation(InterrogationCreator.InterrogationLineCreator(InterrogationCreator.Type.VictimFamily, _one), _one);
                _interrogation.StartDialog();
                _interrStarted = true;
                _interrStarted = true;
            }

            if (_interrStarted && Game.LocalPlayer.Character.Position.DistanceTo(_one.Position) > 50f)
            {
                _vfData = new ReportData(ReportData.Service.VicFamily, _one, _interrogation.InterrgoationText);
                this.Attributes.NextScripts.Clear();
                this.Attributes.NextScripts.Add("Sa_3b_Wait");
                SetScriptFinished();
            }
        }

        protected override void End() { }

        protected void SetScriptFinished()
        {
            var value = _interrogation.QuestionList.Where(q => q.Value == false)
                .Aggregate(100, (current, q) => current - 10);
            
            var medal = MissionPassedScreen.Medal.Gold;
            if (value >= 80 && value < 100) medal = MissionPassedScreen.Medal.Silver;
            else if (value < 80) medal = MissionPassedScreen.Medal.Bronze; 

            var handler = new MissionPassedHandler("Victim Family", value, medal);
            
            handler.AddItem("Spoke to Family", "", MissionPassedScreen.TickboxState.Tick);
            foreach (var q in _interrogation.QuestionList)
            {
                var correct = q.Value ? "Correct" : "Incorrect";
                handler.AddItem($"Question {_interrogation.QuestionList[q.Key]}", correct, MissionPassedScreen.TickboxState.None);
            }

            handler.Show();

            _cData.CurrentStage = CaseData.LastStage.VictimFamily;
            _cData.CompletedStages.Add(CaseData.LastStage.VictimFamily);
            _cData.SajrsUpdates.Add("Talked to Witness Family");
            _cData.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            _cData.LastCompletedStage = CaseData.LastStage.VictimFamily;

            SaveItemToXML<CaseData>(_cData, Main.CDataPath);
            var l = LoadItemFromXML<List<ReportData>>(Main.RDataPath);
            l.Add(_vfData);
            SaveItemToXML(l, Main.RDataPath);

            Functions.PlayScannerAudio("ATTN_DISPATCH CODE_04_PATROL");
            Main.CompAccess = true;
            if (_areaBlip.Exists()) _areaBlip.Delete();
            if (_one) _one.Dismiss();
            SetScriptFinished(true);
        }
    }
}
