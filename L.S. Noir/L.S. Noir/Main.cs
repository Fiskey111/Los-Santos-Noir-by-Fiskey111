using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Reflection;
using System.Linq;
using LSNoir.Extensions;
using LSNoir.Startup;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CaseManager.NewData;
using LSNoir.Callouts.Stages;
using LSNoir.Common;
using LSNoir.Common.UI;
using Debug = Rage.Debug;
using LSNoir.Callouts;
using LSNoir.Callouts.Universal;
using Rage.Native;
using RAGENativeUI.Elements;

[assembly: Rage.Attributes.Plugin("L.S. Noir", Description = "A detective style plugin for LSPDFR", Author = "Fiskey111", PrefersSingleInstance = true)]
namespace LSNoir
{
    public class Main : Plugin
    {
        internal static bool BetterEmsFound = false;
        internal static InternalCaseManager InternalCaseManager;

        public override void Finally() 
        {

        }
        public override void Initialize()
        {
            DebugText.Initialize();
            InitialLoad();
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
        }

        private void InitialLoad()
        {
            Logger.LogDebug(nameof(Main), nameof(InitialLoad), $"Initial load");
            if (!RageCheck.RPHCheck(1.1f)) return;
            Logger.LogDebug(nameof(Main), nameof(InitialLoad), $"RPH check successful");

            if (!VersionCheck.OldLSNCheck()) return;
            Logger.LogDebug(nameof(Main), nameof(InitialLoad), $"Old LSN check successful");

            AppDomain.CurrentDomain.AssemblyResolve += LSPDFRResolveEventHandler;

            Settings.Settings.IniUpdateCheck();
            Logger.LogDebug(nameof(Main), nameof(InitialLoad), $"Settings check successful");

            StageTypesForCases.Initialize();
        }

        static void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            Logger.LogDebug(nameof(Main), nameof(Functions_OnOnDutyStateChanged), $"Starting duty load: {onDuty}");
            if (!onDuty) return;

            /*
            var readme = new FileChecker(@"Plugins\LSPDFR\LSNoir\Readme.txt", FileChecker.FileType.readme);
            readme.StartFileCheck();
            while (readme.IsRunning)
                GameFiber.Yield();

            if (!readme.IsSuccessful) return;

            var license = new FileChecker(@"Plugins\LSPDFR\LSNoir\License.txt", FileChecker.FileType.license);
            license.StartFileCheck();
            while (license.IsRunning)
                GameFiber.Yield();

            if (!license.IsSuccessful) return;

            var ini = new FileChecker(@"Plugins\LSPDFR\LSNoir\Settings.ini", FileChecker.FileType.ini);
            ini.StartFileCheck();
            while (ini.IsRunning)
                GameFiber.Yield();

            if (!ini.IsSuccessful) return;
            */
            //VersionCheck.CheckVersion();

            LoadLsn();
        }
        
        private static void LoadLsn()
        {
            Logger.LogDebug(nameof(Main), nameof(LoadLsn), $"Starting main load");

            Logger.LogDebug(nameof(Main), nameof(LoadLsn), $"Starting to load cases");
            InternalCaseManager = new InternalCaseManager();

            //PoliceStationCheck.PoliceCheck();

            Evid_War_TimeChecker.StartChecker();

            PrintBanner();

            BetterEmsFound = DependencyCheck.BetterEMS();

            InteriorHelper.IsCoronerInteriorEnabled = true;
            "Finished loading L.S. Noir".AddLog(true);
        }
        
        private static void PrintBanner()
        {
            var versInfo = FileVersionInfo.GetVersionInfo(@"Plugins\LSPDFR\L.S. Noir.dll");
            string fileVersion = versInfo.FileVersion;
            string version = fileVersion;
            Game.Console.Print("============== L.S. Noir ==============");
            Game.Console.Print("One Callout Loaded Successfully!");
            Game.Console.Print("THANK YOU FOR DOWNLOADING L.S. NOIRE v" + version);
            Game.Console.Print("");
            Game.Console.Print("IF YOU ENJOY THIS, PLEASE CONSIDER DONATING TO US!");
            Game.Console.Print("VISIT 'FiskeyMods.com' TO DONATE");
            Game.Console.Print("If there are any bugs, please report them to me as soon as possible!");
            Game.Console.Print("============== L.S. Noir ==============");

            ("~~~~~~LSN v" + version + " Loaded Successfully~~~~~~").AddLog(true);
        }

        public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
        {
            return Functions.GetAllUserPlugins().FirstOrDefault(assembly => args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()));
        }
    }
}
