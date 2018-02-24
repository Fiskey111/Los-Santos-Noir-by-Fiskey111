using LSNoir.Cases;
using LSNoir.Computer;
using LSNoir.Settings;
using LSPD_First_Response.Mod.API;
using Rage;
using System.Diagnostics;
using System.IO;
using System.Reflection;

[assembly: Rage.Attributes.Plugin("L.S. Noir", Description = "A detective style plugin for LSPDFR", Author = "Fiskey111", PrefersSingleInstance = true)]
namespace LSNoir
{
    public class Main : Plugin
    {
        private static CasesController casesCtrl;
        private static ComputerController computerCtrl;

        internal static Controls Controls
        {
            get
            {
                if (controls == null)
                {
                    if (!File.Exists(Paths.PATH_CONTROL_CONFIG)) DataAccess.DataProvider.Instance.Save(Paths.PATH_CONTROL_CONFIG, new Controls());
                    controls = DataAccess.DataProvider.Instance.Load<Controls>(Paths.PATH_CONTROL_CONFIG);
                }
                return controls;
            }
        }

        private static Controls controls;

        private static bool isStarted;

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
                //if (!IsRageProLicenseValid()) return;

                PrintConsoleBanner();

                Game.AddConsoleCommands(new[] { ((System.Action)Command_StartLSN).Method });

                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "Welcome to LS Noir!", "", "Use command >startlsn to start LS Noir.");
            }
        }

        [Rage.Attributes.ConsoleCommand(Description = "Start LS Noire", Name = "startlsn")]
        private static void Command_StartLSN()
        {
            if (isStarted) return;
            StartMod();
            isStarted = true;
        }

        private static void StartMod()
        {
            casesCtrl = new CasesController(Paths.PATH_FOLDER_CASES, Paths.FILENAME_CASEDATA);

            casesCtrl.Start();

            computerCtrl = new ComputerController(casesCtrl.GetActiveCases);

            computerCtrl.Start();

            Game.DisplayNotification("Welcome to LS Noire!");
        }

        private bool IsRageProLicenseValid()
        {
            if (RageProRegistration.RegisterRagePro()) return true;
            
            Game.DisplayNotification("~r~LSNoir: obtain a valid license!~s~");
            Game.LogTrivial("LS Noir: obtain a valid license!");
            return false;
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
}
