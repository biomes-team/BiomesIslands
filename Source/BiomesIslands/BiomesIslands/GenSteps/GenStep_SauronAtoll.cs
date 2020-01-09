using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace BiomesIslands.GenSteps
{
    public class GenStep_SauronAtoll : GenStep
    {


        public override int SeedPart
        {
            get
            {
                return 123651228;
            }
        }

        public IntRange mainSizeRange = new IntRange(45, 56);

        public IntRange NoiseRange = new IntRange(-3, 3);
        public IntRange SizeRange = new IntRange(3, 14);
        private float stretch = 1.2f;


        public override void Generate(Map map, GenStepParams parms)
        {

            if (map.Biome.defName != "BiomesIslands_Atoll_NoBeach")
            {
                return;
            }
            if (BiomesIslands_Settings.flagSauronAtoll)
            {
                // spawn eye atoll
            }
            Log.Message("Generating sauron atoll");

            List<IntVec3> list = new List<IntVec3>();
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            TerrainGrid terrainGrid = map.terrainGrid;

            int x = NoiseRange.RandomInRange;
            int z = NoiseRange.RandomInRange;

            IntVec3 outerCenter = map.Center;
            outerCenter.x += x;
            outerCenter.z += z;
            IntVec3 innerCenter = map.Center;
            //innerCenter.x -= x;
            //innerCenter.z -= z;

            int outerRadius = mainSizeRange.RandomInRange;
            int innerRadius = outerRadius - 6;



            List<IntVec3> anchorRing = GenRadial.RadialCellsAround(outerCenter, outerRadius, true).Except(GenRadial.RadialCellsAround(innerCenter, innerRadius, true)).ToList();


            int centerCoord = map.Size.x / 2;
            stretch = 0.7f + Rand.Value;
            Log.Message("Stretch value: " + stretch);

            foreach (IntVec3 a in anchorRing)
            {
                //if (Rand.Bool)
                {
                    IntVec3 ne = a;

                    ne.x = Convert.ToInt32(centerCoord + (ne.x - centerCoord) * this.stretch);

                    List<IntVec3> island = new List<IntVec3>();

                    if (Rand.Chance(0.2f))
                    {
                        island = GenRadial.RadialCellsAround(ne, 2 * SizeRange.RandomInRange, true).ToList();

                    }
                    else
                    {
                        island = GenRadial.RadialCellsAround(ne, SizeRange.RandomInRange, true).ToList();

                    }

                    foreach (IntVec3 groundTile in island)
                    {
                        fertility[groundTile] += 0.035f;
                    }
                }


            }

            List<IntVec3> lagoon = GenRadial.RadialCellsAround(map.Center, outerRadius, true).ToList();

            foreach (IntVec3 current in map.AllCells)
            {

                TerrainDef terrainDef;

                terrainDef = this.TerrainFrom(current, map, elevation[current], fertility[current], false);
                if (terrainDef == TerrainDefOf.WaterOceanDeep)
                {
                    if (lagoon.Contains(current))
                    {
                        terrainDef = TerrainDefOf.WaterOceanShallow;
                    }
                }

                terrainGrid.SetTerrain(current, terrainDef);
            }



            // Other gensteps
            GenStep_OceanRockChunks genStep = new GenStep_OceanRockChunks();
            genStep.Generate(map, parms);
            GenStep_OceanDeepResources deepLumps = new GenStep_OceanDeepResources();
            deepLumps.Generate(map, parms);
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


