using LSNoir.Data;
using LSNoir.Scenes;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.EvidenceLibrary.Evidence;
using LtFlash.Common.EvidenceLibrary.Services;
using Rage;
using RAGENativeUI.Elements;
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
    // - auto-EMS -> coroner -> inspect the body -> evidence -> witness?

    /*
    REQUIRED DATA

        StagesData.xml
            - StageType,
            - SceneID,
            - DelayMinSeconds,
            - DelayMaxSeconds,

        and so on... - complete
    */
    class CSI : Base.StageCalloutScript
    {
        private FirstOfficer officer;
        private float heading;
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
        
        private const string MSG_LEAVE = "Leave the scene.";
        private const string MSG_TALK_TO_PO = "Talk to the ~b~First Officer~w~ at scene to receive a preliminary report.";
        private const string MSG_CSI = "Now that you have checked with the first officer, investigate the crime scene.";
        private const string MSG_COLLECT = "Collect evidence and talk to witnesses.";

        private const string SCANNER_NOT_ACCEPTED = "OFFICER_INTRO_01 UNIT_RESPONDING_DISPATCH_04";
        private const string SCANNER_AT_SCENE = "ATTENTION_ALL_UNITS_05 OFFICERS_AT_SCENE NO_FURTHER_UNITS CRIME_AMBULANCE_REQUESTED_02";
        private const string SCANNER_CALL = "ATTN_UNIT_02 DIV_01 ADAM BEAT_12 WE_HAVE AN_ASSAULT OFFICERS_AT_SCENE RESPOND_CODE3";

        private const string MSG_CORONER_DISPATCHED = "Coroner dispatched to ~y~{0}~s~.";
        private const string MSG_EMS_DISPATCHED = "EMS was dispatched to ~y~{0}~s~.";

        private readonly string msgCallCoroner = $"Press ~y~{KEY_CORONER}~s~ to call coroner.";
        private readonly string msgCallEms = $"Press ~y~{KEY_CALL_EMS}~s~ to call EMS.";

        private bool isImportantEvidenceCollected;
        private bool areImportantWitnessesChecked;

        public CSI(StageData sd) : base()
        {
            Game.LogTrivial("CSI.Ctor");

            stageData = sd;

            var sceneData = stageData.ParentCase.GetSceneData(stageData.SceneID);
            scene = new Scene(sceneData);
        }

        protected override bool Initialize()
        {
            Game.LogTrivial("CSI.Initialize()");

            DisplayCalloutInfo(stageData.NotificationTexDic, stageData.NotificationTexName,
                               stageData.NotificationTitle, stageData.NotificationSubtitle, stageData.NotificationText);

            Functions.PlayScannerAudio(SCANNER_CALL);

            ShowAreaBlip(stageData.CallPosition, stageData.CallBlipRad, true, true);
            PlaySoundPlayerClosingIn = stageData.PlaySoundClosingIn;
            
            return true;
        }

        protected override bool Accepted()
        {
            officer = CreateFirstOfficer(stageData);

            heading = officer.Ped.Heading;

            victim = CreateVictim(stageData);

            victim.Ped.IsPositionFrozen = true;

            witnesses.AddRange(CreateWitnesses(stageData));

            witnesses.ForEach(w => w.CanBeInspected = false);

            CreateEvidenceObject(stageData).ForEach(e => evidenceCtrl.AddEvidence(e));
            
            evidenceCtrl.IsActive = false;

            ems = CreateEMS(stageData, victim.Ped);

            ems.AlwaysNotifyToTeleport = true;

            if (!ems.TakePatientToHospital)
            {
                coroner = CreateCoroner(stageData, victim.Ped);
            }

            ShowAreaWithRoute(stageData.CallPosition, stageData.CallBlipRad, ColorTranslator.FromHtml(stageData.CallBlipColor));

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
                var officerData = stageData.ParentCase.GetOfficerData(stageData.OfficerID);
                stageData.ParentCase.Progress.ModifyCaseProgress(m => m.Officers.Add(officerData.ID));
                stageData.ParentCase.Progress.AddReportsToProgress(officerData.ReportsID);

                GameFiber.Sleep(0500);

                Rage.Native.NativeFunction.Natives.TASK_ACHIEVE_HEADING(officer.Ped, heading, 3000);

                GameFiber.Wait(3000);

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
                var victimData = stageData.ParentCase.GetVictimData(stageData.VictimID);
                stageData.ParentCase.Progress.ModifyCaseProgress(m => m.Victims.Add(victimData.ID));
                stageData.ParentCase.Progress.AddNotesToProgress(victimData.NotesID);

                victim.CanBeInspected = false;
                victim.PlaySoundPlayerNearby = false;

                evidenceCtrl.IsActive = true;

                witnesses.ForEach(w => w.CanBeInspected = true);

                Game.DisplayHelp(msgCallEms);

                ActivateStage(DisplayKeyToCallEMS);

                SwapStages(CheckVictim, WaitForEMSCalled);
            }
        }

        private void DisplayKeyToCallEMS()
        {
            Game.DisplaySubtitle(msgCallEms, 100);
        }

        private void WaitForEMSCalled()
        {
            if(Game.IsKeyDown(KEY_CALL_EMS))
            {
                DeactivateStage(DisplayKeyToCallEMS);

                Game.HideHelp();

                ems.Dispatch();

                Game.DisplayNotification(string.Format(MSG_EMS_DISPATCHED, World.GetStreetName(stageData.CallPosition)));

                SwapStages(WaitForEMSCalled, WaitEMSFinished);
            }
        }

        private void WaitEMSFinished()
        {
            if (ems.IsCollected)
            {
                var emsReport = stageData.ParentCase.GetEMSData(stageData.EmsID).ReportID;
                stageData.ParentCase.Progress.AddReportsToProgress(emsReport);

                if (coroner != null)
                {
                    Game.DisplayHelp(msgCallCoroner);

                    ActivateStage(DisplayKeyToCallCoroner);

                    SwapStages(WaitEMSFinished, CallCoroner);
                }
                else
                {
                    Game.DisplayNotification(MSG_COLLECT);

                    DeactivateStage(WaitEMSFinished);

                    ActivateStage(IsEvidenceCollected);
                    ActivateStage(AreWitnessesChecked);
                    ActivateStage(CanLeaveTheScene);
                }
            }
        }

        private void DisplayKeyToCallCoroner()
        {
            Game.DisplaySubtitle(msgCallCoroner, 100);
        }

        private void CallCoroner()
        {
            if (Game.IsKeyDown(KEY_CORONER))
            {
                DeactivateStage(DisplayKeyToCallCoroner);

                Game.HideHelp();

                coroner.Dispatch();

                var txtCoronerDisp = string.Format(MSG_CORONER_DISPATCHED, World.GetStreetName(stageData.CallPosition));

                Game.DisplayNotification(txtCoronerDisp);

                SwapStages(CallCoroner, IsCoronerDone);
            }
        }

        private void IsCoronerDone()
        {
            if (coroner.IsCollected)
            {
                var coronerReport = stageData.ParentCase.GetCoronerData(stageData.CoronerID).ReportID;
                stageData.ParentCase.Progress.AddReportsToProgress(coronerReport);

                Game.DisplayNotification(MSG_COLLECT);

                DeactivateStage(IsCoronerDone);

                ActivateStage(IsEvidenceCollected);
                ActivateStage(AreWitnessesChecked);
                ActivateStage(CanLeaveTheScene);
            }
        }
        

        private void IsEvidenceCollected()
        {
            var importantEvidence = evidenceCtrl.Evidence.Where(e => e.IsImportant);

            if (importantEvidence.All(e => e.IsCollected))
            {
                importantEvidence.ToList().ForEach(e => stageData.ParentCase.Progress.AddEvidenceToProgress(e.Id));

                isImportantEvidenceCollected = true;

                DeactivateStage(IsEvidenceCollected);
            }
        }

        private void AreWitnessesChecked()
        {
            var importantWitnesses = witnesses.Where(w => w.IsImportant);

            if (importantWitnesses.All(w => w.Checked))
            {
                importantWitnesses.ToList().ForEach(w => AddToProgress(w));

                areImportantWitnessesChecked = true;

                DeactivateStage(AreWitnessesChecked);
            }

            void AddToProgress(Witness wit)
            {
                var witnessData = stageData.ParentCase.GetWitnessData(wit.Id);
                stageData.ParentCase.Progress.SaveWitnessDataToProgress(witnessData);
            }
        }

        private void CanLeaveTheScene()
        {
            if(isImportantEvidenceCollected && areImportantWitnessesChecked)
            {
                SwapStages(CanLeaveTheScene, CheckIfLeftScene);
            }
        }

        private void CheckIfLeftScene()
        {
            Game.DisplaySubtitle(MSG_LEAVE);

            if (DistToPlayer(officer.Ped) > stageData.CallAreaRadius)
            {
                SetSuccessfulyFinishedAndSave();

                DeactivateStage(CheckIfLeftScene);
            }
        }

        private void SetSuccessfulyFinishedAndSave()
        {
            stageData.SetThisAsLastStage();
            stageData.SaveNextScriptsToProgress(stageData.NextScripts.First());

            stageData.ParentCase.Progress.AddReportsToProgress(stageData.ReportsID);
            stageData.ParentCase.Progress.AddNotesToProgress(stageData.NotesID);

            SetScriptFinished(true);

            DisplayMissionPassedScreen();
        }

        private void DisplayMissionPassedScreen()
        {
            int firstOfficer = 10;
            int evidencePercent = 40;
            int bodyPercent = 50;

            int percent = 0;

            float evd = (float)evidenceCtrl.Evidence.Count(e => e.Checked) / (float)evidenceCtrl.Evidence.Count;

            var percentEvd = evd * evidencePercent;

            percent += (int)percentEvd;

            if (victim.Checked) percent += bodyPercent;

            if (officer.Checked) percent += firstOfficer;

            MissionPassedScreen.MedalType medal = percent > 80 ? MissionPassedScreen.MedalType.Gold : 
                percent > 65 ? MissionPassedScreen.MedalType.Silver : MissionPassedScreen.MedalType.Bronze;

            var screen = new MissionPassedScreen("Crime scene", percent, medal);

            var officerItem = new MissionPassedScreenItem("First Officer", "", officer.Checked ? MissionPassedScreenItem.TickboxState.Tick : MissionPassedScreenItem.TickboxState.None);

            screen.Items.Add(officerItem);

            var body = new MissionPassedScreenItem("Victim", "", victim.Checked ? MissionPassedScreenItem.TickboxState.Tick : MissionPassedScreenItem.TickboxState.None);

            screen.Items.Add(body);

            var evdItem = new MissionPassedScreenItem("Evidence", $"{evidenceCtrl.Evidence.Count(e => e.Checked)}/{evidenceCtrl.Evidence.Count}", evd == 1 ? MissionPassedScreenItem.TickboxState.Tick : MissionPassedScreenItem.TickboxState.Empty);

            screen.Items.Add(evdItem);

            screen.Show();
        }

        protected override void End()
        {
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

        protected override void Process()
        {
            if (Game.LocalPlayer.Character.IsDead)
            {
                SetScriptFinished(false);
            }
        }
    }
}
