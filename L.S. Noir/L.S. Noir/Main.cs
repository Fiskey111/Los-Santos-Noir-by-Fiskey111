﻿using LSNoir.Cases;
using LSNoir.Computer;
using LSNoir.Settings;
using LSPD_First_Response.Mod.API;
using Rage;
using System.Diagnostics;
using System.Reflection;

[assembly: Rage.Attributes.Plugin("L.S. Noir", Description = "A detective style plugin for LSPDFR", Author = "Fiskey111", PrefersSingleInstance = true)]
namespace LSNoir
{
    public class Main : Plugin
    {
        private CasesController casesCtrl;
        private ComputerController computerCtrl;

        public Main()
        {
        }

        public override void Initialize()
        {
            Game.LogVeryVerbose("LS Noir initialized.");

            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
        }

        private void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (onDuty)
            {
                //if (!RageProRegistration.RegisterRagePro())
                //{
                //    Game.DisplayNotification("~r~LSNoir: obtain a valid license!~s~");
                //    Game.LogExtremelyVerbose("LS Noir: obtain a valid license!");
                //    return;
                //}

                PrintConsoleBanner();

                casesCtrl = new CasesController(Paths.PATH_FOLDER_CASES, Paths.FILENAME_CASEDATA);

                casesCtrl.Start();

                computerCtrl = new ComputerController(casesCtrl.GetActiveCases);

                computerCtrl.Start();

                Game.DisplayNotification("Script manager started.");
            }
        }

        private static void PrintConsoleBanner()
        {
            string version = "";//GetLSNVersion();
            for (int i = 0; i < Texts.BANNER.Length; i++)
            {
                Game.Console.Print(Texts.BANNER[i].Replace("{v}", version));
            }
        }

        private static string GetLSNVersion()
        {
            //throws an error
            Assembly assembly = Assembly.GetExecutingAssembly();
            var versInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return versInfo.FileVersion;
        }

        public override void Finally()
        {
            //cleanup
            computerCtrl.Stop();
        }
    }

    internal static class DependencyChecker
    {
        public static bool IsBetterEMSUpToDate()
        {
            return true;
        }

        public static bool IsRPHUpToDate()
        {
            return true;
        }
    }
}
