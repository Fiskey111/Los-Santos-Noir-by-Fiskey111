using LSNoir.Extensions;
using Rage;
using Rage.Native;

namespace LSNoir.Callouts.SA.Commons
{
    public class ScenarioHelper
    {
        public string Scenario { get; set; }
        public Ped TargetPed { get; set; }
        public bool Exists { get; set; }
        public bool IsRunning { get; set; }
        private bool _loop = false;

        public ScenarioHelper() { }

        public ScenarioHelper(Ped ped, string scenario)
        {
            Scenario = scenario;
            TargetPed = ped;
        }

        public void Start()
        {
            if (!TargetPed) return;
            Exists = true;
            _loop = true;
            Loop();
        }

        public void Stop()
        {
            if (!TargetPed) return;
            _loop = false;
        }

        private void Loop()
        {
            GameFiber.StartNew(delegate
            {
                IsRunning = true;
                while (_loop)
                {
                    TargetPed.Task_Scenario(Scenario);
                    while (NativeFunction.Natives.IS_PED_USING_ANY_SCENARIO<bool>(TargetPed))
                    {
                        if (!_loop) break;
                        GameFiber.Yield();
                    }
                    GameFiber.Yield();
                }
                IsRunning = false;
            });
        }
    }
}
