using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using LSNoir.Extensions;
using Rage;
using Rage.Native;
using static LtFlash.Common.Serialization.Serializer;

namespace LSNoir.Callouts.SA.Commons
{
    class SecurityCamera
    {
        // System
        internal static bool IsRunning, IsFaded;
        private static bool _play;
        private static DateTime _secTime;
        private static TimeSpan _pauseTime;

        // Locations
        private static Vector3 _playerPos, _targetPos;

        // Entities
        private static Ped _sus, _vic;
        private static Vehicle _veh;

        // Fibers
        private static GameFiber _cameraMovementFiber;

        // Misc
        private static Camera _gameCam, _secCam;
        private static Texture _playTexture = Game.CreateTextureFromFile(@"Plugins\LSPDFR\LSNoir\Textures\play.png");
        private static Texture _pauseTexture = Game.CreateTextureFromFile(@"Plugins\LSPDFR\LSNoir\Textures\pause.png");

        internal static void SecurityCameraStart(CaseData data)
        {
            IsRunning = true;
            GameFiber.StartNew(delegate
            {
                Game.LogTrivial("Starting security camera playback");
                
                Game.IsPaused = false;

                _playerPos = Game.LocalPlayer.Character.Position;
                _targetPos = data.SusTarget.Position;

                FadeScreen(true);

                _gameCam = CreateGameCam();
                _gameCam.Active = false;
                _secCam = CreateSecurityCamera(data);
                _secCam.Active = false;

                Game.LocalPlayer.Character.IsInvincible = true;
                Game.LocalPlayer.Character.Position = new Vector3(data.SecCamSpawn.Position.X, data.SecCamSpawn.Position.Y, data.SecCamSpawn.Position.Z + 3f);
                Game.LocalPlayer.Character.IsPositionFrozen = true;
                
                _secTime = new DateTime(6, 5, 11, 23, 15, 50);

                SetUpSecCamFootage(data);
            });
        }

        private static void FadeScreen(bool fadeOut)
        {
            Game.LogTrivial("Fading screen out: " + fadeOut);
            if (fadeOut)
            {
                NativeFunction.Natives.DO_SCREEN_FADE_OUT(1500);
                while (!NativeFunction.Natives.IS_SCREEN_FADED_OUT<bool>())
                    GameFiber.Yield();
            }
            else
            {
                NativeFunction.Natives.DO_SCREEN_FADE_IN(1500);
                while (NativeFunction.Natives.IS_SCREEN_FADED_OUT<bool>())
                    GameFiber.Yield();
            }
        }

        private static void SetUpSecCamFootage(CaseData data)
        {
            Game.LogTrivial("Setting up security camera");

            var _sData = GetSelectedListElementFromXml<PedData>(Main.SDataPath,
                c => c.FirstOrDefault(s => s.Type == PedType.Suspect));
            var _vData = GetSelectedListElementFromXml<PedData>(Main.PDataPath,
                c => c.FirstOrDefault(v => v.Type == PedType.Victim));

            var modelList =
                Model.VehicleModels.Where(
                    v =>
                        v.IsCar && !v.IsBus && !v.IsBigVehicle && !v.IsBicycle && !v.IsBlimp && !v.IsBoat &&
                        !v.IsEmergencyVehicle && !v.IsHelicopter && !v.IsPlane).ToList();

            var model = modelList[MathHelper.GetRandomInteger(modelList.Count)];

            _veh = new Vehicle(model, data.SusSpawnPoint.Position) { Heading = data.SusSpawnPoint.Heading };

            while (!_veh.Exists())
                GameFiber.Yield();
            _sus = new Ped(_sData.Model, _veh.Position, 0f);
            _vic = new Ped(_vData.Model, _veh.Position, 0f);

            _sus.WarpIntoVehicle(_veh, -1);
            _vic.WarpIntoVehicle(_veh, 0);
            _veh.LicensePlate.PickRandom(7);

            World.DateTime = _secTime;

            NativeFunction.Natives.SET_TIMECYCLE_MODIFIER("scanline_cam");

            SwapCamera(_gameCam, _secCam);

            _secCam.Face(_veh);

            FadeScreen(false);

            _sus.KeepTasks = true;

            _sus.Tasks.DriveToPosition(_targetPos, 5f, VehicleDrivingFlags.Normal, 2f);

            _play = true;

            Game.RawFrameRender += OnRawFrameRender;

            WaitForSuspect(_veh, _targetPos);
        }

