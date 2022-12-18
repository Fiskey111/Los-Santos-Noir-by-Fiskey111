namespace LSNoir.Callouts.SA.Stages
{/*
    public class SA_3_VictimFamily : BaseScript
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
            
            _cData = Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);
            _pData = Serializer.GetSelectedListElementFromXml<PedData>(Main.PDataPath,
                p => Enumerable.FirstOrDefault<PedData>(p, f => f.Type == PedType.VictimFamily));

            $"Case number: {_cData.Number}".AddLog();
            "Sexual Assault Case Update".DisplayNotification("Speak to family of the victim", _cData.Number);
           
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
        private static SpawnPt GetRandomSpawn()
        {
            var spawnlist = new List<SpawnPt>
            {
                new SpawnPt(95.29f, -278.10f, 386.18f, 110.83f),
                new SpawnPt(310.89f, -23.97f, -47.98f, 67.59f),
                new SpawnPt(46.90f, -1087.29f, -1215.88f, 2.35f),
                new SpawnPt(213.37f, 450.28f, -1720.78f, 29.34f),
                new SpawnPt(241.49f, -3184.23f, 1312.09f, 14.57f)
            };

            return spawnlist[Rand.RandomNumber(spawnlist.Count)];
        }

        protected override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(_oneSpawn.Spawn) > 150f) return;

            if (!_one)
            {
                _one = new Ped(_pData.Model, _oneSpawn.Spawn, _oneSpawn.Heading);
                _one.MakeMissionPed();
                ScenarioHelper();
            }

            if (Game.LocalPlayer.Character.Position.DistanceTo(_oneSpawn.Spawn) < 6f && _beginDialogue == false)
            {
                "Beginning Dialog".AddLog();
                
                _beginDialogue = true;
                _one.Tasks.Clear();
                GameFiber.Sleep(0500);
                _one.Face(Game.LocalPlayer.Character);
                Game.DisplayHelp("Press ~y~Y~w~ to start the interrogation");  
            }
            if (Game.IsKeyDown(Keys.Y) && !_interrStarted)
            {
                if (_areaBlip.Exists()) _areaBlip.Delete();
                _one.Face(Game.LocalPlayer.Character);
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
            var num = 0;
            foreach (var q in _interrogation.QuestionList)
            {
                num++;
                var correct = q.Value ? "Correct" : "Incorrect";
                handler.AddItem($"Question {num}", correct, MissionPassedScreen.TickboxState.None);
            }

            handler.Show();

            _cData.CurrentStage = CaseData.LastStage.VictimFamily;
            _cData.CompletedStages.Add(CaseData.LastStage.VictimFamily);
            _cData.SajrsUpdates.Add("Talked to Witness Family");
            _cData.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            _cData.LastCompletedStage = CaseData.LastStage.VictimFamily;

            Serializer.SaveItemToXML<CaseData>(_cData, Main.CDataPath);
            var l = Serializer.LoadItemFromXML<List<ReportData>>(Main.RDataPath);
            l.Add(_vfData);
            Serializer.SaveItemToXML(l, Main.RDataPath);

            $"Case number: {_cData.Number}".AddLog();
            Functions.PlayScannerAudio("ATTN_DISPATCH CODE_04_PATROL");
            Main.CompAccess = true;
            if (_areaBlip.Exists()) _areaBlip.Delete();
            if (_one) _one.Dismiss();
            SetScriptFinished(true);
        }
    }*/
}
