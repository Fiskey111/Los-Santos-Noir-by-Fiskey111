using System;
using System.Collections.Generic;
using System.Linq;
using LSNoir.Common.Process;
using LSNoir.Common.ScriptHandler.ScriptStarters;
using Rage;
using static LSNoir.Common.ScriptHandler.ScriptAttributes;
using SSL = System.Collections.Generic.List<LSNoir.Common.ScriptHandler.ScriptStarters.IScriptStarter>;

namespace LSNoir.Common.ScriptHandler
{
    public class ScriptManager
    {
        public string ID { get; private set; }
        public bool IsRunning { get; private set; }
        public double DefaultTimerIntervalMax { get; set; } = 30000;
        public double DefaultTimerIntervalMin { get; set; } = 15000;
        public bool AutoSwapFromSequentialToTimer { get; set; } = true;
        public bool HasFinished { get; private set; }

        private readonly List<IScript> _off = new List<IScript>();
        private readonly List<IScript> _queue = new List<IScript>();
        private readonly SSL _running = new SSL();

        private readonly Dictionary<string, bool> statusOfScripts = new Dictionary<string, bool>();

        private readonly ProcessHost stages = new ProcessHost();

        public ScriptManager()
        {
        }

        public ScriptManager(string id) : this()
        {
            ID = id;
        }

        //FULL CTOR
        public void AddScript(
            Type typeImplIScript, object[] ctorParams, string id, EStartType initModel,
            List<string> nextScripts, List<List<string>> scriptsToFinishPrior,
            double timerMin, double timerMax)
        {
            IScriptAttributes s = new ScriptAttributes(id);
            s.InitModel = initModel;
            s.TimerIntervalMin = timerMin;
            s.TimerIntervalMax = timerMax;
            s.NextScripts = nextScripts;
            s.ScriptsToFinishPriorThis = scriptsToFinishPrior;
            s.CtorParams = ctorParams;

            AddScript(typeImplIScript, s);
        }

        public void AddScript(
            Type typeImplIScript, string id, EStartType initModel, 
            List<string> nextScripts, List<List<string>> scriptsToFinishPrior,
            double timerMin, double timerMax)
        {
            AddScript(typeImplIScript, null, id, initModel, nextScripts, scriptsToFinishPrior, timerMin, timerMax);
        }

        public void AddScript(
            Type typeImplIScript, string id, 
            EStartType initModel,
            List<string> nextScriptsToRun, List<List<string>> scriptsToFinishPrior)
        {
            AddScript(typeImplIScript, id, initModel,
                nextScriptsToRun, scriptsToFinishPrior,
                DefaultTimerIntervalMin, DefaultTimerIntervalMax);
        }

        public void AddScript(
            Type typeBaseScript, string id,
            EStartType initModel)
        {
            AddScript(
                typeBaseScript, id, initModel,
                new List<string>(), new List<List<string>>(),
                DefaultTimerIntervalMin, DefaultTimerIntervalMin);
        }
        //MAIN CTOR
        public void AddScript(Type typeOfIScript, IScriptAttributes attrib)
        {
            if (!typeOfIScript.GetInterfaces().Contains(typeof(IScript)))
            {
                var msg = $"{nameof(AddScript)}(type, attrib): parameter does not implement {nameof(IScript)} interface: {typeOfIScript}";
                throw new ArgumentException(msg);
            }

            IScript sc = CreateInstance<IScript>(typeOfIScript, attrib.CtorParams);
            sc.Attributes = attrib;

            AddNewScriptToList(sc, attrib.Id);

            Logger.LogDebug(
                nameof(ScriptManager),
                nameof(AddScript),
                $"id: {sc.Attributes.Id}: script added.");
        }

        internal static T CreateInstance<T>(Type t, object[] ctorParams)
        {
            if (ctorParams != null && ctorParams.Length > 0)
                return (T)Activator.CreateInstance(t, ctorParams);
            else return (T)Activator.CreateInstance(t);
        }

