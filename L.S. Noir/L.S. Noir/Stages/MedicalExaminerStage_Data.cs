using LtFlash.Common;
using Rage;

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
    partial class MedicalExaminerStage
    {
        private const string MODEL_CAR = "FBI";
        private const string MSG_PRESS_TO_SKIP = "Press ~y~{0}~s~ to skip the drive.";
        private const string MSG_TALK_TO_DRIVER = "Approach the ~y~Driver~w~ to go to the Medical Examiner's Office";
        private const string MSG_EXIT_OFFICE = "Now that you have the report, ~r~exit~w~ the building";
        private const string MSG_GOTO_ME = "Enter the driver's car or go to the marked destination";
        private const string MSG_ENTER_OFFICE = "Head to the ~y~marker~w~ to enter the Medical Examiner's office";
        private const string MSG_TALK_ME = "Go talk to the ~g~Medical Examiner~w~ in the office";
        private const string MSG_PRESS_TALK_ME = "Press ~y~{0}~s~ to talk to the ~g~Medical Examiner~s~.";
        private const string MSG_DRIVER_GOOD_LUCK = "[Driver] Here you are! Good luck!";
        private const string MSG_DRIVER_GO = "[Driver] Off we go!";
        private const string SCANNER_FINISH = "ATTN_DISPATCH CODE_04_PATROL";
        private const string SCENARIO_DRIVER = "WORLD_HUMAN_COP_IDLES";
        private const string MODEL_DRIVER = "s_m_m_highsec_01";

        private const VehicleDrivingFlags DRIVING_FLAGS = VehicleDrivingFlags.Normal | VehicleDrivingFlags.StopAtDestination | VehicleDrivingFlags.DriveAroundObjects;

        private const float DIST_AWAY = 80f;
        private const float DEACTIVATE_DRIVER_SCENARIO = 15f;
        private readonly SpawnPoint CameraInterpolatorEndPos = new SpawnPoint(200, new Vector3(219, -1422, 35));

        private static readonly MedicalExaminerData MELS = new MedicalExaminerData()
        {
            Name = "Medical Examiner's Office LS",
            Position = new Vector3(240, -1380, 34),
            TransportRequired = false,
            MarkerOffice = new Vector3(237.67f, -1367.89f, 39.53f),
            MarkerEntrance = new Vector3(240, -1380, 34),
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
    }
}
