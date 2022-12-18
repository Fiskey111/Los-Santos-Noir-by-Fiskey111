namespace LSNoir.Common.ScriptHandler.ScriptStarters
{
    public class UnconditionalStartController : IScriptStartController
    {
        public bool CanBeStarted() => true;
    }
}