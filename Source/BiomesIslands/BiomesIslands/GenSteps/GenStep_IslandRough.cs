using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiomesCore.DefModExtensions;
using RimWorld;
using Verse;
using Verse.Noise;

namespace BiomesIslands.GenSteps
{
    public class GenStep_IslandRough : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 2115796768;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            IntVec3 mapCenter = map.Center;
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            float mapSize = map.Size.x;
            ModuleBase noise = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

            // adjusts the size of the final island. Smaller numbers = bigger island
            // 2 = island hits the edges of the map
            float sizeAdj = Rand.Range(2.5f, 3.3f);

            foreach (IntVec3 current in map.AllCells)
            {
                float distance = (float)Math.Sqrt(Math.Pow(current.x - mapCenter.x, 2) + Math.Pow(current.z - mapCenter.z, 2));
                fertility[current] += Math.Max(0, 20 * (1f - (sizeAdj * distance / mapSize)) + 5f * noise.GetValue(current));
            }
        }
    }
}
