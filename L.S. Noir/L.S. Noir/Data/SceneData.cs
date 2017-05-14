using LSNoir.Scenes;
using LtFlash.Common.EvidenceLibrary.Serialization;

namespace LSNoir.Data
{
    public class SceneData : IIdentifiable
    {
        public string ID { get; set; }
        public SceneItem[] Items = new SceneItem[] { };
    }
}
