using System;
using System.Collections.Generic;
using LSNoir.Callouts.Stages;
using LSNoir.Common;

namespace LSNoir.Callouts.Universal
{
    public class StageTypesForCases
    {
        public static List<Type> StageTypeList { get => _stageTypeList; }
        private static List<Type> _stageTypeList = new List<Type>();

        internal static void Initialize()
        {
            Logger.LogDebug(nameof(StageTypesForCases), nameof(Initialize), "Initializing");
            AddType(typeof(DialogueScene));
            AddType(typeof(StageBase));
        }
        
        public static void AddType(Type type)
        {
            Logger.LogDebug(nameof(StageTypesForCases), nameof(AddType), $"Adding type: {type}");
            _stageTypeList.Add(type);
        }
    }
}