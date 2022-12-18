using CaseManager.Resources;
using LSNoir.Common;
using LSNoir.Common.UI;
using LSNoir.Extensions;
using LSNoir.Startup;
using Rage;
using Rage.Attributes;
using Random = LSNoir.Common.Random;

namespace LSNoir
{
    internal class Commands
    {
        [ConsoleCommand]
        internal static void Command_ReloadCases()
        {
            Logger.LogDebug(nameof(Commands), nameof(Command_ReloadCases), $"Reloading cases");
            Main.InternalCaseManager = new InternalCaseManager();
        }
        
        [ConsoleCommand]
        internal static void Command_SetMaxDebugLines(int number)
        {
            Logger.LogDebug(nameof(Commands), nameof(Command_SetMaxDebugLines), $"Setting max debug lines to: {number}");
            DebugText.MaxDebugLines = number;
        }

        [ConsoleCommand]
        internal static void Command_SavePositionHeadingToClipboard()
        {
            Logger.LogDebug(nameof(Commands), nameof(Command_SavePositionHeadingToClipboard), $"Position saving to clipboard");
            Game.DisplayNotification("Saving position to clipboard");
            Game.SetClipboardText(Newtonsoft.Json.JsonConvert.SerializeObject(new SpawnPoint(Game.LocalPlayer.Character.Heading, Game.LocalPlayer.Character.Position)));
        }

        [ConsoleCommand]
        internal static void Command_SavePositionRotationToClipboard()
        {
            Logger.LogDebug(nameof(Commands), nameof(Command_SavePositionHeadingToClipboard), $"Position saving to clipboard");
            Game.DisplayNotification("Saving position to clipboard");
            var spawn = new SpawnPoint()
            {
                Position = Game.LocalPlayer.Character.Position,
                Rotation = Game.LocalPlayer.Character.Rotation
            };
            Game.SetClipboardText(Newtonsoft.Json.JsonConvert.SerializeObject(spawn));
        }

        [ConsoleCommand]
        internal static void Command_SkipEvidWarWaitTime()
        {
            "Skipping evidence/warrant wait time".AddLog(true);
            for (var i = 0; i < Random.RandomInt(5, 10); i++)
            {
                Game.Console.Print("CHEATER");
                GameFiber.Sleep(10);
            }

            Evid_War_TimeChecker.SkipWaitTimes();
        }
        
        [ConsoleCommand]
        internal static void Command_StartNextStage()
        {
            "Starting next stage".AddLog(true);

            
        }
    }
}
