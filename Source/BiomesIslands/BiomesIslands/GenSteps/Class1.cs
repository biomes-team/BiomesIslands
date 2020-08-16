using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesIslands.GenSteps
{
    [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility))]
    [HarmonyPatch(nameof(RimWorld.GenStep_ElevationFertility.Generate))]
    internal static class RemoveHillsPatch
    {
        static void Postfix(Map map, GenStepParams parms)
        {
            if (map.Biome.defName != "BiomesIslands_Oasis")
            {
                return;
            }
            MapGenFloatGrid elevation = MapGenerator.Elevation;

            foreach (IntVec3 current in map.AllCells)
            {
                elevation[current] = 0;
            }
        }
    }
}
