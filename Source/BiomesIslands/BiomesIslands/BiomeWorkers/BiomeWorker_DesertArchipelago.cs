using BiomesCore.WorldMap;
using BiomesIslands.Planet;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesIslands.BiomeWorkers
{
	public class BiomeWorker_DesertArchipelago : BiomeWorker
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
            if (Rand.Value < .995f)
            {
                return -100f;
            }
            return BiomeWorkerUtil.DesertScore(tile);
        }
    }

}