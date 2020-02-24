using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using HarmonyLib;
using RimWorld.Planet;
using System.Reflection;

namespace BiomesIslands.Patches
{

    //[HarmonyPatch(typeof(World), nameof(World.NaturalRockTypesIn))]
    //internal static class World_AddNaturalRockTypes
    //{
    //    internal static void Postfix(int tile, ref IEnumerable<ThingDef> __result, ref World __instance)
    //    {
    //        var world = Traverse.Create(__instance);
    //        WorldGrid worldGrid = world.Field("grid").GetValue<WorldGrid>();
    //        if (worldGrid[tile].biome.defName == "BiomesIslands_Atoll_NoBeach")
    //        {
    //            List<ThingDef> rocks = new List<ThingDef>() { BiomesIslandsDefOf.BiomesIslands_CoralRock };
    //            __result = rocks;
    //        }
    //        else if (__result.Contains(BiomesIslandsDefOf.BiomesIslands_CoralRock))
    //        {
    //            Rand.PushState();
    //            Rand.Seed = tile;

    //            List<ThingDef> rocks = __result.ToList();
    //            rocks.Remove(BiomesIslandsDefOf.BiomesIslands_CoralRock);

    //            List<ThingDef> list = (from d in DefDatabase<ThingDef>.AllDefs
    //                                   where d.category == ThingCategory.Building && d.building.isNaturalRock && !d.building.isResourceRock && !d.IsSmoothed && !rocks.Contains(d) && d.defName != "BiomesIslands_CoralRock"
    //                                   select d).ToList<ThingDef>();
    //            if (!list.NullOrEmpty())
    //            {
    //                rocks.Add(list.RandomElement<ThingDef>());
    //            }

    //            __result = rocks;

    //            Rand.PopState();
    //        }
    //    }

    //}

}
