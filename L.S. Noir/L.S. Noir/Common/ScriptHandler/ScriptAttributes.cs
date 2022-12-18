using System.Collections.Generic;

// From LtFlash Common - https://github.com/LtFlash/LtFlash.Common
namespace LSNoir.Common.ScriptHandler
{
    public class ScriptAttributes : IScriptAttributes
    {        
        public string Id { get; }
        public double TimerIntervalMax { get; set; }
        public double TimerIntervalMin { get; set; }
        public List<string> NextScripts { get; set; } = new List<string>();
        public List<List<string>> ScriptsToFinishPriorThis { get; set; } = new List<List<string>>();
        public EStartType InitModel { get; set; } = EStartType.Sequential;
        public object[] CtorParams { get; set; }

        public ScriptAttributes()
        {
        }

        public ScriptAttributes(string id) : this()
        {
            Id = id;
        }

        public static ScriptAttributes Clone(IScriptAttributes toClone)
        {
            ScriptAttributes s = new ScriptAttributes(toClone.Id);
            s.TimerIntervalMax = toClone.TimerIntervalMax;
            s.TimerIntervalMin = toClone.TimerIntervalMin;
            s.NextScripts = toClone.NextScripts;
            s.ScriptsToFinishPriorThis = toClone.ScriptsToFinishPriorThis;
            s.InitModel = toClone.InitModel;
            s.CtorParams = toClone.CtorParams;
            return s;
        }
        
        public enum EStartType {Sequential, TimerBased, Manual }
    }
    
    public interface IScriptAttributes
    {
        string Id { get; }
        double TimerIntervalMax { get; set; }
        double TimerIntervalMin { get; set; }
        List<string> NextScripts { get; set; }
        List<List<string>> ScriptsToFinishPriorThis { get; set; }
        ScriptAttributes.EStartType InitModel { get; set; }
        object[] CtorParams { get; set; }
    }
}