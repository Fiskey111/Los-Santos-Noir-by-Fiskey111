using LSPD_First_Response.Mod.API;
using Rage;
using System;
using LSNoir.Callouts.SA.Commons;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LSNoir.Callouts.SA;
using LSNoir.Extensions;
using LSNoir.RagePro;
using LSNoir.Startup;
using static LtFlash.Common.Serialization.Serializer;
using Plugin = LSPD_First_Response.Mod.API.Plugin;

[assembly: Rage.Attributes.Plugin("L.S. Noir", Description = "A detective style plugin for LSPDFR", Author = "Fiskey111")]
namespace LSNoir
{
    public class Main : Plugin
    {
        public override void Finally() 
        {

        }
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
        }
        internal static bool CompAccess = false, MenuViewed = false, Warrantissued = false, BetterEmsFound = false, AcceptedUser = false;
        internal static CaseData _cData;
        internal static GameFiber StartFiber = new GameFiber(LoadLsn);
        internal const string CDataPath = @"Plugins\LSPDFR\LSNoir\SA\Data\CaseData.xml";
        internal const string PDataPath = @"Plugins\LSPDFR\LSNoir\SA\Data\PedData.xml";
        internal const string EDataPath = @"Plugins\LSPDFR\LSNoir\SA\Data\EvidenceData.xml";
        internal const string RDataPath = @"Plugins\LSPDFR\LSNoir\SA\Data\ReportData.xml";
        internal const string WDataPath = @"Plugins\LSPDFR\LSNoir\SA\Data\WitnessData.xml";
        internal const string SDataPath = @"Plugins\LSPDFR\LSNoir\SA\Data\SuspectData.xml";

        static void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            GameFiber.StartNew(delegate
            {
                if (onDuty)
                {
                    if (!RageCheck.RPHCheck(0.46f)) return;

                    if (!RageProRegistration.RegisterRagePro()) return;

                    //VersionCheck.CheckVersion();

                    Settings.IniUpdateCheck();
                    
                    LoadLsn();
                }
            });
        }
        
        private static void LoadLsn()
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    //"Checking INIs".AddLog();
                    //while (!INIChecker.INICheck())
                    //    GameFiber.Yield();

                    AppDomain.CurrentDomain.AssemblyResolve += LSPDFRResolveEventHandler;

                    if (!CheckFiles())
                    {
                        Game.DisplayNotification(
                            "L.S. Noir was missing scene data/service data\nplease reinstall this modification");
                        return;
                    }

                    "Starting to load L.S. Noir!".AddLog(true);
                    _cData = LoadItemFromXML<CaseData>(Main.CDataPath);

                    PoliceStationCheck.PoliceCheck();

                    RegisterSAStages.RegisterStages(_cData);

                    PrintBanner();

                    BetterEmsFound = DependencyCheck.BetterEMS();

                    ("Finished loading L.S. Noir; ComputerAccess = " + _cData.ComputerAccess).AddLog(true);
                    InteriorHelper.IsCoronerInteriorEnabled = true;
                }
                catch (Exception ex)
                {
                    $"Error loading LS Noir; exception: {ex}".AddLog(true);
                    Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "L.S. Noir",
                        "Created by Fiskey111, LtFlash, Albo1125",
                        "It looks like L.S. Noir ~r~crashed~w~. \nPlease send ~y~Fiskey111~w~ your log.");
                }
            });
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
            Game.Console.Print("VISIT 'Fiskey111Mods.com' TO DONATE");
            Game.Console.Print("If there are any bugs, please report them to me as soon as possible!");
            Game.Console.Print("============== L.S. Noir ==============");

            ("~~~~~~LSN v" + version + " Loaded Successfully~~~~~~").AddLog(true);
        }
        
        private static bool CheckFiles()
        {
            ValidateFile<CaseData>(@"Plugins\LSPDFR\LSNoir\SA\Data\CaseData.xml");
            ValidateFile<PedData>(@"Plugins\LSPDFR\LSNoir\SA\Data\PedData.xml");
            ValidateFile<EvidenceData>(@"Plugins\LSPDFR\LSNoir\SA\Data\EvidenceData.xml");
            ValidateFile<ReportData>(@"Plugins\LSPDFR\LSNoir\SA\Data\ReportData.xml");
            ValidateFile<PedData>(@"Plugins\LSPDFR\LSNoir\SA\Data\WitnessData.xml");
            ValidateFile<PedData>(@"Plugins\LSPDFR\LSNoir\SA\Data\SuspectData.xml");

            return FileCheck(@"Plugins\LSPDFR\LSNoir\SA\Data\SceneData.xml") &&
                   FileCheck(@"Plugins\LSPDFR\LSNoir\SA\Data\ServiceCoordinates.xml");
        }

        public static void ValidateFile<T>(string filePath) where T : new()
        {
            if (!FileCheck(filePath)) SaveItemToXML<T>(new T(), filePath);
        }
        
        private static bool FileCheck(string path)
        {
            if (!File.Exists(path))
            {
                ("XML file DNE at path: " + path).AddLog(true);
                Game.DisplayNotification("XML file at " + path + " ~r~DOES NOT EXIST~w~. Creating a new one.");
                return false;
            }
            ("XML file at " + path + " found successfully").AddLog();
            return true;
        }

        public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
        {
            return Functions.GetAllUserPlugins().FirstOrDefault(assembly => args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()));
        }
    }
}
