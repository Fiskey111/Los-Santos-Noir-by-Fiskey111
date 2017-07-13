using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using Fiskey111Common;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.SA.Objects;
using LSNoir.Callouts.SA.Services;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using Color = System.Drawing.Color;

namespace LSNoir.Callouts.SA.Stages
{
    public class Sa_4CSuspectWork : BasicScript
    {
        // System
        private bool _stageCreated;
        internal const string SaPath = @"Plugins\LSPDFR\LSNoir\SA\Data\ServiceCoordinates.xml";
        private XDocument Xml;
        private List<Cop> _copList = new List<Cop>();

        // Positions
        private SpawnPt _susSpawn;
        private Vector3 _position;

        // Entities
        private Blip _areaBlip;
        private Cop _swat11, _swat12, _swat21, _swat22, _swat3, _swat4, _traffic1, _traffic2;
        private Ped _sus;

        // Data
        private CaseData _cData;
        private PedData _sData;

        // Misc
        private LHandle _pursuit;
        private ScenarioHelper _scenario = new ScenarioHelper();

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 4c [Suspect Work]".AddLog();

            Xml = XDocument.Load(SaPath);

            _sData = LtFlash.Common.Serialization.Serializer.GetSelectedListElementFromXml<PedData>(Main.SDataPath,
                c => c.FirstOrDefault(s => s.Type == PedType.Suspect));
            _cData = LtFlash.Common.Serialization.Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);

            "Sexual Assault Case Update".DisplayNotification(
                "Meet with your ~b~team~w~ to perform a raid on the ~r~suspect's~w~ work", _cData.Number);

            CreateStage();
            
            while (!_stageCreated)
                GameFiber.Yield();

            _position = GetLocation("Mortuary", "Meeting", "Blip").Spawn;
            _areaBlip = new Blip(_position)
            {
                Sprite = BlipSprite.GangAttackPackage,
                Color = Color.DarkOrange,
                Name = "Meet with SWAT Team"
            };
            
