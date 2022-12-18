using System;
using System.Linq;
using CaseManager.NewData;
using LSNoir.Callouts.Stages;
using LSNoir.Common.Process;
using LSNoir.Extensions;
using Rage;

namespace LSNoir.Common.ScriptHandler
{
    public class CaseController
    {
        private Case _currentCase;

        private StageBase _currentStage;

        public bool? IsCaseRunning => _currentStage?.IsRunning;

        private ProcessHost _processHost;
        
        public CaseController()
        {
            Logger.LogDebug(nameof(CaseController), nameof(CaseController), $"Starting case controller");
            _processHost = new ProcessHost();
            _processHost.StartProcess(Process);
            _processHost.Start();
            Logger.LogDebug(nameof(CaseController), nameof(CaseController), $"Completed CaseController initialization");
        }

        public void Dispose()
        {
            _currentStage?.End(false);
            _currentStage = null;
            _currentCase = null;
            _processHost.Stop();
            _processHost = null;
        }

        public void AddCase(Case caseItem)
        {
            if (caseItem == null)
            {
                Logger.LogDebug(nameof(CaseController), nameof(AddCase), $"Case is null");
                return;
            }
            Logger.LogDebug(nameof(CaseController), nameof(AddCase), $"Adding case {caseItem.Name}");
            _currentCase = caseItem;
        }

        private void Process()
        {           
            Logger.LogDebug(nameof(CaseController), nameof(Process), $"CaseController initialized");
            while (true)
            {
                GameFiber.Sleep(15000);

                StartNextStage();
                
                GameFiber.Yield();
            }
        }

        internal bool StartNextStage()
        {
            Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), "Attempting to start a stage");
            if (_currentStage != null)
            {
                if (_currentStage.IsRunning) return false;
            }
            
            Stage nextStage = null;
            if (_currentCase != null)
            {
                Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), "_currentCase != null");
                foreach (var stage in _currentCase.Stages.Where(stage => stage.Completed))
                {
                    nextStage = _currentCase.Stages.First(s => s.ID == stage.NextStages.PickRandom());
                    Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), $"nextStage == pickrandom");
                }

                Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), $"nextStage == default");
                if (nextStage == null) nextStage = _currentCase.Stages.FirstOrDefault(s => s.PriorStages.Count < 1);

            }
            if (nextStage == null)
            {
                Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), "Next stage = null");
                return false;
            }
            
            Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), $"Getting stage type");
            var stageType = Type.GetType(nextStage.StageType);
            
            if (stageType == null)
            {
                Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), "stage type = null");
                return false;
            }
            
            Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), $"Type: {stageType}");
            var method = stageType.GetMethod("Initialize");
            if (method == null)
            {
                Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), "method = null");
                return false;
            }

            GameFiber.StartNew(() =>
            {
                Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), $"Method: {method.Name}");
                var stage = Activator.CreateInstance(stageType, _currentCase, nextStage);
                _currentStage = stage as StageBase;
            });

            Logger.LogDebug(nameof(CaseController), nameof(StartNextStage), $"Method invoked");
            return true;
        }
    }
}