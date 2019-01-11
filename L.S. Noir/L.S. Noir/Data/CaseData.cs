using LSNoir.DataAccess;
using LSNoir.Resources;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LSNoir.Data
{
    public class CaseData : IIdentifiable
    {
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
        public ResourceTypes ResTypesDef;

        public string CaseProgressPath { get; set; }

        public void SetRootPath(string root)
        {
            var dir = Path.GetDirectoryName(root);

            ResTypesDef = new ResourceTypes()
            {
                new ResourceTypeDefinition(typeof(WitnessData), Join(dir, @"\Data\WitnessesData.xml")),
                new ResourceTypeDefinition(typeof(ObjectData), Join(dir, @"\Data\EvidenceData.xml")),
                new ResourceTypeDefinition(typeof(DeadBodyData), Join(dir, @"\Data\VictimsData.xml")),
                new ResourceTypeDefinition(typeof(FirstOfficerData), Join(dir, @"\Data\OfficersData.xml")),
                new ResourceTypeDefinition(typeof(DialogData), Join(dir, @"\Data\DialogsData.xml")),
                new ResourceTypeDefinition(typeof(ReportData), Join(dir, @"\Data\ReportsData.xml")),
                new ResourceTypeDefinition(typeof(InterrogationData), Join(dir, @"\Data\InterrogationsData.xml")),
                new ResourceTypeDefinition(typeof(CoronerData), Join(dir, @"\Data\CoronersData.xml")),
                new ResourceTypeDefinition(typeof(EMSData), Join(dir, @"\Data\EMSData.xml")),
                new ResourceTypeDefinition(typeof(StageData), Join(dir, @"\Data\StagesData.xml")),
                new ResourceTypeDefinition(typeof(SceneData), Join(dir, @"\Data\ScenesData.xml")),
                new ResourceTypeDefinition(typeof(DocumentData), Join(dir, @"\Data\DocumentsData.xml")),
                new ResourceTypeDefinition(typeof(NoteData), Join(dir, @"\Data\NotesData.xml")),
                new ResourceTypeDefinition(typeof(PersonData), Join(dir, @"\Data\PersonsData.xml")),
                new ResourceTypeDefinition(typeof(SuspectData), Join(dir, @"\Data\SuspectsData.xml")),
            };

            CaseProgressPath = Join(dir, @"\Progress\CaseProgress.xml");

            progress = new CaseProgressHelper(this, CaseProgressPath);
        }

        private static string Join(string s1, string s2) => s1 + s2;

        public T GetResourceByID<T>(string id) where T : class, IIdentifiable
        {
            var path = ResTypesDef.GetPath<T>();
            var data = DataProvider.Instance.GetIdentifiableData<T>(path, id);
            return ProcessResult(data);
        }

        private static T ProcessResult<T>(T x) where T : class, IIdentifiable
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
        public List<T> GetAllCaseResourcesOfType<T>() where T : class, IIdentifiable
        {
            var path = ResTypesDef.GetPath<T>();
            return DataProvider.Instance.Load<List<T>>(path).ToList();
        }

        public DocumentRequestData GetDocuRequestData(string id)
            => Progress.GetCaseProgress().RequestedDocuments.FirstOrDefault(d => d.ID == id);

        public CaseData()
        {
        }
    }
}
