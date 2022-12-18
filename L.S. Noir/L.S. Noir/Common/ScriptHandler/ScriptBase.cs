using System;
using LSNoir.Common.Process;
using LSNoir.Common.ScriptHandler.ScriptStarters;
using Rage;

namespace LSNoir.Common.ScriptHandler
{
    public abstract class ScriptBase
    {
        //PUBLIC
        public bool HasFinished { get; protected set; }
        public bool Completed { get; protected set; }
        public bool HasFinishedSuccessfully => HasFinished && Completed;
        public bool HasFinishedUnsuccessfully => HasFinished && !Completed;
        public bool IsRunning { get; private set; }
        public IScriptAttributes Attributes { get; set; } = new ScriptAttributes();

        //PROTECTED
        protected virtual IScriptStartController StartController { get; } 
            = new UnconditionalStartController();

        protected Vector3 PlayerPos => Game.LocalPlayer.Character.Position;

        //PRIVATE
        private ProcessHost ProcHost = new ProcessHost();

        public ScriptBase()
        {
            //empty, ctor called to check CanBeStarted()
        }

        public bool CanBeStarted() => StartController.CanBeStarted();

        public void Start()
        {
            if(!ProcHost.IsRunning) ProcHost.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            ProcHost.Stop();
            HasFinished = true;
            IsRunning = false;
        }

        protected void ActivateStage(Action stage)
            => ProcHost.StartProcess(stage);

        protected void DeactivateStage(Action stage)
            => ProcHost.StopProcess(stage);

        protected void SwapStages(Action toDisable, Action toEnable)
            => ProcHost.SwapProcesses(toDisable, toEnable);

        protected virtual void Initialize()
        {
        }

        protected abstract void Process();
        protected abstract void End();
    }
}