        public void Start()
        {
            StartScript(_off.First().Attributes.Id);
        }

        public void StartScript(string id)
        {
            GetScriptById(id, _off).Attributes.ScriptsToFinishPriorThis = new List<List<string>>();

            MoveInactiveScriptToQueue(id, _off, _queue);

            RegisterProcesses();
            IsRunning = true;
        }

        public bool HasScriptFinished(string id)
        {
            if(!statusOfScripts.ContainsKey(id))
            {
                throw new KeyNotFoundException($"{nameof(HasScriptFinished)}(id): script with id [{id}] does not exist.");
            }

            return statusOfScripts[id];
        }

        private void RegisterProcesses()
        {
            stages[Process_RunScriptsFromQueue] = true;
            stages[Process_UnsuccessfullyFinishedScripts] = true;
            stages[Process_WaitScriptsForFinish] = true;
            stages[Process_CheckIfAllFinished] = true;
            stages.Start();
        }

        private void Process_RunScriptsFromQueue()
        {
            for (int i = 0; i < _queue.Count; i++)
            {
                if(CheckIfScriptCanBeStarted(_queue[i]))
                {
                    MoveScriptFromQueueToRunning(_queue[i], _queue, _running);
                }
            }
        }

        private void Process_UnsuccessfullyFinishedScripts()
        {
            SSL ufs = GetUnsuccessfullyFinishedScripts(_running);
            //gets only those with Sequential starter and change it to TimerBased
            ufs = GetScriptsWithSequentialStarter(ufs);

            if (ufs.Count < 1) return;

            for (int i = 0; i < ufs.Count; i++)
            {
                ufs[i].Stop();

                Game.LogTrivial("Script finished unsuccessfully: " + ufs[i].Id);

                IScript s = ufs[i].Script;
                //if StartCtrl == Delay -> re-assign the old one!
                IScript newScript = CreateInstance<IScript>(s.GetType(), s.Attributes.CtorParams);
                var newAttrib = ScriptAttributes.Clone(s.Attributes);
                newAttrib.InitModel = EStartType.TimerBased;
                newAttrib.ScriptsToFinishPriorThis = new List<List<string>>();
                newScript.Attributes = newAttrib;

                _queue.Add(newScript);
            }

            Logger.LogDebug(
                nameof(ScriptManager), 
                nameof(Process_UnsuccessfullyFinishedScripts), 
                "pre removing: running; " + _running.Count);

            RemoveScripts(ufs, _running);

            Logger.LogDebug(
                nameof(ScriptManager), 
                nameof(Process_UnsuccessfullyFinishedScripts), 
                "removed: running; " + _running.Count);
        }

        private void Process_WaitScriptsForFinish()
        {
            SSL fs = GetSuccessfullyFinishedScripts(_running);

            if (fs.Count < 1) return;

            SetScriptStatusAsFinished(fs);

            for (int i = 0; i < fs.Count; i++)
            {
                if (fs[i].NextScriptsToRun != null)
                {
                    AddScriptsToQueue(fs[i].NextScriptsToRun);
                }
            }

            RemoveScripts(fs, _running);
        }

        private void Process_CheckIfAllFinished()
        {
            //removed off since different path might cause not all scripts are
            //started 
            if(/*_off.Count == 0 && */_queue.Count == 0 && _running.Count == 0)
            {
                HasFinished = true;
                Stop();

                Logger.LogDebug(
                    nameof(ScriptManager), 
                    nameof(Process_CheckIfAllFinished), 
                    "All scripts finished");
            }
        }

        private void AddNewScriptToList(IScript script, string id)
        {
            _off.Add(script);
            statusOfScripts.Add(id, false);
        }

        private bool CheckIfScriptCanBeStarted(IScript script)
        {
            var priorScripts = script.Attributes.ScriptsToFinishPriorThis;

            return priorScripts.Count < 1 ? true :
                   CheckIfNecessaryScriptsAreFinished(priorScripts, statusOfScripts);
        }

