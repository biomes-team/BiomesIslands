using System.Collections.Generic;
using System.Linq;
using PathfindingFramework;
using PathfindingFramework.Patches;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesIslands.Incidents
{
	public class Incident_AquaticArrival : IncidentWorker
	{
		public static readonly IntRange AnimalsPreyMin = new IntRange(3, 5);
		public static readonly IntRange AnimalsPredatorMin = new IntRange(1, 2);

		public const float MinTotalBodySize = 4f;

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			return TryFindPreyAnimalKind(map.Tile, out PawnKindDef _) && TryFindPredatorAnimalKind(map.Tile, out PawnKindDef _);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			
			if (!TryFindPreyAnimalKind(map.Tile, out PawnKindDef animalPreyKind) || !TryFindPredatorAnimalKind(map.Tile, out PawnKindDef animalPredatorKind))
			{
				return false;
			}

			if (!LocationFinding.TryFindRandomPawnEntryCell(out IntVec3 start, map, CellFinder.EdgeRoadChance_Animal, false, null, animalPreyKind))
			{
				return false;
			}

			Rot4 rot = Rot4.FromAngleFlat((map.Center - start).AngleFlat);
			List<Pawn> prey = GenerateAnimals(animalPreyKind, map.Tile, AnimalsPreyMin);
			
			// Spawn the first pawn in the chosen tile. It will be used to choose the locations of the others.
			Pawn preyPawn = GenSpawn.Spawn(prey[0], start, map) as Pawn;
			for (int preyIndex = 1; preyIndex < prey.Count; ++preyIndex)
			{
				LocationFinding.TryRandomClosewalkCellNear(preyPawn, 10, out IntVec3 closeLocation);
				GenSpawn.Spawn(prey[preyIndex], closeLocation, map, rot);
			}

			List<Pawn> predator = GenerateAnimals(animalPredatorKind, map.Tile, AnimalsPredatorMin);
			for (int predatorIndex = 0; predatorIndex < predator.Count; predatorIndex++)
			{
				LocationFinding.TryRandomClosewalkCellNear(preyPawn, 10, out IntVec3 closeLocation);
				GenSpawn.Spawn(predator[predatorIndex], closeLocation, map, rot);
			}
			string str = string.Format(def.letterText, animalPreyKind.GetLabelPlural(), animalPredatorKind.label).CapitalizeFirst();
			string str2 = string.Format(def.letterLabel, animalPreyKind.GetLabelPlural().CapitalizeFirst(), animalPredatorKind.GetLabelPlural().CapitalizeFirst());
			SendStandardLetter(str2, str, def.letterDef, parms, prey[0]);
			return true;
		}

		private static bool IsAquatic(PawnKindDef animalKind)
		{
			return animalKind.race.MovementDef() == MovementDefOf.PF_Movement_Aquatic;
		}

		protected bool TryFindPreyAnimalKind(int tile, out PawnKindDef animalKind)
		{
			return DefDatabase<PawnKindDef>.AllDefs.Where(k => IsAquatic(k) && k.RaceProps.CanDoHerdMigration && k.RaceProps.foodType == FoodTypeFlags.VegetarianRoughAnimal && Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(tile, k.race)).TryRandomElementByWeight((PawnKindDef x) => Mathf.Lerp(0.2f, 1f, x.RaceProps.wildness), out animalKind);
		}

		protected bool TryFindPredatorAnimalKind(int tile, out PawnKindDef animalKind)
		{
			return DefDatabase<PawnKindDef>.AllDefs.Where(k => IsAquatic(k) && k.RaceProps.foodType == FoodTypeFlags.CarnivoreAnimal && Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(tile, k.race)).TryRandomElementByWeight((PawnKindDef x) => Mathf.Lerp(0.2f, 1f, x.RaceProps.wildness), out animalKind);
		}

		protected List<Pawn> GenerateAnimals(PawnKindDef animalKind, int tile, IntRange minRange)
		{
			int randomInRange = Mathf.Max(minRange.RandomInRange, Mathf.CeilToInt(4f / animalKind.RaceProps.baseBodySize));
			List<Pawn> list = new List<Pawn>();
			for (int i = 0; i < randomInRange; i++)
			{
				Pawn item = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animalKind, null, PawnGenerationContext.NonPlayer, tile));
				list.Add(item);
			}
			return list;
		}
	}
}
