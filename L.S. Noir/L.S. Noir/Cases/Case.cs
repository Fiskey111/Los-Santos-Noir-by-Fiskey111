using LSNoir.Data;
using LtFlash.Common.ScriptManager.Managers;
using LtFlash.Common.ScriptManager.Scripts;
using LtFlash.Common.Serialization;
using Rage;
using System;
using System.Collections.Generic;

namespace LSNoir.Cases
{
    class Case : BasicScript
    {
        //NOTES:
        // - now handles only one-by-one order of execution
        
        private readonly CaseData data;

        private readonly AdvancedScriptManager manager = new AdvancedScriptManager();

        private const string NAMESPACE_STAGES = "LSNoir.Stages";

        public Case(CaseData caseData)
        {
            data = caseData;
        }

        protected override bool Initialize()
        {
            var stagesData = data.GetAllCaseResourcesOfType<StageData>();

            //LSNoir.DataValidation.SimpleValidator.Validate(data);

            stagesData.ForEach(stage =>
            {
                stage.ParentCase = data;
                RegisterStage(manager, stage);
            });

            var caseProgress = data.Progress.GetCaseProgress();

            if(caseProgress.Finished)
            {
                caseProgress = new CaseProgress();
                Serializer.SaveItemToXML(caseProgress, data.CaseProgressPath);
            }

            var nextScripts = caseProgress.NextScripts ?? new List<string>();

            if(nextScripts.Count == 0)
            {
                manager.Start();
            }
            else
            {
                nextScripts.ForEach(scriptId => manager.StartScript(scriptId));
            }

            return true;
        }

        private static void RegisterStage(AdvancedScriptManager mgr, StageData sdata)
        {
            var stageTypeName = $"{NAMESPACE_STAGES}.{sdata.StageType}";

            var stageType = Type.GetType(stageTypeName, true, true);

            var priorScripts = sdata.FinishPriorThis ?? new List<List<string>>();

            var nextScripts = new List<string>();

            if (sdata.NextScripts?.Count > 0)
            {
                nextScripts = sdata.NextScripts[0];
            }

            var attr = new ScriptAttributes(sdata.ID)
            {
                CtorParams = new object[] { sdata },
                InitModel = EInitModels.TimerBased,
                ScriptsToFinishPriorThis = priorScripts,
                NextScripts = nextScripts,
                TimerIntervalMin = sdata.DelayMinSeconds * 1000,
                TimerIntervalMax = sdata.DelayMaxSeconds * 1000,
            };

            mgr.AddScript(stageType, attr);
        }

        protected override void Process()
        {
            if(manager.HasFinished)
            {
                Game.LogTrivial($"Case: {data.ID} was finished");

                SetScriptFinished(true);
            }
        }

        protected override void End()
        {
            Game.LogTrivial($"Case: {data.ID}.End()");

            data.Progress.ModifyCaseProgress(p => p.Finished = true);
        }
    }
}
