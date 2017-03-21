using System;
using System.Collections.Generic;
using LSNoir.Extensions;
using LtFlash.Common.ScriptManager.Managers;
using LtFlash.Common.ScriptManager.Scripts;

namespace LSNoir.StageRegistration
{
    public static class StageRegister
    {
        /// <summary>
        /// Registers a stage to the scriptmanager that is passed as a parameter
        /// </summary>
        /// <param name="scriptManager">The specific scriptmanager</param>
        /// <param name="classType">The class being registered</param>
        /// <param name="id">An string used for identification purposes</param>
        /// <param name="before">The stages required before this stage</param>
        /// <param name="after">The stages to be added after this stage</param>
        /// <param name="noPrior">true=no scripts before will be added; 
        /// false=scripts are required before</param>
        public static void RegisterStage(AdvancedScriptManager scriptManager, Type classType, string id, List<string> before, List<string> after, bool noPrior = false)
        {
            $"Beginning registration of stage {id}; noPrior={noPrior}".AddLog();

            var beforeList = new List<List<string>>();
            var afterList = after;

            if (after == null)  afterList = new List<string>();
            
            if (!noPrior)
            {
                beforeList.Add(before);
            }
            
            scriptManager.AddScript(
                classType, id, EInitModels.TimerBased, afterList, beforeList);

            $"Stage {id} successfully registered".AddLog(true);
        }

        internal static List<string> CreateList(string item1, string item2 = null, string item3 = null)
        {
            var sList = new List<string> { item1 };

            if (item2 != null)
                sList.Add(item2);

            if (item3 != null)
                sList.Add(item3);

            return sList;
        }
    }
}