        private static void WaitForSuspect(Vehicle vehicle, Vector3 point)
        {
            "WaitForSuspect".AddLog();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            _cameraMovementFiber = new GameFiber(CameraMove);
            _cameraMovementFiber.Start();

            while (vehicle.Position.DistanceTo(point) > 4f)
            {
                if (sw.Elapsed.Seconds >= 15)
                {
                    "Stopwatch elapsed, breaking loop".AddLog(true);
                    break;
                }

                GameFiber.Yield();
            }
            "Vehicle at position".AddLog(true);

            _sus.Tasks.PerformDrivingManeuver(vehicle, VehicleManeuver.Wait);

            _play = false;

            _secCam.Face(_veh);

            _pauseTime = World.TimeOfDay;

            var speedZone = World.AddSpeedZone(_targetPos, 60f, 0f);

            for (var i = _secCam.FOV; i >= GetFOVValue(Vector3.Distance(Game.LocalPlayer.Character.Position, _veh)); i += -0.2f)
            {
                _secCam.FOV = i;
                GameFiber.Sleep(0100);
            }

            GameFiber.Sleep(3000);

            FadeScreen(true);

            World.RemoveSpeedZone(speedZone);

            "Ending".AddLog(true);

            _cameraMovementFiber.Abort();

            Game.RawFrameRender -= OnRawFrameRender;

            EndCamera();
        }

        private static void CameraMove()
        {
            Game.DisplayHelp("Move your mouse to pan the camera\nUse the mouse wheel to zoom the camera\nTo exit this mode, press any key \n after you've found something useful");

            while (true)
            {
                float moveSpeed = (_secCam.FOV / 100) * (Game.IsControllerConnected ? 3.5f : 5.25f);

                float yRotMagnitude = NativeFunction.CallByName<float>("GET_DISABLED_CONTROL_NORMAL", 0, (int)GameControl.LookUpDown) * moveSpeed;
                float xRotMagnitude = NativeFunction.CallByName<float>("GET_DISABLED_CONTROL_NORMAL", 0, (int)GameControl.LookLeftRight) * moveSpeed;

                float newPitch = _secCam.Rotation.Pitch - yRotMagnitude;
                float newYaw = _secCam.Rotation.Yaw - xRotMagnitude;
                _secCam.Rotation = new Rotator((newPitch >= 25f || newPitch <= -70f) ? _secCam.Rotation.Pitch : newPitch, 0f, newYaw);

                GameFiber.Yield();
            }
        }

        private static void EndCamera()
        {
            SwapCamera(_secCam, _gameCam);

            FadeScreen(false);

            Game.LocalPlayer.Character.Position = _playerPos;
            Game.LocalPlayer.Character.IsInvincible = false;
            Game.LocalPlayer.Character.IsPositionFrozen = false;

            NativeFunction.Natives.CLEAR_TIMECYCLE_MODIFIER();

            MainCode.PlateNumber = _veh.LicensePlate;

            if (_secCam.Exists()) _secCam.Delete();
            if (_gameCam.Exists()) _gameCam.Delete();
            if (_sus) _sus.Delete();
            if (_vic) _vic.Delete();
            if (_veh) _veh.Delete();

            Game.IsPaused = true;

            IsRunning = false;
        }

        private static float GetFOVValue(float distance)
        {
            float value;
            if (distance >= 50f) value = 1f;
            else if (distance < 50f && distance >= 45f) value = 2f;
            else if (distance < 45f && distance >= 40f) value = 3f;
            else if (distance < 40f && distance >= 35f) value = 4f;
            else if (distance < 35f && distance >= 30f) value = 5f;
            else if (distance < 30f && distance >= 25f) value = 6f;
            else if (distance < 25f && distance >= 20f) value = 7f;
            else if (distance < 20f && distance >= 15f) value = 8f;
            else if (distance < 15f && distance >= 10f) value = 9f;
            else if (distance < 10f && distance >= 5f) value = 10f;
            else value = 11f;
            return value;
        }

        private static void OnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            if (_play)
            {
                e.Graphics.DrawTexture(_playTexture, (Game.Resolution.Width / 2), 50, 75f, 75f);
                e.Graphics.DrawText(World.TimeOfDay.ToString(), "Arial", 18f, new PointF(5f, 5f), Color.White);
            }
            else
            {
                e.Graphics.DrawTexture(_pauseTexture, (Game.Resolution.Width / 2), 50, 75f, 75f);
                e.Graphics.DrawText(_pauseTime.ToString(), "Arial", 18f, new PointF(5f, 5f), Color.White);
            }
        }

        private static void SwapCamera(Camera camFrom, Camera camTo)
        {
            camFrom.Active = false;
            camTo.Active = true;
        }

        private static Camera CreateGameCam()
        {
            var gameCam = new Camera(false)
            {
                FOV = NativeFunction.Natives.GET_GAMEPLAY_CAM_FOV<float>(),
                Position = NativeFunction.Natives.GET_GAMEPLAY_CAM_COORD<Vector3>()
            };
            Vector3 rot = NativeFunction.Natives.GET_GAMEPLAY_CAM_ROT<Vector3>(0);
            var rot1 = new Rotator(rot.X, rot.Y, rot.Z);
            gameCam.Rotation = rot1;
            return gameCam;
        }

        private static Camera CreateSecurityCamera(CaseData data)
        {
            var secCam = new Camera(false)
            {
                Position = data.SecCamSpawn.Position,
                Heading = data.SecCamSpawn.Heading
            };
            return secCam;
        }
    }
}
