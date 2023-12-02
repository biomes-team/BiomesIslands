using RimWorld.Planet;

namespace BiomesIslands.BiomeWorkers
{
	public class BiomeWorker_TropicalIsland : BiomeWorker_Island
	{
		protected override float GetIslandScore(Tile tile, int tileID, float islandPresence)
		{
			return BiomeWorkerUtil.TropicalScore(tile);
		}
	}
}