using System.Collections.Generic;
using CaseManager.NewData;
using LSNoir.Common;
using LSNoir.Common.Process;
using Rage;

namespace LSNoir.Callouts.Stages
{
    public class DialogueScene : StageBase
    {
        public DialogueScene(Case caseRef, Stage stage) : base(caseRef, stage)
        {
        }

        private void DialogueScene_OnArrivedAtScene()
        {

        }


        public override bool Initialize()
        {
            Logger.LogDebug(nameof(DialogueScene), nameof(Initialize), "Initialize");

            OnArrivedAtScene += DialogueScene_OnArrivedAtScene;
            Logger.LogDebug(nameof(DialogueScene), nameof(Initialize), "Initialize completed");

            return true;
        }

        public override void EnRoute() { }

        protected override void Process()
        {            
            
        }

        private void End()
        {            
            Logger.LogDebug(nameof(DialogueScene), nameof(End), $"Ending");
            base.End(true);
        }
    }
}