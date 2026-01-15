using System;
using BiomesCore.Planet;
using BiomesIslands.Planet;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesIslands.BiomeWorkers
{
	[HotSwappable]
	public class BiomeWorker_Atoll : BiomeWorker
    {
        public override float GetScore(BiomeDef biome, Tile tile, PlanetTile planetTile)
        {
			if (tile.elevation > -100f || tile.elevation < -250f || tile.temperature < 15 || tile.rainfall < 600)
			{
				// Atolls are islands. They require a minimum amount of temperature and rainfall.
				return -100f;
			}
			if(!tile.WaterCovered)
			{
				return -100f;
			}
			if (Rand.Value > .005f)
			{
				return -100f;
			}

			return 100f;
		}
	}
}