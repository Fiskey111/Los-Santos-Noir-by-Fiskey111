using LSNoir.Data;
using LSNoir.Scenes;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.EvidenceLibrary.Evidence;
using LtFlash.Common.EvidenceLibrary.Services;
using Rage;
using RAGENativeUI.Elements;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
        private FirstOfficer _officer;
        private float _officerInitHeading;
        private DeadBody _victim;
        private EMS _ems;
        private Coroner _coroner;

        private readonly List<Witness> _witnesses = new List<Witness>();
        private readonly EvidenceController _evidenceCtrl = new EvidenceController();

        private IScene _scene;

        private readonly StageData _stageData;

        private Settings.ControlSet _controlStartDialog = Main.Controls.TalkToPed;
        private Settings.ControlSet _controlCallCoroner = Main.Controls.CallCoroner;
        private Settings.ControlSet _controlCallEms = Main.Controls.CallEMS;

        private const string FirstOfficer = "csi_first_officer";
        private const string Victim = "csi_victim";
        private const string Ems = "csi_ems";
        private const string Coroner = "csi_coroner";

        private const float DistClose = 150f;
        private const float DistOfficer = 25f;
        
        private const string MsgLeave = "Leave the scene.";
        private const string MsgTalkToPo = "Talk to the ~b~First Officer~w~ at scene to receive a preliminary report.";
        private const string MsgCsi = "Now that you have checked with the first officer, investigate the crime scene.";
        private const string MsgCollect = "Collect evidence and talk to witnesses.";

        private const string ScannerNotAccepted = "OFFICER_INTRO_01 UNIT_RESPONDING_DISPATCH_04";
        private const string ScannerAtScene = "ATTENTION_ALL_UNITS_05 OFFICERS_AT_SCENE NO_FURTHER_UNITS CRIME_AMBULANCE_REQUESTED_02";
        private const string ScannerCall = "ATTN_UNIT_02 DIV_01 ADAM BEAT_12 WE_HAVE AN_ASSAULT OFFICERS_AT_SCENE RESPOND_CODE3";

        private const string MsgCoronerDispatched = "Coroner dispatched to ~y~{0}~s~.";
        private const string MsgEmsDispatched = "EMS was dispatched to ~y~{0}~s~.";

        private readonly string _msgCallCoroner = "Press ~y~{0}~s~ to call coroner.";
        private readonly string _msgCallEms = "Press ~y~{0}~s~ to call EMS.";

        private bool _isImportantEvidenceCollected;
        private bool _areImportantWitnessesChecked;

        public CSI(StageData sd) : base()
        {
            Game.LogTrivial("CSI.Ctor");

            _stageData = sd;

            var sceneData = _stageData.ParentCase.GetSceneData(_stageData.SceneID);
            _scene = new Scene(sceneData);
        }

        protected override bool Initialize()
        {
            Game.LogTrivial("CSI.Initialize()");

            _stageData.CallNotification.DisplayNotification();

            Functions.PlayScannerAudio(ScannerCall);

            ShowAreaBlip(_stageData.CallPosition, _stageData.CallBlip.Radius, true, true);
            PlaySoundPlayerClosingIn = _stageData.PlaySoundClosingIn;

            return true;
        }

        protected override bool Accepted()
        {
            _officer = _stageData.GetOfficerData(FirstOfficer).Create(_stageData);

            _officerInitHeading = _officer.Ped.Heading;

            _victim = _stageData.GetVictimData(Victim).Create();

            _victim.Ped.IsPositionFrozen = true;

            _stageData.GetAllWitnesses().ForEach(w => _witnesses.Add(w.Create(_stageData)));

            _witnesses.ForEach(w => w.CanBeInspected = false);

            CreateEvidenceObject(_stageData).ForEach(e => _evidenceCtrl.AddEvidence(e));

            _evidenceCtrl.IsActive = false;

            _ems = _stageData.GetEMSData(Ems).Create(_stageData, _victim.Ped);

            _ems.AlwaysNotifyToTeleport = true;

            if (!_ems.TakePatientToHospital)
            {
                _coroner = _stageData.GetCoronerData(Coroner).Create(_stageData, _victim.Ped);
            }

            ShowAreaWithRoute(_stageData.CallPosition, _stageData.CallBlip.Radius, ColorTranslator.FromHtml(_stageData.CallBlip.Color));

            ActivateStage(Close);

            return true;
        }
        
        protected override void NotAccepted()
        {
            Functions.PlayScannerAudio(ScannerNotAccepted);
        }
        
        private void Close()
        {
            if (DistToPlayer(_victim.Ped) < DistClose)
            {
                _victim.Ped.IsPositionFrozen = false;

                _victim.Ped.IsRagdoll = true;

                //prevent body movement
                GameFiber.StartNew(() =>
                {
                    GameFiber.Wait(3000);
                    _victim.Ped.IsPositionFrozen = true;
                });

                _scene.Create();

                SwapStages(Close, CheckIfAtScene);
            }
        }

        private void CheckIfAtScene()
        {
            if (DistToPlayer(_officer.Ped) < DistOfficer)
            {
                Functions.PlayScannerAudioUsingPosition(ScannerAtScene, _victim.Position);

                Game.DisplayHelp(MsgTalkToPo);

                SwapStages(CheckIfAtScene, WaitForDialogEnded); 
            }
        }
        
        private void WaitForDialogEnded()
        {
            if (_officer.Dialog.HasEnded)
            {
                var officerData = _stageData.GetOfficerData(FirstOfficer);
                _stageData.ParentCase.Progress.ModifyCaseProgress(m => m.Officers.Add(officerData.ID));
                _stageData.ParentCase.Progress.AddReportsToProgress(officerData.ReportsID);

                GameFiber.Sleep(0500);

                Rage.Native.NativeFunction.Natives.TASK_ACHIEVE_HEADING(_officer.Ped, _officerInitHeading, 3000);

                GameFiber.Wait(3000);

                _officer.Ped.Tasks.PlayAnimation("amb@code_human_police_crowd_control@idle_b", "idle_d", 4, AnimationFlags.Loop);

                _victim.CanBeInspected = true;
                _victim.PlaySoundPlayerNearby = true;

                Game.DisplayHelp(MsgCsi);

                SwapStages(WaitForDialogEnded, CheckVictim);
            }
        }

        private void CheckVictim()
        {
            if (_victim.Checked)
            {
                var victimData = _stageData.GetVictimData(Victim);
                _stageData.ParentCase.Progress.ModifyCaseProgress(m => m.Victims.Add(victimData.ID));
                _stageData.ParentCase.Progress.AddNotesToProgress(victimData.NotesID);

                _victim.CanBeInspected = false;
                _victim.PlaySoundPlayerNearby = false;

                _evidenceCtrl.IsActive = true;

                _witnesses.ForEach(w => w.CanBeInspected = true);

                _controlCallEms.ColorTag = "y";
                Game.DisplayHelp(string.Format(_msgCallEms, _controlCallEms.GetDescription()));

                ActivateStage(DisplayKeyToCallEms);

                SwapStages(CheckVictim, WaitForEmsCalled);
            }
        }

        private void DisplayKeyToCallEms()
        {
            Game.DisplaySubtitle(string.Format(_msgCallEms, _controlCallEms.GetDescription()), 100);
        }

        private void WaitForEmsCalled()
        {
            if(_controlCallEms.IsActive())
            {
                DeactivateStage(DisplayKeyToCallEms);

                Game.HideHelp();

                _ems.Dispatch();

                Game.DisplayNotification(string.Format(MsgEmsDispatched, World.GetStreetName(_stageData.CallPosition)));

                SwapStages(WaitForEmsCalled, WaitEmsFinished);
            }
        }

        private void WaitEmsFinished()
        {
            if (_ems.IsCollected)
            {
                var emsReport = _stageData.GetEMSData(Ems);
                _stageData.ParentCase.Progress.AddReportsToProgress(emsReport.ID);

                if (_coroner != null)
                {
                    _controlCallCoroner.ColorTag = "~y~";
                    Game.DisplayHelp(string.Format(_msgCallCoroner, _controlCallCoroner.GetDescription()));

                    ActivateStage(DisplayKeyToCallCoroner);

                    SwapStages(WaitEmsFinished, CallCoroner);
                }
                else
                {
                    Game.DisplayNotification(MsgCollect);

                    DeactivateStage(WaitEmsFinished);

                    ActivateStage(IsEvidenceCollected);
                    ActivateStage(AreWitnessesChecked);
                    ActivateStage(CanLeaveTheScene);
                }
            }
        }

        private void DisplayKeyToCallCoroner()
        {
            Game.DisplaySubtitle(_msgCallCoroner, 100);
        }

        private void CallCoroner()
        {
            if (_controlCallCoroner.IsActive())
            {
                DeactivateStage(DisplayKeyToCallCoroner);

                Game.HideHelp();

                _coroner.Dispatch();

                var txtCoronerDisp = string.Format(MsgCoronerDispatched, World.GetStreetName(_stageData.CallPosition));

                Game.DisplayNotification(txtCoronerDisp);

                SwapStages(CallCoroner, IsCoronerDone);
            }
        }

        private void IsCoronerDone()
        {
            if (_coroner.IsCollected)
            {
                var coronerReport = _stageData.GetCoronerData(Coroner).ReportID;
                _stageData.ParentCase.Progress.AddReportsToProgress(coronerReport);

                Game.DisplayNotification(MsgCollect);

                DeactivateStage(IsCoronerDone);

                ActivateStage(IsEvidenceCollected);
                ActivateStage(AreWitnessesChecked);
                ActivateStage(CanLeaveTheScene);
            }
        }
        

        private void IsEvidenceCollected()
        {
            var importantEvidence = _evidenceCtrl.Evidence.Where(e => e.IsImportant);

            if (importantEvidence.All(e => e.IsCollected))
            {
                importantEvidence.ToList().ForEach(e => _stageData.ParentCase.Progress.AddEvidenceToProgress(e.Id));

                _isImportantEvidenceCollected = true;

                DeactivateStage(IsEvidenceCollected);
            }
        }

        private void AreWitnessesChecked()
        {
            var importantWitnesses = _witnesses.Where(w => w.IsImportant);

            if (importantWitnesses.All(w => w.Checked))
            {
                importantWitnesses.ToList().ForEach(w => AddToProgress(w));

                _areImportantWitnessesChecked = true;

                DeactivateStage(AreWitnessesChecked);
            }

            void AddToProgress(Witness wit)
            {
                var witnessData = _stageData.ParentCase.GetWitnessData(wit.Id);
                _stageData.ParentCase.Progress.SaveWitnessDataToProgress(witnessData);
            }
        }

        private void CanLeaveTheScene()
        {
            if(_isImportantEvidenceCollected && _areImportantWitnessesChecked)
            {
                SwapStages(CanLeaveTheScene, CheckIfLeftScene);
            }
        }

        private void CheckIfLeftScene()
        {
            Game.DisplaySubtitle(MsgLeave);

            if (DistToPlayer(_officer.Ped) > _stageData.CallAreaRadius)
            {
                SetSuccessfulyFinishedAndSave();

                DeactivateStage(CheckIfLeftScene);
            }
        }

        private void SetSuccessfulyFinishedAndSave()
        {
            _stageData.ParentCase.Progress.SetLastStage(_stageData.ID);
            _stageData.ParentCase.Progress.SetNextScripts(_stageData.NextScripts.First());

            _stageData.ParentCase.Progress.AddReportsToProgress(_stageData.Reports.Select(r => r.Value).ToArray());
            _stageData.ParentCase.Progress.AddNotesToProgress(_stageData.Notes.Select(n => n.Value).ToArray());

            SetScriptFinished(true);

            DisplayMissionPassedScreen();
        }

        private void DisplayMissionPassedScreen()
        {
            int firstOfficer = 10;
            int evidencePercent = 40;
            int bodyPercent = 50;

            int percent = 0;

            float evd = (float)_evidenceCtrl.Evidence.Count(e => e.Checked) / (float)_evidenceCtrl.Evidence.Count;

            var percentEvd = evd * evidencePercent;

            percent += (int)percentEvd;

            if (_victim.Checked) percent += bodyPercent;

            if (_officer.Checked) percent += firstOfficer;

            MissionPassedScreen.MedalType medal = percent > 80 ? MissionPassedScreen.MedalType.Gold : 
                percent > 65 ? MissionPassedScreen.MedalType.Silver : MissionPassedScreen.MedalType.Bronze;

            var screen = new MissionPassedScreen("Crime scene", percent, medal);

            var officerItem = new MissionPassedScreenItem("First Officer", "", _officer.Checked ? MissionPassedScreenItem.TickboxState.Tick : MissionPassedScreenItem.TickboxState.None);

            screen.Items.Add(officerItem);

            var body = new MissionPassedScreenItem("Victim", "", _victim.Checked ? MissionPassedScreenItem.TickboxState.Tick : MissionPassedScreenItem.TickboxState.None);

            screen.Items.Add(body);

            var evdItem = new MissionPassedScreenItem("Evidence", $"{_evidenceCtrl.Evidence.Count(e => e.Checked)}/{_evidenceCtrl.Evidence.Count}", evd == 1 ? MissionPassedScreenItem.TickboxState.Tick : MissionPassedScreenItem.TickboxState.Empty);

            screen.Items.Add(evdItem);

            screen.Show();
        }

        protected override void End()
        {
            _evidenceCtrl?.Evidence?.ForEach(e => e.Dismiss());
            _ems?.Dispose();
            _coroner?.Dispose();
            //officer?.Ped.DeleteValid();
            _officer?.Dismiss();

            _victim?.Dismiss();

            //witnesses?.ForEach(w => w.Ped.DeleteValid());
            _witnesses?.ForEach(w => w.Dismiss());
            _scene?.Dispose();
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
