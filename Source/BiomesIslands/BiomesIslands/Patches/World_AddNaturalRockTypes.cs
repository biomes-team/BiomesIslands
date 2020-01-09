using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Harmony;
using RimWorld.Planet;
using System.Reflection;

namespace BiomesIslands.Patches
{
    [HarmonyPatch(typeof(World), nameof(World.NaturalRockTypesIn))]
    internal static class World_AddNaturalRockTypes
    {
        internal static bool Prefix(int tile, ref IEnumerable<ThingDef> __result, ref World __instance)
        {
            var world = Traverse.Create(__instance);
            WorldGrid worldGrid = world.Field("grid").GetValue<WorldGrid>();
            if (worldGrid[tile].biome.defName == "BiomesIslands_IslandAtoll_NoBeach")
            {
                List<ThingDef> rocks = new List<ThingDef>() { BiomesIslandsDefOf.BiomesIslands_CoralRock };
                __result = rocks;
                return false;
            }
            return true;
        }


    }




}
