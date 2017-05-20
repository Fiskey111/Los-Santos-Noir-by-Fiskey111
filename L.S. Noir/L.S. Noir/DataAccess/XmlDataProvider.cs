using LtFlash.Common.Serialization;
using System;
using System.IO;

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

        public void Modify<T>(string filePath, Action<T> modification)
        {
            if (!File.Exists(filePath))
            {
                var msg = $"{nameof(XmlDataProvider)}.{nameof(Modify)}(): specified file could not be found: {filePath}";
                throw new FileNotFoundException(msg);
            }

            Serializer.ModifyItemInXML<T>(filePath, modification);
        }
    }
}
