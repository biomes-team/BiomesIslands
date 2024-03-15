using System.Collections.Generic;
using System.Linq;
using PathfindingFramework;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesIslands.Incidents
{
	public class Incident_CrabMigration : IncidentWorker
	{
		private static readonly IntRange AnimalsMin = new IntRange(3, 5);

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map) parms.target;
			return TryFindAnimalKind(map.Tile, out PawnKindDef _);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map) parms.target;

			if (!TryFindAnimalKind(map.Tile, out PawnKindDef crabKindDef))
			{
				return false;
			}

			if (!LocationFinding.TryFindRandomPawnEntryCell(out IntVec3 start, map, CellFinder.EdgeRoadChance_Animal, false,
				    null, crabKindDef))
			{
				return false;
			}

			Rot4 rot = Rot4.FromAngleFlat((map.Center - start).AngleFlat);
			List<Pawn> crabs = GenerateAnimals(crabKindDef, map.Tile, AnimalsMin);
			// Spawn the first pawn in the chosen tile. It will be used to choose the locations of the others.
			Pawn crabPawn = GenSpawn.Spawn(crabs[0], start, map) as Pawn;
			for (int crabPawnIndex = 1; crabPawnIndex < crabs.Count; crabPawnIndex++)
			{
				Pawn newThing = crabs[crabPawnIndex];
				// Choose a nearby location valid for the crabs.
				LocationFinding.TryRandomClosewalkCellNear(crabPawn, 10, out IntVec3 closeLocation);
				GenSpawn.Spawn(newThing, closeLocation, map, rot);
			}

			Find.MusicManagerPlay.ForcePlaySong(BiomesIslandsDefOf.Biomes_LobsterRave, false);

			string crabText = crabKindDef.GetLabelPlural().CapitalizeFirst();
			string letterLabel = string.Format(def.letterLabel, crabText);
			string letterText = string.Format(def.letterText, crabText);
			SendStandardLetter(letterLabel, letterText, def.letterDef, parms, crabPawn);
			return true;
		}

		protected bool TryFindAnimalKind(int tile, out PawnKindDef animalKind)
		{
			return DefDatabase<PawnKindDef>.AllDefs
				.Where(pawnKindDef => pawnKindDef.RaceProps.CanDoHerdMigration && pawnKindDef.race.tradeTags != null &&
				                      pawnKindDef.race.tradeTags.Contains("BMT_TradeTag_Crab") &&
				                      Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(tile,
					                      pawnKindDef.race))
				.TryRandomElementByWeight(pawnKindDef => Mathf.Lerp(0.2f, 1f, pawnKindDef.RaceProps.wildness), out animalKind);
		}

		protected List<Pawn> GenerateAnimals(PawnKindDef animalKind, int tile, IntRange minRange)
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