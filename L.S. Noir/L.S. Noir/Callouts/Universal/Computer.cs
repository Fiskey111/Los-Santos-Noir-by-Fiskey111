using LSNoir.Callouts.SA.Computer;

namespace LSNoir.Callouts.Universal
{
    internal static class Computer
    {
        internal static ComputerController Controller { get; set; }
        internal static bool IsRunning { get; set; }

        internal static void StartComputerHandler()
        {
            IsRunning = true;
            Background.EnableBackground("form_background.jpg");
            Controller = new ComputerController();
            Controller.StartFiber(ComputerController.Fibers.MainFiber);
        }

        internal static void AbortController()
        {
            IsRunning = false;
            Background.DisableBackground(Background.Type.Computer);
            Controller.AbortFiber(Controller.MainFiber);
        }
    }
}
