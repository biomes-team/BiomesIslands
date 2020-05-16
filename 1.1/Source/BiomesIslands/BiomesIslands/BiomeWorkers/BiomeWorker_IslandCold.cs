using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using RimWorld;
using Verse;
using Verse.Noise;
using UnityEngine;
using BiomesIslands.BiomeWorkersvariables;

namespace BiomesIslands.BiomeWorkers
{
    public class BiomeWorker_IslandCold : Variables
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
            if (tile.temperature < -20f || tile.temperature > 5f)
            {
                return 0f;
            }
            if (PerlinNoise == null)
            { // if we have never come across it yet, initialise it
                PerlinNoise = new Perlin(0.1, 60, 0.6, 8, Rand.Range(0, int.MaxValue), QualityMode.Low);
            }
            float PerlinNoiseValue = PerlinNoise.GetValue(Find.WorldGrid.GetTileCenter(tileID));
            if (PerlinNoiseValue < 0.4f)
            {
                return 0f;
            }
            return (MEAN_ELEVATION + tile.elevation) + (Mathf.Pow((10 * PerlinNoiseValue), 10));
        }

    }
}