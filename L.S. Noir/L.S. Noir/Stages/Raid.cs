using LSNoir.Data;
using LSNoir.Resources;
using LSNoir.Scenes;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LSNoir.Stages
{
    //TODO: 
    // - can't remove suspect in End() - the player might want to transport the arrestee to the station

    //https://github.com/Fiskey111/Los-Santos-Noir-by-Fiskey111/blob/master/L.S.%20Noir/L.S.%20Noir/Callouts/SA/Stages/Sa_4CSuspectWork.cs

    /// <summary>
    /// Uses WitnessData to store suspect data
    /// <para>Uses SceneData to store cops and their vehicles</para> 
    /// <para>Might be used with any kind of cops, FIB agents etc. - models are customizable in SceneItem</para>
    /// </summary>
    public class Raid : BasicScript
    {
        //TECHNICAL REQUIREMENTS:
        // - StageData: SceneID, Notification details, Blip details, CallPos, 1 WitnessID (replace with Suspect?)
        // - SceneData stores info about cops and their vehicles

        private readonly StageData data;
        private readonly SceneWithIdProps scene;
        private readonly List<DriverFollow> followingVehs = new List<DriverFollow>();

        private Ped Player => Game.LocalPlayer.Character;

        private float DistToPlayer(Vector3 p) => Vector3.Distance(Player.Position, p);

        private const string MSG_EXIT_VEH = "Exit your vehicle to continue with the raid setup.";
        private const string MSG_OFFICERS_READY = "Officers are ready. Enter your vehicle to get directions to location";
        private const string MSG_PRESS_TO_PREPARE = "To have your officers prepare for the raid, press ~y~{0}~w~";
        private const string MSG_DECIDE_WHEN_RAID = "~w~Decide when you would like the raid to occur" +
                                                    "\nMorning (0600) ~y~1~w~" +
                                                    "\nMidday (1200) ~y~2~w~" +
                                                    "\nEvening (1800) ~y~3~w~" +
                                                    "\nNight (2300) ~y~4~w~" +
                                                    "\nCurrent Time {0}00 ~y~5~w~";

        private const string MSG_SEARCH = "Search for the ~r~suspect~s~";

        private const string SCANNER_AT_SCENE = "ATTENTION_ALL_UNITS_05 OFFICERS_AT_SCENE";
        private const string SCANNER_ENROUTE = "ATTN_DISPATCH OFFICER EN_ROUTE_CODE2 WARRANT_ISSUED";
        private const string SCANNER_CAUTION = "OFFICERS_AT_SCENE PROCEED_CAUTION";

        private const string MSG_LEAVE = "Leave the area.";
        private const float DIST_COPS_AT_SCENE = 7;

        private const Keys KEY_PREPARE = Keys.Y;
        private Blip blipSuspectArea;
        private Blip blipMeetingArea;
        private Ped suspect;
        private PedScenarioLoop pedScenarioHelper;
        private LHandle pursuit;
        private readonly string suspectsGun = MathHelper.Choose("WEAPON_CARBINERIFLE", "WEAPON_PISTOL", "WEAPON_KNIFE");

        private Vector3 crewDestination = new Vector3();
        private Vector3 posToCalcLeaveDist = new Vector3();

        public Raid(StageData stageData)
        {
            data = stageData;

            var sceneId = data.SceneID;
            var sceneData = data.ParentCase.GetSceneData(sceneId);
            scene = new SceneWithIdProps(sceneData);
        }

        protected override bool Initialize()
        {
            Base.SharedStageMethods.DisplayNotification(data);

            blipMeetingArea = new Blip(data.CallPosition)
            {
                Sprite = data.CallBlipSprite,
                Color = ColorTranslator.FromHtml(data.CallBlipColor),
                Name = data.CallBlipName,
            };

            ActivateStage(Away);

            return true;
        }

        private void Away()
        {
            if(DistToPlayer(data.CallPosition) < 90)
            {
                RemoveVehiclesFromSpawnPoint(data.CallPosition, 35);

                scene.Create();

                ActivateStage(IsAnySceneItemInvalid);

                SwapStages(Away, AwaitingArrival);
            }
        }

        private void IsAnySceneItemInvalid()
        {
            if(scene.Peds.Any(p => !p))
            {
                throw new NullReferenceException("An invalid ped was found!");
            }
            if(scene.Vehicles.Any(v =>!v))
            {
                throw new NullReferenceException("An invalid veh was found!");
            }
        }

        private void RemoveVehiclesFromSpawnPoint(Vector3 pos, float radius)
        {
            foreach (var veh in World.GetAllVehicles())
            {
                if (veh && Vector3.Distance(pos, veh) < radius)
                {
                    if (veh != Player.CurrentVehicle)
                    {
                        veh.Delete();
                    }
                }
            }
        }
        
        private void AwaitingArrival()
        {
            if (DistToPlayer(data.CallPosition) < 15)
            {
                if (blipMeetingArea) blipMeetingArea.Delete();

                Functions.PlayScannerAudio(SCANNER_AT_SCENE);

                Game.DisplayHelp(MSG_EXIT_VEH, true);

                SwapStages(AwaitingArrival, PlayerExitVeh);
            }
            else if(DistToPlayer(data.CallPosition) > 120)
            {
                scene.Dispose();

                SwapStages(AwaitingArrival, Away);
            }
        }

        private void PlayerExitVeh()
        {
            if(!Player.IsInAnyVehicle(false))
            {
                Game.HideHelp();

                NotifyToPickTime();

                SwapStages(PlayerExitVeh, WaitForDecision);
            }
        }

        private void NotifyToPickTime()
        {
            var hr = World.DateTime.Hour.ToString();
            Game.DisplayHelp(string.Format(MSG_DECIDE_WHEN_RAID, hr), true);
        }

        private void WaitForDecision()
        {
            int time = 0;

            if (Game.IsKeyDown(Keys.D1)) time = 6;
            else if (Game.IsKeyDown(Keys.D2)) time = 12;
            else if (Game.IsKeyDown(Keys.D3)) time = 18;
            else if (Game.IsKeyDown(Keys.D4)) time = 23;
            else if (Game.IsKeyDown(Keys.D5)) time = 1;

            if (time == 0) return;

            Game.HideHelp();

            if (time != 1)
            {
                var cameraInterpolator = new CameraInterpolator();
                cameraInterpolator.Start(Player.AbovePosition, 0);

                TimeWarp(time);

                cameraInterpolator.Stop();
            }

            Game.DisplayHelp(string.Format(MSG_PRESS_TO_PREPARE, KEY_PREPARE));

            SwapStages(WaitForDecision, AwaitingAcceptance);
        }
        
        private static void TimeWarp(int time)
        {
            World.IsTimeOfDayFrozen = false;

            while (World.DateTime.Hour != time)
            {
                World.DateTime = World.DateTime.AddMinutes(1);
                GameFiber.Yield();
            }
        }

        private void AwaitingAcceptance()
        {
            if (Game.IsKeyDown(KEY_PREPARE))
            {
                scene.PedsEnterTheirVeh();

                Game.DisplayHelp(MSG_OFFICERS_READY);

                SwapStages(AwaitingAcceptance, AllOfficersAndPlayerInVehicles);
            }
        }

        private void AllOfficersAndPlayerInVehicles()
        {
            if (scene.Peds.All(o => o.IsInAnyVehicle(false)) && Player.IsInAnyVehicle(false))
            {
                var witId = data.WitnessID[0];
                var witData = data.ParentCase.GetWitnessData(witId);
                crewDestination = World.GetNextPositionOnStreet(witData.Spawn.Position);
                
                GameFiber.StartNew(() => SpawnSuspect(witData));

                blipSuspectArea = new Blip(witData.Spawn.Position, 40f)
                {
                    Color = Color.Yellow,
                    Alpha = 0.45f,
                    RouteColor = Color.Yellow,
                    IsRouteEnabled = true,
                };

                Functions.PlayScannerAudioUsingPosition(SCANNER_ENROUTE, witData.Spawn.Position);

                TaskDriveToDest();

                SwapStages(AllOfficersAndPlayerInVehicles, ClosingIn);
            }
        }

        private void SpawnSuspect(WitnessData witData)
        {
            suspect = new Ped(witData.Model, witData.Spawn.Position, witData.Spawn.Heading);
            suspect.MakePersistent();
            pedScenarioHelper = new PedScenarioLoop(suspect, witData.Scenario);
            pedScenarioHelper.IsActive = true;
        }

        private void TaskDriveToDest()
        {
            var drivers = scene.Vehicles.Select(v => v.GetPedOnSeat(-1)).ToArray();
            var leaderSpeed = 10;

            for (int i = 0; i < drivers.Length; i++)
            {
                if (i == 0)
                {
                    drivers[i].Tasks.DriveToPosition(crewDestination, leaderSpeed, VehicleDrivingFlags.Emergency);
                }
                else
                {
                    var d = new DriverFollow(drivers[i], drivers[i - 1], leaderSpeed);
                    d.Start();
                    followingVehs.Add(d);

                }
            }
        }

        private void ClosingIn()
        {
            if(DistToPlayer(blipSuspectArea.Position) < 30)
            {
                Functions.PlayScannerAudio(SCANNER_CAUTION);

                if (blipSuspectArea) blipSuspectArea.Delete();

                Game.DisplayHelp(MSG_SEARCH);

                SwapStages(ClosingIn, AreAtScene);
            }
        }

        private void AreAtScene()
        {
            if (AreVehiclesAtScene(scene.Vehicles))
            {
                followingVehs.ForEach(d => d.Stop());

                scene.Peds.Where(p => p).ToList().ForEach(p => p.Tasks.LeaveVehicle(LeaveVehicleFlags.None));

                scene.Peds.Where(p => p).ToList().ForEach(p => p.Inventory.GiveNewWeapon(WeaponHash.Pistol, 100, true));

                SwapStages(AreAtScene, AreOfficersOutVehicles);
            }
        }

        private bool AreVehiclesAtScene(List<Vehicle> vehs)
        {
            return Vector3.Distance(vehs[0].Position, crewDestination) < 5 && vehs.All(v => v.Speed == 0);
        }

        private void AreOfficersOutVehicles()
        {
            if(!scene.Peds.All(p=>p.IsInAnyVehicle(false)))
            {
                scene.Peds.ForEach(p => p.Tasks.GoToWhileAiming(suspect, 3, 15));

                SwapStages(AreOfficersOutVehicles, Search);
            }
        }

        private void Search()
        {
            if (NativeFunction.Natives.HAS_ENTITY_CLEAR_LOS_TO_ENTITY<bool>(Player, suspect, 17))
            {
                if (!blipMeetingArea)
                {
                    blipMeetingArea = new Blip(suspect);
                    blipMeetingArea.Scale = 0.75f;
                }
            }
            else
            {
                if (blipMeetingArea)
                {
                    blipMeetingArea.Delete();
                }
            }

            if (DistToPlayer(suspect.Position) < 10)
            {
                if(pedScenarioHelper != null && pedScenarioHelper.IsActive)
                {
                    pedScenarioHelper.IsActive = false;
                }

                SuspectReaction(suspect);

                SwapStages(Search, CanFinish);
            }
        }

        private void CanFinish()
        {
            if (pursuit != null && !Functions.IsPursuitStillRunning(pursuit))
            {
                posToCalcLeaveDist = suspect.Position;

                SwapStages(CanFinish, NotifyLeaveTheArea);
            }

            if (Functions.IsPedArrested(suspect) || suspect.IsDead)
            {
                posToCalcLeaveDist = suspect.Position;

                SwapStages(CanFinish, NotifyLeaveTheArea);
            }
        }

        private void NotifyLeaveTheArea()
        {
            Game.DisplaySubtitle(MSG_LEAVE, 100);

            if(DistToPlayer(posToCalcLeaveDist) > 80)
            {
                SetScriptFinishedSuccessfulyAndSave();
            }
        }

        private void SuspectReaction(Ped s)
        {
            var rnd = MathHelper.GetRandomInteger(50);

            if (rnd < 4)
            {
                pursuit = Functions.CreatePursuit();
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                Functions.AddPedToPursuit(pursuit, s);
            }
            else if (rnd < 8)
            {
                var weapon = new Weapon(new WeaponAsset(suspectsGun), s.Position, 200);
                weapon.GiveTo(s);
                s.KeepTasks = true;
                s.RelationshipGroup = new RelationshipGroup("Suspect");
                s.RelationshipGroup.SetRelationshipWith(Game.LocalPlayer.Character.RelationshipGroup, Relationship.Hate);
                s.RelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
                s.Tasks.FightAgainstClosestHatedTarget(50f);
            }
        }

        protected override void Process()
        {
            if(Game.IsKeyDown(Keys.End))
            {
                SetScriptFinishedSuccessfulyAndSave();
            }
        }

        protected override void End()
        {
            if (blipMeetingArea) blipMeetingArea.Delete();
            scene?.Dispose();
            suspect?.Delete();
        }

        //~Raid()
        //{
        //    End();
        //}

        private void SetScriptFinishedSuccessfulyAndSave()
        {
            DisplayMissionPassedScreen();

            data.SetThisAsLastStage();

            SetScriptFinished(true);
        }

        private void DisplayMissionPassedScreen()
        {
            //var value = 100;

            //if (suspect)
            //{
            //    if (suspect.IsDead) value = 70;
            //    else if (Functions.IsPedArrested(suspect)) value = 100;
            //}

            //value = scene.Peds.Where(c => c && c.IsDead).Aggregate(value, (current, c) => current - 10);

            //var medal = MissionPassedScreen.Medal.Gold;
            //if (value >= 80 && value < 100) medal = MissionPassedScreen.Medal.Silver;
            //else if (value < 80) medal = MissionPassedScreen.Medal.Bronze;

            //var passed = new MissionPassedHandler("Suspect Raid", value, medal);

            //passed.AddItem("Suspect Arrested", "", Functions.IsPedArrested(suspect) ? MissionPassedScreen.TickboxState.Tick : MissionPassedScreen.TickboxState.Empty);

            //passed.AddItem("All Officers Survived", "", scene.Peds.Any(c => c && c.IsDead) ? MissionPassedScreen.TickboxState.Empty : MissionPassedScreen.TickboxState.Tick);

            //passed.Show();
        }
    }
}
