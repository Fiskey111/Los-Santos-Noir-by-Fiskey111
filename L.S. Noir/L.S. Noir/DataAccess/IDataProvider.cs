﻿using System;

namespace LSNoir.DataAccess
{
    interface IDataProvider
    {
        T Load<T>(string path);
        void Save<T>(string filePath, T item);
        void Modify<T>(string filePath, Action<T> modification);
    }
}
