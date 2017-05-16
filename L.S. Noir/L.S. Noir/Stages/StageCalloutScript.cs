using LSNoir.Data;
using LtFlash.Common.EvidenceLibrary.Evidence;
using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.EvidenceLibrary.Services;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using System.Collections.Generic;

namespace LSNoir.Stages
{
    abstract class StageCalloutScript : CalloutScript
    {
        //TODO:
        // - add filePath validation
        //create a static entity factory

        protected static Ped Player => Game.LocalPlayer.Character;

        protected static float DistToPlayer(Entity e) => Vector3.Distance(Player.Position, e.Position);

        protected static FirstOfficer CreateFirstOfficer(StageData stageData)
        {
            var officerId = stageData.OfficerID;
            var officerData = stageData.ParentCase.GetOfficerData(officerId);
            var officerDialog = stageData.ParentCase.GetDialogData(officerData.DialogID);
            return EvidenceFactory.CreateFirstOfficer(officerData, officerDialog.Dialog);
        }

        protected static DeadBody CreateVictim(StageData stageData)
        {
            var vid = stageData.VictimID;
            var vd = stageData.ParentCase.GetVictimData(vid);
            return EvidenceFactory.CreateDeadBody(vd);
        }

        protected static List<Witness> CreateWitnesses(StageData stageData)
        {
            var ids = stageData.WitnessID;
            var result = new List<Witness>();
            for (int i = 0; i < ids.Length; i++)
            {
                var data = stageData.ParentCase.GetWitnessData(ids[i]);
                var dialog = stageData.ParentCase.GetDialogData(data.DialogID);
                var wit = EvidenceFactory.CreateWitness(data, dialog.Dialog);
                result.Add(wit);
            }
            return result;
        }

        protected static List<LtFlash.Common.EvidenceLibrary.Evidence.Object> CreateEvidenceObject(StageData stageData)
        {
            var oid = stageData.EvidenceID;
            var result = new List<LtFlash.Common.EvidenceLibrary.Evidence.Object>();
            for (int i = 0; i < oid.Length; i++)
            {
                var d = stageData.ParentCase.GetEvidenceData(oid[i]);
                var obj = EvidenceFactory.CreateEvidenceObject(d);
                result.Add(obj);
            }
            return result;
        }

        protected static EMS CreateEMS(StageData stageData, Ped patient)
        {
            var eid = stageData.EmsID;
            var ed = stageData.ParentCase.GetEMSData(eid);
            var dialogEms = stageData.ParentCase.GetDialogData(ed.DialogID);
            return ServiceFactory.CreateEMS(patient, dialogEms.Dialog, ed);
        }

        protected static Coroner CreateCoroner(StageData stageData, Ped victim)
        {
            var cid = stageData.CoronerID;
            var cd = stageData.ParentCase.GetCoronerData(cid);
            var dial = stageData.ParentCase.GetDialogData(cd.DialogID);
            return ServiceFactory.CreateCoroner(victim, dial.Dialog, cd);
        }
    }
}
