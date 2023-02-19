using System;
using System.IO;
using CaseManager.NewData;
using LSNoir.Common;
using LSNoir.Common.ScriptHandler;
using LSNoir.Startup;
using Rage;

namespace LSNoir
{
    internal class InternalCaseManager
    {
        private const string RootCaseLocation = @"Plugins\LSPDFR\LSNoir\Cases";

        private CaseController _caseController;

        internal InternalCaseManager()
        {
            try
            {
                _caseController = new CaseController();
                var files = Directory.GetFiles(RootCaseLocation, "*.caseData", SearchOption.AllDirectories);
                Logger.LogDebug(nameof(InternalCaseManager), nameof(InternalCaseManager), $"Found cases: {files.Length}");
                foreach (var file in files)
                {
                    Logger.LogDebug(nameof(InternalCaseManager), nameof(InternalCaseManager), $"Initializing CaseLoader: {file}");
                    var loader = new CaseLoader();
                    var valid = loader.LoadCase(file);
                    if (!valid) continue;
                    Logger.LogDebug(nameof(InternalCaseManager), nameof(InternalCaseManager), $"Case loaded successfully");
                    _caseController.AddCase(loader);
                }
            }
            catch (Exception ex)
            {
                Game.Console.Print("");
                Game.Console.Print("");
                Game.Console.Print("");
                Game.Console.Print("**********************************************************************************************");
                Game.Console.Print("");
                Game.Console.Print(ex.TargetSite.Name);
                Game.Console.Print(ex.ToString());
                Game.Console.Print(ex.StackTrace);
                Game.Console.Print(ex.Message);
                Game.Console.Print(ex.Source);
                Game.Console.Print(ex.InnerException?.TargetSite.Name);
                Game.Console.Print(ex.InnerException?.Source);
                Game.Console.Print(ex.InnerException?.Message);
                Game.Console.Print(ex.InnerException?.StackTrace);
                Game.Console.Print(ex.InnerException?.Source);
                Game.Console.Print("");
                Game.Console.Print("**********************************************************************************************");
                Game.Console.Print("");
                Game.Console.Print("");
                Game.Console.Print("");
            }
        }

        internal Case GetCase(string caseName)
        {
            var files = Directory.GetFiles(RootCaseLocation, "*.caseData", SearchOption.AllDirectories);
            Logger.LogDebug(nameof(InternalCaseManager), nameof(InternalCaseManager), $"Found cases: {files.Length}");
            foreach (var file in files)
            {
                if (!file.ToLowerInvariant().Contains(caseName.ToLowerInvariant())) continue;
                Logger.LogDebug(nameof(InternalCaseManager), nameof(InternalCaseManager), $"Initializing CaseLoader: {file}");
                var caseLoader = new CaseLoader();
                var valid = caseLoader.LoadCase(file);
                if (!valid) continue;
                Logger.LogDebug(nameof(InternalCaseManager), nameof(InternalCaseManager), $"Case loaded successfully");
                return caseLoader.LoadedCase;
            }

            return null;
        }
    }
}