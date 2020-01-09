using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;
using Verse;

namespace BiomesIslands.BiomeWorkers
{
    public class BiomeWorker_Atoll : BiomeWorker
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
            if (Rand.Value < 0.997f)
            {
                return 0f;
            }


            if (tile.temperature < 15f)
            {
                return 0f;
            }
            //if (tile.rainfall < 2000f)
            //{
            //    return 0f;
            //}

            return tile.temperature + 0.0001f;

        }
    }
}