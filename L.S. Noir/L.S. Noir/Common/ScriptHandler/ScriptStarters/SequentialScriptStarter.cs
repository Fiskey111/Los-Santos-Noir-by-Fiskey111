using System.Timers;

namespace LSNoir.Common.ScriptHandler.ScriptStarters
{
    public class SequentialScriptStarter : ScriptStarterBase
    {
        private const double INTERVAL = 500;
        private Timer timer = new Timer(INTERVAL);

        public SequentialScriptStarter(IScript s, bool autoRestart) 
            : base(s, autoRestart)
        {
            timer.Elapsed += TimerTick;
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            if (!ScriptStarted || Script.HasFinishedUnsuccessfully)
            {
                if(ScriptStarted && Script.HasFinishedUnsuccessfully && !AutoRestart)
                {
                    timer.Stop();
                    HasFinishedUnsuccessfully = true;
                    return;
                }

                StartScriptInThisTick = true;

                Logger.LogDebug(nameof(SequentialScriptStarter),
                    nameof(TimerTick), ScriptStarted.ToString());
            }
            else if (Script.HasFinishedSuccessfully)
            {
                timer.Stop();
            }
        }

        public override void Start() => timer.Start();

        public override void Stop() => timer.Stop();
    }
}