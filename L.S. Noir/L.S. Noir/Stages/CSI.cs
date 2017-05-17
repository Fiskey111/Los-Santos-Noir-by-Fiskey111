using LSNoir.Data;
using LSNoir.Scenes;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.EvidenceLibrary.Evidence;
using LtFlash.Common.EvidenceLibrary.Services;
using Rage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LSNoir.Stages
{
    //NOTES:
    // - what if an ID is added to a list in CaseProgress, then a stage fails?

    //TODO:
    // - witness nor officer does not get deleted on End()
    // - save reports on End to avoid errors caused by crash in the middle of a stage?

    class CSI : Base.StageCalloutScript
    {
        private FirstOfficer officer;
        private DeadBody victim;
        private EMS ems;
        private Coroner coroner;

        private readonly List<Witness> witnesses = new List<Witness>();
        private readonly EvidenceController evidenceCtrl = new EvidenceController();

        private IScene scene;

        private readonly StageData stageData;

        private Keys KEY_START_DIALOG = Settings.Controls.KeyTalkToPed;
        private const Keys KEY_CORONER = Keys.D8;
        private const Keys KEY_CALL_EMS = Keys.D8;

        private const float DIST_CLOSE = 150f;
        private const float DIST_OFFICER = 25f;
        private const float DIST_LEFT = 80f;
        private const string MSG_LEAVE = "Leave the scene";
        private const string MSG_TALK_TO_PO = "Talk to the ~b~First Officer~w~ at scene to receive a preliminary report.";
        private const string MSG_CSI = "Now that you have checked with the first officer, investigate the crime scene.";
        private const string MSG_COLLECT = "Collect evidence and talk to witnesses.";
        private const string SCANNER_NOT_ACCEPTED = "OFFICER_INTRO_01 UNIT_RESPONDING_DISPATCH_04";
        private const string SCANNER_AT_SCENE = "ATTENTION_ALL_UNITS_05 OFFICERS_AT_SCENE NO_FURTHER_UNITS CRIME_AMBULANCE_REQUESTED_02";
        private const string SCANNER_CALL = "ATTN_UNIT_02 DIV_01 ADAM BEAT_12 WE_HAVE AN_ASSAULT OFFICERS_AT_SCENE RESPOND_CODE3";

        private readonly string msgCallCoroner = $"Press ~y~{KEY_CORONER}~s~ to call coroner.";
        private readonly string msgCallEms = $"Press ~y~{KEY_CALL_EMS}~s~ to call EMS.";
        private const string MSG_CORONER_DISPATCHED = "Coroner dispatched to ~y~{0}~s~";

        public CSI(StageData sd) : base()
        {
            stageData = sd;

            var sceneData = stageData.ParentCase.GetSceneData(stageData.SceneID);
            scene = new Scene(sceneData);
        }

        protected override bool Initialize()
        {
            DisplayCalloutInfo(stageData.NotificationTexDic, stageData.NotificationTexName,
                               stageData.NotificationTitle, stageData.NotificationSubtitle, stageData.NotificationText);

            Functions.PlayScannerAudio(SCANNER_CALL);

            ShowAreaBlip(stageData.CallPosition, stageData.CallBlipRad);
            PlaySoundPlayerClosingIn = stageData.PlaySoundClosingIn;

            return true;
        }

        protected override bool Accepted()
        {
            officer = CreateFirstOfficer(stageData);

            victim = CreateVictim(stageData);

            victim.Ped.IsPositionFrozen = true;

            witnesses.AddRange(CreateWitnesses(stageData));

            CreateEvidenceObject(stageData).ForEach(e => evidenceCtrl.AddEvidence(e));

            ems = CreateEMS(stageData, victim.Ped);

            ems.AlwaysNotifyToTeleport = true;

            if (!ems.TakePatientToHospital)
            {
                coroner = CreateCoroner(stageData, victim.Ped);
            }

            ShowAreaWithRoute(stageData.CallPosition, stageData.CallBlipRad, Color.Yellow);

            ActivateStage(Close);

            return true;
        }
        
        protected override void NotAccepted()
        {
            Functions.PlayScannerAudio(SCANNER_NOT_ACCEPTED);
        }
        
        private void Close()
        {
            if (DistToPlayer(victim.Ped) < DIST_CLOSE)
            {
                victim.Ped.IsPositionFrozen = false;

                victim.Ped.IsRagdoll = true;

                //prevent body movement
                GameFiber.StartNew(() =>
                {
                    GameFiber.Wait(3000);
                    victim.Ped.IsPositionFrozen = true;
                });

                scene.Create();

                SwapStages(Close, CheckIfAtScene);
            }
        }

        private void CheckIfAtScene()
        {
            if (DistToPlayer(officer.Ped) < DIST_OFFICER)
            {
                Functions.PlayScannerAudioUsingPosition(SCANNER_AT_SCENE, victim.Position);

                Game.DisplayHelp(MSG_TALK_TO_PO);

                SwapStages(CheckIfAtScene, WaitForDialogEnded); 
            }
        }
        
        private void WaitForDialogEnded()
        {
            if (officer.Dialog.HasEnded)
            {
                var officerReportId = stageData.ParentCase.GetOfficerData(stageData.OfficerID).ReportsID;
                stageData.ParentCase.AddReportsToProgress(officerReportId);

                GameFiber.Sleep(0500);
                //TODO: reset officer's heading!
                officer.Ped.Tasks.PlayAnimation("amb@code_human_police_crowd_control@idle_b", "idle_d", 4, AnimationFlags.Loop);

                victim.CanBeInspected = true;
                victim.PlaySoundPlayerNearby = true;

                Game.DisplayHelp(MSG_CSI);

                SwapStages(WaitForDialogEnded, CheckVictim);
            }
        }

        private void CheckVictim()
        {
            if (victim.Checked)
            {
                var notesVictim = stageData.ParentCase.GetVictimData(stageData.VictimID).NotesID;
                stageData.ParentCase.AddNotesToProgress(notesVictim);

                victim.CanBeInspected = false;
                victim.PlaySoundPlayerNearby = false;

                Game.DisplayHelp(msgCallEms);

                SwapStages(CheckVictim, WaitForEMSCalled);
            }
        }

        private void WaitForEMSCalled()
        {
            if(Game.IsKeyDown(KEY_CALL_EMS))
            {
                ems.Dispatch();
                SwapStages(WaitForEMSCalled, WaitEMSFinished);
            }
        }

        private void WaitEMSFinished()
        {
            if (ems.IsCollected)
            {
                var emsReport = stageData.ParentCase.GetEMSData(stageData.EmsID).ReportID;
                stageData.ParentCase.AddReportsToProgress(emsReport);

                if (coroner != null)
                {
                    Game.DisplayNotification(msgCallCoroner);
                    SwapStages(WaitEMSFinished, CallCoroner);
                }
                else
                {
                    Game.DisplayNotification(MSG_COLLECT);
                    SwapStages(WaitEMSFinished, CollectEvidence);
                }
            }
        }

        private void CallCoroner()
        {
            if (Game.IsKeyDown(KEY_CORONER))
            {
                coroner.Dispatch();
                Game.DisplayNotification(string.Format(MSG_CORONER_DISPATCHED, World.GetStreetName(victim.Position)));
                SwapStages(CallCoroner, IsCoronerDone);
            }
        }

        private void IsCoronerDone()
        {
            if (coroner.IsCollected)
            {
                var coronerReport = stageData.ParentCase.GetCoronerData(stageData.CoronerID).ReportID;
                stageData.ParentCase.AddReportsToProgress(coronerReport);

                Game.DisplayNotification(MSG_COLLECT);

                SwapStages(IsCoronerDone, CollectEvidence);
            }
        }

        private void CollectEvidence()
        {
            if(evidenceCtrl.Evidence.All(e => e.IsImportant && e.IsCollected) &&
                           witnesses.All(w => w.IsImportant && w.IsCollected))
            {
                foreach (var e in evidenceCtrl.Evidence)
                {
                    if(e.IsCollected)
                    {
                        var c = new PieceOfEvidence(e.Id, DateTime.Now);
                        stageData.ParentCase.ModifyCaseProgress(m => m.CollectedEvidence.Add(c));
                    }
                }

                Game.DisplayNotification(MSG_LEAVE);

                SwapStages(CollectEvidence, CheckIfLeftScene);
            }
        }

        private void CheckIfLeftScene()
        {
            Game.DisplaySubtitle(MSG_LEAVE);

            if (DistToPlayer(officer.Ped) > DIST_LEFT)
            {
                SetScriptFinished(true);
            }
        }

        protected override void End()
        {
            stageData.ParentCase.ModifyCaseProgress(c => c.LastStageID = stageData.ID);
            stageData.ParentCase.AddReportsToProgress(stageData.ReportsID);
            stageData.ParentCase.AddNotesToProgress(stageData.NotesID);

            evidenceCtrl?.Evidence?.ForEach(e => e.Dismiss());
            ems?.Dispose();
            coroner?.Dispose();
            //officer?.Ped.DeleteValid();
            officer?.Dismiss();

            victim?.Dismiss();

            //witnesses?.ForEach(w => w.Ped.DeleteValid());
            witnesses?.ForEach(w => w.Dismiss());
            scene?.Dispose();
        }

        ~CSI()
        {
            End();
        }

        protected override void Process()
        {
            if (Game.LocalPlayer.Character.IsDead) SetScriptFinished(false);
            //TODO: for tests
            if(Game.IsKeyDown(Keys.End)) SetScriptFinished(true);
        }
    }
}
