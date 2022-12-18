using System.Collections.Generic;

namespace LSNoir.Common.ScriptHandler.ScriptStarters
{
    public interface IScriptStarter
    {
        bool HasFinishedSuccessfully { get; }
        bool HasFinishedUnsuccessfully { get; }
        string Id { get; }
        List<string> NextScriptsToRun { get; }
        void Start();
        void Stop();
        IScript Script { get; }
    }
}