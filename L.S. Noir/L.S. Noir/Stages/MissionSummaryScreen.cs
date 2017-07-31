using LSNoir.Data;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using RAGENativeUI.Elements;

namespace LSNoir.Stages
{
    class MissionSummaryScreen : BasicScript
    {
        private StageData data;

        public MissionSummaryScreen(StageData stageData)
        {
            data = stageData;
        }

        protected override bool Initialize()
        {
            Game.LogTrivial("Summary.Initialization()");

            var percentage = 100;
            var medal = MissionPassedScreen.MedalType.Gold;
            var progress = data.ParentCase.GetCaseProgress();

            var witnesses = new MissionPassedScreenItem("Witnesses interrogated", progress.WitnessesInterviewed?.Count.ToString() ?? "0");

            var evidenceCollected = new MissionPassedScreenItem("Evidence collected", progress.CollectedEvidence?.Count.ToString() ?? "0");

            var requestedDocs = new MissionPassedScreenItem("Requested documents", progress.RequestedDocuments?.Count.ToString() ?? "0");

            var screen = new MissionPassedScreen("Case summary", data.ParentCase.Name, percentage, medal);

            screen.Items.Add(witnesses);

            screen.Items.Add(evidenceCollected);

            screen.Items.Add(requestedDocs);

            screen.Show();

            screen.ContinueHit += (s) => SetScriptFinished(true);

            return true;
        }

        protected override void Process()
        {
            
        }

        protected override void End()
        {
        }
    }
}
