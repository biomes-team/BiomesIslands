using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesIslands.Patches
{
	[HarmonyPatch(typeof(ThingDefGenerator_Meat), nameof(ThingDefGenerator_Meat.ImpliedMeatDefs))]
	public static class ThingDefGenerator_Meat_ImpliedMeatDefs_Patch
	{
		public static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> meatThings)
		{
			// [DefOf] initialization seems to happen too early for implied Defs. strings are used instead.
			foreach (var thingDef in meatThings)
			{
				if (thingDef.defName.EndsWith("BlueSeaSnail"))
				{
					thingDef.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Insect";
				}

				if (thingDef.defName.EndsWith("RimCrab"))
				{
					thingDef.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Insect";
					thingDef.description =
						"Raw butchered crab flesh. Delicious when cooked into meals, but can also be eaten raw.";
					thingDef.ingestible.specialThoughtAsIngredient = BiomesIslandsDefOf.BiomesIslands_Crab;
				}

				if (thingDef.defName.EndsWith("Whale"))
				{
					thingDef.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Insect";
					thingDef.description = "Raw fat from an aquatic mammal. Not as tasty as meat, but nutrient dense.";
					thingDef.stackLimit = 150;
					// thingDef.ingestible.specialThoughtAsIngredient = ThoughtDef.Named("BMT_Whale");
				}

				yield return thingDef;
			}
		}
	}
}