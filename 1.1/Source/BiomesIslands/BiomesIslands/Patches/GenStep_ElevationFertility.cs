using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using Verse;


namespace BiomesIslands.Patches
{
    // this patch modifies 
    [HarmonyPatch(typeof(RimWorld.GenStep_ElevationFertility), nameof(RimWorld.GenStep_ElevationFertility.Generate))]
    static class GenStep_ElevationFertility
    {
        static void Postfix(Map map, GenStepParams parms)
        {
            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return;
            }
            if (!map.Biome.GetModExtension<BiomesMap>().isIsland)
            {
                return;
            }

            Log.Message("Running island maker");
            (new GenSteps.GenStep_Island()).Generate(map, parms);

            (new GenSteps.GenStep_IslandElevation()).Generate(map, parms);


        }

    }
}
