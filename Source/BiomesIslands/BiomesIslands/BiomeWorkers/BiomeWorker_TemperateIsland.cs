using BiomesCore.WorldMap;
using RimWorld.Planet;

namespace BiomesIslands.BiomeWorkers
{
	public class BiomeWorker_TemperateIsland : BiomeWorker_Island
	{
		protected override float GetIslandScore(Tile tile, int tileID, float islandPresence)
		{
			return BiomeWorkerUtil.TemperateScore(tile);
		}
	}
}