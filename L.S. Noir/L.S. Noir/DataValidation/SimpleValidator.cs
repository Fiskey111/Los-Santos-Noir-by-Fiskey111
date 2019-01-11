using LSNoir.Data;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;
using System.Collections.Generic;
using System.Linq;
using static LSNoir.Data.StageData;

namespace LSNoir.DataValidation
{
    //NOTE:
    // - dialogs and interrogations may not be ref in StageData but in Suspect, Person etc.
    static class SimpleValidator
    {
        public static void Validate(CaseData cd)
        {
            var ssd = cd.GetAllCaseResourcesOfType<StageData>();

            if(ssd.Count < 1)
            {
                Game.LogTrivial($"Case has no stages: {cd.ID}");
                return;
            }
            var allResRef = new List<ResourceData>();

            foreach (var s in ssd)
            {
                if(s.Resources == null || s.Resources.Length < 0)
                {
                    Game.LogTrivial($"Stage has no resources: {s.ID}");
                    continue;
                }
                allResRef.AddRange(s.Resources);
            }

            if(allResRef.Count < 0)
            {
                Game.LogTrivial($"No resources for case was found: {cd.ID}");
                return;
            }

            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<WitnessData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<SuspectData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<PersonData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<InterrogationData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<DeadBodyData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<FirstOfficerData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<ReportData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<CoronerData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<EMSData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<DocumentData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<NoteData>().ToList<IIdentifiable>(), allResRef, cd);
            AreAllDefinedResourcesReferenced(cd.GetAllCaseResourcesOfType<ObjectData>().ToList<IIdentifiable>(), allResRef, cd);

        }

        private static bool IsReferenced(List<ResourceData> r, IIdentifiable i)
        {
            return r.Any(k => k.ID == i.ID);
        }

        private static bool AreAllDefinedResourcesReferenced(List<IIdentifiable> resourcesOfType, List<ResourceData> r, CaseData cd)
        {
            var res = true;
            foreach (var b in resourcesOfType)
            {
                if(!IsReferenced(r, b))
                {
                    Game.LogTrivial($"SimpleValidator: no ref for resource in StageData. Case: {cd.ID}, id: {b.ID}.");
                    res = false;
                }
            }
            return res;
        }
    }
}
