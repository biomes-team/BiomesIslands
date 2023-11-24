using PathfindingFramework;
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

			return LocationFinding.TryFindRandomPawnEntryCell(out IntVec3 _, map, CellFinder.EdgeRoadChance_Animal, false,
				null, BiomesIslandsDefOf.BiomesIslands_RimCrab);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map) parms.target;
			if (!LocationFinding.TryFindRandomPawnEntryCell(out IntVec3 result, map, CellFinder.EdgeRoadChance_Animal, false,
				    null, BiomesIslandsDefOf.BiomesIslands_RimCrab))
			{
				return false;
			}

			int freeColonistsCount = map.mapPawns.FreeColonistsCount;
			float randomInRange = CountPerColonistRange.RandomInRange;
			int num = Mathf.Clamp(GenMath.RoundRandom(freeColonistsCount * randomInRange), MinCount, MaxCount);

			// The first crab spawns in the location chosen before, which should be valid for its movement type.
			Pawn firstCrab = ((Pawn) GenSpawn.Spawn(PawnGenerator.GeneratePawn(BiomesIslandsDefOf.BiomesIslands_RimCrab),
				result, map));

			for (int crabIndex = 1; crabIndex < num; crabIndex++)
			{
				// Choose a nearby location valid for the next crab.
				LocationFinding.TryRandomClosewalkCellNear(firstCrab, 10, out IntVec3 closeLocation);
				Pawn spawnedCrab = ((Pawn) GenSpawn.Spawn(PawnGenerator.GeneratePawn(BiomesIslandsDefOf.BiomesIslands_RimCrab),
					closeLocation, map));
				spawnedCrab.needs.food.CurLevelPercentage = 1f;
			}

			Find.MusicManagerPlay.ForceStartSong(BiomesIslandsDefOf.Biomes_LobsterRave, false);
			SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, new TargetInfo(result, map));
			return true;
		}
	}
}