using LSNoir.Data;
using LSNoir.Resources;
using LSNoir.Scenes;
using LSPD_First_Response.Mod.API;
using LtFlash.Common;
using LtFlash.Common.EvidenceLibrary;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LSNoir.Stages
{
    struct MedicalExaminerData
    {
        public Vector3 Position;
        public Vector3 MarkerEntrance;
        public Vector3 MarkerExit;
        public SpawnPoint VehicleSpawn;
        public SpawnPoint DriverSpawn;
        public string Name;
        public bool TransportRequired;
        public Vector3 MarkerOffice;
    }

    //https://github.com/Fiskey111/Los-Santos-Noir-by-Fiskey111/blob/master/L.S.%20Noir/L.S.%20Noir/Callouts/SA/Stages/ME/ME_Creator.cs

    /// <summary>
    /// Requires 2 DialogID stored in StageData.DialogsID[0] and [1]
    /// </summary>
    public class MedicalExaminerStage : BasicScript
    {
        //TODO:
        // - get dialog data from StageData
        // - add data to MEData
        // - replace distances with const float?
        // - extract common method from Driving and DrivingBack?
        // - blink minimap on start
        // - add tex to StageData: 
        //   Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Case #: " + number, subtitle, body);

        //NOTES:

        private Vector3 PlayerPos => Game.LocalPlayer.Character.Position;
        private Ped Player => Game.LocalPlayer.Character;
        private static float DistToPlayer(Vector3 p) => Vector3.Distance(Game.LocalPlayer.Character.Position, p);

        private Keys KeySkipDrive = Keys.D0;
        private Keys KeyWarpPlayerIntoVeh = Keys.F;

        private Camera gameCam;
        private Camera swapCam;

        private static readonly MedicalExaminerData MELS = new MedicalExaminerData()
        {
            Name = "Medical Examiner's Office LS",
            Position = new Vector3(240, -1380, 34),
            //VehicleSpawn = new SpawnPoint(, new Vector3()),
            TransportRequired = false,
            MarkerOffice = new Vector3(237.67f, -1367.89f, 39.53f),
            MarkerEntrance = new Vector3(240, -1380, 34),
            //NOTE: unconfirmed coord
            MarkerExit = new Vector3(252, -1366, 40),
        };

        private static readonly MedicalExaminerData MEPB = new MedicalExaminerData()
        {
            Name = "Paleto Bay Sheriffs Office",
            Position = new Vector3(-452, 6038, 32),
            VehicleSpawn = new SpawnPoint(313, new Vector3(-453, 6034, 31)),
            TransportRequired = true,
        };

        private static readonly MedicalExaminerData MESS = new MedicalExaminerData()
        {
            Name = "Sandy Shores Sheriffs Office",
            Position = new Vector3(1840, 3673, 34),
        };

        private static readonly MedicalExaminerData[] MEOffices = { MELS, MEPB, MESS };

        private MedicalExaminerData me = MEOffices.OrderBy(o => DistToPlayer(o.Position)).FirstOrDefault();

        private const string SCENARIO_DRIVER = "WORLD_HUMAN_COP_IDLES";
        private const string MODEL_DRIVER = "s_m_m_highsec_01";
        private const string MODEL_ME = "s_m_m_doctor_01";
        private const string MODEL_CAR = "FBI";
        private const string MSG_PRESS_TO_SKIP = "Press ~y~{0}~s~ to skip the drive.";
        private const string MSG_TALK_TO_DRIVER = "Approach the ~y~Driver~w~ to go to the Medical Examiner's Office";
        private const string MSG_EXIT_OFFICE = "Now that you have the report, ~y~exit~w~ the building";
        private const string MSG_GOTO_ME = "Enter the driver's car or go to the marked destination";
        private const string MSG_ENTER_OFFICE = "Head to the ~y~marker~w~ to enter the Medical Examiner's office";
        private const string MSG_TALK_ME = "Go talk to the ~g~Medical Examiner~w~ in the office";
        private const string MSG_PRESS_TALK_ME = "Press ~y~Y~w~ to talk to the ~g~Medical Examiner~w~.";
        private const string MSG_DRIVER_GOOD_LUCK = "[Driver] Here you are! Good luck!";
        private const string MSG_DRIVER_GO = "[Driver] Off we go!";
        private const string SCANNER_FINISH = "ATTN_DISPATCH CODE_04_PATROL";

        private const VehicleDrivingFlags DRIVING_FLAGS = VehicleDrivingFlags.Normal | VehicleDrivingFlags.StopAtDestination | VehicleDrivingFlags.DriveAroundObjects;

        private IScene sceneOffice;

        private Ped medExaminer;
        private Ped driver;
        private PedScenarioLoop driverScenario;
        private Vehicle meCar;

        private readonly Stopwatch timerTipSkipDrive = new Stopwatch();

        private StageData data;

        private Dialog driverDialog;
        private Dialog meDialog;

        private Blip mainBlip;

        private IMarker markerEntrance;
        private IMarker markerExit;
        private IMarker markerOffice;

        public MedicalExaminerStage(StageData stageData) : base()
        {
            data = stageData;
        }

        protected override bool Initialize()
        {
            mainBlip = new Blip(me.Position)
            {
                Sprite = BlipSprite.GangAttackPackage,
                Color = Color.DarkOrange,
                Name = "Medical Examiner"
            };

            NativeFunction.Natives.FlashMinimapDisplay();

            //DisplayNotification(data.ParentCase.Name, "Visit ~o~Medical Examiner~w~ for update", caseNo);
            Base.SharedStageMethods.DisplayNotification(data);

            var nextStage = me.TransportRequired ? (Action)IsFarAway : IsPlayerCloseToOffice;
            ActivateStage(nextStage);

            return true;
        }

        protected override void Process()
        {
            //TODO: tests
            if (Game.IsKeyDown(Keys.End)) SetScriptFinished();
        }

        private void IsFarAway()
        {
            if(DistToPlayer(me.Position) < 80f)
            {
                driver = new Ped(MODEL_DRIVER, me.DriverSpawn.Position, me.DriverSpawn.Heading);
                driver.MakePersistent();

                driverScenario = new PedScenarioLoop(driver, SCENARIO_DRIVER);
                driverScenario.IsActive = true;

                meCar = new Vehicle(MODEL_CAR, me.VehicleSpawn.Position, me.VehicleSpawn.Heading);
                meCar.MakePersistent();

                SwapStages(IsFarAway, IsClose);
            }
        }

        private void PlayPedScenario()
        {
            if (DistToPlayer(driver.Position) < 15f)
            {
                driverScenario.IsActive = false;
                DeactivateStage(PlayPedScenario);
            }
        }

        private void IsClose()
        {
            if (DistToPlayer(me.Position) < 10f)
            {
                driver.Face(Player);

                Game.DisplayHelp(MSG_TALK_TO_DRIVER);

                SwapStages(IsClose, CanStartDialog);
            }
            else if(DistToPlayer(me.Position) > 85f)
            {
                //remove entities and back to the initial state
                if (driver) driver.Delete();
                if (meCar) meCar.Delete();

                SwapStages(IsClose, IsFarAway);
            }
        }

        private void CanStartDialog()
        {
            if(DistToPlayer(driver.Position) < 5f && Player.IsOnFoot)
            {
                //TODO: [Driver] you can go with me or take your own car, it's up to you
                var dialog = data.ParentCase.GetDialogData(data.DialogsID[0]);
                driverDialog = new Dialog(dialog.Dialog);

                driverDialog.PedOne = Player;
                driverDialog.PedTwo = driver;

                driverDialog.StartDialog();

                SwapStages(CanStartDialog, IsDialogDriverEnded);
            }
        }

        private void IsDialogDriverEnded()
        {
            if (driverDialog.HasEnded)
            {
                TaskMEDriveAway();

                SwapStages(IsDialogDriverEnded, EnteringTheCarToMELS);
            }
        }

        private void TaskMEDriveAway()
        {
            Game.DisplayHelp(MSG_GOTO_ME);

            driver.KeepTasks = true;
            driver.Tasks.GoStraightToPosition(meCar.LeftPosition, 3f, 0f, 0f, 1000).WaitForCompletion(3000);
            driver.Tasks.EnterVehicle(meCar, -1);
        }

        private void EnteringTheCarToMELS()
        {
            if(HandleEntering(MELS.VehicleSpawn.Position))
            {
                timerTipSkipDrive.Start();

                ActivateStage(TipSkipDriving);
                SwapStages(EnteringTheCarToMELS, Driving);
            }

            //player can drive to the destination by himself
            if(Player.IsInAnyVehicle(false) && Player.CurrentVehicle != meCar)
            {
                driver.Tasks.CruiseWithVehicle(24f);
                ActivateStage(RemoveDriverWhenAway);

                mainBlip.IsRouteEnabled = true;
                SwapStages(EnteringTheCarToMELS, IsPlayerCloseToOffice);
            }
        }

        private void RemoveDriverWhenAway()
        {
            if(DistToPlayer(driver.Position) > 80f)
            {
                driver.Delete();
                meCar.Delete();

                DeactivateStage(RemoveDriverWhenAway);
            }
        }

        private void TipSkipDriving()
        {
            if (timerTipSkipDrive.Elapsed.Seconds >= 35)
            {
                Game.DisplayHelp(string.Format(MSG_PRESS_TO_SKIP, KeySkipDrive));
                timerTipSkipDrive.Restart();
            }
        }

        private void Driving()
        {
            if (Game.IsKeyDown(KeySkipDrive) && DistToPlayer(MELS.VehicleSpawn.Position) > 10f)
            {
                DeactivateStage(TipSkipDriving);

                timerTipSkipDrive.Stop();

                SwapCamerasAndFadeOut();

                SwapStages(Driving, CamFollowMeCar);
            }
            else if(DistToPlayer(MELS.VehicleSpawn.Position) < 5f)
            {
                DeactivateStage(TipSkipDriving);

                timerTipSkipDrive.Stop();

                SwapStages(Driving, IsPlayerCloseToOffice);
            }
        }

        //NOTE: used when player skips the drive
        private void SwapCamerasAndFadeOut()
        {
            swapCam = new Camera(false);
            swapCam.Position = new Vector3(meCar.AbovePosition.X, meCar.AbovePosition.Y, meCar.AbovePosition.Z + 7);
            swapCam.PointAtEntity(meCar, Vector3.Zero, false);

            gameCam = GetCurrentGameCam();

            swapCam.Active = true;

            Game.FadeScreenOut(7500);
        }

        private static Camera GetCurrentGameCam()
        {
            var c = new Camera(false);
            c.FOV = NativeFunction.Natives.GET_GAMEPLAY_CAM_FOV<float>();
            c.Position = NativeFunction.Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
            Vector3 rot = NativeFunction.Natives.GET_GAMEPLAY_CAM_ROT<Vector3>(0);
            var rot1 = new Rotator(rot.X, rot.Y, rot.Z);
            c.Rotation = rot1;
            return c;
        }

        //NOTE: cam keeps it's pos but follows peds driving to the sunset
        private void CamFollowMeCar()
        {
            if(HandleCamFollowCar(swapCam, gameCam, meCar))
            {
                SwapStages(CamFollowMeCar, Transport);
            }
        }

        private static bool HandleCamFollowCar(Camera c, Camera gameCam, Vehicle v)
        {
            c.PointAtEntity(v, Vector3.Zero, false);

            if (Game.IsScreenFadedOut)
            {
                c.Active = false;
                c.Delete();
                gameCam.Active = true;

                return true;
            }
            return false;
        }

        //NOTE: player tp to ME LS from another office
        private void Transport()
        {
            meCar.Position = MELS.VehicleSpawn.Position;
            meCar.Heading = MELS.VehicleSpawn.Heading;

            Game.FadeScreenIn(1000, true);

            Game.DisplaySubtitle(MSG_DRIVER_GOOD_LUCK);

            Player.Tasks.LeaveVehicle(LeaveVehicleFlags.None);

            SwapStages(Transport, IsPlayerCloseToOffice);
        }

        //NOTE: common stage for +- Transport
        private void IsPlayerCloseToOffice()
        {
            if(DistToPlayer(MELS.MarkerEntrance) < 15f)
            {
                markerEntrance = new Marker(MELS.MarkerEntrance, Color.Orange);

                markerEntrance.Visible = true;

                Game.DisplayHelp(MSG_ENTER_OFFICE);

                SwapStages(IsPlayerCloseToOffice, CheckIfEntered);
            }
        }

        private void CheckIfEntered()
        {
            if (DistToPlayer(MELS.MarkerEntrance) <= 1f)
            {
                markerEntrance.Visible = false;

                var ci = new CameraInterpolator();
                ci.Start(new Vector3(219, -1422, 35), 200);

                Game.FadeScreenOut(1000, true);

                ci.Stop();

                if (mainBlip) mainBlip.Delete();

                if(meCar) meCar.Delete();
                if(driver) driver.Delete();

                //TODO: tp player here
                Player.Position = MELS.MarkerExit;

                Player.Heading = 180;

                InteriorHelper.IsCoronerInteriorEnabled = true;

                sceneOffice = new SceneMEOffice();

                Player.IsPositionFrozen = true;

                Game.LocalPlayer.HasControl = false;

                sceneOffice.Create();

                Player.IsPositionFrozen = false;

                Game.LocalPlayer.HasControl = true;

                medExaminer = new Ped(MODEL_ME, MELS.MarkerOffice, 0);
                medExaminer.MakePersistent();
                medExaminer.AttachBlip();

                Game.FadeScreenIn(1000, true);

                NotifyToSpeakME();

                SwapStages(CheckIfEntered, InME);
            }
        }

        private void NotifyToSpeakME()
        {
            GameFiber.Sleep(0500);

            markerOffice = new Marker(MELS.MarkerOffice, Color.Green);

            markerOffice.Visible = true;

            Game.DisplayHelp(MSG_TALK_ME);
        }
        
        private void InME()
        {
            if (DistToPlayer(medExaminer.Position) < 4f)
            {
                Game.DisplayHelp(MSG_PRESS_TALK_ME);

                markerOffice.Dispose();

                //TODO: add a proper dialog
                var dialog = data.ParentCase.GetDialogData(data.DialogsID[1]);

                meDialog = new Dialog(dialog.Dialog);

                meDialog.PedOne = Player;
                meDialog.PedTwo = medExaminer;

                meDialog.StartDialog();

                SwapStages(InME, FinishedTalkingWithME);
            }
        }

        private void FinishedTalkingWithME()
        {
            if(meDialog.HasEnded)
            {
                //ModifyCaseProgress(p => p.DialogsPassed.Add(meDialogID));
                //ModifyCaseProgress(p => p.ReportsReceived.Add(meReportID));

                //TODO: ME Report added to case files

                Game.DisplayHelp(MSG_EXIT_OFFICE);

                markerExit = new Marker(MELS.MarkerExit, Color.Red);

                markerExit.Visible = true;

                SwapStages(FinishedTalkingWithME, ExitingMe);
            }
        }

        private void ExitingMe()
        {
            if (DistToPlayer(me.MarkerExit) < 1f)
            {
                markerExit.Dispose();

                Player.IsPositionFrozen = true;

                Game.LocalPlayer.HasControl = false;

                Game.FadeScreenOut(1000, true);

                sceneOffice.Dispose();

                if(medExaminer) medExaminer.Delete();

                Player.Position = me.MarkerEntrance;

                Player.IsPositionFrozen = false;

                Game.LocalPlayer.HasControl = true;

                if (me.TransportRequired)
                {
                    CreateVehWDriver();

                    SwapStages(ExitingMe, Returning);

                }
                else
                {
                    SetScriptFinished();
                }


                Game.FadeScreenIn(1000, true);
            }
        }

        private void CreateVehWDriver()
        {
            meCar = new Vehicle(MODEL_CAR, MELS.VehicleSpawn.Position);
            meCar.Heading = MELS.VehicleSpawn.Heading;
            meCar.IsInvincible = true;
            meCar.TopSpeed = 100f;
            driver = new Ped(MODEL_DRIVER, MELS.DriverSpawn.Position, 0f);

            driver.WarpIntoVehicle(meCar, -1);
            meCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Both;
        }

        private void Returning()
        {
            if (Game.IsKeyDown(KeyWarpPlayerIntoVeh))
            {
                driver.WarpIntoVehicle(meCar, -1);
                Player.WarpIntoVehicle(meCar, -2);
            }

            if (driver.IsInVehicle(meCar, false) && Player.IsInVehicle(meCar, false))
            {
                Game.DisplaySubtitle(MSG_DRIVER_GO);

                driver.Tasks.DriveToPosition(me.VehicleSpawn.Position, 24f, DRIVING_FLAGS);

                timerTipSkipDrive.Start();

                ActivateStage(TipSkipDriving);

                SwapStages(Returning, DrivingBack);
            }
        }

        private void DrivingBack()
        {
            if (Game.IsKeyDown(KeySkipDrive) && DistToPlayer(me.VehicleSpawn.Position) > 10f)
            {
                DeactivateStage(TipSkipDriving);

                timerTipSkipDrive.Stop();

                SwapCamerasAndFadeOut();

                SwapStages(DrivingBack, CamFollowInWayBack);
            }
            else if (DistToPlayer(me.VehicleSpawn.Position) < 5f)
            {
                DeactivateStage(TipSkipDriving);

                timerTipSkipDrive.Stop();

                GameFiber.Sleep(1000);

                SetScriptFinished();
            }
        }

        private void CamFollowInWayBack()
        {
            if (HandleCamFollowCar(swapCam, gameCam, meCar))
            {
                SwapStages(CamFollowMeCar, Ending);
            }
        }

        //NOTE: player tp to the initial position!
        private void Ending()
        {
            meCar.Position = me.VehicleSpawn.Position;
            meCar.Heading = me.VehicleSpawn.Heading;

            Game.FadeScreenIn(1000, true);

            Game.DisplaySubtitle(MSG_DRIVER_GOOD_LUCK);

            Player.Tasks.LeaveVehicle(LeaveVehicleFlags.None);

            GameFiber.Sleep(1000);

            SetScriptFinished();
        }

        //NOTE: to handle both ways of transporting a player
        private bool HandleEntering(Vector3 destination)
        {
            if (Game.IsKeyDown(KeyWarpPlayerIntoVeh))
            {
                driver.WarpIntoVehicle(meCar, -1);
                Player.WarpIntoVehicle(meCar, -2);
            }

            if (driver.IsInVehicle(meCar, false) && Player.IsInVehicle(meCar, false))
            {
                Game.DisplaySubtitle(MSG_DRIVER_GO);
                driver.Tasks.DriveToPosition(destination, 24f, DRIVING_FLAGS);

                return true;
            }
            return false;
        }

        protected void SetScriptFinished()
        {
            DisplayMissionPassedScreen();

            data.ParentCase.ModifyCaseProgress(p => p.LastStageID = data.ID);

            Functions.PlayScannerAudio(SCANNER_FINISH);

            SetScriptFinished(true);
        }

        private void DisplayMissionPassedScreen()
        {
            ////var value = _isImportant == true ? 100 : 50;
            ////var medal = value < 100 ? MissionPassedScreen.Medal.Silver : MissionPassedScreen.Medal.Gold;
            ////var tick = _isImportant == true ? MissionPassedScreen.TickboxState.Tick : MissionPassedScreen.TickboxState.Empty;

            ////var handler = new MissionPassedHandler("Medical Examiner", value, medal);

            ////handler.AddItem("Updated by Medical Examiner", "Details Received", MissionPassedScreen.TickboxState.Tick);
            ////handler.AddItem("Suspect Acquired", "", tick);

            ////handler.Show();
        }

        protected override void End()
        {
            if(medExaminer) medExaminer.Dismiss();
            if(driver) driver.Dismiss();
            if(meCar) meCar.Dismiss();

            if(mainBlip) mainBlip.Delete();
            if (markerEntrance != null) markerEntrance.Dispose();
            if (markerExit != null) markerExit.Dispose();
            if (markerOffice != null) markerOffice.Dispose();

            if(swapCam) swapCam.Delete();

            timerTipSkipDrive.Stop();

            if (sceneOffice != null) sceneOffice.Dispose();
        }
    }
}