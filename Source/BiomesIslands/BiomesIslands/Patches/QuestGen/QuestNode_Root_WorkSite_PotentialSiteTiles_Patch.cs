using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BiomesIslands.Patches.QuestGen
{
	/// <summary>
	/// Allows Ideology work sites to spawn when the colony is surrounded by ocean.
	/// This uses a destructive prefix due to how extensive the required changes are.
	/// It might be necessary to rework this if mod conflicts arise.
	/// </summary>
	[HarmonyPatch(typeof(QuestNode_Root_WorkSite), nameof(QuestNode_Root_WorkSite.PotentialSiteTiles))]
	public class QuestNode_Root_WorkSite_PotentialSiteTiles_Patch
	{
		/// <summary>
		/// Identical to vanilla check, but it allows the flood filler to traverse ocean tiles.
		/// </summary>
		/// <param name="tileID">Tile being checked.</param>
		/// <param name="rootID">Player base tile.</param>
		/// <returns>True if the flood fill algorithm can explore this node.</returns>
		private static bool PassCheck(int tileID, int rootID)
		{
			return (!Find.World.Impassable(tileID) || Find.WorldGrid.tiles[tileID].biome == BiomeDefOf.Ocean) &&
			       Find.WorldGrid.ApproxDistanceInTiles(tileID, rootID) <= 9.0;
		}

		/// <summary>
		/// Identical to vanilla, with an additional impassability check.
		/// </summary>
		/// <param name="tileID">Tile being checked.</param>
		/// <param name="rootID">Player base tile.</param>
		/// <param name="tiles">List of potential site tiles.</param>
		private static void Processor(int tileID, int rootID, ref List<int> tiles)
		{
			if (!Find.World.Impassable(tileID) && Find.WorldGrid.ApproxDistanceInTiles(tileID, rootID) >= 3.0F)
			{
				tiles.Add(tileID);
			}
		}

		public static bool Prefix(ref List<int> __result, int root)
		{
			List<int> tiles = new List<int>();
			Find.WorldFloodFiller.FloodFill(root,
				tileId => PassCheck(tileId, root),
				tileId => Processor(tileId, root, ref tiles));
			__result = tiles;

			return false;
		}
	}
}