using LSNoir.Cases;
using LSNoir.Computer;
using LSNoir.Settings;
using LSNoir.Startup;
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
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
        }

        private void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (onDuty)
            {
                if (!RageCheck.RPHCheck(0.46f)) return;

                //if (!RageProRegistration.RegisterRagePro()) return;

                if (!VersionCheck.OldLSNCheck()) return;

                //Settings.IniUpdateCheck();

                casesCtrl = new CasesController(Paths.PATH_FOLDER_CASES, Paths.FILENAME_CASEDATA);

                casesCtrl.Start();

                computerCtrl = new ComputerController(casesCtrl.GetActiveCases()/*() => cd/*cc.CurrentCase*/);

                computerCtrl.Start();

                Game.DisplayNotification("Script manager started.");

                PrintBanner();
            }
        }

        private static void PrintBanner()
        {
            string version = GetLSNVersion();
            for (int i = 0; i < Texts.BANNER.Length; i++)
            {
                Game.Console.Print(Texts.BANNER[i].Replace("{v}", version));
            }
        }

        private static string GetLSNVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var versInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return versInfo.FileVersion;
        }

        public override void Finally()
        {
            //cleanup
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

    internal class Updater
    {
        //dl xml from internet and compare file md5, update
        public Updater(string resourceFileAddress)
        {

        }
    }



    //public class Main : Plugin
    //{
    //    public override void Finally() 
    //    {

    //    }
    //    public override void Initialize()
    //    {
    //        Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
    //    }

    //    internal static GameFiber StartFiber = new GameFiber(LoadLsn);


    //    static void Functions_OnOnDutyStateChanged(bool onDuty)
    //    {
    //        GameFiber.StartNew(delegate
    //        {
    //            bool isLoadStarted = false;
    //            "Loading L.S. Noir".AddLog(); 
    //            if (!onDuty) return;

    //            if (!RageCheck.RPHCheck(0.46f)) return;

    //            if (!RageProRegistration.RegisterRagePro()) return;

    //            if (!VersionCheck.OldLSNCheck()) return;

    //            Settings.IniUpdateCheck();

    //            "LoadLSN".AddLog();
    //            LoadLsn();
    //        });
    //    }

    //    private static void LoadLsn()
    //    {
    //        GameFiber.StartNew(delegate
    //        {
    //            try
    //            {
    //                "Starting".AddLog();

    //                AppDomain.CurrentDomain.AssemblyResolve += LSPDFRResolveEventHandler;


    //                "Starting to load L.S. Noir!".AddLog(true);

    //                _cData = LoadItemFromXML<CaseData>(Main.CDataPath);

    //                PoliceStationCheck.PoliceCheck();

    //                RegisterSAStages.RegisterStages(_cData);

    //                Evid_War_TimeChecker.StartChecker();

    //                PrintBanner();

    //                BetterEmsFound = DependencyCheck.BetterEMS();

    //                ("Finished loading L.S. Noir; ComputerAccess = " + _cData.ComputerAccess).AddLog(true);
    //                InteriorHelper.IsCoronerInteriorEnabled = true;
    //            }
    //            catch (Exception ex)
    //            {
    //                $"Error loading LS Noir; exception: {ex}".AddLog(true);
    //                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "L.S. Noir",
    //                    "Created by Fiskey111, LtFlash, Albo1125",
    //                    "It looks like L.S. Noir ~r~crashed~w~. \nPlease send ~y~Fiskey111~w~ your log.");
    //            }
    //        });
    //    }

    //    private static void PrintBanner()
    //    {
    //        var versInfo = FileVersionInfo.GetVersionInfo(@"Plugins\LSPDFR\L.S. Noir.dll");
    //        string fileVersion = versInfo.FileVersion;
    //        string version = fileVersion;
    //        Game.Console.Print("============== L.S. Noir ==============");
    //        Game.Console.Print("One Callout Loaded Successfully!");
    //        Game.Console.Print("THANK YOU FOR DOWNLOADING L.S. NOIRE v" + version);
    //        Game.Console.Print("");
    //        Game.Console.Print("IF YOU ENJOY THIS, PLEASE CONSIDER DONATING TO US!");
    //        Game.Console.Print("VISIT 'Fiskey111Mods.com' TO DONATE");
    //        Game.Console.Print("If there are any bugs, please report them to me as soon as possible!");
    //        Game.Console.Print("============== L.S. Noir ==============");

    //        ("~~~~~~LSN v" + version + " Loaded Successfully~~~~~~").AddLog(true);
    //    }


    //    public static void ValidateFile<T>(string filePath) where T : new()
    //    {
    //        if (!FileCheck(filePath)) SaveItemToXML<T>(new T(), filePath);
    //    }

    //    private static bool FileCheck(string path)
    //    {
    //        if (!File.Exists(path))
    //        {
    //            ("XML file DNE at path: " + path).AddLog(true);
    //            Game.DisplayNotification("XML file at " + path + " ~r~DOES NOT EXIST~w~. Creating a new one.");
    //            return false;
    //        }
    //        ("XML file at " + path + " found successfully").AddLog();
    //        return true;
    //    }

    //    public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
    //    {
    //        return Functions.GetAllUserPlugins().FirstOrDefault(assembly => args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()));
    //    }
    //}
}
