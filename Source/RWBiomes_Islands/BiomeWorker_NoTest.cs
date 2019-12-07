using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;

namespace RWBiomes_Islands
{
    // Never spawns the biome
    public class BiomeWorker_NoTest : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {

            return -100;
        }
    }
}