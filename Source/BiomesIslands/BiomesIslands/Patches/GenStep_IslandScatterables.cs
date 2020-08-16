using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using Verse;

namespace BiomesIslands.Patches
{
    //[HarmonyPatch(typeof(RimWorld.GenStep_ScatterRuinsSimple))]
    //[HarmonyPatch(nameof(RimWorld.GenStep_ScatterRuinsSimple.Generate))]

    //[HarmonyPatch(typeof(GenStep_ScatterRuinsSimple), nameof(GenStep_ScatterRuinsSimple.Generate))]
    //internal static class GenStep_IslandRuins
    //{
    //    internal static bool Prefix(Map map, GenStepParams parms)
    //    {
    //        if (map.Biome.HasModExtension<IslandMap>())
    //        {
    //            if (map.Biome.GetModExtension<IslandMap>().hasRuins == false)
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }
    //}



    // simple ruins, 1st piece of shrines
    [HarmonyPatch(typeof(Verse.GenStep_Scatterer), nameof(Verse.GenStep_Scatterer.Generate))]
    static class Scatterer_WaterBiomeFix                
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo waterBiome = AccessTools.Field(type: typeof(GenStep_Scatterer), name: nameof(GenStep_Scatterer.allowInWaterBiome));

            CodeInstruction[] codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            foreach (CodeInstruction instruction in codeInstructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand == waterBiome)
                {
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(Scatterer_WaterBiomeFix), name: nameof(Scatterer_WaterBiomeFix.AllowedInWaterBiome)));
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        static bool AllowedInWaterBiome(GenStep_Scatterer step, Map map)
        {
            if (map.Biome.HasModExtension<BiomesMap>())
            {
                if (map.Biome.GetModExtension<BiomesMap>().hasScatterables)
                {
                    return true;
                }
            }
            return step.allowInWaterBiome;
        }

    }





}
