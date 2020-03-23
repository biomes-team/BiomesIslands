using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;
using Verse;

namespace BiomesIslands.BiomeWorkers
{
    public class BiomeWorker_IslandTemperate : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {

            if (!tile.WaterCovered)
            {
                return -100f;
            }
            if (tile.elevation < -500f || tile.elevation > -80f)
            {
                return 0f;
            }
            if (Rand.Value < 0.998f)
            {
                return 0f;
            }
            if (tile.temperature < 0f || tile.temperature > 12f)
            {
                return 0f;
            }


            return 10f;

        }
    }
}