using LSNoir.DataAccess;
using LtFlash.Common.EvidenceLibrary.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LSNoir.Data
{
    public class CaseProgressHelper
    {
        private string Path { get; }
        private CaseData ParentCase { get; }
        private IDataProvider Provider { get; }

        public CaseProgressHelper(CaseData parent, string progressPath)
        {
            Path = progressPath;
            ParentCase = parent;
            Provider = DataProvider.Instance;
        }

        public void ModifyCaseProgress(Action<CaseProgress> modifier)
        {
            Provider.Modify(Path, modifier);
        }

        public void AddReportsToProgress(params string[] id)
        {
            AddUniqueStringIDToCaseProgress(id, c => c.ReportsReceived);
        }

        public void AddNotesToProgress(params string[] notes)
        {
            AddUniqueStringIDToCaseProgress(notes, c => c.NotesMade);
        }

        public void AddDialogsToProgress(params string[] ids)
        {
            AddUniqueStringIDToCaseProgress(ids, c => c.DialogsPassed);
        }

        public void AddInterrogations(params string[] ids)
        {
            AddUniqueStringIDToCaseProgress(ids, c => c.InterrogationsPassed);
        }

        public void AddSuspectsKilled(params string[] ids)
        {
            AddUniqueStringIDToCaseProgress(ids, c => c.SuspectsKilled);
        }

        public void AddSuspectsArrested(params string[] ids)
        {
            AddUniqueStringIDToCaseProgress(ids, c => c.SuspectsArrested);
        }

        public void AddPersonsTalkedTo(params string[] ids)
        {
            AddUniqueStringIDToCaseProgress(ids, c => c.PersonsTalkedTo);
        }

        public void AddEvidenceToProgress(params string[] ids)
        {
            if (ids == null || ids.Length < 1) return;

            var progress = Provider.Load<CaseProgress>(Path);

            for (int i = 0; i < ids.Length; i++)
            {
                var elem = progress.CollectedEvidence.FirstOrDefault(e => e.ID == ids[i]);

                if (elem == null)
                {
                    var collectedEvidence = new CollectedEvidenceData(ids[i], DateTime.Now);
                    progress.CollectedEvidence.Add(collectedEvidence);
                }
            }

            Provider.Save(Path, progress);
        }


        private void AddUniqueStringIDToCaseProgress(string[] ids, Func<CaseProgress, ICollection<string>> getCollection)
        {
            if (ids == null || ids.Length < 1) return;

            var progress = Provider.Load<CaseProgress>(Path);

            for (int i = 0; i < ids.Length; i++)
            {
                var collection = getCollection(progress);

                if (!collection.Contains(ids[i]))
                {
                    collection.Add(ids[i]);
                }
            }

            Provider.Save(Path, progress);
        }

        public CaseProgress GetCaseProgress()
        {
            if (!File.Exists(Path))
            {
                Provider.Save(Path, new CaseProgress());
            }

            return Provider.Load<CaseProgress>(Path);
        }

        public void SaveWitnessDataToProgress(WitnessData data)
        {
            var cp = Provider.Load<CaseProgress>(Path);

            cp.WitnessesInterviewed.Add(data.ID);

            if (!string.IsNullOrEmpty(data.DialogID)) cp.DialogsPassed.Add(data.DialogID);
            if (data.NotesID != null && data.NotesID.Length > 0) cp.NotesMade.AddRange(data.NotesID);
            if (data.ReportsID != null && data.ReportsID.Length > 0) cp.ReportsReceived.AddRange(data.ReportsID);

            Provider.Save(Path, cp);
        }

        public void SetLastStage(string id)
        {
            Provider.Modify<CaseProgress>(Path, p => p.LastStageID = id);
            Provider.Modify<CaseProgress>(Path, p => p.StagesPassed.Add(id));
        }

        public void SetNextScripts(List<string> next)
        {
            Provider.Modify<CaseProgress>(Path, p => p.NextScripts = next);
        }
    }
}
