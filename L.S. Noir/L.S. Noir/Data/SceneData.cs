using LSNoir.Scenes;
using LtFlash.Common.EvidenceLibrary.Serialization;
using System;

namespace LSNoir.Data
{
    public class SceneData : IIdentifiable
    {
        public string ID { get; set; }
        public string SceneType;
        public SceneItem[] Items = new SceneItem[] { };

        public IScene GetScene()
        {
            if(string.IsNullOrEmpty(SceneType))
            {
                var msg = $"{nameof(SceneData)}.{nameof(GetScene)}(): SceneType cannot be empty. Scene id: {ID}";
                throw new ArgumentException(msg);
            }

            var sceneType = Type.GetType($"LSNoir.Scenes.{SceneType}", true, true);
            var instance = Activator.CreateInstance(sceneType, this);
            return instance as IScene;
        }
    }
}
