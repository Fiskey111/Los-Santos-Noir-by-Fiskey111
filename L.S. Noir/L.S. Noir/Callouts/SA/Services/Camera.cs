using Rage;
using Rage.Native;

namespace LSNoir.Callouts.SA.Commons
{
    internal class CamClass
    {
        //-- Common
        public Vector3 OfficerSP, AmbulanceSP, AmbulanceSpeedUpSP;
        public Blip OfficerBlip, AmbulanceBlip;
        public Ped OfficerPed, AmbulancePed1, AmbulancePed2;
        public Vehicle OfficerVehicle, AmbulanceVehicle, LocalPlayersVehicle;
        public HitResult LineTestPullover, LineTestFront, LineTestBack, LineTestLeft, LineTestRight;

        //-- Random
        public int ranNum1;
        public float? OfficerGroundPosition;
        public float OfficerValue, OfficerHeading, AmbulanceHeading, AmbulanceSpeedUpHeading;
        public const string TD_SHOTGUN_REAR_KILL = "TD_SHOTGUN_REAR_KILL";

        //-- Distance Checks
        public bool Check50f, Check15f;
        //-- Line Tests
        public bool RoadTestPullover, RoadTestFront, RoadTestBack, LineTestBoolPullover, LineTestBoolFront, LineTestBoolBack, FrontBlocked, BackBlocked,
            RoadTestLeft, LineTestBoolLeft, LeftBlocked, RoadTestRight, LineTestBoolRight, RightBlocked;
        //-- Other
        public bool WaitTime1, Arrived, PoliceVehicleTrue, RequestEMS, SpeedUp, Tasks1, Tasks2, DoorOpen, InAmbulance, OfficerNode, AmbulanceNode, AmbulanceSpeedUpNode, OfficerLayingDown, WalkToOfficer,
            AmbulanceDistanceCheck;
        //-- Subtitles
        public bool TalkA1, TalkA2,/* TalkA3, TalkA4, TalkA5, TalkA6, TalkA7, TalkA8, TalkA9, TalkA10,*/ TalkAEnd;

        internal static Camera Camera, GameCam;

        internal static void FocusCamOnObjectWithInterpolation(Vector3 camPos, float face)
        {
            Camera = new Camera(false);

            Camera.Position = camPos;
            Camera.Heading = face;

            GameCam = RetrieveGameCam();
            GameCam.Active = true;
            CamInterpolate(GameCam, Camera, 6000, true, true, true);
            Camera.Active = true;

            SetLocalPlayerPropertiesWhileCamOn(true);
        }

        private static void SetLocalPlayerPropertiesWhileCamOn(bool on)
        {
            NativeFunction.Natives.FreezeEntityPosition(Game.LocalPlayer.Character, on);

            Game.LocalPlayer.Character.IsInvincible = on;
        }

        internal static void InterpolateCameraBack()
        {
            if (GameCam == null || Camera == null) return;

            CamInterpolate(Camera, GameCam, 1000, true, true, true);

            Camera.Active = false;
            Camera.Delete();
            Camera = null;

            GameCam.Delete();
            GameCam = null;

            SetLocalPlayerPropertiesWhileCamOn(false);
        }
        
        internal static void DisableCustomCam()
        {
            Game.FadeScreenOut(1000);

            if (Camera.Exists())
            {
                Camera.Active = false;
                Camera.Delete();
            }

            Game.FadeScreenIn(1000);

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
