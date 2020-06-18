using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.Noise;
using BiomesCore_BiomeControl;

namespace BiomesIslands.GenSteps
{
    public class GenStep_Oasis2 : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1449027791;
            }
        }

        private float oasisSize = Rand.Range(0.3f, 0.5f);               // as a proportion of map size. 0.5 is about half the width of the map.
        private float beachSize = Rand.Range(8f, 20f);                  // in tiles. approximate.

        public override void Generate(Map map, GenStepParams parms)
        {
            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return;
            }
            if (!map.Biome.GetModExtension<BiomesMap>().isOasis)
            {
                return;
            }

            MapGenFloatGrid oasisGrid = MapGenerator.FloatGridNamed("OasisGrid");
            IntVec3 mapCenter = map.Center;
            ModuleBase moduleBase = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.Medium);

            float distortion = Rand.Range(1.0f, 2.0f);

            foreach (IntVec3 current in map.AllCells)
            {
                float distance = DistanceBetweenPoints(current, mapCenter);
                oasisGrid[current] = 0f + distortion * moduleBase.GetValue(current) + 0.1f * (oasisSize - distance);
            }

            TerrainGrid terrainGrid = map.terrainGrid;
            TerrainDef terrShore = TerrainDef.Named("SoilRich");
            TerrainDef terrMud = TerrainDef.Named("Mud");

            float deepBelow = oasisGrid[mapCenter] * (1 - 0.5f);         // how much of the lake is deep water

            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;

            foreach (IntVec3 current in map.AllCells)
            {
                // leaves mountains & most surrounding gravel untouched, if there is any mountain in the oasis area
                // otherwise, mountains get water placed under them
                if (elevation[current] < 0.65f)                         
                {
                    if (oasisGrid[current] > deepBelow)
                    {
                        terrainGrid.SetTerrain(current, TerrainDefOf.WaterDeep);
                    }
                    else if (!terrainGrid.TerrainAt(current).IsRiver)
                    {
                        if (oasisGrid[current] > 0f)
                        {
                            terrainGrid.SetTerrain(current, TerrainDefOf.WaterShallow);
                        }
                        else if (oasisGrid[current] > 0f - 0.1f * beachSize)
                        {
                            if(oasisGrid[current] + 2 * fertility[current] > 1f - 0.1f * beachSize)
                            {
                                terrainGrid.SetTerrain(current, terrMud);
                            }
                            else
                            {
                                terrainGrid.SetTerrain(current, terrShore);
                            }
                        }
                    }
                }
            }
        }


        private float DistanceBetweenPoints(IntVec3 point1, IntVec3 point2)
        {
            float dist = 0;
            double xDist = Math.Pow(point1.x - point2.x, 2);
            double zDist = Math.Pow(point1.z - point2.z, 2);
            dist = (float)Math.Sqrt(xDist + zDist);

            return dist;
        }
    }
}