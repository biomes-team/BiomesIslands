using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace BiomesIslands.Incidents
{
	public class Incident_AquaticArrival : IncidentWorker
	{
		private static readonly IntRange AnimalsPreyMin = new IntRange(3, 5);
		private static readonly IntRange AnimalsPredatorMin = new IntRange(1, 2);

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map) parms.target;

			bool foundPredatorPrey = TryFindPreyPredatorPair(map.Tile, out PawnKindDef preyKindDef, out PawnKindDef _);
			bool foundSpawnSpot = IslandsUtil.TryFindAquaticSpawnPoint(map, out IntVec3 _);

			return foundPredatorPrey && foundSpawnSpot;
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			const int preySpawnRadius = 8;
			const int predatorSpawnRadius = 12;
			Map map = (Map) parms.target;

			if (!TryFindPreyPredatorPair(map.Tile, out PawnKindDef preyKindDef, out PawnKindDef predatorKindDef))
			{
                return false;
			}

			if (!IslandsUtil.TryFindAquaticSpawnPoint(map, out IntVec3 start))
			{
				return false;
			}

			Rot4 rot = Rot4.FromAngleFlat((map.Center - start).AngleFlat);
			List<Pawn> prey = GenerateAnimals(preyKindDef, map.Tile, AnimalsPreyMin);

			// Spawn one of the prey animals first. It will be used to choose the location of other prey.
			Pawn preyPawn = GenSpawn.Spawn(prey[0], start, map) as Pawn;
			IntVec3 closeLocation;
			for (int preyIndex = 1; preyIndex < prey.Count; ++preyIndex)
			{
				closeLocation = IslandsUtil.FindAquaticSpawnPointNear(start, map, preySpawnRadius);
				GenSpawn.Spawn(prey[preyIndex], closeLocation, map, rot);
			}

			List<Pawn> predator = GenerateAnimals(predatorKindDef, map.Tile, AnimalsPredatorMin);
			for (int predatorIndex = 0; predatorIndex < predator.Count; predatorIndex++)
            {
                closeLocation = IslandsUtil.FindAquaticSpawnPointNear(start, map, predatorSpawnRadius);
                GenSpawn.Spawn(predator[predatorIndex], closeLocation, map, rot);
			}

			string preyText = preyKindDef.GetLabelPlural().CapitalizeFirst();
			string predatorText = predatorKindDef.GetLabelPlural().CapitalizeFirst();
			string letterLabel = string.Format(def.letterLabel, preyText, predatorText);
			string letterText = string.Format(def.letterText, preyText, predatorText);
			SendStandardLetter(letterLabel, letterText, def.letterDef, parms, preyPawn);

			return true;
		}

		private static bool IsAquatic(PawnKindDef animalKind)
		{
			return animalKind.race.HasModExtension<BiomesCore.WaterWalkerExtension>();
		}

		private static float AnimalWeight(PawnKindDef pawnKindDef)
		{
			return Mathf.Lerp(0.2F, 1.0F, pawnKindDef.race.statBases.GetStatValueFromList(StatDefOf.Wildness, 1f));
		}

		private static bool TryFindPreyPredatorPair(int tile, out PawnKindDef preyKindDef, out PawnKindDef predatorKindDef)
		{
			preyKindDef = null;
			predatorKindDef = null;

			List<PawnKindDef> pawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading;
			List<PawnKindDef> preyKindDefs = new List<PawnKindDef>();
			List<PawnKindDef> predatorKindDefs = new List<PawnKindDef>();

			TileTemperaturesComp tileTemperatures = Find.World.tileTemperatures;
			for (int pawnKindIndex = 0; pawnKindIndex < pawnKindDefs.Count; ++pawnKindIndex)
			{
				PawnKindDef pawnKindDef = pawnKindDefs[pawnKindIndex];
				if (IsAquatic(pawnKindDef) && tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(tile, pawnKindDef.race))
				{
					// Allow only aquatic creatures that can spawn with this season and temperature are allowed.
					// Filter each aquatic animal into their list.
					switch (pawnKindDef.RaceProps.foodType)
					{
						case FoodTypeFlags.VegetarianRoughAnimal:
							preyKindDefs.Add(pawnKindDef);
							break;
						case FoodTypeFlags.CarnivoreAnimal:
							predatorKindDefs.Add(pawnKindDef);
							break;
					}
                }
            }

            if (preyKindDefs.Count == 0 || predatorKindDefs.Count == 0)
			{
                return false;
			}

			return preyKindDefs.TryRandomElementByWeight(AnimalWeight, out preyKindDef) &&
			       predatorKindDefs.TryRandomElementByWeight(AnimalWeight, out predatorKindDef);
		}

		private static List<Pawn> GenerateAnimals(PawnKindDef animalKind, int tile, IntRange minRange)
		{
			int randomInRange = Mathf.Max(minRange.RandomInRange, Mathf.CeilToInt(4f / animalKind.RaceProps.baseBodySize));
			List<Pawn> list = new List<Pawn>();
			for (int i = 0; i < randomInRange; i++)
			{
				Pawn item = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animalKind, null,
					PawnGenerationContext.NonPlayer, tile));
				list.Add(item);
			}

			return list;
		}
	}
}