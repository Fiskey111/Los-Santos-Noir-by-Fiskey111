using LSNoire.Data;
using LSNoire.Stages;
using LtFlash.Common.ScriptManager.Managers;
using LtFlash.Common.ScriptManager.Scripts;
using LtFlash.Common.Serialization;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Cases
{
    class Case : BasicScript
    {
        //NOTES:
        // - now handles only one-by-one order of execution
        
        //the definition of case
        private readonly CaseData data;

        //CaseProgress contains the player's progress, it is specific for each case
        private CaseProgress Progress => data.GetCaseProgress();

        private readonly List<StageData> stages = new List<StageData>();
        private readonly AdvancedScriptManager manager = new AdvancedScriptManager();

        private const string NAMESPACE_STAGES = "LSNoire.Stages";

        public Case(CaseData cd)
        {
            data = cd;

            //get all stages of this case
            var stagesData = data.GetAllStagesData();
        
            //set this case as a ParentCase to each stage
            Array.ForEach(stagesData, s => s.ParentCase = data);

            stages.AddRange(stagesData);
        }

        private static Type GetStageTypeByName(string name)
        {
            return Type.GetType($"{NAMESPACE_STAGES}.{name}", true, true);
        }

        private static void RegisterStages(AdvancedScriptManager mgr, ICollection<StageData> data)
        {
            foreach (var s in data)
            {
                var type = GetStageTypeByName(s.StageType);
                var ctorParams = new object[] { s };
                var prior = s.FinishPriorThis ?? new List<List<string>>();
                var next = s.NextScripts?.ToList() ?? new List<string>();

                mgr.AddScript(type, ctorParams, s.ID, EInitModels.TimerBased, next, prior, 2000, 3000);
            }
        }

        protected override bool Initialize()
        {
            RegisterStages(manager, stages);

            //if this case was already finished before start it from the beginning
            if(data.GetCaseProgress().Finished)
            {
                data.ModifyCaseProgress(c => c = new CaseProgress());
            }

            //when CaseProgress.LastStageID is empty == the case was just
            //  initiated we want start the 1st script on the list,
            //  otherwise we use the list of all callout stages to get
            //  the next script after the recently finished one
            if (!string.IsNullOrEmpty(Progress.LastStageID))
            {
                //TODO: use CaseProgress.NextScripts?
                var next = GetNextScriptID(data.Stages, Progress.LastStageID);

                manager.StartScript(data.Stages[next]);
            }
            else
            {
                manager.Start();
            }

            return true;
        }

        private static int GetNextScriptID(string[] stages, string lastStage)
        {
            var id = stages.ToList().IndexOf(lastStage) + 1;

            if (id >= stages.Length)
            {
                var msg = $"{nameof(Case)}.{nameof(Initialize)}: next script id is out of range: {id}/{stages.Length}";
                throw new IndexOutOfRangeException(msg);
            }

            return id;
        }

        protected override void Process()
        {
            if(manager.HasFinished)
            {
                SetScriptFinished(true);
            }
        }

        protected override void End()
        {
            data.ModifyCaseProgress(p => p.Finished = true);
        }
    }
}
