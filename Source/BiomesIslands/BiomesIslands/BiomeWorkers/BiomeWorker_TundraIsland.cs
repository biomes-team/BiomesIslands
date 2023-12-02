using RimWorld.Planet;

namespace BiomesIslands.BiomeWorkers
{
	public class BiomeWorker_TundraIsland : BiomeWorker_Island
	{
		protected override float GetIslandScore(Tile tile, int tileID, float islandPresence)
		{
			return BiomeWorkerUtil.TundraScore(tile);
		}
	}
}