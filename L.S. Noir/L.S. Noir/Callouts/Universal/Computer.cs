using LSNoir.Callouts.SA.Computer;

namespace LSNoir.Callouts.Universal
{
    internal static class Computer
    {
        internal static ComputerController Controller { get; set; }

        internal static void StartComputerHandler()
        {
            Background.EnableBackground("form_background.jpg");
            Controller = new ComputerController();
        }
    }
}
