using LSNoir.Data;
using LSNoir.Resources;
using LSPD_First_Response.Engine;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LSNoir.Scenes
{
    class SceneMEOffice : SceneBase, IScene
    {
        private readonly SceneData data;
        private readonly List<Entity> entities = new List<Entity>();
        //====

        internal static List<SpawnPoint> StaticSpawnPoints, AmbientSpawnPoints;

        internal void GetSpawnPoint()
        {
            StaticSpawnPoints = new List<SpawnPoint>();
            AmbientSpawnPoints = new List<SpawnPoint>();
            // P1
            StaticSpawnPoints.Add(new SpawnPoint(290f, 250f, -1374f, 40f));
            // P2
            StaticSpawnPoints.Add(new SpawnPoint(150f, 243f, -1370f, 40f));
            // P3
            StaticSpawnPoints.Add(new SpawnPoint(163f, 242f, -1377f, 40f));
            // P4
            StaticSpawnPoints.Add(new SpawnPoint(125f, 243f, -1378f, 40f));
            // P5
            StaticSpawnPoints.Add(new SpawnPoint(282f, 247f, -1378f, 40f));
            // P6
            StaticSpawnPoints.Add(new SpawnPoint(134f, 238f, -1359f, 40f));
            // A1
            AmbientSpawnPoints.Add(new SpawnPoint(273f, 230f, -1368f, 40f));
            // A2
            AmbientSpawnPoints.Add(new SpawnPoint(220f, 240f, -1376f, 40f));
            // A3
            AmbientSpawnPoints.Add(new SpawnPoint(138f, 239f, -1360f, 40f));
            // A4
            AmbientSpawnPoints.Add(new SpawnPoint(309f, 249f, -1365f, 40f));
            // A5
            AmbientSpawnPoints.Add(new SpawnPoint(322f, 260f, -1377f, 40f));
            // A6
            AmbientSpawnPoints.Add(new SpawnPoint(322f, 260f, -1377f, 40f));

            //PlayerSpawnPoint = new SpawnPoint(145f, 252f, -1366f, 40f);

            //MedicalExaminerSpawnPoint = new SpawnPoint(118.28f, 230.72f, -1367.60f, 40f);

        }

        private static List<SpawnPoint> s;

        private readonly List<string> models = new List<string>
        {
            "s_m_y_autopsy_01",
            "s_m_m_doctor_01",
            "s_f_y_scrubs_01",
            "s_m_m_scientist_01",
        };

        public SceneMEOffice()
        {
            //data = sceneData
            GetSpawnPoint();
            s = new List<SpawnPoint>(StaticSpawnPoints.Concat(AmbientSpawnPoints));
        }

        public void Create()
        {
            LoadInterior();

            //Array.ForEach(data.Items, i => entities.Add(GenerateItem(i)));
            //for (int i = 0; i < data.Items.Length; i++)
            //{
            //    var k = GenerateItem(data.Items[i]);
            //    entities.Add(k);
            //}

            foreach(var p in s)
            {
                var e = new Ped(MathHelper.Choose<string>(models), p.Position, p.Heading);
                e.MakePersistent();
                entities.Add(e);
            }
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
