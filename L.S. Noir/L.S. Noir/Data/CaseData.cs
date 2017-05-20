using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.Serialization;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Data
{
    public class CaseData : IIdentifiable
    {
        //TODO:
        // - consider storing CaseProgress in a separate folder outside the caseData folder
        // - extract methods and make 'em extensions to keep CaseData dumb

        //SERIALIZABLE PART
        public string ID { get; set; }
        public string Name;
        public string[] Stages = { };
        public string City;
        public string Address;
        public string FirstOfficer;

        //NON SERIALIZABLE PART
        private string WitnessesPath { get; set; }
        private string EvidencePath { get; set; }
        private string VictimsPath { get; set; }
        private string OfficersPath { get; set; }
        private string DialogsPath { get; set; }
        private string ReportsPath { get; set; }
        private string InterrogationsPath { get; set; }
        private string CoronersPath { get; set; }
        private string EmsPath { get; set; }
        private string StagesPath { get; set; }
        private string CaseProgressPath { get; set; }
        private string ScenesDataPath { get; set; }
        private string DocumentsDataPath { get; set; }
        private string NotesDataPath { get; set; }

        public void SetRootPath(string root)
        {
            //var curr = Directory.GetCurrentDirectory();
            //var caseDir = Path.GetDirectoryName(root);
            //var data = "Data";
            //var progress = "Progress";
            //Game.LogTrivial("TEST: GetCurrentDir: " + Path.Combine(curr, caseDir, data));

            var dir = Path.GetDirectoryName(root);
            WitnessesPath = Combine(dir, @"\Data\WitnessesData.xml");
            EvidencePath = Combine(dir, @"\Data\EvidenceData.xml");
            VictimsPath = Combine(dir, @"\Data\VictimsData.xml");
            OfficersPath = Combine(dir, @"\Data\OfficersData.xml");
            DialogsPath = Combine(dir, @"\Data\DialogsData.xml");
            ReportsPath = Combine(dir, @"\Data\ReportsData.xml");
            InterrogationsPath = Combine(dir, @"\Data\InterrogationsData.xml");
            CoronersPath = Combine(dir, @"\Data\CoronersData.xml");
            EmsPath = Combine(dir, @"\Data\EMSData.xml");
            StagesPath = Combine(dir, @"\Data\StagesData.xml");
            ScenesDataPath = Combine(dir, @"\Data\ScenesData.xml");
            DocumentsDataPath = Combine(dir, @"\Data\DocumentsData.xml");
            NotesDataPath = Combine(dir, @"\Data\NotesData.xml");

            CaseProgressPath = Combine(dir, @"\Progress\CaseProgress.xml");
        }

        private static string Combine(string s1, string s2)
            => s1 + s2;

        public WitnessData GetWitnessData(string id)
            => GetData<WitnessData>(WitnessesPath, id);

        public ObjectData GetEvidenceData(string id)
            => GetData<ObjectData>(EvidencePath, id);

        public DeadBodyData GetVictimData(string id)
            => GetData<DeadBodyData>(VictimsPath, id);

        public FirstOfficerData GetOfficerData(string id)
            => GetData<FirstOfficerData>(OfficersPath, id);

        public DialogData GetDialogData(string id)
            => GetData<DialogData>(DialogsPath, id);

        public InterrogationData GetInterrogationData(string id)
            => GetData<InterrogationData>(InterrogationsPath, id);

        public CoronerData GetCoronerData(string id)
            => GetData<CoronerData>(CoronersPath, id);

        public EMSData GetEMSData(string id)
            => GetData<EMSData>(EmsPath, id);

        public ReportData GetReportData(string id)
            => GetData<ReportData>(ReportsPath, id);

        public StageData GetStageData(string id)
            => GetData<StageData>(StagesPath, id);

        public NoteData GetNoteData(string id)
            => GetData<NoteData>(NotesDataPath, id);

        public StageData[] GetAllStagesData()
        {
            var allStages = DataAccess.DataProvider.Instance.Load<List<StageData>>(StagesPath);
            allStages.RemoveAll(s => !Stages.Contains(s.ID));
            return allStages.ToArray();
        }

        public List<DocumentData> GetRequestableDocuments()
        {
            var allDocs = DataAccess.DataProvider.Instance.Load<List<DocumentData>>(DocumentsDataPath);
            var requestableDocs = allDocs.FindAll(w => CanDocumentBeRequested(w));
            return requestableDocs;
        }

        public DocumentRequestData GetDocuRequestData(string id)
        {
            return GetCaseProgress().RequestedDocuments.FirstOrDefault(d =>d.ID == id);
        }

        public SceneData GetSceneData(string id)
            => GetData<SceneData>(ScenesDataPath, id);

        public void AddReportsToProgress(params string[] id)
        {
            AddUniqueStringIDToCaseProgress(id, c => c.ReportsReceived);
        }

        public void AddNotesToProgress(params string[] notes)
        {
            AddUniqueStringIDToCaseProgress(notes, c => c.NotesMade);
        }

        public void AddEvidenceToProgress(params string[] ids)
        {
            if (ids == null || ids.Length < 1) return;

            for (int i = 0; i < ids.Length; i++)
            {
                var collectedEvidence = new CollectedEvidenceData(ids[i], DateTime.Now);
                ModifyCaseProgress(c => AddUniqueElementToCollection(collectedEvidence, c.CollectedEvidence, (c1, c2) => c1.ID == c2.ID));
            }
        }

        public void AddDialogsToProgress(params string[] ids)
        {
            AddUniqueStringIDToCaseProgress(ids, c => c.DialogsPassed);
        }

        private void AddUniqueStringIDToCaseProgress(string[] ids, Func<CaseProgress, ICollection<string>> getCollection)
        {
            if (ids == null || ids.Length < 1) return;

            for (int i = 0; i < ids.Length; i++)
            {
                ModifyCaseProgress(m => AddUniqueElementToCollection(ids[i], getCollection(m), (s1, s2) => s1 == s2));
            }
        }

        private void AddUniqueElementToCollection<T>(T element, ICollection<T> collection, Func<T, T, bool> comparator)
        {
            if (element == null) return;

            if (!collection.Any(c => comparator(element, c)))
            {
                collection.Add(element);
            }
        }

        private void AddUniqueIDToCaseProgress<T>(T id, Func<CaseProgress, IEnumerable<T>> getList, Func<T, T, bool> comparator)
        {
            if (id == null) return;

            var caseProgress = GetCaseProgress();
            var list = getList(caseProgress);

            if(!list.Any(c => comparator(id, c)))
            {
                list.ToList().Add(id);
            }

            ModifyCaseProgress(m => m = caseProgress);
        }

        public CaseProgress GetCaseProgress()
        {
            if(!File.Exists(CaseProgressPath))
            {
                DataAccess.DataProvider.Instance.Save(CaseProgressPath, new CaseProgress());
            }

            return DataAccess.DataProvider.Instance.Load<CaseProgress>(CaseProgressPath);
        }

        public void ModifyCaseProgress(Action<CaseProgress> modifier)
        {
            DataAccess.DataProvider.Instance.Modify(CaseProgressPath, modifier);
        }

        public bool CanDocumentBeRequested(DocumentData id)
        {
            var caseProgress = GetCaseProgress();

            if (!id.DialogIDRequiredToRequest.All(d => caseProgress.DialogsPassed.Contains(d)))
            {
                return false;
            }

            if (!id.EvidenceIDRequiredToRequest.All(e => caseProgress.CollectedEvidence.FirstOrDefault(l => l.ID == e) != null))
            {
                return false;
            }

            if(!id.ReportIDRequiredToRequest.All(r => caseProgress.ReportsReceived.Contains(r)))
            {
                return false;
            }

            if(!id.StageIDRequiredToRequest.All(s => caseProgress.StagesPassed.Contains(s)))
            {
                return false;
            }

            return true;
        }

        public DocumentData GetDocumentDataById(string id)
        {
            return GetData<DocumentData>(DocumentsDataPath, id);
        }

        public bool CanDocumentRequestBeAccepted(string id)
        {
            var caseProgress = GetCaseProgress();
            var docuData = GetDocumentDataById(id);

            if (!docuData.DialogIDRequiredToAccept.All(d => caseProgress.DialogsPassed.Contains(d)))
            {
                return false;
            }

            if (!docuData.EvidenceIDRequiredToAccept.All(e => caseProgress.CollectedEvidence.FirstOrDefault(l => l.ID == e) != null))
            {
                return false;
            }

            if (!docuData.ReportIDRequiredToAccept.All(r => caseProgress.ReportsReceived.Contains(r)))
            {
                return false;
            }

            if (!docuData.StageIDRequiredToAccept.All(s => caseProgress.StagesPassed.Contains(s)))
            {
                return false;
            }

            return true;
        }

        private static T GetData<T>(string path, string id) where T : class, IIdentifiable
        {
            if(!File.Exists(path))
            {
                var msg = $"{nameof(CaseData)}.{nameof(GetData)}(): a specified file could not be found: {path}";
                throw new FileNotFoundException(msg);
            }

            if(string.IsNullOrEmpty(id))
            {
                var msg = $"{nameof(CaseData)}.{nameof(GetData)}(): an ID cannot be empty. Path: {path}";
                throw new ArgumentException(msg);
            }

            var list = DataAccess.DataProvider.Instance.Load<List<IIdentifiable>>(path);

            IIdentifiable item = list.SingleOrDefault(l => l.ID == id);

            if (item == default(T))
            {
                var msg = $"{nameof(CaseData)}.{nameof(GetData)}(): an item with specified ID could not be found. ID: {id}, Path: {path}";
                throw new KeyNotFoundException(msg);
            }

            return item as T;
        }

        public CaseData()
        {
        }
    }
}
