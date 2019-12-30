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
                return 12651228;
            }
        }

        public IntRange NoiseRange = new IntRange(-20, 20);
        public IntRange SizeRange = new IntRange(3, 14);
        public float stretch = 1.4f;


        public override void Generate(Map map, GenStepParams parms)
        {

            if (map.Biome.defName != "RWB_Atoll_NoBeach")
            {
                return;
            }

            List<IntVec3> list = new List<IntVec3>();
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            TerrainGrid terrainGrid = map.terrainGrid;

            IntVec3 mapCenter = map.Center;

            int outerRadius = 55;
            int innerRadius = 50;
            //int innerRadius = 52;
            List<IntVec3> anchorRing = GenRadial.RadialCellsAround(mapCenter, outerRadius, true).Except(GenRadial.RadialCellsAround(mapCenter, innerRadius, true)).ToList();


            int centerCoord = map.Size.x / 2;

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
                        fertility[groundTile] += 0.015f;
                    }
                }


            }

            List<IntVec3> lagoon = GenRadial.RadialCellsAround(mapCenter, outerRadius, true).ToList();

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


