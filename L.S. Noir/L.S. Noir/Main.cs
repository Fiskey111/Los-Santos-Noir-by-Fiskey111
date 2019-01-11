using LSNoir.Cases;
using LSNoir.Computer;
using LSNoir.Settings;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Attributes;
using Rage.ConsoleCommands;
using Rage.ConsoleCommands.AutoCompleters;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

[assembly: Plugin("L.S. Noir", Description = "A detective style plugin for LSPDFR", Author = "Fiskey111", PrefersSingleInstance = true)]
namespace LSNoir
{
    public class Main : Plugin
    {
        internal static CasesController casesCtrl;
        internal static ComputerController computerCtrl;

        internal CasesController Cases
        {
            get
            {
                if(Cases == null) casesCtrl = new CasesController(Paths.PATH_FOLDER_CASES, Paths.FILENAME_CASEDATA);
                return casesCtrl;
            }
        }

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
        private static SDK.SDK_Host SDK;

        private static bool isStarted;
        private static Main ModInstance;

        public Main()
        {
            ModInstance = this;
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

                Game.AddConsoleCommands(new[]
                {
                    ((Action<string>)Command_StartLSN).Method,
                    ((Action)Command_StartLSNSDK).Method,
                    ((Action)Command_StopLSNSDK).Method
                });

                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "Welcome to LS Noir!", "", "Use command >startlsn to start LS Noir.");
            }
        }

        [ConsoleCommand(Description = "Start LS Noir.", Name = "startlsn")]
        private static void Command_StartLSN(
            [ConsoleCommandParameter(AutoCompleterType = typeof(CmdStartLSNParam), 
            Description = "Pick a case you want to start.", 
            Name = "Case to start.", NoAutoCompletion = false)] string caseID = "")
        {
            if (isStarted) return;
            StartMod(caseID);
            isStarted = true;
        }

        [ConsoleCommandParameterAutoCompleter(typeof(string))]
        private class CmdStartLSNParam : ConsoleCommandParameterAutoCompleter
        {
            public CmdStartLSNParam(Type type) : base(type) { }

            public override void UpdateOptions()
            {
                Options.Clear();
                var cases = CasesController.GetAllCasesFromFolder(Paths.PATH_FOLDER_CASES, Paths.FILENAME_CASEDATA);
                cases.ForEach(c => Options.Add(new AutoCompleteOption(c.ID, c.ID, c.Name)));
            }
        }

        [ConsoleCommand(Description ="Loads LS Noir SDK", Name = "startlsnsdk")]
        private static void Command_StartLSNSDK()
        {
            if (SDK is null)
            {
                SDK = new SDK.SDK_Host(ModInstance);
            }
            else
            {
                Game.Console.Print("SDK is already running.");
            }
        }

        [ConsoleCommand(Description = "Stops LS Noir SDK", Name = "stoplsnsdk")]
        private static void Command_StopLSNSDK()
        {
            if (SDK is null)
            {
                Game.Console.Print("SDK is inactive.");
            }
            else
            {
                SDK.Stop();
                SDK = null;
            }
        }

        private static void StartMod(string caseId)
        {
            casesCtrl = new CasesController(Paths.PATH_FOLDER_CASES, Paths.FILENAME_CASEDATA);

            casesCtrl.Start(caseId);

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
            SDK?.Stop();
        }
    }
}
