using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Data
{
    class DataProvider
    {
        public static T GetData<T>(string id, string filepath) where T : IIdentifiable
        {
            if (!File.Exists(filepath))
            {
                var msg = $"{nameof(DataProvider)}.{nameof(GetData)}(): Specified file could not be found: {filepath}";
                throw new IOException(msg);
            }

            var elements = Serializer.LoadFromXML<T>(filepath);
            var selected = elements.Where<T>(e => e.ID.ToLowerInvariant() == id.ToLowerInvariant());

            if (selected.Count() > 1)
            {
                var msg = $"{nameof(DataProvider)}.{nameof(GetData)}(): More than one item with specified id: {id} was found in file: {filepath}";
                throw new ArgumentException(msg);
            }
            else return selected.FirstOrDefault<T>();
        }
    }
}
