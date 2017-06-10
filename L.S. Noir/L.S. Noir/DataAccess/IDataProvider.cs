using LtFlash.Common.EvidenceLibrary.Serialization;
using System;

namespace LSNoir.DataAccess
{
    interface IDataProvider
    {
        T Load<T>(string path);
        void Save<T>(string filePath, T item);
        void Modify<T>(string filePath, Func<T, T> modification);
        void Modify<T>(string filePath, Action<T> modification);
        T GetIdentifiableData<T>(string filePath, string id) where T : class, IIdentifiable;
    }
}
