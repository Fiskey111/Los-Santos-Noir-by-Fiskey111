using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Fiskey111Common;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Callouts.SA.Creators;
using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.SA.Services;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.EvidenceLibrary.Evidence;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Evid = LtFlash.Common.EvidenceLibrary.Evidence;
using Marker = Fiskey111Common.Marker;
using SpawnPoint = LSNoir.Callouts.SA.Services.SpawnPoint;
using static LtFlash.Common.Serialization.Serializer;

namespace LSNoir.Callouts.SA.Stages
{
    public class Sa1Csi : CalloutScript
    {
        // System
        private string _genderstring;
        private int _missionValue = 30;
        private bool _closeCheckGo, _betteremsSupport, _informed, _emsdispatched, _rancheck, _informed2,
            _emstransport, _vicgender, _barriers, _vicCheck, _emscollect, _coronercollected;
        private Dictionary<Witness, bool> _witDictionary = new Dictionary<Witness, bool>();

        // Services
        private Ems _ems; 
        private Coroner _coroner;
        private Services.LtFlash.Common.EvidenceLibrary.Evidence.Witness _wit1, _wit2;

        // Data
        private CaseData _caseData;
        private PedData _vicData, _wit1Data, _wit2Data;
        private EvidenceData _eData, _fData, _dData;
        private ReportData _emsData, _corData, _foData;
        private List<PedData> _sDataList = new List<PedData>();
        private List<PedData> _wDataList = new List<PedData>();
        private List<PedData> _pDataList = new List<PedData>();
        private List<ReportData> _rDataList = new List<ReportData>();
        private List<EvidenceData> _eDataList = new List<EvidenceData>();
        private List<int> _wDataIDs = new List<int>();

        // Evidence
        internal Evid.Object ObjectDrink, ObjectElectronic, ObjectFood;
        private EvidenceController _eController = new EvidenceController();
        private Model _fooditem, _drink, _electronic;
        private Dictionary<Evid.Object, EvidenceData> _evidenceObjData = new Dictionary<Evid.Object, EvidenceData>();

        // Dialog
        internal Dialog FoDialog, Wit1Dialog, Wit2Dialog;
        private Dictionary<LSNoir.Callouts.SA.Services.LtFlash.Common.EvidenceLibrary.Evidence.Witness, Dialog> _witList = new Dictionary<LSNoir.Callouts.SA.Services.LtFlash.Common.EvidenceLibrary.Evidence.Witness, Dialog>();

        // Misc
        private EnumState _state;
        private CheckState _availablestate;
        private Stopwatch _sw = new Stopwatch();


        // todo -- get all spawnpoints from xml
        private SpawnPoint _dispEmsTo;

        private SpawnPoint _dispCoronerTo;
        // todo -- get all spawnpoints from xml ^^

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 1 [CSI]".AddLog();

            _betteremsSupport = Main.BetterEmsFound;

            _sw.Start();

            // Resets previous Data
            SaveItemToXML(new PedData(), Main.PDataPath);
            SaveItemToXML(new PedData(), Main.SDataPath);
            SaveItemToXML(new WitnessData(), Main.WDataPath);
            SaveItemToXML(new CaseData(), Main.CDataPath);
            SaveItemToXML(new EvidenceData(), Main.EDataPath);
            SaveItemToXML(new ReportData(), Main.RDataPath);

            CsiCreator.CreateScene(Game.LocalPlayer.Character.Position);

            while (!CsiCreator.Completed && !CsiCreator.AComplete && !CsiCreator.CComplete && !CsiCreator.loadCreated)
                GameFiber.Yield();
            
            _caseData = LoadItemFromXML<CaseData>(Main.CDataPath);

            UpdateandSaveCaseData();
            GameFiber.Sleep(1000);
            _caseData = LoadItemFromXML<CaseData>(Main.CDataPath);

            DisplayCallout();

            _dispEmsTo = new Services.SpawnPoint(CsiCreator.EmsLast.Heading, CsiCreator.EmsLast.Position);
            _dispCoronerTo = new Services.SpawnPoint(CsiCreator.EmsLast.Heading, CsiCreator.EmsLast.Position);

            _sw.Stop();
            ("***TIME_LOGGER: " + _sw.Elapsed.Seconds.ToString() + " to run Initialize()").AddLog();
            _sw.Reset();

            return true;
        }

