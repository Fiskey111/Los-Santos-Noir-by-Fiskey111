using LSNoir.Data;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;

namespace LSNoir.Scenes
{
    class SceneMEOffice : SceneBase, IScene
    {
        private readonly SceneData data;
        private readonly List<Entity> entities = new List<Entity>();

        public SceneMEOffice(SceneData sceneData)
        {
            data = sceneData;
        }

        public void Create()
        {
            LoadInterior();

            Array.ForEach(data.Items, i => entities.Add(GenerateItem(i)));
        }

        private void LoadInterior()
        {
            const int id = 60418;
            NativeFunction.Natives.SET_INTERIOR_ACTIVE(id, true);
            NativeFunction.Natives.x2CA429C029CCF247(id);
        }

        public void Dispose()
        {
            entities.ForEach(e => { if (e) e.Delete(); });
        }
    }
}
