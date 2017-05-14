using LSNoir.Data;
using Rage;
using System.Collections.Generic;

namespace LSNoir.Scenes
{
    class Scene : SceneBase, IScene
    {
        protected readonly SceneItem[] items;
        protected readonly List<Entity> entities = new List<Entity>();

        public Scene(SceneData data)
        {
            items = data.Items;
        }

        public virtual void Create()
        {
            for (int i = 0; i < items.Length; i++)
            {
                var e = GenerateItem(items[i]);
                entities.Add(e);
            }
        }

        public virtual void Dispose()
        {
            entities.ForEach(e => { if (e) e.Delete(); });
        }
    }
}
