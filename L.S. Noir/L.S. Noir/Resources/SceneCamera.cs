using System;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using LtFlash.Common.Processes;
using Rage;
using Rage.Native;
using Graphics = System.Drawing.Graphics;

namespace LSNoir.Resources
{
    internal class SceneCamera
    {
        internal bool IsRunning { get; private set; }
        internal string SaveLocation { get; set; }

        private Camera _adjustableCamera, _gameCamera;

        private const Keys CameraPanLeft = Keys.A;
        private const Keys CameraPanRight = Keys.D;
        private const Keys CameraPanUp = Keys.W;
        private const Keys CameraPanDown = Keys.S;

        private const Keys CameraZoomIn = Keys.Q;
        private const Keys CameraZoomOut = Keys.E;

        private const Keys CameraRotateLeft = Keys.Z;
        private const Keys CameraRotateRight = Keys.X;

        private const Keys CameraTakePhoto = Keys.R;
        private const Keys CameraExit = Keys.T;

        private const string CameraHelpMessage = "Use your ~y~WASD~w~ keys to pan the camera.\nTo zoom in/out use ~y~Q~w~ and ~y~E~w~.\nTo rotate left/right use ~y~Z~w~ and ~y~X~w~\nTo take a photograph, press ~y~R~w~.\nTo exit, press ~y~T~w~.";

        private ProcessHost _p, _p2;

        internal SceneCamera(string saveLocation)
        {
            SaveLocation = saveLocation;
        }

        internal void Start()
        {
            IsRunning = true;

            _p = new ProcessHost();
            _p.ActivateProcess(PrepareForCameraControl);
            _p.Start();

            Game.FadeScreenOut(2000, true);

            _p2 = new ProcessHost();
            _p2.ActivateProcess(HideHudAndMap);
            _p2.Start();
        }

        private void PrepareForCameraControl()
        {
            // Give the player control
            Game.LocalPlayer.HasControl = false;

            // Create our custom camera
            _adjustableCamera = new Camera(false)
            {
                Position = Game.LocalPlayer.Character.GetBonePosition(PedBoneId.Head),
                Rotation = Game.LocalPlayer.Character.Rotation
            };

            Game.LocalPlayer.Character.IsVisible = false;
            _gameCamera = RetrieveGameCam();
            _adjustableCamera.Active = true;

            Game.FadeScreenIn(2000, true);

            Game.RawFrameRender += Game_RawFrameRender;

            _p.SwapProcesses(PrepareForCameraControl, CameraControl);
        }

        private void Game_RawFrameRender(object sender, GraphicsEventArgs e)
        {
            Vector2 topLeft = new Vector2(200, 200);
            Vector2 topRight = new Vector2(Game.Resolution.Width - 200, 200);

            Vector2 bottomLeft = new Vector2(200, Game.Resolution.Height - 200);
            Vector2 bottomRight = new Vector2(Game.Resolution.Width - 200, Game.Resolution.Height - 200);

            Color line = Color.FromArgb(200, Color.White);

            e.Graphics.DrawLine(topLeft, bottomLeft, line); // left
            e.Graphics.DrawLine(bottomLeft, bottomRight, line); // bottom

            e.Graphics.DrawLine(topRight, bottomRight, line); // right
            e.Graphics.DrawLine(topLeft, topRight, line); // top

            int hours = NativeFunction.Natives.GET_CLOCK_HOURS<int>();
            int mins = NativeFunction.Natives.GET_CLOCK_MINUTES<int>();
            int sec = NativeFunction.Natives.GET_CLOCK_SECONDS<int>();

            if (World.DateTime != null && !Game.IsPaused) e.Graphics.DrawText($"{World.DateTime.Date.ToShortDateString()} {hours}:{mins}:{sec}", "ARIAL", 30f, new PointF(topLeft.X, topLeft.Y), line);
            e.Graphics.DrawText("FOR OFFICIAL USE ONLY", "ARIAL", 30f, new PointF(topRight.X - 370, topRight.Y), line);
        }

