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
            var r = Serializer.LoadFromXML<StageData>(StagesPath);
            r.RemoveAll(s => !Stages.Contains(s.ID));
            return r.ToArray();
        }

        public List<DocumentData> GetRequestableDocuments()
        {
            return Serializer.LoadFromXML<DocumentData>(DocumentsDataPath).FindAll(w => CanDocumentBeRequested(w));
        }

        public DocumentRequestData GetDocuRequestData(string id)
        {
            return GetCaseProgress().RequestedDocuments.FirstOrDefault(d =>d.ID == id);
        }

        public SceneData GetSceneData(string id)
            => GetData<SceneData>(ScenesDataPath, id);

        public void AddReportById(params string[] id)
        {
            if (id == null || id.Length < 1) return;

            ModifyCaseProgress(m => m.ReportsReceived.AddRange(id));
        }
        
        public CaseProgress GetCaseProgress()
        {
            if(!File.Exists(CaseProgressPath))
            {
                Serializer.SaveItemToXML(new CaseProgress(), CaseProgressPath);
            }

            return Serializer.LoadItemFromXML<CaseProgress>(CaseProgressPath);
        }

        public void ModifyCaseProgress(Action<CaseProgress> modifier)
        {
            Serializer.ModifyItemInXML(CaseProgressPath, modifier);
        }

        public bool CanDocumentBeRequested(DocumentData id)
        {
            var cp = GetCaseProgress();

            if (!id.DialogIDRequiredToRequest.All(d => cp.DialogsPassed.Contains(d)))
            {
                Game.LogTrivial("DIALOG");
                return false;
            }

            if (!id.EvidenceIDRequiredToRequest.All(e => cp.CollectedEvidence.FirstOrDefault(l => l.ID == e) != null))
            {
                Game.LogTrivial("evd");

                return false;
            }

            if(!id.ReportIDRequiredToRequest.All(r => cp.ReportsReceived.Contains(r)))
            {
                Game.LogTrivial("rep");

                return false;
            }

            if(!id.StageIDRequiredToRequest.All(s => cp.StagesPassed.Contains(s)))
            {
                Game.LogTrivial("stage");

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
            var cp = GetCaseProgress();
            var dd = GetDocumentDataById(id);

            if (!dd.DialogIDRequiredToAccept.All(d => cp.DialogsPassed.Contains(d)))
            {
                return false;
            }

            if (!dd.EvidenceIDRequiredToAccept.All(e => cp.CollectedEvidence.FirstOrDefault(l => l.ID == e) != null))
            {
                return false;
            }

            if (!dd.ReportIDRequiredToAccept.All(r => cp.ReportsReceived.Contains(r)))
            {
                return false;
            }

            if (!dd.StageIDRequiredToAccept.All(s => cp.StagesPassed.Contains(s)))
            {
                return false;
            }

            return true;
        }

        private static T GetData<T>(string path, string id) where T: IIdentifiable
        {
            return Serializer.GetSelectedListElementFromXml<T>(path, l => l.SingleOrDefault(e => e.ID == id));
        }

        public CaseData()
        {
        }
    }
}
