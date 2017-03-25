using LSNoir.Callouts.SA.Commons;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Fiskey111Common;
using LSPD_First_Response.Mod.API;
using LSNoir.Callouts.SA.Creators;
using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using StageCreator;
using static LtFlash.Common.Serialization.Serializer;

namespace LSNoir.Callouts
{
    public class Sa_2BMedicalExaminer : BasicScript
    {
        // System
        private string _nearestMe;
        private bool _main = true;
        private static Stopwatch _timer = new Stopwatch();
        internal static bool _isImportant = false;

        // Locations
        private Vector3 _position,
            _mainMe = new Vector3(236, -1390, 31),
            _blaineMe = new Vector3(1840, 3673, 34),
            _paletoMe = new Vector3(-452, 6038, 32);
        private Fiskey111Common.SpawnPt CarSpawn, MainSpawn;

        // Data
        private CaseData _cData = LoadItemFromXML<CaseData>(Main.CDataPath);
        private ReportData _meData;

        // Dialog
        private Dialog _driverDialog, _meDialog;

        // Entities
        private Ped _me;
        private Vehicle _meCar, _playerCar;

        // Misc
        private Blip _MEBlip;
        private ELocation _state;
        private EDialog _dialogstate;
        private Marker _meMarker, _meDriverMarker;


        // todo -- fix all this crap below
        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 2b [Medical Examiner]".AddLog();
            _position = GetNearestMe(Game.LocalPlayer.Character.Position);
            ExtensionMethods.LogDistanceFromCallout(_position);

            if (GetNearestMe(Game.LocalPlayer.Character.Position) == _blaineMe)
            {
                CarSpawn = new Fiskey111Common.SpawnPt(31, 1839, 3666, 34);
                _meCar = new Vehicle("FBI", CarSpawn.Spawn);
                _meCar.Heading = CarSpawn.Heading;
                _me = new Ped("s_m_m_highsec_01", _position, 233f);
                _main = false;
                _nearestMe = "SS";
                "Closest to Sandy Shores Location".AddLog();
            }
            if (GetNearestMe(Game.LocalPlayer.Character.Position) == _paletoMe)
            {
                CarSpawn = new Fiskey111Common.SpawnPt(313, -453, 6034, 31);
                _meCar = new Vehicle("FBI", CarSpawn.Spawn);
                _meCar.Heading = CarSpawn.Heading;
                _meCar.IsInvincible = true;
                _meCar.TopSpeed = 100f;
                _me = new Ped("s_m_m_highsec_01", _position, 0f);
                _main = false;
                _nearestMe = "PB";
                "Closest to Paleto Bay Location".AddLog();
            }

            _MEBlip = new Blip(_position)
            {
                Sprite = BlipSprite.GangAttackPackage,
                Color = Color.DarkOrange,
                Name = "Medical Examiner"
            };

            "Sexual Assault Case Update".DisplayNotification("Visit ~o~Medical Examiner~w~ for update");
   
            _state = ELocation.Dispatched;
            
            InteriorHelper.IsCoronerInteriorEnabled = true;

            AddStage(EnRoute);
            AddStage(Close);
            AddStage(VeryClose);
            return true;
        }

        public static Vector3 GetNearestMe(Vector3 target)
        {
            Vector3 myMe = new Vector3();
            List<Vector3> me = new List<Vector3>();
            me.Add(new Vector3(236, -1390, 31));
            me.Add(new Vector3(1840, 3673, 34));
            me.Add(new Vector3(-452, 6038, 32));
            float closestRange = 100000;

            foreach (Vector3 sp in me)
            {
                if (sp.DistanceTo(target) < closestRange)
                {
                    closestRange = sp.DistanceTo(target);
                    myMe = sp;
                }
            }
            return myMe;
        }
        bool _atloc = false;
        protected override void Process()
        {
            if (_main == false)
            {
                if (Vector3.Distance(Game.LocalPlayer.Character.Position, _position) < 80f && _state == ELocation.Dispatched)
                {
                    "80m away".AddLog();
                    ActivateStage(EnRoute);
                }
                if (Vector3.Distance(Game.LocalPlayer.Character.Position, _position) < 10f && _state == ELocation.Within80)
                {
                    "10m away".AddLog();
                    ActivateStage(Close);
                }
                if (Vector3.Distance(Game.LocalPlayer.Character.Position, _position) < 5f && _state == ELocation.Within15 && Game.LocalPlayer.Character.IsOnFoot)
                {
                    "5m away".AddLog();
                    ActivateStage(VeryClose);
                }
            }
            else
            {
                if (Vector3.Distance(Game.LocalPlayer.Character.Position, _mainMe) < 5f && Game.LocalPlayer.Character.IsOnFoot && !_atloc)
                {
                    "5m away - main".AddLog();
                    _atloc = true;
                    if (_MEBlip.Exists()) _MEBlip.Delete();
                    ActivateStage(VeryClose);
                }
            }
        }

