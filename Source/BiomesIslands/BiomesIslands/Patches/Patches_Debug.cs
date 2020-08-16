using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesIslands.Patches
{
    // Put debug stuff here. Comment it out when you're done, but don't delete it so it's still there if we need to use it again


    //// Counts steel and puts the number in the debug log.
    //// Useful for checking ore spawns
    //[HarmonyPatch(typeof(GenStep_FindPlayerStartSpot), nameof(GenStep_FindPlayerStartSpot.Generate))]
    //internal static class CouuntMinableSteel
    //{
    //    internal static void Postfix(Map map, GenStepParams parms)
    //    {
    //        int count = map.spawnedThings.Count(thing => thing.def == ThingDefOf.MineableSteel);
    //        Log.Message(String.Format("{0} steel found", count));
    //    }
    //}

}
