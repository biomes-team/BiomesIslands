using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.Noise;
using BiomesCore.DefModExtensions;

namespace BiomesIslands.GenSteps
{
    public class GenStep_Oasis : GenStep
    {
        public override int SeedPart
        {
            get
            {
                return 1449027791;
            }
        }


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


            float oasisSize = Rand.Range(30f, 70f);
            float beachSize = Rand.Range(30f, 50f);
            MapGenFloatGrid oasisGrid = MapGenerator.FloatGridNamed("OasisGrid");
            IntVec3 oasisCenter = map.Center;
            ModuleBase moduleBase = new Perlin(Rand.Range(0.015f, 0.028f), 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.Medium);

            Rot4 rot4 = Find.World.CoastDirectionAt(map.Tile);
            if (rot4 == Rot4.North)
            {
                oasisCenter.z -= 50;
                oasisSize -= Rand.Range(30f, 50f);
            }
            else if (rot4 == Rot4.South)
            {
                oasisCenter.z += 50;
                oasisSize -= Rand.Range(30f, 50f);
            }
            else if (rot4 == Rot4.East)
            {
                oasisCenter.x -= 50;
                oasisSize -= Rand.Range(30f, 50f);
            }
            else if (rot4 == Rot4.West)
            {
                oasisCenter.x += 50;
                oasisSize -= Rand.Range(30f, 50f);
            }

            float distanceVariance = Rand.Range(1.0f, 1.5f);
            float perlinVariance = 5f;
            if (oasisSize >= 50f)
            {
                perlinVariance = 6f;
            }
            foreach (IntVec3 current in map.AllCells)
            {
                float distance = DistanceBetweenPoints(current, oasisCenter);
                oasisGrid[current] = (moduleBase.GetValue(current) * perlinVariance ) + 0.1f * ((oasisSize) - (distance * distanceVariance));
            }

            TerrainGrid terrainGrid = map.terrainGrid;
            TerrainDef richSoil = TerrainDef.Named("SoilRich");
            TerrainDef soil = TerrainDef.Named("Soil");

            float deepBelow = oasisGrid[oasisCenter] * (1 - 0.5f);         // how much of the lake is deep water

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
                            if(oasisGrid[current] + 0.1 * fertility[current] > 1f - 0.1f * (beachSize * 0.8))
                            {
                                terrainGrid.SetTerrain(current, richSoil);
                            }
                            else
                            {
                                terrainGrid.SetTerrain(current, soil);
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