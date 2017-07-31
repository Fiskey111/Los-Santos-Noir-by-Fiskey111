using LSNoir.DataAccess;
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
        public string CaseProgressPath { get; set; }
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
            => DataProvider.Instance.GetIdentifiableData<WitnessData>(WitnessesPath, id);

        public ObjectData GetEvidenceData(string id)
            => DataProvider.Instance.GetIdentifiableData<ObjectData>(EvidencePath, id);

        public DeadBodyData GetVictimData(string id)
            => DataProvider.Instance.GetIdentifiableData<DeadBodyData>(VictimsPath, id);

        public FirstOfficerData GetOfficerData(string id)
            => DataProvider.Instance.GetIdentifiableData<FirstOfficerData>(OfficersPath, id);

        public DialogData GetDialogData(string id)
            => DataProvider.Instance.GetIdentifiableData<DialogData>(DialogsPath, id);

        public InterrogationData GetInterrogationData(string id)
            => DataProvider.Instance.GetIdentifiableData<InterrogationData>(InterrogationsPath, id);

        public CoronerData GetCoronerData(string id)
            => DataProvider.Instance.GetIdentifiableData<CoronerData>(CoronersPath, id);

        public EMSData GetEMSData(string id)
            => DataProvider.Instance.GetIdentifiableData<EMSData>(EmsPath, id);

        public ReportData GetReportData(string id)
            => DataProvider.Instance.GetIdentifiableData<ReportData>(ReportsPath, id);

        public StageData GetStageData(string id)
            => DataProvider.Instance.GetIdentifiableData<StageData>(StagesPath, id);

        public NoteData GetNoteData(string id)
            => DataProvider.Instance.GetIdentifiableData<NoteData>(NotesDataPath, id);

        public StageData[] GetAllStagesData()
        {
            var allStages = DataProvider.Instance.Load<List<StageData>>(StagesPath);

            var result = allStages.Where(s => Stages.Contains(s.ID));

            //allStages.RemoveAll(s => !Stages.Contains(s.ID));
            return allStages.ToArray();
        }

        public List<DocumentData> GetRequestableDocuments()
        {
            var allDocs = DataProvider.Instance.Load<List<DocumentData>>(DocumentsDataPath);
            var requestableDocs = allDocs.FindAll(w => CanDocumentBeRequested(w));
            return requestableDocs;
        }

        public DocumentRequestData GetDocuRequestData(string id)
        {
            return GetCaseProgress().RequestedDocuments.FirstOrDefault(d =>d.ID == id);
        }

        public SceneData GetSceneData(string id)
            => DataProvider.Instance.GetIdentifiableData<SceneData>(ScenesDataPath, id);

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

            var progress = DataProvider.Instance.Load<CaseProgress>(CaseProgressPath);
            for (int i = 0; i < ids.Length; i++)
            {
                var elem = progress.CollectedEvidence.FirstOrDefault(e => e.ID == ids[i]);

                if (elem == null)
                {
                    var collectedEvidence = new CollectedEvidenceData(ids[i], DateTime.Now);
                    progress.CollectedEvidence.Add(collectedEvidence);
                }
            }
            DataProvider.Instance.Save(CaseProgressPath, progress);
        }

        public void AddDialogsToProgress(params string[] ids)
        {
            AddUniqueStringIDToCaseProgress(ids, c => c.DialogsPassed);
        }

        private void AddUniqueStringIDToCaseProgress(string[] ids, Func<CaseProgress, ICollection<string>> getCollection)
        {
            if (ids == null || ids.Length < 1) return;

            var progress = DataProvider.Instance.Load<CaseProgress>(CaseProgressPath);
            for (int i = 0; i < ids.Length; i++)
            {
                var collection = getCollection(progress);
                if (!collection.Contains(ids[i]))
                {
                    collection.Add(ids[i]);
                }
            }
            DataProvider.Instance.Save(CaseProgressPath, progress);
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
                DataProvider.Instance.Save(CaseProgressPath, new CaseProgress());
            }

            return DataProvider.Instance.Load<CaseProgress>(CaseProgressPath);
        }

        public void ModifyCaseProgress(Action<CaseProgress> modifier)
        {
            DataProvider.Instance.Modify(CaseProgressPath, modifier);
        }

        //public void ModifyCaseProgress(Func<CaseProgress, CaseProgress> modifier)
        //{
        //    var progress = DataProvider.Instance.Load<CaseProgress>(CaseProgressPath);
        //    var modified = modifier(progress);
        //    DataProvider.Instance.Save(CaseProgressPath, progress);
        //}

        //TODO: move as a static member to DocumentData (string id, CaseData data)?
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
            return DataProvider.Instance.GetIdentifiableData<DocumentData>(DocumentsDataPath, id);
        }

        //TODO: move to DocumentRequestData (string id, CaseData data)
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

        public void SaveWitnessDataToProgress(WitnessData data)
        {
            var cp = DataProvider.Instance.Load<CaseProgress>(CaseProgressPath);

            cp.WitnessesInterviewed.Add(data.ID);

            if (!string.IsNullOrEmpty(data.DialogID)) cp.DialogsPassed.Add(data.DialogID);
            if (data.NotesID != null && data.NotesID.Length > 0) cp.NotesMade.AddRange(data.NotesID);
            if (data.ReportsID != null && data.ReportsID.Length > 0) cp.ReportsReceived.AddRange(data.ReportsID);

            DataProvider.Instance.Save(CaseProgressPath, cp);
        }

        public CaseData()
        {
        }
    }
}
