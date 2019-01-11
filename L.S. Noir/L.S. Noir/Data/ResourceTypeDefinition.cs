using System;
using System.Collections.Generic;
using System.Linq;

namespace LSNoir.Data
{
    public class ResourceTypeDefinition
    {
        public Type Type;
        public string ResourceFilePath;

        public ResourceTypeDefinition(Type t, string p)
        {
            Type = t;
            ResourceFilePath = p;
        }
        public ResourceTypeDefinition() { } //required by serialization
    }

    public class ResourceTypes : List<ResourceTypeDefinition>
    {
        public string GetPath<T>() => this.FirstOrDefault(s => s.Type == typeof(T)).ResourceFilePath;
    }
}
