
using BiomesCore.DefModExtensions;
using BiomesKit;
using HarmonyLib;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesIslands.Patches
{
    //Lets the hilliness label on the world map be something other than flat. Does not affect terrain generation
    [HarmonyPatch(typeof(WorldGenStep_Terrain))]
    [HarmonyPatch("GenerateTileFor")]
    internal static class WorldGenStepTerrain_IslandHillliness
    {
        static void Postfix(int tileID, ref Tile __result)
        {
            if (!__result.biome.HasModExtension<BiomesMap>())
            {
                return;
            }

            if (!__result.biome.GetModExtension<BiomesMap>().hasHilliness)
            {
                return;
            }

            switch (Rand.Range(0, 4))
            {
                case 0:
                    __result.hilliness = Hilliness.Flat;
                    break;
                case 1:
                    __result.hilliness = Hilliness.SmallHills;
                    break;
                case 2:
                    __result.hilliness = Hilliness.LargeHills;
                    break;
                case 3:
                    __result.hilliness = Hilliness.Mountainous;
                    break;
            }
        }
    }
}
