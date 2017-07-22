using LSNoir.Data;
using LtFlash.Common.ScriptManager.Scripts;
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
            int percentage = 0;
            MissionPassedScreen.MedalType medal = MissionPassedScreen.MedalType.Gold;

            //victims

            //evidence

            //witnesses

            //suspect arrested/killed - how to check that?!

            var screen = new MissionPassedScreen(data.ParentCase.Name, "Case summary", percentage, medal);

            screen.Show();

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
