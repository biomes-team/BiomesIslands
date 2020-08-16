using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.Noise;

namespace BiomesIslands.BiomeWorkersvariables
{
    public class Variables : BiomeWorker
    {

        public static ModuleBase PerlinNoise = null;
        public const int MEAN_ELEVATION = 50;
        public float LOW_DEPTH_BONUS = 0;

        public override float GetScore(Tile tile, int tileID)
        {
            throw new NotImplementedException();
        }
    }
}