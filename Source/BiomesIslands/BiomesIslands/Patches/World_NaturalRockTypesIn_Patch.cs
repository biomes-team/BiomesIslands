using System.Collections.Generic;
using Verse;
using HarmonyLib;
using RimWorld.Planet;

namespace BiomesIslands.Patches
{
	/// <summary>
	/// Atoll must always have coral rock.
	/// Other biomes must not have coral rock.
	/// </summary>
	[HarmonyPatch(typeof(World), nameof(World.NaturalRockTypesIn))]
	public static class World_NaturalRockTypesIn_Patch
	{
		public static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> values, int tile, World __instance)
		{
			if (values == null)
			{
				yield break;
			}

			if (tile >= 0 && __instance.grid[tile].biome == BiomesIslandsDefOf.BMT_Atoll)
			{
				// Atolls must only have coral rock.
				yield return BiomesIslandsDefOf.BMT_CoralRock;
			}
			else
			{
				// Other biomes must not contain coral rock.
				foreach (ThingDef rockType in values)
				{
					if (rockType != BiomesIslandsDefOf.BMT_CoralRock)
					{
						yield return rockType;
					}
				}
			}
		}
	}
}