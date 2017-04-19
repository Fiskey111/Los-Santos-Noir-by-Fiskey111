using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSNoir.Callouts.SA;
using LSNoir.Extensions;
using Rage;
using Rage.Attributes;

namespace LSNoir
{
    internal class Commands
    {
        [ConsoleCommand]
        internal static void Command_SkipEvidWarWaitTime()
        {
            "Skipping evidence/warrant wait time".AddLog(true);

            for (var i = 0; i < Fiskey111Common.Rand.RandomNumber(5, 25); i++)
            {
                Game.Console.Print("CHEATER");
                GameFiber.Sleep(10);
            }

            Evid_War_TimeChecker.SkipWaitTimes();
        }
    }
}
