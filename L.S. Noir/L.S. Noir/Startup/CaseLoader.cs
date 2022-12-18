using System.IO;
using CaseManager.NewData;
using LSNoir.Common;
using LSNoir.Extensions;
using Newtonsoft.Json;

namespace LSNoir.Startup
{
    public class CaseLoader
    {
        public Case LoadedCase { get; private set; }

        private string _caseLocation;
        
        public CaseLoader() { }

        public bool LoadCase(string caseLocation)
        {
            Logger.LogDebug(nameof(InternalCaseManager), nameof(LoadedCase), $"Loading case at location: {caseLocation}");
            var caseLoad = JsonConvert.DeserializeObject<Case>(File.ReadAllText(caseLocation));
            if (caseLoad == null)
            {
                Logger.LogDebug(nameof(InternalCaseManager), nameof(InternalCaseManager), $"caseLoad = null || {caseLocation}");
                return false;
            }
            
            _caseLocation = caseLocation;
            LoadedCase = caseLoad;
            return true;
        } 

        public void SaveCase()
        {
            $"SaveCase - {_caseLocation}".AddLog(true);
            var data = JsonConvert.SerializeObject(LoadedCase, Formatting.Indented);
            File.WriteAllText(_caseLocation, data);
        }
    }
}