using System;
using LSNoir.Extensions;
using Rage;

namespace LSNoir.Callouts.SA.Commons
{
    public class TestLoop
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public bool IsTested { get; set; }
        public bool IsRunning { get; set; } 

        public TestLoop()
        {
            IsTested = false;
        }

        public TestLoop(bool getRandomTime)
        {
            if (!getRandomTime) return;
            Hour = rnd.Next(DateTime.Now.Hour, 24);
            Minute = rnd.Next(DateTime.Now.Minute, 60);
            Month = DateTime.Now.Month;
            Day = DateTime.Now.Day;
            IsTested = false;
        }

        public void Start()
        {
            GameFiber.StartNew(delegate
            {
                "Starting testing loop".AddLog();
                IsRunning = true;
                while (!IsTested)
                {
                    if (Month <= DateTime.Now.Month && Day <= DateTime.Now.Day || Hour <= DateTime.Now.Hour && Minute <= DateTime.Now.Minute)
                        Stop();
                    GameFiber.Yield();
                }
            });
        }

        public void Stop()
        {
            IsTested = true;
            "Evidence Lab".DisplayNotification("Your ~g~evidence~w~ has finished testing!");
            IsRunning = false;
        }

        internal static Random rnd = new Random();
    }
}
