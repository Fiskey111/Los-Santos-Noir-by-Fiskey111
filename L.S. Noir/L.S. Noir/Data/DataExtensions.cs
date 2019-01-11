using LtFlash.Common.EvidenceLibrary.Evidence;
using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.EvidenceLibrary.Services;
using Rage;
using System.Collections.Generic;
using System.Linq;

namespace LSNoir.Data
{
    internal static class DataExtensions
    {
        public static FirstOfficer Create(this FirstOfficerData data, StageData ownerStage)
        {
            var dialog = ownerStage.ParentCase.GetResourceByID<DialogData>(data.DialogID);
            return EvidenceFactory.CreateFirstOfficer(data, dialog.Dialog);
        }

        public static DeadBody Create(this DeadBodyData body) => EvidenceFactory.CreateDeadBody(body);

        public static Witness Create(this WitnessData w, StageData stageData)
        {
            var dialog = stageData.ParentCase.GetResourceByID<DialogData>(w.DialogID);
            var wit = EvidenceFactory.CreateWitness(w, dialog.Dialog);
            return wit;
        }

        public static LtFlash.Common.EvidenceLibrary.Evidence.Object Create(this ObjectData objectData)
        {
            return EvidenceFactory.CreateEvidenceObject(objectData);
        }

        public static List<LtFlash.Common.EvidenceLibrary.Evidence.Object> Create(this List<ObjectData> objectData)
        {
            return objectData.Select(o => EvidenceFactory.CreateEvidenceObject(o)).ToList();
        }

        public static EMS Create(this EMSData data, StageData stageData, Ped patient)
        {
            var dialogEms = stageData.ParentCase.GetResourceByID<DialogData>(data.DialogID);
            return ServiceFactory.CreateEMS(patient, dialogEms.Dialog, data);
        }

        public static Coroner Create(this CoronerData data, StageData stageData, Ped victim)
        {
            var dial = stageData.ParentCase.GetResourceByID<DialogData>(data.DialogID);
            return ServiceFactory.CreateCoroner(victim, dial.Dialog, data);
        }
    }
}
