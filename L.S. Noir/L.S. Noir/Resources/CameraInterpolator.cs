using Rage;
using Rage.Native;

namespace LSNoir.Resources
{
    class CameraInterpolator
    {
        private Camera cam;
        private Camera gameCam;

        public CameraInterpolator()
        {
        }

        public void Start(Vector3 camPos, float face)
        {
            cam = new Camera(false);

            cam.Position = camPos;
            cam.Heading = face;

            gameCam = RetrieveGameCam();
            gameCam.Active = true;

            CamInterpolate(gameCam, cam, 6000, true, true, true);

            cam.Active = true;

            SetLocalPlayerPropertiesWhileCamOn(true);
        }

        public void Stop()
        {
            if (gameCam == null || cam == null) return;

            CamInterpolate(cam, gameCam, 1000, true, true, true);

            cam.Active = false;
            cam.Delete();
            cam = null;

            gameCam.Delete();
            gameCam = null;

            SetLocalPlayerPropertiesWhileCamOn(false);
        }

        private static Camera RetrieveGameCam()
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

        private static void SetLocalPlayerPropertiesWhileCamOn(bool on)
        {
            NativeFunction.Natives.FreezeEntityPosition(Game.LocalPlayer.Character, on);

            Game.LocalPlayer.Character.IsInvincible = on;
        }

        private static void CamInterpolate(
            Camera camfrom, Camera camto,
            int totaltime,
            bool easeLocation, bool easeRotation, bool waitForCompletion,
            float x = 0f, float y = 0f, float z = 0f)
        {
            NativeFunction.Natives.SET_CAM_ACTIVE_WITH_INTERP(
                camto, camfrom,
                totaltime, easeLocation, easeRotation);

            if (waitForCompletion) GameFiber.Sleep(totaltime);
        }
    }
}
