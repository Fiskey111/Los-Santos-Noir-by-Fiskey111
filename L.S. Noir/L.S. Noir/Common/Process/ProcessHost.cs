using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Rage;

// From LtFlash Common - https://github.com/LtFlash/LtFlash.Common
namespace LSNoir.Common.Process
{
    public class ProcessHost
    {
        public bool IsRunning { get; private set; }        
        public bool this[Action proc]
        {
            get => this._processList.Contains(proc); 
            set
            {
                if (value) StartProcess(proc);
                else StopProcess(proc);
            }
        }

        private List<Action> _processList { get; set; }

        private GameFiber _mainFiber;
        public ProcessHost([CallerMemberName] string callerName = "")
        {
            Logger.LogDebug(callerName, nameof(ProcessHost), $"Creating new thread");
            _processList = new List<Action>();
            _mainFiber = new GameFiber(Process);
        }

        public void Start()
        {
            IsRunning = true;
            _mainFiber.Start();
        }
        
        public void Stop()
        {
            IsRunning = false;
        }

        public void StartProcess(Action process)
        {
            if (!DoesProcessListContainProcess(process)) _processList.Add(process);
        }
        
        public void StopProcess(Action process)
        {
            if (DoesProcessListContainProcess(process)) _processList.Remove(process);
        }
        
        public void SwapProcesses(Action processStop, Action processStart)
        {
            StopProcess(processStop);
            StartProcess(processStart);
        }

        public bool DoesProcessListContainProcess(Action process) => _processList.Contains(process);

        private void Process()
        {
            while (IsRunning)
            {
                LoopProcesses();
                GameFiber.Yield();
            }
        }

        private void LoopProcesses()
        {
            for (var i = 0; i < _processList.Count; i++)
            {
                if (!IsRunning) return;
                _processList[i]?.Invoke();
            }
        }
    }
}