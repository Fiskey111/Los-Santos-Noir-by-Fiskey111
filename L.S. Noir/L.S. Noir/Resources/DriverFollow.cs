using LtFlash.Common.Processes;
using Rage;
using System.Diagnostics;

namespace LSNoir.Resources
{
    class DriverFollow
    {
        private Entity leader;
        private Ped driver;
        private float leaderSpeed;
        private Vector3 currentDestination;

        private Vector3 LeaderRearPos => leader.GetOffsetPosition(new Vector3(0, -6, 0));
        private float DistToLeaderRear => Vector3.Distance(driver.Position, LeaderRearPos);
        private float Distance => Vector3.Distance(driver.Position, leader);

        private Stopwatch s = new Stopwatch();
        private ProcessHost p = new ProcessHost();

        public DriverFollow(Ped driver, Entity leader, float leaderSpeed)
        {
            this.driver = driver;
            this.leader = leader;
            this.leaderSpeed = leaderSpeed;
        }

        public void Start()
        {
            p.ActivateProcess(CanStart);
            p.Start();
        }

        public void Stop()
        {
            p.Stop();
        }

        private void CanStart()
        {
            if(Distance > 8)
            {
                driver.Tasks.DriveToPosition(LeaderRearPos, 6, VehicleDrivingFlags.Emergency);
                currentDestination = LeaderRearPos;

                s.Reset();

                s.Start();

                p.SwapProcesses(CanStart, Follow);
            }
        }

        //NOTE: refresh rate depends on current speed
        private void Follow()
        {
            IsStopped();

            AntiBraking();

            var updateRate = driver.Speed < 7 ? 5 : 2;

            if (s.Elapsed.Seconds < updateRate) return;

            s.Restart();

            if(Vector3.Distance(driver.Position, leader) > 20)
            {
                driver.Tasks.DriveToPosition(LeaderRearPos, 12, VehicleDrivingFlags.Emergency);
                currentDestination = LeaderRearPos;
            }
            else
            {
                driver.Tasks.DriveToPosition(LeaderRearPos, leaderSpeed, VehicleDrivingFlags.Emergency);
                currentDestination = LeaderRearPos;
            }
        }

        private void IsStopped()
        {
            if (Distance < 7 && leader.Speed == 0)
            {
                s.Stop();

                driver.Tasks.DriveToPosition(driver.Position, 1, VehicleDrivingFlags.Emergency);

                p.SwapProcesses(Follow, CanStart);
            }
        }

        private void AntiBraking()
        {
            var distLeaderToCurrPos = Vector3.Distance(leader.Position, currentDestination);
            var distDriverToCurrPos = Vector3.Distance(driver.Position, currentDestination);

            if (distLeaderToCurrPos > distDriverToCurrPos)
            {
                driver.Tasks.DriveToPosition(LeaderRearPos, 12, VehicleDrivingFlags.Emergency);
                currentDestination = LeaderRearPos;
            }
        }
    }
}
