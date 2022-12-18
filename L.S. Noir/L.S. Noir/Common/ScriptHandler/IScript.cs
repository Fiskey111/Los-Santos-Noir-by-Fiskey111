namespace LSNoir.Common.ScriptHandler
{
// From LtFlash Common - https://github.com/LtFlash/LtFlash.Common
    public interface IScript
    {
        bool IsRunning { get; }
        bool HasFinished { get; }
        bool Completed { get; }
        bool HasFinishedSuccessfully { get;}
        bool HasFinishedUnsuccessfully { get; }
        IScriptAttributes Attributes { get; set; }

        bool CanBeStarted();
        void Start();
        void SetScriptFinished(bool completed);
    }
}