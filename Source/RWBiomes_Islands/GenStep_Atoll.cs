using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.BaseGen;
using Verse;


namespace RWBiomes_Islands
{
    public class GenStep_Atoll : GenStep
    {


        public override int SeedPart
        {
            get
            {
                return 2115796768;
            }
        }

        public IntRange SizeRange = new IntRange(3, 16);

        public float stretch = 1.4f;
        public int totalDist = 150;

        private IntRange totalRange = new IntRange(100, 150);


        public override void Generate(Map map, GenStepParams parms)
        {

            if (map.Biome.defName != "RWB_IslandAtoll_NoBeach")
            {
                return;
            }

            List<IntVec3> list = new List<IntVec3>();
            MapGenFloatGrid fertility = MapGenerator.Fertility;

            IntRange NoiseRange = new IntRange(-50, 50);
            NoiseRange.min = (int)(0 - 0.2 * map.Size.x);
            NoiseRange.max = (int)(0.2 * map.Size.x);

            IntVec3 mapCenter = map.Center;
            int x = NoiseRange.RandomInRange;
            int z = NoiseRange.RandomInRange;

            IntVec3 focus1 = mapCenter;
            focus1.x += x;
            focus1.z += z;

            IntVec3 focus2 = mapCenter;
            focus2.x -= x;
            focus2.z -= z;

            List<IntVec3> shape = MakeEllipse(focus1, focus2, map);

            IntVec3 circleTest = focus2;
            focus2.x -= 3 * x;
            focus2.z -= 3 * z;
            List<IntVec3> originalShape = shape;

            IntRange cutoutSizes = new IntRange(Math.Min(totalRange.min / 8, 56), Math.Min(totalRange.max / 3, 56));

            int numCutouts = 6;

            for (int i = 0; i < numCutouts; i++)
            {
                IntVec3 tempCenter = map.AllCells.RandomElement();

                if (!originalShape.Contains(tempCenter))
                {
                    shape = shape.Except(GenRadial.RadialCellsAround(tempCenter, cutoutSizes.RandomInRange, true)).ToList();
                }
            }

            Makelandmass(shape, ref fertility, map);
            SetNewTerrains(map);

            GenStep_OceanRockChunks genStep = new GenStep_OceanRockChunks();
            genStep.Generate(map, parms);
            GenStep_OceanDeepResources deepLumps = new GenStep_OceanDeepResources();
            deepLumps.Generate(map, parms);

        }


        private List<IntVec3> MakeEllipse(IntVec3 focus1, IntVec3 focus2, Map map)
        {
            SizeRange.max = 5 + map.Size.x / 20;

            totalRange.max = (int)(Math.Min(0.6 * map.Size.x, map.Size.x - (3 * SizeRange.max)));
            totalRange.min = Math.Max((int)(0.45 * map.Size.x), SizeRange.min + (int)DistanceBetweenPoints(focus1, focus2));

            totalDist = totalRange.RandomInRange;

            List<IntVec3> output = new List<IntVec3>();

            foreach (IntVec3 current in map.AllCells)
            {
                if (DistanceBetweenPoints(focus1, current) + DistanceBetweenPoints(focus2, current) < totalDist)
                {
                    output.Add(current);
                }
            }
            return output;
        }


        private float DistanceBetweenPoints(IntVec3 point1, IntVec3 point2)
        {
            float dist = 0;
            double xDist = Math.Pow(point1.x - point2.x, 2);
            double zDist = Math.Pow(point1.z - point2.z, 2);

            dist = (float)Math.Sqrt(xDist + zDist);

            return dist;
        }


        private void SetTestTerrains(Map map, List<IntVec3> shape)
        {
            TerrainGrid terrainGrid = map.terrainGrid;
            TerrainDef testTerr = TerrainDefOf.MetalTile;
            foreach (IntVec3 current in shape)
            {
                terrainGrid.SetTerrain(current, testTerr);
            }

        }


        private void SetNewTerrains(Map map)
        {
            TerrainGrid terrainGrid = map.terrainGrid;
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;

            foreach (IntVec3 current in map.AllCells)
            {

                TerrainDef terrainDef;

                terrainDef = this.TerrainFrom(current, map, elevation[current], fertility[current], false);


                terrainGrid.SetTerrain(current, terrainDef);
            }

        }


        private void Makelandmass(List<IntVec3> landCells, ref MapGenFloatGrid fertility, Map map)
        {

            float fertIncrement = 0.06f;

            if (totalDist < 90)
            {
                fertIncrement = 0.09f;
            }
            if (totalDist < 70)
            {
                fertIncrement = 0.12f;
            }
            if (totalDist < 55)
            {
                fertIncrement = 0.2f;
            }
            if (totalDist < 30)
            {
                fertIncrement = 0.3f;
            }

            foreach (IntVec3 a in landCells)
            {
                if (Rand.Bool)
                {
                    IntVec3 ne = a;

                    List<IntVec3> island = new List<IntVec3>();

                    if (Rand.Chance(0.3f))
                    {
                        if (Rand.Chance(0.2f))
                        {
                            island = GenRadial.RadialCellsAround(ne, Math.Min(56, 3.5f * SizeRange.RandomInRange), true).ToList();

                        }
                        else
                        {
                            island = GenRadial.RadialCellsAround(ne, Math.Min(56, 2.0f * SizeRange.RandomInRange), true).ToList();

                        }

                    }
                    else
                    {
                        island = GenRadial.RadialCellsAround(ne, Math.Min(56, SizeRange.RandomInRange), true).ToList();

                    }

                    foreach (IntVec3 groundTile in island)
                    {
                        if (groundTile.InBounds(map))
                        {
                            fertility[groundTile] += fertIncrement;

                        }
                    }
                }


            }
        }


        private TerrainDef TerrainFrom(IntVec3 c, Map map, float elevation, float fertility, bool preferSolid)
        {
            TerrainDef terrainDef = null;

            if (terrainDef == null && preferSolid)
            {
                return GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
            }
            TerrainDef terrainDef2 = BeachMaker.BeachTerrainAt(c, map.Biome);
            if (terrainDef2 == TerrainDefOf.WaterOceanDeep)
            {
                return terrainDef2;
            }
            if (terrainDef != null && terrainDef.IsRiver)
            {
                return terrainDef;
            }
            if (terrainDef2 != null)
            {
                return terrainDef2;
            }
            if (terrainDef != null)
            {
                return terrainDef;
            }
            for (int i = 0; i < map.Biome.terrainPatchMakers.Count; i++)
            {
                terrainDef2 = map.Biome.terrainPatchMakers[i].TerrainAt(c, map, fertility);
                if (terrainDef2 != null)
                {
                    return terrainDef2;
                }
            }
            if (elevation > 0.55f && elevation < 0.61f)
            {
                return TerrainDefOf.Gravel;
            }
            if (elevation >= 0.61f)
            {
                return GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
            }
            terrainDef2 = TerrainThreshold.TerrainAtValue(map.Biome.terrainsByFertility, fertility);
            if (terrainDef2 != null)
            {
                return terrainDef2;
            }

            return TerrainDefOf.Sand;
        }



    }
}