        private void UpdateandSaveCaseData()
        {
            "Updating case data".AddLog();
            _caseData.Number = RandomNumberGenerator.RandomNumber(800, 3000);
            _caseData.CurrentStatus = "Unsolved";
            _caseData.CrimeTime = World.DateTime;
            _caseData.SecCamSpawn = CsiCreator.SecCamSpawnPoint;
            _caseData.SusSpawnPoint = CsiCreator.SusSpawnPoint;
            _caseData.SusTarget = CsiCreator.SusTargetPoint;
            SaveItemToXML<CaseData>(_caseData, Main.CDataPath);
        }

        private void DisplayCallout()
        {
            _genderstring = CsiCreator.Victim.Ped.IsMale ? "~y~male~w~" : "~y~female~w~";
            
            "Displaying Callout".AddLog();
            DisplayCalloutInfo("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~y~Sexual Assault Case", "Reports of a sexual assault");
            ShowAreaBlip(CsiCreator.Victim.Ped.Position, 150f);
            PlaySoundPlayerClosingIn = true;

            Functions.PlayScannerAudio("ATTN_UNIT_02 " + Settings.UnitName() + " WE_HAVE AN_ASSAULT OFFICERS_AT_SCENE RESPOND_CODE3");
        }

        protected override bool Accepted()
        {
            GameFiber.StartNew(delegate
            {
                _sw.Start();
                "Accepted Started".AddLog();

                ShowAreaWithRoute(CsiCreator.Victim.Ped.Position, 75f, Color.Yellow);

                "Case Accepted".DisplayNotification("Investigate the scene\nReports of a " + _genderstring + " on the ground not responding.");

                _state = EnumState.EnRoute;
                _availablestate = CheckState.Start;
                                
                CreateEvidence();

                CreateEntityData(EntityType.Vic);
                CreateEntityData(EntityType.VicFamily);
                CreateEntityData(EntityType.Wit1);
                CreateEntityData(EntityType.Wit2);
                CreateEntityData(EntityType.Fo);

                for (var i = 1; i < 5; i++)
                    CreateEntityData(Sa1Csi.EntityType.Sus, i);

                _availablestate = CheckState.Fo;

                AddNextScripts(_emstransport);

                _eController.AddEvidence(ObjectDrink);
                _eController.AddEvidence(ObjectElectronic);
                _eController.AddEvidence(ObjectFood);

                ("***TIME_LOGGER: " + _sw.Elapsed.Seconds.ToString() + " to run Accepted()").AddLog();
                _sw.Reset();

                ActivateStage(Away);
            });
            return true;
        }

