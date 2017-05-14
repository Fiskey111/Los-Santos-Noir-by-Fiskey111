using Rage;
using Rage.Native;

namespace LSNoir.Resources
{
    public class PedScenarioLoop
    {
        public bool IsActive
        {
            get => run;
            set
            {
                if (value && !run)
                {
                    run = true;
                    StartScenario();
                    GameFiber.StartNew(Process);
                }
                else
                {
                    run = false;
                }
            }
        }

        private bool run;
        private readonly string scenario;
        private readonly Ped p;

        public PedScenarioLoop(Ped ped, string scenario)
        {
            if (string.IsNullOrEmpty(scenario)) return;
            if (!ped) return;

            p = ped;
            this.scenario = scenario;
        }

        private void StartScenario()
        {
            if (string.IsNullOrEmpty(scenario)) return;
            if (!p) return;

            NativeFunction.Natives.TASK_START_SCENARIO_IN_PLACE(p, scenario, 0, true);
        }

        private void Process()
        {
            while (run)
            {
                if (!p)
                {
                    run = false;
                    break;
                }

                if (!NativeFunction.Natives.IsPedUsingAnyScenario<bool>(p))
                {
                    StartScenario();
                }

                GameFiber.Yield();
            }
        }
    }
}
