using BiomesCore.WorldMap;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesIslands.BiomeWorkers
{
	public class BiomeWorker_DesertIsland : BiomeWorker
	{
        public override float GetScore(BiomeDef biome, Tile tile, PlanetTile planetTile)
        {
            if (tile.elevation > -100 || tile.elevation < -250f)
            {
                return -100f;
            }
            if (!tile.WaterCovered)
            {
                return -100f;
            }
            if (Rand.Value > .005f)
            {
                return -100f;
            }
            return BiomeWorkerUtil.DesertScore(tile);
		}
	}
}