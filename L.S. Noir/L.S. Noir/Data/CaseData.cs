using LSNoir.DataAccess;
using LtFlash.Common.EvidenceLibrary.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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
        public CaseProgressHelper Progress => progress;
        [NonSerialized]
        private CaseProgressHelper progress;

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
        private string ScenesDataPath { get; set; }
        private string DocumentsDataPath { get; set; }
        private string NotesDataPath { get; set; }
        private string PersonsDataPath { get; set; }
        private string SuspectsDataPath { get; set; }

        public string CaseProgressPath { get; set; }

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
            PersonsDataPath = Combine(dir, @"\Data\PersonsData.xml");
            SuspectsDataPath = Combine(dir, @"\Data\SuspectsData.xml");

            CaseProgressPath = Combine(dir, @"\Progress\CaseProgress.xml");

            progress = new CaseProgressHelper(this, CaseProgressPath);
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

        public PersonData GetPersonData(string id)
            => DataProvider.Instance.GetIdentifiableData<PersonData>(PersonsDataPath, id);

        public SuspectData GetSuspectData(string id)
            => DataProvider.Instance.GetIdentifiableData<SuspectData>(SuspectsDataPath, id);

        public StageData[] GetAllStagesData()
        {
            var allStages = DataProvider.Instance.Load<List<StageData>>(StagesPath);
            var result = allStages.FindAll(stage => Stages.Contains(stage.ID));

            if(result.Count < Stages.Length)
            {
                var msg = $"Case {ID}: not all stages are defined.";
                throw new InvalidDataException(msg);
            }

            return result.ToArray();
        }

        public List<DocumentData> GetRequestableDocuments()
        {
            var allDocs = DataProvider.Instance.Load<List<DocumentData>>(DocumentsDataPath);
            return allDocs.FindAll(doc => doc.CanDocumentBeRequested(this));
        }

        public DocumentRequestData GetDocuRequestData(string id)
            => Progress.GetCaseProgress().RequestedDocuments.FirstOrDefault(d =>d.ID == id);

        public SceneData GetSceneData(string id)
            => DataProvider.Instance.GetIdentifiableData<SceneData>(ScenesDataPath, id);
        
        public DocumentData GetDocumentDataById(string id)
            => DataProvider.Instance.GetIdentifiableData<DocumentData>(DocumentsDataPath, id);

        //private Dictionary<Type, string> paths = new Dictionary<Type, string>
        //{
        //    [typeof(WitnessData)] = "", 
        //};

        //public T GetIdentifiableData<T>(string id) where T : IIdentifiable
        //{
        //    return DataProvider.Instance.GetIdentifiableData<T>(paths[typeof(T)], id);
        //}

        public CaseData()
        {
        }
    }
}
