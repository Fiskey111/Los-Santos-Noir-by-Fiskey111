using System.Collections.Generic;
using System.Linq;
using LtFlash.Common.EvidenceLibrary;
using Rage;

namespace LSNoir.Callouts.SA.Services
{
    internal static class PoliceStations
    {
        private static readonly Dictionary<EPoliceStations, SpawnPoint> _stations = new Dictionary<EPoliceStations, SpawnPoint>
        {
            { EPoliceStations.SouthCentral, new SpawnPoint(230.815689f, new Vector3(399.9877f,-1598.8855f,28.7242641f)) },
            { EPoliceStations.LaMesa, new SpawnPoint(79.28939f, new Vector3(843.606f,-1324.98145f, 25.6899166f)) },
            { EPoliceStations.Vinewood, new SpawnPoint(248.667511f, new Vector3(625.3828f, 29.9815f, 87.81716f)) },
            { EPoliceStations.RockfordHills, new SpawnPoint(59.58184f, new Vector3(-560.5574f, -147.012451f, 37.59976f))},
            { EPoliceStations.VespucciBeach, new SpawnPoint(36.7247162f, new Vector3(-1144.33984f,-851.7561f, 13.4567442f)) },
            { EPoliceStations.LAX, new SpawnPoint(147.858536f, new Vector3(-910.1795f, -2400.20679f, 14.1866388f)) },
        };

        public static SpawnPoint GetClosestPoliceStationSpawn(Vector3 pos)
        {
            return _stations.Values.OrderBy(s => Vector3.Distance(pos, s.Position)).First();
        }

        public static SpawnPoint GetPoliceStationSpawn(EPoliceStations station)
        {
            return _stations[station];
        }
    }
}