        internal void CreateEntityData(EntityType type, int num = 0)
        {
            ("Creating entity: " + type).AddLog();
            switch (type)
            {
                case EntityType.Vic:
                    $"Victim exists: {CsiCreator.Victim.Exists()}".AddLog();
                    var boneIDs = new List<PedBoneId>
                    {
                        PedBoneId.Head,
                        PedBoneId.LeftFoot,
                        PedBoneId.RightFoot,
                        PedBoneId.LeftForeArm,
                        PedBoneId.RightForearm,
                        PedBoneId.LeftHand,
                        PedBoneId.RightHand,
                        PedBoneId.LeftThigh,
                        PedBoneId.RightThigh,
                        PedBoneId.LeftUpperArm,
                        PedBoneId.RightUpperArm,
                        PedBoneId.Neck
                    };

                    Array traces = Enum.GetValues(typeof(EvidenceData.Traces));
                    Traces trace = (Traces)traces.GetValue(Fiskey111Common.RandomNumberGenerator.RandomNumber().Next(traces.Length));

                    _vicData = new PedData(CsiCreator.Victim.Ped, PedType.Victim, true, false, boneIDs[MathHelper.GetRandomInteger(1, boneIDs.Count - 1)].ToString(),
                        boneIDs[MathHelper.GetRandomInteger(1, boneIDs.Count - 1)].ToString(),
                        boneIDs[MathHelper.GetRandomInteger(1, boneIDs.Count - 1)].ToString(), trace, true);

                    if (MathHelper.GetRandomInteger(6) == 1)
                        _vicData.IsImportant = true;

                    _vicData.Model = CsiCreator.Victim.Ped.Model.Name;

                    CsiCreator.Victim.KeyLeave = Keys.D9;
                    break;
                case EntityType.Fo:
                    $"FO exists: {CsiCreator.FirstOfficer.Exists}".AddLog();
                    FoDialog = new Dialog(ConversationCreator.DialogLineCreator(ConversationCreator.ConversationType.Fo, CsiCreator.FirstOfficer.Ped, _wDataIDs.Count), CsiCreator.FirstOfficer.Ped.Position);
                    FoDialog.AddPed(0, Game.LocalPlayer.Character);
                    FoDialog.AddPed(1, CsiCreator.FirstOfficer.Ped);

                    FoDialog.DistanceToStop = 3f;

                    _foData = new ReportData(ReportData.Service.FO,
                        Functions.GetPersonaForPed(CsiCreator.FirstOfficer.Ped).FullName, FoDialog.Dialogue)
                    {
                        Transcript = FoDialog.Dialogue,
                        Exists = true
                    };
                    _rDataList.Add(_foData);
                    break;
                case EntityType.Wit1:
                    if (MathHelper.GetRandomInteger(2) == 1)
                    {
                        "Creating Witness 1".AddLog();
                         _wit1 = new Services.LtFlash.Common.EvidenceLibrary.Evidence.Witness(
                                "W1", "Witness 1", new Services.SpawnPoint(CsiCreator._W1SpawnPoint.Heading, CsiCreator._W1SpawnPoint.Position),
                                Model.PedModels.ToList()[MathHelper.GetRandomInteger(Model.PedModels.ToList().Count)],
                                null, CsiCreator.EmsLast.Position);

                        _wit1.Ped.IsInvincible = true;
                        _wit1.Ped.RandomizeVariation();
                        _wit1.Ped.MakeMissionPed();

                        _wit1Data = new PedData(_wit1.Ped, PedType.Witness1, MathHelper.GetRandomInteger(3) == 1);

                        if (_wit1Data.IsImportant)
                        {
                            var keys = new List<int>(WitnessData.WhatSeen().Keys);
                            var random = Fiskey111Common.RandomNumberGenerator.RandomNumber(0, 8);
                            _wit1Data.WhatSeenInt = keys[random];
                            _wit1Data.WhatSeenString = WitnessData.WhatSeen()[_wit1Data.WhatSeenInt];
                        }
                        
                        Wit1Dialog = new Dialog(ConversationCreator.DialogLineCreator(ConversationCreator.ConversationType.Witness, _wit1.Ped, 0, false, null, null, _wit1Data), _wit1.Ped.Position);
                        Wit1Dialog.AddPed(0, Game.LocalPlayer.Character);
                        Wit1Dialog.AddPed(1, _wit1.Ped);
                        Wit1Dialog.DisableFirstKeypress = false;

                        _wit1Data.Conversation = Wit1Dialog.Dialogue;
                        _caseData.WitnessIDs.Add(1);
                        _wDataList.Add(_wit1Data);
                        _witList.Add(_wit1, Wit1Dialog);
                        GameFiber.Sleep(0500);
                    }
                    break;
                case EntityType.Wit2:
                    if (MathHelper.GetRandomInteger(2) == 1)
                    {
                        "Creating Witness 2".AddLog();
                        _wit2 = new Services.LtFlash.Common.EvidenceLibrary.Evidence.Witness(
                               "W2", "Witness 2", new Services.SpawnPoint(CsiCreator._W2SpawnPoint.Heading, CsiCreator._W2SpawnPoint.Position),
                               Model.PedModels.ToList()[MathHelper.GetRandomInteger(Model.PedModels.ToList().Count)],
                               null, CsiCreator.EmsLast.Position);

                        _wit2.Ped.IsInvincible = true;
                        _wit2.Ped.RandomizeVariation();
                        _wit2.Ped.MakeMissionPed();

                        _wit2Data = new PedData(_wit2.Ped, PedType.Witness2, MathHelper.GetRandomInteger(3) == 1);

                        if (_wit2Data.IsImportant)
                        {
                            var keys = new List<int>(WitnessData.WhatSeen().Keys);
                            var random = Fiskey111Common.RandomNumberGenerator.RandomNumber(0, 8);
                            _wit2Data.WhatSeenInt = keys[random];
                            _wit2Data.WhatSeenString = WitnessData.WhatSeen()[_wit2Data.WhatSeenInt];
                        }

                        Wit2Dialog = new Dialog(ConversationCreator.DialogLineCreator(ConversationCreator.ConversationType.Witness, _wit2.Ped, 0, false, null, null, _wit2Data), _wit2.Ped.Position);
                        Wit2Dialog.AddPed(0, Game.LocalPlayer.Character);
                        Wit2Dialog.AddPed(1, _wit2.Ped);
                        Wit2Dialog.DisableFirstKeypress = false;

                        _wit2.Dialog = Wit2Dialog;

                        _wit2Data.Conversation = Wit2Dialog.Dialogue;
                        _caseData.WitnessIDs.Add(2);
                        _wDataList.Add(_wit2Data);
                        _witList.Add(_wit2, Wit2Dialog);
                        GameFiber.Sleep(0500);
                    }
                    break;
                case EntityType.Sus:
                {
                    "Suspect being created".AddLog();
                    var s = new Ped(Model.PedModels.Where(m => m.IsPed).ToList()[MathHelper.GetRandomInteger(Model.PedModels.Where(m => m.IsPed).ToList().Count)], new Vector3(0, 0, 0), 0f);
                    GameFiber.Sleep(1000);
                    _sDataList.Add(new PedData(s, PedType.Suspect, false, num == 1));
                    GameFiber.Sleep(0500);
                    if (s) s.Delete();
                }
                    break;
                case EntityType.VicFamily:
                {
                    "Creating vicfamily member".AddLog();
                    var s = new Ped(new Vector3(0, 0, 0));
                    GameFiber.Sleep(1000);
                    var data = new PedData(s, PedType.VictimFamily);
                    GameFiber.Sleep(0500);
                    _pDataList.Add(data);
                    GameFiber.Sleep(0500);
                    if (s) s.Delete();
                }
                    break;
            }

            ("Entity " + type + " created successfully").AddLog();
        }

