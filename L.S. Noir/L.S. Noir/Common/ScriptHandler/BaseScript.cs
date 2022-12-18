

// From LtFlash Common - https://github.com/LtFlash/LtFlash.Common
namespace LSNoir.Common.ScriptHandler
{
    public class BaseScript
    {
        public bool HasFinished { get; protected set; }
        public bool Completed { get; protected set; }
        public bool HasFinishedSuccessfully => HasFinished && Completed;
        public bool HasFinishedUnsuccessfully => HasFinished && !Completed;
        public bool IsRunning { get; private set; }
        public IScriptAttributes Attributes { get; set; } = new ScriptAttributes();
    }
}