        #region Stages
        public void EnRoute()
        {
            _state = ELocation.Within80;
            GameFiber.StartNew(delegate
            {
                while (true)
                {
                    _me.Task_Scenario("WORLD_HUMAN_COP_IDLES");
                    while (NativeFunction.Natives.IS_PED_USING_ANY_SCENARIO<bool>(_me))
                    {
                        if (_state == ELocation.Within15)
                            break;
                        GameFiber.Yield();
                    }
                    if (_state == ELocation.Within15)
                        break;
                    GameFiber.Yield();
                }
            });
            DeactivateStage(EnRoute);
        }

        public void Close()
        {
            _state = ELocation.Within15;
            if (_MEBlip.Exists()) _MEBlip.Delete();
            NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(_me, Game.LocalPlayer.Character, -1);
            Game.DisplayHelp("Approach the ~y~Driver~w~ to go to the Medical Examiner's Office");
            _dialogstate = EDialog.Pre;
            Dialog d = new Dialog(ConversationCreator.DialogLineCreator(ConversationCreator.ConversationType.MeDriver, _me), _me.Position);
            d.AddPed(0, Game.LocalPlayer.Character);
            d.AddPed(1, _me);

            DeactivateStage(Close);
        }

        bool _notified;
        public void VeryClose()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(_mainMe) <= 15f)
            {
                "At main ME's office".AddLog();
                SwapStages(VeryClose, AtLocation);
            }
            if (!_main)
            {
                _state = ELocation.Done;
                if (_dialogstate == EDialog.Pre)
                {
                    _dialogstate = EDialog.During;
                    "Dialog Starting".AddLog();
                    _driverDialog.StartDialog();
                }

                if (_driverDialog.HasEnded && _dialogstate == EDialog.During)
                {
                    "Dialog Ending".AddLog();
                    Game.DisplayHelp("Enter the driver's car");
                    _dialogstate = EDialog.Post;
                    _playerCar = Game.LocalPlayer.Character.LastVehicle;
                    _me.KeepTasks = true;
                    _me.Tasks.GoStraightToPosition(_meCar.LeftPosition, 3f, 0f, 0f, 1000);
                    GameFiber.Sleep(1000);
                    _me.Tasks.EnterVehicle(_meCar, -1);
                    
                    SwapStages(VeryClose, GettingInCar);
                }
            }
        }

        private void GettingInCar()
        {
            if (Game.IsKeyDown(System.Windows.Forms.Keys.F))
            {
                "Warped into Vehicle".AddLog();
                _me.WarpIntoVehicle(_meCar, -1);
                Game.LocalPlayer.Character.WarpIntoVehicle(_meCar, -2);
            }
            if (_me.IsInVehicle(_meCar, false) && Game.LocalPlayer.Character.IsInVehicle(_meCar, false) && _notified == false)
            {
                _notified = true;
                Game.DisplaySubtitle("[Driver]: Off we go!");
                _me.Tasks.DriveToPosition(_mainMe, 24f, VehicleDrivingFlags.Normal | VehicleDrivingFlags.StopAtDestination | VehicleDrivingFlags.DriveAroundObjects);
                _timer.Start();
                GameFiber.StartNew(delegate
                {
                    TimerBar.Main(_mainMe);
                });
                SwapStages(VeryClose, Driving);
            }
        }

        private void Driving()
        {
            while (true)
            {
                if (_timer.Elapsed.Seconds >= 35)
                {
                    Game.DisplayHelp("Press 0 to skip the drive.");
                    _timer.Restart();
                }

                if (_transport)
                {
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.D0) && Game.LocalPlayer.Character.Position.DistanceTo(CarSpawn.Spawn) > 10f)
                    {
                        "Player chose to skip the drive".AddLog();
                        break;
                    }
                    else if (Game.LocalPlayer.Character.DistanceTo(CarSpawn.Spawn.Around(5f)) < 1f)
                    {
                        GameFiber.Sleep(1000);
                        SetScriptFinished();
                    }
                }
                else
                {
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.D0) && Game.LocalPlayer.Character.Position.DistanceTo(_mainMe) > 10f)
                    {
                        "Player chose to skip the drive".AddLog();
                        break;
                    }
                    else if (Game.LocalPlayer.Character.DistanceTo(_mainMe) < 1f)
                    {
                        SwapStages(Driving, AtLocation);
                    }
                }
                GameFiber.Yield();
            }
            SwapStages(Driving, CameraSwap);
        }
        bool _cam1 = false;
        Camera _gameCam, _swapCam;
        private void CameraSwap()
        {
            if (_cam1 == false)
            {
                "Starting to swap cameras".AddLog();
                _cam1 = true;
                _swapCam = new Camera(false);
                _swapCam.Position = new Vector3(_meCar.AbovePosition.X, _meCar.AbovePosition.Y, _meCar.AbovePosition.Z + 7);
                _swapCam.PointAtEntity(_meCar, Vector3.Zero, false);
                
                _gameCam = new Camera(false);
                _gameCam.FOV = NativeFunction.Natives.GET_GAMEPLAY_CAM_FOV<float>();
                _gameCam.Position = NativeFunction.Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
                Vector3 rot = NativeFunction.Natives.GET_GAMEPLAY_CAM_ROT<Vector3>(0);
                var rot1 = new Rotator(rot.X, rot.Y, rot.Z);
                _gameCam.Rotation = rot1;

                "Swapping camera".AddLog();
                _swapCam.Active = true;
                NativeFunction.Natives.DO_SCREEN_FADE_OUT(7500);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    _swapCam.PointAtEntity(_meCar, Vector3.Zero, false);
                    if (NativeFunction.Natives.IS_SCREEN_FADED_OUT<bool>() == true)
                    {
                        "Screen faded, swapping stages".AddLog();
                        _swapCam.Active = false;
                        break;
                    }
                    GameFiber.Yield();
                }
                if (_transport)
                    SwapStages(CameraSwap, Ending);
                else
                    SwapStages(CameraSwap, Transport);
            }
        }

        private void Ending()
        {
            _meCar.Position = CarSpawn.Spawn;
            _meCar.Heading = CarSpawn.Heading;
            _gameCam.Active = true;
            _swapCam.Delete();


            NativeFunction.Natives.DO_SCREEN_FADE_IN(1000);
            Game.DisplaySubtitle("[Driver] Here you are!  Good luck!");
            Game.LocalPlayer.Character.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
            "Swapping to AtLocation".AddLog();
            GameFiber.Sleep(1000);
            SetScriptFinished();
        }

        private void Transport()
        {
            _meCar.Position = new Vector3(233, -1390, 30);
            _meCar.Heading = 50;
            _gameCam.Active = true;
            _swapCam.Delete();
            

            NativeFunction.Natives.DO_SCREEN_FADE_IN(1000);
            Game.DisplaySubtitle("[Driver] Here you are!  Good luck!");
            Game.LocalPlayer.Character.Tasks.LeaveVehicle(LeaveVehicleFlags.None);
            "Swapping to AtLocation".AddLog();
            SwapStages(Transport, AtLocation);
        }

        bool _atlocation = false, _startingswap = false;
        Vector3 _markerLoc = new Vector3(240, -1380, 34);
        private void AtLocation()
        {
            if (!_atlocation)
            {
                _atlocation = true;

                "Notified to go to marker".AddLog();
                Game.DisplayHelp("Head to the ~y~marker~w~ to enter the Medical Examiner's office");
                _meMarker = new Marker(_markerLoc, Color.Yellow);
                _meMarker.Start();
            }
            if (Game.LocalPlayer.Character.Position.DistanceTo(_markerLoc) <= 1f && _startingswap == false)
            {
                _startingswap = true;
                _meMarker.Stop();
                GameFiber.StartNew(delegate
                {
                    "Entering ME's office".AddLog();

                    CamClass.FocusCamOnObjectWithInterpolation(new Vector3(219, -1422, 35), 200);
                    NativeFunction.Natives.DO_SCREEN_FADE_OUT(8000);
                    GameFiber.Sleep(8100);
                    MeCreator.CreateScene(Game.LocalPlayer.Character.Position, _mainMe);
                    while (true)
                    {
                        if (NativeFunction.Natives.IS_SCREEN_FADED_OUT<bool>() == true)
                        {
                            "Screen faded, swapping stages".AddLog();
                            GameFiber.Sleep(1000);
                            ("Does ME Exist " + MeCreator.MedicalExaminer.Ped.Exists()).AddLog();
                            break;
                        }
                        GameFiber.Yield();
                    }
                    CamClass.InterpolateCameraBack();
                    SwapStages(AtLocation, InMe);
                });
            }

        }
        #endregion

        bool _notified2 = false;
        private void InMe()
        {
            if (!_notified2)
            {
                _notified2 = true;
                _startingswap = false;
                NativeFunction.Natives.DO_SCREEN_FADE_IN(1000);

                var vicData = GetSelectedListElementFromXml<PedData>(Main.PDataPath,
                    p => p.FirstOrDefault(v => v.Type == PedType.Victim));
                var susData = GetSelectedListElementFromXml<PedData>(Main.SDataPath,
                    s => s.FirstOrDefault(v => v.IsPerp == true));

                ("Does ME Exist " + MeCreator.MedicalExaminer.Ped.Exists()).AddLog();

                GameFiber.Sleep(0500);

                _meDialog = new Dialog(ConversationCreator.DialogLineCreator(ConversationCreator.ConversationType.MedicalExaminer, MeCreator.MedicalExaminer.Ped, 0, false, vicData, susData), MeCreator.MedicalExaminer.Ped.Position);
                _meDialog.AddPed(0, Game.LocalPlayer.Character);
                _meDialog.AddPed(1, MeCreator.MedicalExaminer.Ped);
                _meDialog.DisableFirstKeypress = false;

                _meDialog.DistanceToStop = 6f;

                Vector3 markerPos = new Vector3(MeCreator.MedicalExaminer.Ped.Position.X, MeCreator.MedicalExaminer.Ped.Position.Y, MeCreator.MedicalExaminer.Ped.AbovePosition.Z);

                _meMarker = new Marker(new Vector3(237.67f, -1367.89f, 39.53f), Color.Green);
                _meMarker.Start();

                "Notified to go speak to ME".AddLog();
                Game.DisplayHelp("Go talk to the ~g~Medical Examiner~w~ in the office");
                Vector3 p7Pos = new Vector3(MeCreator.MedicalExaminer.Ped.Position.X, MeCreator.MedicalExaminer.Ped.Position.Y, MeCreator.MedicalExaminer.Ped.AbovePosition.Z + 1.5f);
            }
            if (Game.LocalPlayer.Character.Position.DistanceTo(MeCreator.MedicalExaminer.Ped.Position) < 4f)
            {
                Game.DisplayHelp("Press ~y~Y~w~ to talk to the ~g~Medical Examiner~w~.");
                _meDialog.StartDialog();

                _meMarker.Stop();

                while (!_meDialog.HasEnded)
                {
                    GameFiber.Yield();
                }
                "Sexual Assault Case Update".DisplayNotification("Medical Examiner Conversation \nAdded to ~b~SAJRS");
                Game.DisplayHelp("Now that you have the report, ~y~exit~w~ the building");

                var rList = LoadItemFromXML<List<ReportData>>(Main.RDataPath);
                _meData = new ReportData(ReportData.Service.ME, MeCreator.MedicalExaminer.Ped, _meDialog.Dialogue, true);
                rList.Add(_meData);
                SaveItemToXML<List<ReportData>>(rList, Main.RDataPath);

                _meMarker = new Marker(MeCreator.PPos, Color.Yellow);
                _meMarker.Start();
                SwapStages(InMe, ExitingMe);
            }

        }
        bool _transport = false;
        private void ExitingMe()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(MeCreator.PPos) < 1f && _startingswap == false)
            {
                _startingswap = true;
                _meMarker.Stop();
                Game.LocalPlayer.Character.IsPositionFrozen = true;
                GameFiber.StartNew(delegate
                {
                    "Exiting ME's office".AddLog();
                    NativeFunction.Natives.DO_SCREEN_FADE_OUT(4000);
                    GameFiber.Sleep(4100);
                    Game.LocalPlayer.Character.Position = _markerLoc;
                    if (_nearestMe == "PB" || _nearestMe == "SS")
                    {
                        MainSpawn = new Fiskey111Common.SpawnPt(50, -233, -1390, 30);
                        _meCar = new Vehicle("FBI", MainSpawn.Spawn);
                        _meCar.Heading = MainSpawn.Heading;
                        _meCar.IsInvincible = true;
                        _meCar.TopSpeed = 100f;
                        _me = new Ped("s_m_m_highsec_01", _position, 0f);
                        _me.WarpIntoVehicle(_meCar, -1);
                        _meCar.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Both;
                        _transport = true;
                    }
                    while (true)
                    {
                        if (NativeFunction.Natives.IS_SCREEN_FADED_OUT<bool>() == true)
                        {
                            "Screen faded, swapping stages".AddLog();
                            GameFiber.Sleep(1000);
                            break;
                        }
                        GameFiber.Yield();
                    }
                    Game.LocalPlayer.Character.IsPositionFrozen = false;
                    NativeFunction.Natives.DO_SCREEN_FADE_IN(1000);
                    GameFiber.Sleep(1500);
                    if (_transport)
                    {
                        SwapStages(ExitingMe, Returning);
                    }
                    else
                        SetScriptFinished();
                });
            }
        }

        private void Returning()
        {
            if (Game.IsKeyDown(System.Windows.Forms.Keys.F))
            {
                "Warped into Vehicle".AddLog();
                _me.WarpIntoVehicle(_meCar, -1);
                Game.LocalPlayer.Character.WarpIntoVehicle(_meCar, -2);
            }
            if (_me.IsInVehicle(_meCar, false) && Game.LocalPlayer.Character.IsInVehicle(_meCar, false) && _notified == false)
            {
                _notified = true;
                Game.DisplaySubtitle("[Driver]: Off we go!");
                _me.Tasks.DriveToPosition(CarSpawn.Spawn, 24f, VehicleDrivingFlags.Normal | VehicleDrivingFlags.StopAtDestination | VehicleDrivingFlags.DriveAroundObjects);
                _timer.Start();
                SwapStages(Returning, Driving);
            }
        }

        protected override void End()
        {

        }

        protected void SetScriptFinished()
        {
            var value = _isImportant == true ? 100 : 50;
            var medal = value < 100 ? MissionPassedScreen.Medal.Silver : MissionPassedScreen.Medal.Gold;
            var tick = _isImportant == true ? MissionPassedScreen.TickboxState.Tick : MissionPassedScreen.TickboxState.Empty;

            var handler = new MissionPassedHandler("Medical Examiner", value, medal);

            handler.AddItem("Updated by Medical Examiner", "Details Received", MissionPassedScreen.TickboxState.Tick);
            handler.AddItem("Suspect Acquired", "", tick);

            handler.Show();
            
            _cData.CurrentStage = CaseData.LastStage.MedicalExaminer;
            _cData.LastCompletedStage = CaseData.LastStage.MedicalExaminer;
            _cData.CompletedStages.Add(CaseData.LastStage.MedicalExaminer);
            _cData.SajrsUpdates.Add("Medical Examiner Report Added");
            _cData.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            SaveItemToXML(_cData, Main.CDataPath);
            Functions.PlayScannerAudio("ATTN_DISPATCH CODE_04_PATROL");
            "Terminating L.S. Noir Callout: Sexual Assault -- Stage 2b [ME]".AddLog();
            if (_me.Exists()) _me.Dismiss();
            if (_meCar.Exists()) _meCar.Dismiss();
            SetScriptFinished(true);
        }

        public enum ELocation
        {
            Dispatched, Within80, Within15, Within5, Done
        }
        public enum EDialog
        {
            Pre, During, Post
        }
    }
}