        internal enum EntityType { Vic, Fo, Wit1, Wit2, Sus, VicFamily }

        #region EvidenceStuff
        private void CreateEvidence()
        {
            var evidenceDictionary = new Dictionary<string, string>
            {
                {"drink", "prop_cs_bs_cup"},
                {"electronic", "prop_cs_tablet"},
                {"food", "prop_food_bs_chips"}
            };

            foreach (var evidType in evidenceDictionary.Keys)
            {
                ("Creating evidence: " + evidType).AddLog();
                switch (evidType)
                {
                    case "drink":
                        ObjectDrink = new Evid.Object(evidType, evidType, evidenceDictionary[evidType], CsiCreator.Victim.Position.Around(2f, 4f));
                        CompleteEvidCreation(ObjectDrink);
                        _dData = new EvidenceData(EvidenceData.DataType.Drink, "Drink", ObjectDrink.@object.Model, ObjectDrink.IsImportant);
                        GetTraces(_dData);
                        _evidenceObjData.Add(ObjectDrink, _dData);
                        _eDataList.Add(_dData);
                        break;
                    case "electronic":
                        ObjectElectronic = new Evid.Object(evidType, evidType, evidenceDictionary[evidType], CsiCreator.Victim.Position.Around2D(3f, 5f));
                        CompleteEvidCreation(ObjectElectronic);
                        _eData = new EvidenceData(EvidenceData.DataType.Electronic, "Electronic", ObjectElectronic.@object.Model, ObjectElectronic.IsImportant);
                        GetTraces(_eData);
                        _evidenceObjData.Add(ObjectElectronic, _eData);
                        _eDataList.Add(_eData);
                        break;
                    case "food":
                        ObjectFood = new Evid.Object(evidType, evidType, evidenceDictionary[evidType], CsiCreator.Victim.Position.Around2D(3f, 5f));
                        CompleteEvidCreation(ObjectFood);
                        _fData = new EvidenceData(EvidenceData.DataType.Food, "Food", ObjectFood.@object.Model, ObjectFood.IsImportant);
                        GetTraces(_fData);
                        _evidenceObjData.Add(ObjectFood, _fData);
                        _eDataList.Add(_fData);
                        break;
                }

                ("Evidence: " + evidType + " created successfully").AddLog();
            }
        }

        private void CompleteEvidCreation(Evid.Object obj)
        {
            if (MathHelper.GetRandomInteger(3) == 1)
            {
                obj.IsImportant = true;
                obj.PlaySoundImportantEvidenceCollected = true;
            }

            UpdateEvidence(obj);
        }

