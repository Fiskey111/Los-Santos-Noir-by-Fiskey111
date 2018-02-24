using LSNoir.Data;
using LSNoir.Resources;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LSNoir.Stages
{
    class CanvassAreaAndArrest : BasicScript
    {
        private float DistToPlayer(Vector3 s) => s.DistanceTo(Game.LocalPlayer.Character);

        private readonly StageData data;

        private Ped suspect;
        private string suspectId;
        private Vector3 lastPosSuspect;
        private Blip blipSuspect;
        private Blip blipCall;

        private const string MSG_LEAVE_ARRESTED = "Leave the area or deliver the suspect to a station.";
        private const string MSG_LEAVE_KILLED = "Leave the area.";
        private string msg_leave;

        private const string SUSPECT = "canvass_suspect";

        private RouteAdvisor ra;

        public CanvassAreaAndArrest(StageData stageData)
        {
            data = stageData;
        }

        protected override bool Initialize()
        {
            data.CallNotification.DisplayNotification();

            blipCall = Base.SharedStageMethods.CreateBlip(data);

            NativeFunction.Natives.FlashMinimapDisplay();

            ra = new RouteAdvisor(data.CallPosition);

            ra.Start(false, true);

            ActivateStage(Away);

            return true;
        }
        
        private void Away()
        {
            if(DistToPlayer(data.CallPosition) < 150f)
            {
                suspect = SpawnSuspect();

                SwapStages(Away, Close);
            }
        }

        private Ped SpawnSuspect()
        {
            var pd = data.GetSuspectData(SUSPECT);
            suspectId = pd.ID;

            var ped = new Ped(pd.Model, pd.Spawn.Position, pd.Spawn.Heading);
            ped.MakePersistent();

            if (!string.IsNullOrEmpty(pd.Scenario) && ped)
            {
                //NativeFunction.Natives.TASK_START_SCENARIO_IN_PLACE(suspect, pd.Scenario, 0, true);
            }

            return ped;
        }

        private void Close()
        {
            if (!suspect) suspect = SpawnSuspect();

            if(DistToPlayer(data.CallPosition) < 10f)
            {
                ra.Stop();

                blipCall.Delete();

                Game.DisplayHelp("Find and arrest the ~r~suspect~s~.");

                SwapStages(Close, DoesPlayerSeeSuspect);
            }
        }

        private void DoesPlayerSeeSuspect()
        {
            if(suspect.IsOnScreen && DistToPlayer(suspect.Position) < 15f)
            {
                //suspect noticed - camera focus, suspect reaction
                blipSuspect = new Blip(suspect);
                blipSuspect.Color = Color.Red;

                Game.DisplayHelp("Arrest the ~r~suspect~s~.");

                SwapStages(DoesPlayerSeeSuspect, IsDone);
            }
        }
        
        private void IsDone()
        {
            if(suspect.IsDead)
            {
                msg_leave = MSG_LEAVE_KILLED;
                SwapStages(IsDone, LeaveArea);
            }

            if (Functions.IsPedArrested(suspect))
            {
                msg_leave = MSG_LEAVE_ARRESTED;
                SwapStages(IsDone, LeaveArea);
            }
        }
        
        private void LeaveArea()
        {
            Game.DisplaySubtitle(msg_leave, 100);

            if (suspect) lastPosSuspect = suspect.Position; //prevents an exception when suspect
                                                            // gets disposed in jail
            if (DistToPlayer(lastPosSuspect) > 50f)
            {
                SetFinishedAndSaveProgress(false);

                DeactivateStage(LeaveArea);
            }
        }

        private void SetFinishedAndSaveProgress(bool killed)
        {
            data.ParentCase.Progress.SetLastStage(data.ID);

            List<string> nextScripts;

            if (killed)
            {
                data.ParentCase.Progress.AddSuspectsKilled(suspectId);
                nextScripts = data.NextScripts?[1];
            }
            else
            {
                data.ParentCase.Progress.AddSuspectsArrested(suspectId);
                nextScripts = data.NextScripts?[0];
            }

            if (nextScripts == null) nextScripts = new List<string>();

            data.ParentCase.Progress.SetNextScripts(nextScripts);

            SetScriptFinished(true);
        }

        protected override void Process()
        {
        }

        protected override void End()
        {
            ra?.Stop();

            if (blipSuspect) blipSuspect.Delete();
            if (blipCall) blipCall.Delete();
        }
    }
}