        private bool _helpDisplayed = false;
        private void CameraControl()
        {
            if (!_helpDisplayed) Game.DisplayHelp(CameraHelpMessage, true);

            KeyboardState state = Game.GetKeyboardState();

            if (state.PressedKeys.Count < 1)
            {
                if (_isRunning) StopZoomAudio();
                return;
            }

            Rotator camRotation = _adjustableCamera.Rotation;

            switch (state.PressedKeys.First())
            {
                case CameraPanLeft:
                    camRotation.Yaw = camRotation.Yaw + GetAdjustedMoveScale(_adjustableCamera.FOV);
                    _adjustableCamera.Rotation = camRotation;
                    break;
                case CameraPanRight:
                    camRotation.Yaw = camRotation.Yaw - GetAdjustedMoveScale(_adjustableCamera.FOV);
                    _adjustableCamera.Rotation = camRotation;
                    break;
                case CameraPanUp:
                    camRotation.Pitch = camRotation.Pitch + GetAdjustedMoveScale(_adjustableCamera.FOV);
                    _adjustableCamera.Rotation = camRotation;
                    break;
                case CameraPanDown:
                    camRotation.Pitch = camRotation.Pitch - GetAdjustedMoveScale(_adjustableCamera.FOV);
                    _adjustableCamera.Rotation = camRotation;
                    break;
                case CameraZoomIn:
                    GameFiber.StartNew(PlayZoomAudio);
                    for (int i = 0; i < 5; i++)
                    {
                        _adjustableCamera.FOV = _adjustableCamera.FOV - 0.2f;
                    }
                    break;
                case CameraZoomOut:
                    for (int i = 0; i < 5; i++)
                    {
                        _adjustableCamera.FOV = _adjustableCamera.FOV + 0.2f;
                    }
                    GameFiber.StartNew(PlayZoomAudio);
                    break;
                case CameraRotateLeft:
                    camRotation.Roll = _adjustableCamera.Rotation.Roll - 0.5f;
                    _adjustableCamera.Rotation = camRotation;
                    break;
                case CameraRotateRight:
                    camRotation.Roll = _adjustableCamera.Rotation.Roll + 0.5f;
                    _adjustableCamera.Rotation = camRotation;
                    break;
                case CameraTakePhoto:
                    // S/O to MS https://code.msdn.microsoft.com/windowsdesktop/Saving-a-screenshot-using-C-6883abb3
                    try
                    {
                        Bitmap memoryImage = new Bitmap(Game.Resolution.Width - 405, Game.Resolution.Height - 405);
                        Size s = new Size(memoryImage.Width, memoryImage.Height);

                        Graphics memoryGraphics = Graphics.FromImage(memoryImage);

                        memoryGraphics.CopyFromScreen(200, 200, 0, 0, s);
                        memoryImage.Save(SaveLocation + $"{0000}-{NativeFunction.Natives.GET_CLOCK_HOURS<int>()}{NativeFunction.Natives.GET_CLOCK_MINUTES<int>()}{NativeFunction.Natives.GET_CLOCK_SECONDS<int>()}.png");

                        SoundPlayer soundPlayer = new SoundPlayer(@"Plugins/LSPDFR/LSNoir/Audio/CameraShutter.wav");
                        soundPlayer.Play();

                        Game.FadeScreenOut(0100);
                        GameFiber.Sleep(0100);
                        Game.FadeScreenIn(0100);
                        GameFiber.Sleep(0100);
                    }
                    catch (Exception ex)
                    {
                        Game.DisplayNotification("Error taking screenshot. Check your log and send it to Fiskey111");
                        $"Error taking screenshot:\n {ex}".AddLog();
                    }
                    break;
                case CameraExit:
                    "Exiting".AddLog();

                    _p.SwapProcesses(CameraControl, Ending);
                    break;
            }
        }

        private float GetAdjustedMoveScale(float fov)
        {
            if (fov >= 40)
            {
                return 1.0f;
            }
            else if (IsInRange(fov, 40, 20))
            {
                return 0.7f;
            }
            else if (IsInRange(fov, 20, 15))
            {
                return 0.5f;
            }
            else if (IsInRange(fov, 15, 8))
            {
                return 0.25f;
            }
            else
            {
                return 0.1f;
            }
        }

        /// <summary>
        /// Returns true if c is in the range
        /// </summary>
        /// <param name="c">Value to compare</param>
        /// <param name="e1">Higher bound</param>
        /// <param name="e2">Lower bound (equal to)</param>
        /// <returns></returns>
        private bool IsInRange(float c, float e1, float e2)
        {
            return c < e1 && c >= e2;
        }

        private bool _isRunning = false;
        private SoundPlayer zoomPlayer = new SoundPlayer(@"Plugins/LSPDFR/LSNoir/Audio/CameraZoom.wav");

        private void PlayZoomAudio()
        {
            if (_isRunning) return;

            _isRunning = true;
            zoomPlayer.Play();
            GameFiber.Sleep(0750);
            _isRunning = false;
        }

        private void StopZoomAudio()
        {
            zoomPlayer?.Stop();
            _isRunning = false;
        }

        private void Ending()
        {
            Game.RawFrameRender -= Game_RawFrameRender;

            Game.FadeScreenOut(2000, true);

            if (_adjustableCamera) _adjustableCamera.Delete();
            if (_gameCamera) _gameCamera.Delete();


            Game.LocalPlayer.Character.IsVisible = true;
            Game.HideHelp();

            Game.FadeScreenIn(2000, true);

            Game.LocalPlayer.Character.Tasks.Clear();
            Game.LocalPlayer.HasControl = true;
            Game.DisplayNotification("Done");
            _p.Stop();
        }

        private Camera RetrieveGameCam()
        {
            Camera gamecam = new Camera(false);
            gamecam.FOV = NativeFunction.Natives.GET_GAMEPLAY_CAM_FOV<float>();
            gamecam.Position = NativeFunction.Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
            Vector3 rot = NativeFunction.Natives.GET_GAMEPLAY_CAM_ROT<Vector3>(0);
            //doesn't work with Rotator as a return val
            var rot1 = new Rotator(rot.X, rot.Y, rot.Z);
            gamecam.Rotation = rot1;

            gamecam.Heading = NativeFunction.Natives.GetGameplayCamRelativeHeading<float>();

            return gamecam;
        }

        private void HideHudAndMap()
        {
            NativeFunction.Natives.HIDE_HUD_AND_RADAR_THIS_FRAME();
        }
    }
}