        private void GetTraces(EvidenceData data)
        {
            Array values = Enum.GetValues(typeof(EvidenceData.Traces));
            data.Trace = (EvidenceData.Traces)values.GetValue(RandomNumberGenerator.RandomNumber().Next(values.Length));
        }

        private void UpdateEvidence(Evid.Object obj)
        {
            if (!obj.Exists()) return;
            obj.TextHelpWhileExamining = "Press ~y~R~w~ to flip the item \nPress ~y~C~w~ to add the item to evidence \nPress ~y~L~w~ to leave the item \n";
        }
        #endregion

        private void AddNextScripts(bool isTransported)
        {
            Attributes.NextScripts.Clear();
            if (isTransported)
            {
                Attributes.NextScripts.Add("Sa_2aHospital");
                "Next stage added: SA_2a_Hospital".AddLog();
            }
            else
            {
                Attributes.NextScripts.Add("Sa_2BMedicalExaminer");
                "Next stage added: SA_2b_MedicalExaminer".AddLog();
            }
        }

        protected override void NotAccepted()
        {
            if (Settings.AiAudio())
                Functions.PlayScannerAudio("OFFICER_INTRO_01 UNIT_RESPONDING_DISPATCH_04");
            CsiCreator.End();
        }

        protected void Away()
        {
            SwapStages(Away, CheckIfAtScene);
        }

        protected override void Process()
        {
            if (!_barriers && Game.LocalPlayer.Character.Position.DistanceTo(CsiCreator.FirstOfficer.Spawn) < 150f)
            {
                _barriers = true;
                CsiCreator.AddBarriers();
                CsiCreator.Victim.Ped.IsGravityDisabled = false;
                CsiCreator.Victim.Ped.IsRagdoll = true;

                GameFiber.Sleep(1000);

                CsiCreator.Victim.Ped.IsPositionFrozen = true;
            }

            CheckForWitness();
        }
        
        private void CheckForWitness()
        {
            foreach (var obj in _witList.Keys)
            {
                if (!obj.Exists()) continue;

                if (_witList[obj].IsRunning || obj.IsCollected) continue;

                if (Game.LocalPlayer.Character.Position.DistanceTo(obj.Ped.Position) < 2f)
                    _witList[obj].StartDialog();
            }
        }
        float heading;
        #region Process Stages
        private void CheckIfAtScene() //1.
        {
            if (!CsiCreator.FirstOfficer.Ped.Exists()) return;

            if (!_rancheck && Game.LocalPlayer.Character.Position.DistanceTo(CsiCreator.FirstOfficer.Ped.Position) < 25f)
            {
                _rancheck = true;
                Vector3 markerPos = new Vector3(CsiCreator.FirstOfficer.Spawn.X, CsiCreator.FirstOfficer.Spawn.Y, (CsiCreator.FirstOfficer.Spawn.Z + 1.6f));
                Marker.StartMarker(markerPos, Color.LightBlue);
                FoDialog.Position = CsiCreator.FirstOfficer.Ped.Position;
                "Arrived at scene".AddLog();

                Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 OFFICERS_AT_SCENE NO_FURTHER_UNITS CRIME_AMBULANCE_REQUESTED_02", CsiCreator.Victim.Position);

                Game.DisplayHelp("Talk to the ~b~First Officer~w~ at scene to receive a preliminary report.");
                if (_emsdispatched == false)
                {
                    _emsdispatched = true;
                    if (_betteremsSupport)
                    {
                        //ApiWrapper.RequestEms(CsiCreator.EmsLast, CsiCreator.EmsQueue);
                        ("BetterEMS Transport = " + _emstransport).AddLog();
                        var surviveprob = _emstransport ? 1f : 0f;

                        ApiWrapper.SetVictimData(CsiCreator.Victim.Ped, _vicData.BruiseLocation, "Cause: Sexual Assault", surviveprob);
                    }
                    else
                    {
                        CreateServices(ServiceType.Ems);
                        GameFiber.Sleep(0500);
                        ("LtFlashEMS Called; EMT exists: " + _ems.PedWorker.Exists()).AddLog();
                    }
                    "~g~EMS~w~ Dispatched".DisplayNotification("EMS requested by the first officer");
                }
            }

            if (!(Game.LocalPlayer.Character.Position.DistanceTo(CsiCreator.FirstOfficer.Ped.Position) < 3f)) return;

            if (!_foNotified)
            {
                _foNotified = true;
                heading = CsiCreator.FirstOfficer.Heading;
                Game.DisplayHelp("Press ~y~Y~w~ to speak to the First Officer");
                Marker.CreateMarker.Abort();
            }
            if (Game.IsKeyDown(Keys.Y) && !FoDialog.IsRunning)
            {
                _missionValue = _missionValue + 5;
                $"Mission value changed to: {_missionValue}".AddLog();
                FoDialog.StartDialog();
                _caseData.SajrsUpdates.Add("Obtained report from First Officer");
            }

            if (!FoDialog.HasEnded) return;

            "First Officer Collected".AddLog();

            "Sexual Assault Case Update".DisplayNotification("First Officer Conversation \nAdded to ~b~SAJRS");
            Game.DisplayHelp("Now that you have checked with the first officer, investigate the crime scene.");

            CsiCreator.FirstOfficer.Ped.Heading = heading;
            GameFiber.Sleep(0500);
            CsiCreator.FirstOfficer.Ped.Tasks.PlayAnimation("amb@code_human_police_crowd_control@idle_b", "idle_d", 4, AnimationFlags.Loop);

            SwapStages(CheckIfAtScene, Finished_FO);
        }

