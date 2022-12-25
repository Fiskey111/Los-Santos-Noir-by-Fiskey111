using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace LSNoir.Common.IO
{
    public class JsonHelper
    {
        public static T ReadFileJson<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Logger.LogDebug(nameof(JsonHelper), nameof(ReadFileJson), $"File does not exist at path {filePath}");
                return default(T);
            }
            Logger.LogDebug(nameof(JsonHelper), nameof(ReadFileJson), $"Retrieving data from: {filePath}");
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }

        public static bool SaveFileJson(string filePath, object data)
        {
            var split = filePath.Split('\\');
            var s = string.Empty;
            for (int i = 0; i < split.Length - 1; i++)
            {
                s += split[i] + "\\";
            }
            if (!Directory.Exists(s))
            {
                Logger.LogDebug(nameof(JsonHelper), nameof(SaveFileJson), $"Directory does not exist at {s}, creating...");
                Directory.CreateDirectory(s);
            }
            
            if (data == null)
            {
                Logger.LogDebug(nameof(JsonHelper), nameof(SaveFileJson), $"Object is null");
                return false;
            }
            
            Logger.LogDebug(nameof(JsonHelper), nameof(ReadFileJson), $"Saving data to: {filePath}");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
            return true;
        }
    }
}