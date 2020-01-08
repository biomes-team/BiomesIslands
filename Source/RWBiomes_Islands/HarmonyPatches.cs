using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Harmony;
using RimWorld.Planet;
using System.Reflection;

namespace RWBiomes_Islands
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {



            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.rwb_islands");

            // find the AddFoodPoisoningHediff method of the class RimWorld.FoodUtility
            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.Planet.World), "NaturalRockTypesIn");

            // find the static method to call before (i.e. Prefix) the targetmethod
            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(RWBiomes_Islands.HarmonyPatches).GetMethod("NaturalRockTypesIn_Prefix"));

            // patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod (i.e. null)
            harmony.Patch(targetmethod, prefixmethod, null);
        }


        public static bool NaturalRockTypesIn_Prefix(int tile, ref IEnumerable<ThingDef> __result, ref World __instance)
        {
            var world = Traverse.Create(__instance);
            WorldGrid worldGrid = world.Field("grid").GetValue<WorldGrid>();
            if (worldGrid[tile].biome.defName == "RWB_IslandAtoll_NoBeach")
            {
                List<ThingDef> rocks = new List<ThingDef>() { RWB_Islands_DefOf.RWB_CoralRock };
                //ThingDef coral = RWB_Islands_DefOf.RWB_CoralRock;
                //__result.Add(coral);
                __result = rocks;
                return false;
            }
            return true;
        }


    }




}
