using System.Linq;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.SA.Stages;
using LSNoir.Callouts.SA.Stages.CSI;
using LSNoir.Callouts.SA.Stages.ME;
using LSNoir.Extensions;
using LSNoir.Startup;
using LtFlash.Common.ScriptManager.Managers;
using Rage;

namespace LSNoir.Callouts.SA
{
    internal static class RegisterSAStages
    {
        internal static AdvancedScriptManager Asm;
        internal static void RegisterStages(CaseData cData)
        {
            $"Started AdvancedScriptManager with min: {Settings.Settings.MinValue()} max: {Settings.Settings.MaxValue()}".AddLog();
            Asm = new AdvancedScriptManager
            {
                DefaultTimerIntervalMin = Settings.Settings.MinValue(),
                DefaultTimerIntervalMax = Settings.Settings.MaxValue()
            };

            StageRegister.RegisterStage(Asm, typeof(Sa1Csi), nameof(Sa1Csi), null, StageRegister.CreateList("Sa_2aHospital", "SA_2b_MedicalExaminer"), true);
            //StageRegister.RegisterStage(Asm, typeof(Sa_2aHospital), nameof(Sa_2aHospital), StageRegister.CreateList("Sa1Csi"), StageRegister.CreateList("SA_3_VictimFamily"));
            StageRegister.RegisterStage(Asm, typeof(Sa_2BMedicalExaminer), nameof(Sa_2BMedicalExaminer), StageRegister.CreateList("Sa1Csi"), StageRegister.CreateList("Sa_2CStation"));
            StageRegister.RegisterStage(Asm, typeof(Sa_2CStation), nameof(Sa_2CStation), StageRegister.CreateList("Sa_2BMedicalExaminer"), StageRegister.CreateList("SA_3_VictimFamily"));
            StageRegister.RegisterStage(Asm, typeof(SA_3_VictimFamily), nameof(SA_3_VictimFamily), StageRegister.CreateList("Sa_2CStation"), StageRegister.CreateList("Sa_3b_Wait"));
            StageRegister.RegisterStage(Asm, typeof(Sa_3b_Wait), nameof(Sa_3b_Wait), StageRegister.CreateList("SA_3_VictimFamily"), StageRegister.CreateList("Sa_4ASuspectHome"));
            StageRegister.RegisterStage(Asm, typeof(Sa_4ASuspectHome), nameof(Sa_4ASuspectHome), StageRegister.CreateList("Sa_3b_Wait"), StageRegister.CreateList("Sa_4BWait"));
            StageRegister.RegisterStage(Asm, typeof(Sa_4BWait), nameof(Sa_4BWait), StageRegister.CreateList("Sa_4ASuspectHome"), StageRegister.CreateList("Sa_4CSuspectWork"));
            StageRegister.RegisterStage(Asm, typeof(Sa_4CSuspectWork), nameof(Sa_4CSuspectWork), StageRegister.CreateList("Sa_4BWait"), null);

            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(20000);

                if (string.IsNullOrWhiteSpace(cData.StartingStage))
                {
                    "Starting script from CSI -- no starting stage found".AddLog();
                    Asm.StartScript(nameof(Sa1Csi));
                }
                else
                {
                    ("Starting script from stage: " + cData.StartingStage).AddLog();
                    Asm.StartScript(cData.StartingStage);
                } 

                var stage = CaseData.LastStage.None;

                if (cData.CompletedStages.Count > 0) stage = cData.CompletedStages.LastOrDefault();

                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "L.S. Noir",
                    "Created by Fiskey111 and LtFlash",
                    "Loading L.S. Noir...\n\nLast completed stage: ~y~" + ConvertEnumToReadableString(stage, cData));

                Game.DisplayHelp("You have loaded L.S. Noir!\nI would recommend disabling callouts for now if you're planning on working on the case.\n\nThis is a temporary fix, hopefully a permanent one will be available soon.");
            });
        }

        private static string ConvertEnumToReadableString(CaseData.LastStage stage, CaseData caseData)
        {
            switch (stage)
            {
                case CaseData.LastStage.CSI:
                    return "Crime Scene Investigation";
                case CaseData.LastStage.Hospital:
                    return "Visited Hospital";
                case CaseData.LastStage.MedicalExaminer:
                    return "Retreived Medical Examiner Report";
                case CaseData.LastStage.Station:
                    return "Viewed SAJRS";
                case CaseData.LastStage.SuspectHome:
                    return "Interrogated Suspect";
                case CaseData.LastStage.VictimFamily:
                    return "Met with Victim's Family";
                case CaseData.LastStage.Wait:
                    return "In-between Stage";
                default:
                    return "No case found";
            }
        }
    }
}
