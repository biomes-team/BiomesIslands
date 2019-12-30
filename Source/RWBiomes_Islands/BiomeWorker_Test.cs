using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;

namespace RWBiomes_Islands
{
    // Spawns the biome on every tile
    public class BiomeWorker_Test : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            if(!tile.WaterCovered)
            {
                return -100;
            }
            return 100;

        }
    }
}
