using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesIslands.Incidents
{
	class Incident_RavenousCrabs : IncidentWorker
	{
		private static readonly FloatRange CountPerColonistRange = new FloatRange(2f, 3f);

		private const int MinCount = 2;

		private const int MaxCount = 20;

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}

			Map map = (Map) parms.target;

			//return LocationFinding.TryFindRandomPawnEntryCell(out IntVec3 _, map, CellFinder.EdgeRoadChance_Animal, false,
			//	null, BiomesIslandsDefOf.BMT_RimCrab);
			//return RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 _, map, 0.1f);
			return RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 _, map, CellFinder.EdgeRoadChance_Animal);
        }

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map) parms.target;
			//if (!LocationFinding.TryFindRandomPawnEntryCell(out IntVec3 result, map, CellFinder.EdgeRoadChance_Animal, false,
			//	    null, BiomesIslandsDefOf.BMT_RimCrab))
			if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 result, map, CellFinder.EdgeRoadChance_Animal))
			{
				return false;
			}

			int freeColonistsCount = map.mapPawns.FreeColonistsCount;
			float randomInRange = CountPerColonistRange.RandomInRange;
			int num = Mathf.Clamp(GenMath.RoundRandom(freeColonistsCount * randomInRange), MinCount, MaxCount);

			// The first crab spawns in the location chosen before, which should be valid for its movement type.
			Pawn firstCrab = ((Pawn) GenSpawn.Spawn(PawnGenerator.GeneratePawn(BiomesIslandsDefOf.BMT_RimCrab),
				result, map));

			for (int crabIndex = 1; crabIndex < num; crabIndex++)
			{

				// Choose a nearby location valid for the next crab.
				//LocationFinding.TryRandomClosewalkCellNear(firstCrab, 10, out IntVec3 closeLocation);
				IntVec3 closeLocation = CellFinder.RandomClosewalkCellNear(result, map, 10);


                Pawn spawnedCrab = ((Pawn) GenSpawn.Spawn(PawnGenerator.GeneratePawn(BiomesIslandsDefOf.BMT_RimCrab),
					closeLocation, map));
				spawnedCrab.needs.food.CurLevelPercentage = 1f;
			}

			Find.MusicManagerPlay.ForcePlaySong(BiomesIslandsDefOf.Biomes_LobsterRave, false);
			SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, new TargetInfo(result, map));
			return true;
		}
	}
}