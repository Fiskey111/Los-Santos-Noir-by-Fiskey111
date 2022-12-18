using System.Collections.Generic;
using LSNoir.Common.Process;

namespace LSNoir.Common.ScriptHandler.ScriptStarters
{
    public abstract class ScriptStarterBase : IScriptStarter
    {
        public string Id => Script.Attributes.Id;
        public bool HasFinishedSuccessfully => Script.HasFinishedSuccessfully;

        public bool HasFinishedUnsuccessfully
        {
            get { return finishedUnsuccessfully || Script.HasFinishedUnsuccessfully; }
            protected set { finishedUnsuccessfully = value; }
        }

        public List<string> NextScriptsToRun => Script.Attributes.NextScripts;

        public IScript Script { get; private set; }

        protected bool StartScriptInThisTick { get; set; }
        protected bool ScriptStarted { get; private set; }
        protected bool AutoRestart { get; private set; }
        protected ProcessHost Stages { get; private set; } 
            = new ProcessHost();

        private bool finishedUnsuccessfully;

        public ScriptStarterBase(IScript script, bool autoRestart)
        {
            Script = script;

            AutoRestart = autoRestart;

            Stages.StartProcess(InternalProcess);
            Stages.Start();
        }

        public abstract void Start();
        public abstract void Stop();

        private void InternalProcess()
        {
            if(StartScriptInThisTick/* && ss.IsRunning*/)
            {
                ScriptStarted = Start(Script);
                StartScriptInThisTick = false;
            }
        }

        public bool Start(IScript script)
        {
            if (Script.HasFinished)
            {
                IScriptAttributes s = ScriptAttributes.Clone(script.Attributes);
                Script = ScriptManager.CreateInstance<IScript>(script.GetType(), s.CtorParams);
                Script.Attributes = s;
            }
            bool b = Script.CanBeStarted();

            if (b) Script.Start();
            return b;
        }
    }
}