using Rage;
using Rage.Attributes;

namespace LSNoir
{
    internal class Commands
    {
        [ConsoleCommand]
        internal static void Command_SkipEvidWarWaitTime()
        {
            Game.LogTrivial("Skipping evidence/warrant wait time");

            for (var i = 0; i < MathHelper.GetRandomInteger(5, 25); i++)
            {
                Game.Console.Print("CHEATER");
                GameFiber.Sleep(10);
            }
            //currentCase.RequestedDocs - set TimeDecision to Now
        }
    }
}
