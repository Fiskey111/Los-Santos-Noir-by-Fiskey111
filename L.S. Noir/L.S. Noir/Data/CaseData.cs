﻿using LSNoir.DataAccess;
using LtFlash.Common.EvidenceLibrary.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using LSNoir.Resources;

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
        { 
            var dd = DataProvider.Instance.GetIdentifiableData<DialogData>(DialogsPath, id);
            dd.Dialog.RevalStrings();
            return dd;
        }

        public InterrogationData GetInterrogationData(string id)
        {
            var idata = DataProvider.Instance.GetIdentifiableData<InterrogationData>(InterrogationsPath, id);

            for (int i = 0; i < idata.Lines.Length; i++)
            {
                var e = idata.Lines[i];

                e.PlayerResponseTruth.RevalStrings();
                e.Answer.RevalStrings();

                e.InterrogeeReactionDoubt.RevalStrings();
                e.InterrogeeReactionLie.RevalStrings();
                e.InterrogeeReactionTruth.RevalStrings();

                e.PlayerResponseDoubt.RevalStrings();
                e.PlayerResponseLie.RevalStrings();
                e.Question.RevalStrings();
            }
            return idata;
        }

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

    //=========================================================================
    
    public class CaseData_REDUCED : IIdentifiable
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

        [NonSerialized]
        public ResourceTypes Data;

        public string CaseProgressPath { get; set; }

        public void SetRootPath(string root)
        {
            var dir = Path.GetDirectoryName(root);

            Data = new ResourceTypes()
            {
                new ResourceTypeDefinition(typeof(WitnessData), Combine(dir, @"\Data\WitnessesData.xml")),
                new ResourceTypeDefinition(typeof(EvidenceData), Combine(dir, @"\Data\EvidenceData.xml")),
                new ResourceTypeDefinition(typeof(DeadBodyData), Combine(dir, @"\Data\VictimsData.xml")),
                new ResourceTypeDefinition(typeof(FirstOfficerData), Combine(dir, @"\Data\OfficersData.xml")),
                new ResourceTypeDefinition(typeof(DialogData), Combine(dir, @"\Data\DialogsData.xml")),
                new ResourceTypeDefinition(typeof(ReportData), Combine(dir, @"\Data\ReportsData.xml")),
                new ResourceTypeDefinition(typeof(InterrogationData), Combine(dir, @"\Data\InterrogationsData.xml")),
                new ResourceTypeDefinition(typeof(CoronerData), Combine(dir, @"\Data\CoronersData.xml")),
                new ResourceTypeDefinition(typeof(EMSData), Combine(dir, @"\Data\EMSData.xml")),
                new ResourceTypeDefinition(typeof(StageData), Combine(dir, @"\Data\StagesData.xml")),
                new ResourceTypeDefinition(typeof(SceneData), Combine(dir, @"\Data\ScenesData.xml")),
                new ResourceTypeDefinition(typeof(DocumentData), Combine(dir, @"\Data\DocumentsData.xml")),
                new ResourceTypeDefinition(typeof(NoteData), Combine(dir, @"\Data\NotesData.xml")),
                new ResourceTypeDefinition(typeof(PersonData), Combine(dir, @"\Data\PersonsData.xml")),
                new ResourceTypeDefinition(typeof(SuspectData), Combine(dir, @"\Data\SuspectsData.xml")),
            };

            CaseProgressPath = Combine(dir, @"\Progress\CaseProgress.xml");

            progress = new CaseProgressHelper(this, CaseProgressPath);
        }

        private static string Combine(string s1, string s2) => s1 + s2;

        public T GetResourceByID<T>(string id) where T : class, IIdentifiable
        {
            var path = Data.GetPath(typeof(T));
            var x = DataProvider.Instance.GetIdentifiableData<T>(path, id);

            return ProcessResult(x);
        }

        public T ProcessResult<T>(T x) where T : class, IIdentifiable
        {
            if (x is InterrogationData)
            {
                for (int i = 0; i < (x as InterrogationData).Lines.Length; i++)
                {
                    var e = (x as InterrogationData).Lines[i];

                    e.PlayerResponseTruth.RevalStrings();
                    e.Answer.RevalStrings();

                    e.InterrogeeReactionDoubt.RevalStrings();
                    e.InterrogeeReactionLie.RevalStrings();
                    e.InterrogeeReactionTruth.RevalStrings();

                    e.PlayerResponseDoubt.RevalStrings();
                    e.PlayerResponseLie.RevalStrings();
                    e.Question.RevalStrings();
                }
            }
            else if (x is DialogData)
            {
                (x as DialogData).Dialog.RevalStrings();
            }
            return x;
        }
        
        //STAGE LEVEL =/= CASE LEVEL!!!
        public IEnumerable<T> GetAllCaseResourcesOfType<T>() where T : class, IIdentifiable
        {
            var path = Data.GetPath(typeof(T));
            var allStages = DataProvider.Instance.Load<List<T>>(path);
            return allStages;
        }

        public CaseData_REDUCED()
        {
        }
    }







}
