using System;
using LSNoir.Common.UI;
using Rage;

namespace LSNoir.Common
{
    public class Logger
    {
        public static void LogDebug(string className, string process, string log)
        {
            Game.LogTrivial($"LS NOIR: {className} || {process} || {log}");
            DebugText.AddText($"{className} : {process} || {log}");
        }
    }
}