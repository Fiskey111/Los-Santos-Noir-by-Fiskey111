using LtFlash.Common.EvidenceLibrary.Evidence;
using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.EvidenceLibrary.Services;
using Rage;
using System.Collections.Generic;

namespace LSNoir.Data
{
    internal static class DataExtensions
    {
        public static FirstOfficer Create(this FirstOfficerData data, StageData ownerStage)
        {
            var dialog = ownerStage.ParentCase.GetDialogData(data.DialogID);
            return EvidenceFactory.CreateFirstOfficer(data, dialog.Dialog);
        }

        public static DeadBody Create(this DeadBodyData body) => EvidenceFactory.CreateDeadBody(body);

        public static Witness Create(this WitnessData w, StageData stageData)
        {
            var dialog = stageData.ParentCase.GetDialogData(w.DialogID);
            var wit = EvidenceFactory.CreateWitness(w, dialog.Dialog);
            return wit;
        }

        public static List<LtFlash.Common.EvidenceLibrary.Evidence.Object> CreateEvidenceObject(StageData stageData)
        {
            var oid = stageData.GetAllEvidenceData();
            var result = new List<LtFlash.Common.EvidenceLibrary.Evidence.Object>();
            for (int i = 0; i < oid.Count; i++)
            {
                var obj = EvidenceFactory.CreateEvidenceObject(oid[i]);
                result.Add(obj);
            }
            return result;
        }

        public static EMS Create(this EMSData data, StageData stageData, Ped patient)
        {
            var dialogEms = stageData.ParentCase.GetDialogData(data.DialogID);
            return ServiceFactory.CreateEMS(patient, dialogEms.Dialog, data);
        }

        public static Coroner Create(this CoronerData data, StageData stageData, Ped victim)
        {
            var dial = stageData.ParentCase.GetDialogData(data.DialogID);
            return ServiceFactory.CreateCoroner(victim, dial.Dialog, data);
        }
    }
}