        public void Finished_FO() //3.
        {
            CsiCreator.Victim.CanBeInspected = true;
            CsiCreator.Victim.PlaySoundPlayerNearby = true;

            UpdateEvidence(ObjectDrink);
            UpdateEvidence(ObjectElectronic);
            UpdateEvidence(ObjectFood);

            _informed = false;

            SwapStages(Finished_FO, InvestigatingScene);
        }
        
        public void InvestigatingScene()
        {
            CheckVictim();

            if (!_emscollect)
                CheckEMSCollected();

            if (_emscollect && !_emstransport)
            {
                if (!_informed)
                {
                    _missionValue = _missionValue + 5;
                    $"Mission value changed to: {_missionValue}".AddLog();
                    InformCoroner();
                }

                if (!_coronerCalled && Game.IsKeyDown(Keys.D8))
                {
                    _coronerCalled = true; _informed2 = false;

                    CreateServices(ServiceType.Coroner);
                    "~g~Coroner~w~ Dispatched".DisplayNotification(
                        $"Coroner dispatched to ~y~{World.GetStreetName(Game.LocalPlayer.Character.Position)}");
                    "Begin Coroner".AddLog();
                }

                if (_coronerCalled && _coroner.IsCollected && !_informed2)
                {
                    _informed2 = true; _coronercollected = true;

                    "Sexual Assault Case Update".DisplayNotification("Coroner Conversation \nAdded to ~b~SAJRS");
                    "Coroner Collected == true".AddLog();
                    _caseData.SajrsUpdates.Add("Obtained report from Coroner");

                    _sw.Start();
                }
                else
                    CheckEMSCollected();
            }

            if (_sw.Elapsed.Seconds > 45)
            {
                _sw.Restart();
                Game.DisplayHelp("Once the scene has been thoroughly investigated, leave the scene to complete this stage");
            }

            EvidenceCollected();

            if (!(Game.LocalPlayer.Character.Position.DistanceTo(CsiCreator.FirstOfficer.Spawn) >= 80f)) return;
            
            float dist = Vector3.Distance(Game.LocalPlayer.Character.Position, CsiCreator.FirstOfficer.Spawn);
            ("Distance from FO: " + dist + "; Swapping to TaskLeaveTheScene()").AddLog();
            SwapStages(InvestigatingScene, TaskLeaveTheScene);
        }

        private void CheckVictim()
        {
            if (_vicCheck) return;

            if (!CsiCreator.Victim.Checked) return;

            _missionValue = _missionValue + 5;
            $"Mission value changed to: {_missionValue}".AddLog();

            _vicCheck = true;
            "Victim Checked".AddLog();
            _vicData.Checked = true;

            _caseData.SajrsUpdates.Add("Examined Victim");

            Game.DisplaySubtitle("[~p~" + Settings.OfficerName() + "~w~] Let's see what I can find from the victim.");
            Game.DisplayNotification(
                $"Your inspection found the following: \n ~y~Identification~w~: " +
                $"{_vicData.Name}\n ~y~Bruises~w~: {_vicData.BruiseLocation}\n ~y~Marks~w~: " +
                $"{_vicData.MarkLocation} \n ~y~Cuts~w~: {_vicData.CutLocation}");

            if (CsiCreator.Victim.Exists())
            {
                CsiCreator.Victim.CanBeInspected = false;
                CsiCreator.Victim.PlaySoundPlayerNearby = false;
            }
        }

