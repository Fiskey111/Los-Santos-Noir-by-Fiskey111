using System;

namespace LSNoir.Callouts.SA.Objects
{
    [Serializable]
    class StageData
    {
        public Stage StageType { get; protected set; }

        public StageData(Stage stage)
        {
            StageType = stage;
        }

        public enum Stage { SexualAssault, Murder }
    }
}
