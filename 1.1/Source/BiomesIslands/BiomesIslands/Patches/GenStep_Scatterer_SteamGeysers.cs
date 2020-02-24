using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace BiomesIslands.Patches
{

    //[HarmonyPatch(typeof(GenStep_ScatterThings), "Generate", null)]
    //public static class GenStep_IslandSteamGeysers
    //{
    //    public static bool Prefix(Map map, GenStepParams parms, ref GenStep_ScatterThings __instance)
    //    {
    //        if (__instance.thingDef == ThingDefOf.SteamGeyser)
    //        {
    //            if (map.Biome.defName == "BiomesIslands_TropicalIsland_NoBeach" && __instance.allowInWaterBiome == false)
    //            {
    //                bool seaSpawns = __instance.allowInWaterBiome;
    //                FloatRange range = __instance.countPer10kCellsRange;
    //                __instance.allowInWaterBiome = true;
    //                __instance.countPer10kCellsRange.min /= 2;
    //                __instance.countPer10kCellsRange.max /= 2;
    //                __instance.Generate(map, parms);
    //                __instance.allowInWaterBiome = seaSpawns;
    //                __instance.countPer10kCellsRange = range;
    //                return false;
    //            }
    //        }
           
    //        return true;
    //    }
    //}

}
