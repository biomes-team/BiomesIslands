using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RWBiomes_Islands
{
    // Never spawns the biome
    public class BiomeWorker_IslandTropSwamp : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {

            if (!tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.elevation < -300f || tile.elevation > -120f)
            {
                return 0f;
            }
            if (Rand.Value < 0.985f)
            {
                return 0f;
            }


            if (tile.temperature < 15f)
            {
                return 0f;
            }
            if (tile.rainfall < 2000f)
            {
                return 0f;
            }
            if (tile.swampiness < 0.5f)
            {
                return 0f;
            }
            return 28f + (tile.temperature - 20f) * 1.5f + (tile.rainfall - 600f) / 165f + tile.swampiness * 3f;

        }
    }
}