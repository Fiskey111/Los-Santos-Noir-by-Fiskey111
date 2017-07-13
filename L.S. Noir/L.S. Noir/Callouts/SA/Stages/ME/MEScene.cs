using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSNoir.Callouts.SA.Objects;
using LtFlash.Common;
using Rage;

namespace LSNoir.Callouts.SA.Stages.ME
{
    internal static class MESceneSpawnPoints
    {
        internal static List<SpawnPoint> StaticSpawnPoints, AmbientSpawnPoints;
        internal static SpawnPoint MedicalExaminerSpawnPoint, PlayerSpawnPoint;
        
        internal static void GetSpawnPoint()
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

            PlayerSpawnPoint = new SpawnPoint(145f, 252f, -1366f, 40f);

            MedicalExaminerSpawnPoint = new SpawnPoint(118.28f, 230.72f, -1367.60f, 40f);

        }
    }
}
