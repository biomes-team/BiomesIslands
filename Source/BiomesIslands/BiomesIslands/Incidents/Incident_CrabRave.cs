using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BiomesIslands.Incidents
{
	class Incident_CrabMigration : IncidentWorker
	{
		public static readonly IntRange AnimalsMin = new IntRange(3, 5);

		public const float MinTotalBodySize = 4f;

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			return TryFindAnimalKind(map.Tile, out PawnKindDef _);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 start, map, CellFinder.EdgeRoadChance_Animal))
			{
				return false;
			}
			if (!TryFindAnimalKind(map.Tile, out PawnKindDef crabKind))
			{
				return false;
			}
			Rot4 rot = Rot4.FromAngleFlat((map.Center - start).AngleFlat);
			List<Pawn> crabs = GenerateAnimals(crabKind, map.Tile, AnimalsMin);
			for (int i = 0; i < crabs.Count; i++)
			{
				Pawn newThing = crabs[i];
				IntVec3 loc = CellFinder.RandomClosewalkCellNear(start, map, 10);
				GenSpawn.Spawn(newThing, loc, map, rot);
			}
			string letterText = string.Format(def.letterText, crabKind.GetLabelPlural(), crabKind.label).CapitalizeFirst();
			SendStandardLetter(def.letterLabel, letterText, def.letterDef, parms, crabs[0]);
			return true;
		}

		protected bool TryFindAnimalKind(int tile, out PawnKindDef animalKind)
		{
			return DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef k) => k.RaceProps.CanDoHerdMigration && k.RaceProps.meatLabel == "crab meat" && Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(tile, k.race)).TryRandomElementByWeight((PawnKindDef x) => Mathf.Lerp(0.2f, 1f, x.RaceProps.wildness), out animalKind);
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

	class Incident_RavenousCrabs : IncidentWorker
    {
		protected static readonly FloatRange CountPerColonistRange = new FloatRange(2f, 3f);

		protected const int MinCount = 2;

		protected const int MaxCount = 20;

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}
			Map map = (Map)parms.target;
			IntVec3 result;
			return RCellFinder.TryFindRandomPawnEntryCell(out result, map, CellFinder.EdgeRoadChance_Animal);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			PawnKindDef crab = PawnKindDef.Named("BiomesIslands_RimCrab");
			if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 result, map, CellFinder.EdgeRoadChance_Animal))
			{
				return false;
			}
			int freeColonistsCount = map.mapPawns.FreeColonistsCount;
			float randomInRange = CountPerColonistRange.RandomInRange;
			int num = Mathf.Clamp(GenMath.RoundRandom((float)freeColonistsCount * randomInRange), MinCount, MaxCount);
			for (int i = 0; i < num; i++)
			{
				IntVec3 loc = CellFinder.RandomClosewalkCellNear(result, map, 10);
				Pawn spawnedCrab = ((Pawn)GenSpawn.Spawn(PawnGenerator.GeneratePawn(crab), loc, map));
				spawnedCrab.needs.food.CurLevelPercentage = 1f;
			}
			SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, new TargetInfo(result, map));
			return true;
		}
	}
}
