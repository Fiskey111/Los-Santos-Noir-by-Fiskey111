using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.Serialization;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LSNoir.DataAccess
{
    class XmlDataProvider : IDataProvider
    {
        public T Load<T>(string filePath)
        {
            if(!File.Exists(filePath))
            {
                var msg = $"{nameof(XmlDataProvider)}.{nameof(Load)}(): specified file could not be found: {filePath}";
                throw new FileNotFoundException(msg);
            }

            return Serializer.LoadItemFromXML<T>(filePath);
        }

        public void Save<T>(string filePath, T item)
        {
            Serializer.SaveItemToXML<T>(item, filePath);
        }

        public void Modify<T>(string filePath, Func<T, T> modification)
        {
            var element = Load<T>(filePath);
            var modified = modification(element);
            Save(filePath, modified);
        }

        public void Modify<T>(string filePath, Action<T> modification)
        {
            var element = Load<T>(filePath);
            modification(element);
            Save(filePath, element);
        }

        public T GetIdentifiableData<T>(string path, string id) where T : class, IIdentifiable
        {
            if (!File.Exists(path))
            {
                var msg = $"{nameof(XmlDataProvider)}.{nameof(GetIdentifiableData)}(): a specified file could not be found: {path}";
                throw new FileNotFoundException(msg);
            }

            if (string.IsNullOrEmpty(id))
            {
                var msg = $"{nameof(XmlDataProvider)}.{nameof(GetIdentifiableData)}(): an ID cannot be empty. Path: {path}";
                throw new ArgumentException(msg);
            }

            var list = Load<List<T>>(path);

            IIdentifiable item = list.SingleOrDefault(l => l.ID == id);

            if (item == default(T))
            {
                var msg = $"{nameof(XmlDataProvider)}.{nameof(GetIdentifiableData)}(): an item with specified ID could not be found. ID: {id}, Path: {path}";
                throw new KeyNotFoundException(msg);
            }

            return item as T;
        }
    }
}
