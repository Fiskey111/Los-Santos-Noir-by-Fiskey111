namespace LSNoir.Callouts.Universal
{
    internal static class Computer
    {
        internal static ComputerController Controller { get; set; }

        internal static void StartComputerHandler() => Controller = new ComputerController();
    }
}
