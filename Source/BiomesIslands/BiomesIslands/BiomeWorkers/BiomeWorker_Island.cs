using BiomesCore.Planet;
using BiomesIslands.Planet;
using RimWorld;
using RimWorld.Planet;

namespace BiomesIslands.BiomeWorkers
{
	public abstract class BiomeWorker_Island : BiomeWorker
	{
		/// <summary>
		/// Islands should return this value when they cannot be generated, to ensure that ocean takes precedence.
		/// </summary>
		public const float IgnoreMe = float.MinValue;

		public override float GetScore(Tile tile, int tileID)
		{
			float islandPresence = WorldGenInfoHandler.Get<WorldGenInfo_IslandPresence>().GetValue(tileID);
			return WorldGenInfo_IslandPresence.AllowsIsland(islandPresence)
				? GetIslandScore(tile, tileID, islandPresence)
				: IgnoreMe;
		}

		protected abstract float GetIslandScore(Tile tile, int tileID, float islandPresence);
	}
}