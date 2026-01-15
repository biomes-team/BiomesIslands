using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace BiomesIslands
{
    public static class IslandsUtil
    {
        public static bool FindAquaticSpawnPoint(Map map, out IntVec3 initialSpawn)
        {
            if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.terrainGrid.TerrainAt(c).IsWater, map, 0f, out initialSpawn))
            {
                return true;
            }
            return false;

        }

        public static IntVec3 FindAquaticSpawnPointNear(IntVec3 start, Map map, int radius)
        {
            if (CellFinder.TryFindRandomCellNear(start, map, radius, (IntVec3 c) => map.terrainGrid.TerrainAt(c).IsWater, out IntVec3 initialSpawn))
            {
                return initialSpawn;
            }
            return start;

        }

    }
}
