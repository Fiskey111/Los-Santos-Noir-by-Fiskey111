using System.IO;
using CaseManager.NewData;
using LSNoir.Common;
using LSNoir.Common.IO;
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
            var caseLoad = JsonHelper.ReadFileJson<Case>(caseLocation);
            if (caseLoad == null)
            {
                Logger.LogDebug(nameof(InternalCaseManager), nameof(InternalCaseManager), $"caseLoad = null || {caseLocation}");
                return false;
            }
            
            _caseLocation = caseLocation;
            LoadedCase = caseLoad;
            return true;
        }

        public bool GetExtraCaseData(string identifier, out string filePath)
        {
            filePath = string.Empty;
            var caseDirectory = Directory.GetDirectoryRoot(_caseLocation);
            filePath = Path.Combine(caseDirectory, identifier);
            return File.Exists(filePath);
        }

        public void SaveCase()
        {
            $"SaveCase - {_caseLocation}".AddLog(true);
            var valid = JsonHelper.SaveFileJson(_caseLocation, LoadedCase);
        }
    }
}