        private bool CheckEMSCollected()
        {
            if (_emscollect) return true;
            
            if (_betteremsSupport)
            {
                if (ApiWrapper.WasPedRevived(CsiCreator.Victim.Ped) != false) return false;

                _emscollect = true;
                "BetterEMS Collected".AddLog();
                "Sexual Assault Case Update".DisplayNotification("EMS Report \nAdded to ~b~SAJRS");
                _caseData.SajrsUpdates.Add("Obtained report from Paramedic");
                _missionValue = _missionValue + 5;
                $"Mission value changed to: {_missionValue}".AddLog();
                return true;
            }
            else if (_ems.IsCollected)
            {
                _emscollect = true;
                "LtFlashEMS Collected".AddLog();
                "Sexual Assault Case Update".DisplayNotification("EMS Report \nAdded to ~b~SAJRS");
                _caseData.SajrsUpdates.Add("Obtained report from Paramedic");
                _missionValue = _missionValue + 5;
                $"Mission value changed to: {_missionValue}".AddLog();
                return true;
            }
            else
                return false;
        }

        private void PickFunString()
        {
            string[] value =
            {

                "[~g~Paramedic~w~]: I heard that someone else made a 'BetterUs' mod or something? I think it only works in parks or something though.",
                "[~g~Paramedic~w~]: Apparently someone made a "
        };
        }

        private void InformCoroner()
        {
            _informed = true;
            "Informed to Call Coroner".AddLog();
            Game.DisplayHelp("Press 8 to call a ~y~coroner");
        }

        private void CreateServices(ServiceType type)
        {
            switch (type)
            {
                case ServiceType.Ems:
                    "Creating EMS".AddLog();
                    if (RandomNumberGenerator.RandomNumber().Next(2, 3) == 1)
                        _emstransport = true;

                    _ems = new Ems(CsiCreator.Victim.Ped, _dispEmsTo, null, _emstransport);

                    _ems.Dispatch();

                    GameFiber.Sleep(1500);

                    var d = new Dialog(ConversationCreator.DialogLineCreator(ConversationCreator.ConversationType.Ems, _ems.PedDriver), CsiCreator.Victim.Ped.LeftPosition);
                    d.AddPed(0, Game.LocalPlayer.Character);
                    d.AddPed(1, _ems.PedWorker);
                    d.DistanceToStop = 3f;

                    _ems.Dialogue = d;

                    _emsData = new ReportData(ReportData.Service.EMS, _ems.PedDriver, d.Dialogue);
                    _rDataList.Add(_emsData);

                    _ems.VehicleDrivingSpeed = 9f;
                    _ems.VehDrivingFlags = VehicleDrivingFlags.Emergency;
                    break;
                case ServiceType.Coroner:
                    "Creating Coroner".AddLog();
                    _coroner = new Coroner(CsiCreator.Victim.Ped, _dispCoronerTo, null); GameFiber.Sleep(0500);

                    _coroner.ModelPedDriver = "s_m_m_scientist_01";
                    _coroner.ModelPedWorker = "s_m_m_scientist_01";
                    _coroner.ModelVehicle = Settings.CoronerModel();

                    _coroner.Dispatch();

                    GameFiber.Sleep(1500);

                    _coroner.PedDriver.RandomizeVariation();
                    _coroner.PedWorker.RandomizeVariation();

                    Dialog c = new Dialog(ConversationCreator.DialogLineCreator(ConversationCreator.ConversationType.Coroner, _coroner.PedWorker), CsiCreator.Victim.Ped.LeftPosition);
                    c.AddPed(0, Game.LocalPlayer.Character);
                    c.AddPed(1, _coroner.PedWorker);
                    c.DistanceToStop = 3f;

                    _coroner.Dialogue = c;

                    _corData = new ReportData(ReportData.Service.Coroner, _coroner.PedWorker, c.Dialogue);
                    _rDataList.Add(_corData);
                    break;
            }
        }

        private void EvidenceCollected()
        {
            foreach (var obj in _evidenceObjData.Keys.ToList())
            {
                if (!obj.Checked || !obj.@object.Exists() || !_evidenceObjData.ContainsKey(obj)) continue;
                _evidenceObjData[obj].Collected = true;
                _evidenceObjData.Remove(obj);
            }
        }
        