            ActivateStage(AwaitingArrival);
            return true;
        }

        private void AwaitingArrival()
        {
            if (!_areaBlip.Exists() || Vector3.Distance(Game.LocalPlayer.Character.Position, _position) > 15f) return;
            if (_areaBlip) _areaBlip.Delete();
            Functions.PlayScannerAudio("ATTENTION_ALL_UNITS_05 OFFICERS_AT_SCENE");

            Game.DisplayHelp("Exit your vehicle to continue with the raid setup.");

            SwapStages(AwaitingArrival, AwaitingExitVehicle);
        }

        private bool _timePicked, _notified;
        private static uint _notification;
        private void AwaitingExitVehicle()
        {
            if (Game.LocalPlayer.Character.IsInAnyVehicle(false)) return;

            if (!_notified)
            {
                _notified = true;
                var hr = World.DateTime.ToUniversalTime().Hour;
                Game.DisplayHelp("~w~Decide when you would like the raid to occur" +
                                                         "\nMorning (0600) ~y~1~w~" +
                                                         "\nMidday (1200) ~y~2~w~" +
                                                         "\nEvening (1800) ~y~3~w~" +
                                                         "\nNight (2300) ~y~4~w~" +
                                                         $"\nCurrent Time {hr}00 ~y~5~w~", true);
            }

            var time = 0;

            if (Game.IsKeyDown(Keys.D1))
                time = 6;
            else if (Game.IsKeyDown(Keys.D2))
                time = 12;
            else if (Game.IsKeyDown(Keys.D3))
                time = 18;
            else if (Game.IsKeyDown(Keys.D4))
                time = 23;
            else if (Game.IsKeyDown(Keys.D5))
                time = 1;

            if (time == 0) return;

            _timePicked = true;
            Game.HideHelp();

            if (time != 1)
                TimeWarp(time);
            else
            {
                Game.DisplayHelp("To have your officers prepare for the raid, press ~y~Y~w~");

                SwapStages(AwaitingExitVehicle, AwaitingAcceptance);
            }
        }

        private void TimeWarp(int time)
        {
            if (World.IsTimeOfDayFrozen) World.IsTimeOfDayFrozen = false;
            GameFiber.StartNew(delegate
            {
                CamClass.FocusCamOnObjectWithInterpolation(Game.LocalPlayer.Character.AbovePosition, 0f);
                var checkTime = time;
                while (World.DateTime.Hour != checkTime)
                {
                    var finalDate = World.DateTime.AddMinutes(1d);
                    World.DateTime = finalDate;
                    GameFiber.Yield();
                }
                CamClass.InterpolateCameraBack();

                Game.DisplayHelp("To have your officers prepare for the raid, press ~y~Y~w~");

                SwapStages(AwaitingExitVehicle, AwaitingAcceptance);
            });
        }

        private void AwaitingAcceptance()
        {
            if (!Game.IsKeyDown(Keys.Y)) return;

            foreach (var c in _copList)
                c.EnterVehicle();

            Game.DisplayHelp("Officers are ready. Enter your vehicle to get directions to location");
            SwapStages(AwaitingAcceptance, WaitingToGo);
        }

        private void WaitingToGo()
        {
            if (!Game.LocalPlayer.Character.IsInAnyVehicle(false)) return;
            _areaBlip = new Blip(GetLocation("Mortuary", "Suspect", "Blip").Spawn, 40f)
            {
                Color = Color.Yellow,
                Alpha = 0.45f
            };
            _areaBlip.EnableRoute(Color.Yellow);

            while (_copList.Any(c => !c.Ped.IsInAnyVehicle(false)))
                GameFiber.Yield();

            foreach (var c in _copList)
                c.DriveToTargetPosition();

            Functions.PlayScannerAudioUsingPosition("ATTN_DISPATCH OFFICER EN_ROUTE_CODE2 WARRANT_ISSUED",
                _sus.Position);

            SwapStages(WaitingToGo, AwaitingArriveAtScene);
        }

        private void AwaitingArriveAtScene()
        {
            if (!(Game.LocalPlayer.Character.Position.DistanceTo(_areaBlip.Position) < 30f)) return;
            Functions.PlayScannerAudio("OFFICERS_AT_SCENE PROCEED_CAUTION");
            if (_areaBlip.Exists()) _areaBlip.Delete();
            
            ContinueDrivingTask();

            SwapStages(AwaitingArriveAtScene, OnScene);
        }

        private void ContinueDrivingTask()
        {
            GameFiber.StartNew(delegate
            {
                var sw = new Stopwatch();
                sw.Start();

                while (_copList.Count > 0 && _copList.Any(c => c.Ped && !c.IsSecondCop && c.Ped.DistanceTo(c.TargetPosition.Spawn) > 7f) || sw.Elapsed.Seconds < 15)
                {
                    foreach (var cop in _copList.Where(c => c.Position.DistanceTo(c.TargetPosition.Spawn) <= 7f && c.Ped.IsInAnyVehicle(false)))
                    {
                        if (!cop.Ped) return;
                        if (MathHelper.GetRandomInteger(3) == 1 && !cop.IsSecondCop) cop.TurnOnSiren();
                        cop.ExitVehicle();
                        cop.SetNotBusy();
                        if (cop.IsSecondCop) cop.Ped.Tasks.GoToOffsetFromEntity(Game.LocalPlayer.Character, MathHelper.GetRandomSingle(0, 4f),
                            MathHelper.GetRandomSingle(0, 4f), 8f);
                    }             
                    GameFiber.Yield();
                    if (this.HasFinished) break;
                }
            });
        }

        private void OnScene()
        {
            Game.DisplayHelp("Search for the ~r~suspect");
            "Sexual Assault Case Update".DisplayNotification(
                "Officers on scene\nBeginning search for ~r~suspect", _cData.Number);

            SwapStages(OnScene, Searching);
        }

        private bool _isPursuit, _isBusy;
        private void Searching()
        {
            if (NativeFunction.Natives.HAS_ENTITY_CLEAR_LOS_TO_ENTITY<bool>(Game.LocalPlayer.Character, _sus, 17))
            {
                if (!_areaBlip.Exists())
                    _areaBlip = new Blip(_sus) {Scale = 0.75f};
            }

            if (!NativeFunction.Natives.HAS_ENTITY_CLEAR_LOS_TO_ENTITY<bool>(Game.LocalPlayer.Character, _sus, 17))
                if (_areaBlip.Exists()) _areaBlip.Delete();

            if (Game.LocalPlayer.Character.Position.DistanceTo(_sus) < 10f && !_isPursuit && !_isBusy)
            {
                var helper = MathHelper.GetRandomInteger(50);
                switch (helper)
                {
                    case 1:
                    case 2:
                    case 3:
                        if (_scenario.Exists)
                            if (_scenario.IsRunning) _scenario.Stop();

                        _isPursuit = true;
                        _pursuit = Functions.CreatePursuit();
                        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        Functions.AddPedToPursuit(_pursuit, _sus);
                        _isBusy = true;
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        if (_scenario.Exists)
                            if (_scenario.IsRunning) _scenario.Stop();
                        string[] gun = {"WEAPON_CARBINERIFLE", "WEAPON_PISTOL", "WEAPON_KNIFE"};
                        var weapon = new Weapon(new WeaponAsset(gun[MathHelper.GetRandomInteger(gun.Length)]), _sus.Position, 200);
                        weapon.GiveTo(_sus);
                        _sus.KeepTasks = true;
                        _sus.RelationshipGroup = new RelationshipGroup("Suspect");
                        _sus.RelationshipGroup.SetRelationshipWith(Game.LocalPlayer.Character.RelationshipGroup, Relationship.Hate);
                        _sus.RelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
                        _sus.Tasks.FightAgainstClosestHatedTarget(50f);
                        _isBusy = true;
                        break;
                }
            }

            if (Functions.IsPedGettingArrested(_sus) || Functions.IsPedStoppedByPlayer(_sus))
                if (_scenario.Exists)
                    if (_scenario.IsRunning) _scenario.Stop();

            if (_isPursuit && !Functions.IsPursuitStillRunning(_pursuit))
                this.SetScriptFinished();

            if (!Functions.IsPedArrested(_sus) && !_sus.IsDead) return;
            
            this.SetScriptFinished();
        }

        protected override void Process() { }

        protected override void End() { }

        protected void SetScriptFinished()
        {
            "Sexual Assault Case Update".DisplayNotification("Case completed successfully \n\n~g~Good job ~b~Officer~w~!", _cData.Number);
            _cData.LastCompletedStage = CaseData.LastStage.SuspectWork;
            _cData.CompletedStages.Add(CaseData.LastStage.SuspectWork);
            _cData.SajrsUpdates.Add("Case completed!");
            _cData.StartingStage = "Sa1Csi";
            LtFlash.Common.Serialization.Serializer.SaveItemToXML<CaseData>(_cData, Main.CDataPath);

            var value = 100;

            if (_sus)
            {
                if (_sus.IsDead) value = 70;
                else if (Functions.IsPedArrested(_sus)) value = 100;
            }

            value = _copList.Where(c => c.Ped && c.Ped.IsDead).Aggregate(value, (current, c) => current - 10);

            var medal = MissionPassedScreen.Medal.Gold;
            if (value >= 80 && value < 100) medal = MissionPassedScreen.Medal.Silver;
            else if (value < 80) medal = MissionPassedScreen.Medal.Bronze;

            var passed = new MissionPassedHandler("Suspect Raid", value, medal);

            passed.AddItem("Suspect Arrested", "", Functions.IsPedArrested(_sus) ? MissionPassedScreen.TickboxState.Tick : MissionPassedScreen.TickboxState.Empty);
            passed.AddItem("All Officers Survived", "", _copList.Any(c => c.Ped && c.Ped.IsDead) ? MissionPassedScreen.TickboxState.Empty : MissionPassedScreen.TickboxState.Tick);
            
            passed.Show();

            if (_areaBlip.Exists()) _areaBlip.Delete();
            foreach (var c in _copList)
            {
                c.DismissPed();
                c.DismissVeh();
            }
            SetScriptFinished(true);
        }

        private void CreateStage()
        {
            var vehs = World.GetAllVehicles();
            foreach (var v in vehs)
            {
                if (!v || Game.LocalPlayer.Character.CurrentVehicle == v) continue;
                if (v.DistanceTo(GetLocation("Mortuary", "Meeting", "P2").Spawn) > 35f) continue;
                v.Delete();
            }

            _swat11 = new Cop("POLICE4", GetLocation("Mortuary", "Meeting", "P1"),
                GetLocation("Mortuary", "Location", "P1"), true, _copList);
            _swat12 = new Cop(_swat11.Veh, true, _copList);
            _swat21 = new Cop("FBI", GetLocation("Mortuary", "Meeting", "P2"),
                GetLocation("Mortuary", "Location", "P2"), true, _copList);
            _swat22 = new Cop(_swat21.Veh, true, _copList);
            _swat3 = new Cop("FBI2", GetLocation("Mortuary", "Meeting", "P3"),
                GetLocation("Mortuary", "Location", "P3"), true, _copList);
            _swat4 = new Cop("POLICE4", GetLocation("Mortuary", "Meeting", "P4"),
                GetLocation("Mortuary", "Location", "P4"), true, _copList);
            _traffic1 = new Cop("Police2", GetLocation("Mortuary", "Meeting", "T1"),
                GetLocation("Mortuary", "Location", "T1"), false, _copList);
            _traffic2 = new Cop("Police3", GetLocation("Mortuary", "Meeting", "T2"),
                GetLocation("Mortuary", "Location", "T2"), false, _copList);

            var loc = MathHelper.GetRandomInteger(1, 5);
            var name = "";
            if (loc == 1)
                name = "Wall";
            else if (loc == 2)
                name = "Clip";
            else if (loc == 3)
                name = "Clip2";
            else if (loc == 4)
                name = "Tool";
            else
                name = "Phone";

            _susSpawn = GetLocation("Mortuary", "Suspect", name);
            _sus = new Ped(_sData.Model, _susSpawn.Spawn, _susSpawn.Heading);
            SetAnimation(name);

            _stageCreated = true;
        }

        /// <param name="location">e.g. Mortuary, etc</param>
        /// <param name="type">Location/Meeting/Suspect</param>
        /// <param name="name">P1/T1 or Wall/Clip/Clip2/Tool/Phone</param>
        private SpawnPt GetLocation(string location, string type, string name)
        {
            try
            {
                var loc = "SusWork/" + location + "/" + type + "/" + name;

                return new SpawnPt(float.Parse(Xml.Root.XPathSelectElement(loc + "/h").Value, CultureInfo.InvariantCulture), float.Parse(Xml.Root.XPathSelectElement(loc + "/x").Value, CultureInfo.InvariantCulture),
                    float.Parse(Xml.Root.XPathSelectElement(loc + "/y").Value, CultureInfo.InvariantCulture), float.Parse(Xml.Root.XPathSelectElement(loc + "/z").Value, CultureInfo.InvariantCulture));
            }
            catch (Exception e)
            {
                Game.LogTrivial("[L.S. Noir ERROR: " + e);
                return new SpawnPt(0, Vector3.Zero);
            }
        }

        private void SetAnimation(string type)
        {
            if (type == "Wall")
                _sus.Tasks.PlayAnimation("amb@world_human_leaning@male@wall@back@smoking@idle_a", "idle_a", 1f,
                    AnimationFlags.Loop);
            else if (type == "Clip" || type == "Clip2")
                StartScenario("WORLD_HUMAN_CLIPBOARD");
            else if (type == "Tool")
                StartScenario("WORLD_HUMAN_STAND_MOBILE");
        }

        private void StartScenario(string type)
        {
            _scenario = new ScenarioHelper(_sus, type);
            _scenario.Start();
        }
    }
}
