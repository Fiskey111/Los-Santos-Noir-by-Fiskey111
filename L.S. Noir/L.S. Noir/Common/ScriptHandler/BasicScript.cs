namespace LSNoir.Common.ScriptHandler
{
    public abstract class BasicScript : ScriptBase, IScript
    {
        public BasicScript()
        {
            RegisterStages();
        }

        private void RegisterStages()
        {
            ActivateStage(InternalInitialize);
        }

        private void InternalInitialize()
        {
            Initialize();
            SwapStages(InternalInitialize, Process);
        }

        private void InternalEnd()
        {
            End();
            Stop();
        }

        public void SetScriptFinished(bool completed)
        {
            Completed = completed;
            HasFinished = true;
            InternalEnd();
        }
    }
}