        private bool _coronerCalled;
        private bool _foNotified;

        private void TaskLeaveTheScene() //9.
        {
            if (!(Vector3.Distance(Game.LocalPlayer.Character.Position, CsiCreator.Victim.Position) > 80f)) return;

            _missionValue = _missionValue + 15;
            $"Mission value changed to: {_missionValue}".AddLog();
            SaveEndingData();

            foreach (var obj in _evidenceObjData.Keys.ToList())
            {
                if (!obj.@object.Exists() || !obj.IsCollected) continue;
                _missionValue = _missionValue + 5;
                $"Mission value changed to: {_missionValue}".AddLog();
            }

            var medal = MissionPassedScreen.Medal.Bronze;
            if (_missionValue > 60 && _missionValue < 85) medal = MissionPassedScreen.Medal.Silver;
            else if (_missionValue >= 85) medal = MissionPassedScreen.Medal.Gold;

            var handler = new MissionPassedHandler("Crime Scene Investiagion", _missionValue, medal);

            handler.AddItem("Accepted Call", "Accepted", MissionPassedScreen.TickboxState.Tick);
            handler.AddItem("Updated by First Officer", "Completed", MissionPassedScreen.TickboxState.Tick);
            handler.AddItem("Updated by EMS", "Completed", MissionPassedScreen.TickboxState.Tick);
            handler.AddItem("Body taken to ME", "Completed", MissionPassedScreen.TickboxState.Tick);

            foreach (var evid in _eDataList)
            {
                if (evid.Collected) handler.AddItem(evid.Name, "Collected", MissionPassedScreen.TickboxState.Tick);
                else handler.AddItem(evid.Name, "Missed", MissionPassedScreen.TickboxState.Empty);
            }

            var witnessNum = 0;
            foreach (var wit in _witList.Keys.ToList())
            {
                witnessNum++;
                var tick = wit.IsCollected
                    ? MissionPassedScreen.TickboxState.Tick
                    : MissionPassedScreen.TickboxState.Empty;
                handler.AddItem($"Witness {witnessNum} Statement Taken", "", tick);
            }
            
            handler.AddItem("Scene Cleared", "Completed", MissionPassedScreen.TickboxState.Tick);
            
            handler.Show();
            
            Functions.PlayScannerAudio("ATTN_DISPATCH CODE_04_PATROL");

            "Terminating L.S. Noir Callout: Sexual Assault -- Stage 1 [CSI]".AddLog();
            SetScriptFinished();
        }
        #endregion

        private void SaveEndingData()
        {
            _caseData.SajrsUpdates.Add("Scene investigated");
            _caseData.CompletedStages.Add(CaseData.LastStage.CSI);
            _caseData.CurrentStage = CaseData.LastStage.CSI;
            _caseData.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            _caseData.CurrentSuspect = "";
            SaveItemToXML<CaseData>(_caseData, Main.CDataPath);
            _pDataList.Add(_vicData);
            if (_wDataList.Count > 0) SaveItemToXML<List<PedData>>(_wDataList, Main.WDataPath);
            SaveItemToXML<List<PedData>>(_sDataList, Main.SDataPath);
            SaveItemToXML<List<PedData>>(_pDataList, Main.PDataPath);
            SaveItemToXML<List<EvidenceData>>(_eDataList, Main.EDataPath);
            SaveItemToXML<List<ReportData>>(_rDataList, Main.RDataPath);
            _eController.Dispose();
            _missionValue = _missionValue + 5;
            $"Mission value changed to: {_missionValue}".AddLog();
        }

        protected override void End()
        { }

        protected void SetScriptFinished()
        {
            CsiCreator.End();
            if (ObjectElectronic.Exists()) ObjectElectronic.@object.Delete();
            if (ObjectDrink.Exists()) ObjectDrink.@object.Delete();
            if (ObjectFood.Exists()) ObjectFood.@object.Delete();

            _ems?.Dispose();
            _coroner?.Dispose();

            SetScriptFinished(true);
        }

        ~Sa1Csi()
        {
            SetScriptFinished();
        }

        public enum EnumState
        { EnRoute, OnScene, Ended }

        public enum CheckState
        { Start, Fo, Vic1, Vic2, Wit }


        private enum ServiceType { Ems, Coroner }
    }
}
