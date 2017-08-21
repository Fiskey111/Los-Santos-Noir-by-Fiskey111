using LSNoir.Data;
using LSNoir.DataAccess;
using LSNoir.Settings;
using LtFlash.Common.ScriptManager.Managers;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LSNoir.Cases
{
    class CasesController
    {
        //TODO:
        // - make RandomScriptManager accepts ctorParams and use it
        // - additional ctor or method to add only a selected CaseData.xml for tests and shit

        private readonly List<CaseData> casesData = new List<CaseData>();

        private ScriptManagerBase manager = new ScriptManagerBase();

        public CasesController(string caseFolderPath, string filenameCaseData)
        {
            var casesData = GetAllCasesFromFolder(caseFolderPath, filenameCaseData);

            this.casesData.AddRange(casesData);

            manager.OnScriptFinished += OnCaseFinished;
        }

        private void OnCaseFinished(string id)
        {
            Game.LogTrivial("CasesController.OnCaseFinished: " + id);

            DataProvider.Instance.Modify<OverallProgress>(Paths.PATH_OVERALL_PROGRESS, (m) => m.LastCases.Add(id));

            var caseNo = casesData.FirstOrDefault(c => c.ID == id).Progress.GetCaseProgress().CaseNo;

            DataProvider.Instance.Modify<OverallProgress>(Paths.PATH_OVERALL_PROGRESS, (m) => m.LastCaseNo = caseNo);

            Start();
        }

        private static List<CaseData> GetAllCasesFromFolder(string folderPath, string dataFileName)
        {
            if (!Directory.Exists(folderPath))
            {
                var msg = $"{nameof(CasesController)}.{nameof(GetAllCasesFromFolder)}: folder was not found: {folderPath}";
                throw new DirectoryNotFoundException(msg);
            }

            string[] caseDataPaths = Directory.GetFiles(folderPath, dataFileName, SearchOption.AllDirectories);

            List<CaseData> cases = new List<CaseData>();

            Array.ForEach(caseDataPaths, path => LoadCaseDataSetRootAndAddToList(path, cases));

            return cases;
        }

        private static void LoadCaseDataSetRootAndAddToList(string path, ICollection<CaseData> col)
        {
            var data = DataProvider.Instance.Load<CaseData>(path);
            data.SetRootPath(path);
            col.Add(data);
        }

        public void Start()
        {
            Game.LogTrivial("CasesController.Start()");

            var anyActive = GetAnyActiveCase();

            if(anyActive != null)
            {
                AddAndStart(anyActive.ID);
                return;
            }

            if(!File.Exists(Paths.PATH_OVERALL_PROGRESS))
            {
                DataProvider.Instance.Save(Paths.PATH_OVERALL_PROGRESS, new OverallProgress());
            }

            var overallProgress = DataProvider.Instance.Load<OverallProgress>(Paths.PATH_OVERALL_PROGRESS);

            var notRecentlyUsed = GetCaseNotRecenlyUsed(overallProgress.LastCases, casesData);

            //finished can't be restarted!!!
            AddAndStart(notRecentlyUsed);

            casesData.FirstOrDefault().Progress.ModifyCaseProgress(m => m.CaseNo = overallProgress.LastCaseNo + 1);
        }

        private void AddAndStart(string id)
        {
            manager = new ScriptManagerBase();

            manager.OnScriptFinished += OnCaseFinished;

            var caseData = casesData.FirstOrDefault(s => s.ID == id);

            manager.AddScript(id, typeof(Case), new object[] { caseData });
            manager.StartScriptById(id);
        }

        private CaseData GetAnyActiveCase()
        {
            var active = GetActiveCases();
            if (active.Count < 1) return null;
            return MathHelper.Choose<CaseData>(active);
        }

        private static string GetCaseNotRecenlyUsed(List<string> lastCases, List<CaseData> data)
        {
            var notUsedRecently = data.Select(c => c.ID).Except(lastCases).ToList();

            if (notUsedRecently.Count < 1)
            {
                var lastCase = lastCases.Last();

                DataProvider.Instance.Modify<OverallProgress>(Paths.PATH_OVERALL_PROGRESS, (m) => m.LastCases.Clear());

                notUsedRecently = data.Select(s => s.ID).ToList();

                if(data.Count > 1)
                {
                    notUsedRecently.Remove(lastCase); //prevents the last passed case from restarting
                }
            }

            return MathHelper.Choose<string>(notUsedRecently);
        }

        public List<CaseData> GetActiveCases()
        {
            return casesData.FindAll(caseData => IsCaseActive(caseData));

            bool IsCaseActive(CaseData cd)
            {
                var progress = cd.Progress.GetCaseProgress();
                if (progress.Finished) return false;
                if (string.IsNullOrEmpty(progress.LastStageID)) return false;
                return true;
            }
        }
    }
}
