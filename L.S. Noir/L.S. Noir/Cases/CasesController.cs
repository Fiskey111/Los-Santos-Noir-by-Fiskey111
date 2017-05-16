using LSNoir.Data;
using LtFlash.Common.ScriptManager.Managers;
using LtFlash.Common.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace LSNoir.Cases
{
    class CasesController
    {
        //TODO:
        // - make RandomScriptManager accepts ctorParams and use it
        // - additional ctor or method to add only a selected CaseData.xml for tests and shit

        public CaseData CurrentCase { get; private set; }

        //stores data of all cases found in a folder passed as a ctor param
        private readonly List<CaseData> data = new List<CaseData>();

        private readonly AdvancedScriptManager manager = new AdvancedScriptManager();

        public CasesController(string caseFolderPath, string filenameCaseData)
        {
            var di = GetAllCasesFromFolder(caseFolderPath, filenameCaseData);

            data.AddRange(di);

            RegisterCases(manager, data);
        }

        /// <summary>
        /// Finds all files with name CaseData.xml and returns deserialized content
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="dataFileName"></param>
        /// <returns></returns>
        private static List<CaseData> GetAllCasesFromFolder(string folderPath, string dataFileName)
        {
            if (!Directory.Exists(folderPath))
            {
                var msg = $"{nameof(CasesController)}.{nameof(GetAllCasesFromFolder)}: folder was not found: {folderPath}";
                throw new DirectoryNotFoundException(msg);
            }

            string[] caseDataPaths = Directory.GetFiles(folderPath, dataFileName, SearchOption.AllDirectories);

            List<CaseData> cases = new List<CaseData>();

            Array.ForEach(caseDataPaths, path => LoadCaseDataToList(path, cases));

            return cases;
        }

        private static void LoadCaseDataToList(string path, List<CaseData> list)
        {
            var p = Serializer.LoadItemFromXML<CaseData>(path);
            p.SetRootPath(path);
            list.Add(p);
        }

        //TODO:
        // - replace with ASM with RandomScriptManager or register and start by random


        /// <summary>
        /// Uses a collection of CaseData to register cases to a script manager
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="data"></param>
        private static void RegisterCases(AdvancedScriptManager mgr, ICollection<CaseData> data)
        {
            foreach (var d in data)
            {
                mgr.AddScript(typeof(Case), new object[] { d }, d.ID, LtFlash.Common.ScriptManager.Scripts.EInitModels.TimerBased, new List<string>(), new List<List<string>>(), 1000, 1100);
            }
        }

        public void Start()
        {
            //TODO:
            // - always starts the 1st registered script!
            //   save the latest case + rnd next to prevent that
            manager.Start();
        }

        public List<CaseData> GetActiveCases()
        {
            return data.FindAll(d => !d.GetCaseProgress().Finished && !string.IsNullOrEmpty(d.GetCaseProgress().LastStageID));
        }
    }
}