        private IScript GetScriptById(string id, List<IScript> from)
        {
            IScript s = from.FirstOrDefault(ss => ss.Attributes.Id == id);

            if(s == null)
            {
                var msg = $"{nameof(GetScriptById)}: Script with id [{id}] does not exist.";
                throw new ArgumentException(msg);
            }
            else return s;
        }

        private IScriptStarter CreateScriptStarterByScriptId(
            string id, 
            List<IScript> scriptsToRun)
        {
            IScript s = GetScriptById(id, scriptsToRun); 
            return CreateScriptStarter(s);
        }

        private SSL CreateScriptsStartersByIds(string[] ids, List<IScript> scripts)
        {
            return ids.Select(i => CreateScriptStarterByScriptId(i, scripts)).ToList();
        }

        private IScriptStarter CreateScriptStarter(IScript ss)
        {
            switch (ss.Attributes.InitModel)
            {
                case ScriptAttributes.EStartType.Sequential:
                    return new SequentialScriptStarter(ss, true);

                case ScriptAttributes.EStartType.TimerBased:
                default: //TODO: autoStart to some damn var 
                    return new TimerControlledScriptStarter(ss, true);
            }
        }

        private bool CheckIfNecessaryScriptsAreFinished(
            List<List<string>> scripts, 
            Dictionary<string, bool> status)
        {
            List<bool> arrays = new List<bool>();

            for (int i = 0; i < scripts.Count; i++)
            {
                //TODO: protection check for non-existant key - no sense in running
                //the mod when a crucial script might be missing?
                //implement a function to CheckIfAll()?
                arrays.Add(scripts[i].All(s => status[s])); 
            }

            return arrays.Any(b => b == true);
        }

        private void StartScripts(SSL starters)
        {
            starters.ForEach(s => s.Start());
        }

        private void RemoveScripts(SSL scriptsToRemove, SSL from)
        {
            for (int i = 0; i < scriptsToRemove.Count; i++)
            {
                for (int j = 0; j < from.Count; j++)
                {
                    if (from[j].Id == scriptsToRemove[i].Id) from.RemoveAt(j);
                }
            }
        }

        private SSL GetSuccessfullyFinishedScripts(SSL running)
            => GetScripts(running, s => s.HasFinishedSuccessfully);

        private SSL GetUnsuccessfullyFinishedScripts(SSL running)
            => GetScripts(running, s => s.HasFinishedUnsuccessfully);
        

        private SSL GetScriptsWithSequentialStarter(SSL running)
            => GetScripts(running, s => s.Script.Attributes.InitModel == EStartType.Sequential);
        

        private SSL GetScripts(SSL running, Func<IScriptStarter, bool> conditions)
            => running.Where(conditions).ToList();

        private void SetScriptStatusAsFinished(SSL scripts)
        {
            scripts.ForEach(s => statusOfScripts[s.Id] = true);
        }

        private void MoveInactiveScriptToQueue(
            string scriptId, 
            List<IScript> from, List<IScript> to)
        {
            IScript s = GetScriptById(scriptId, from);
            to.Add(s);
            from.Remove(s);

            Logger.LogDebug(
                nameof(ScriptManager), 
                nameof(MoveInactiveScriptToQueue), s.Attributes.Id);
        }

        private void MoveScriptFromQueueToRunning(
            IScript scriptToRun, 
            List<IScript> from, SSL to)
        {
            IScriptStarter s = CreateScriptStarter(scriptToRun);
            s.Start();
            to.Add(s);
            from.Remove(scriptToRun);
            Logger.LogDebug(
                nameof(ScriptManager), 
                nameof(MoveScriptFromQueueToRunning), s.Id);
        }

        private void AddScriptsToQueue(List<string> scriptsToRun)
        {
            scriptsToRun.ForEach(s => MoveInactiveScriptToQueue(s, _off, _queue));
        }

        private void Stop()
        {
            stages.Stop();
        }

        ~ScriptManager()
        {
            //TODO: terminate script
            Stop();
        }
    }
}