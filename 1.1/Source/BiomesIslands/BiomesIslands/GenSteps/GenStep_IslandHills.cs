using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using RimWorld.Planet;
using Verse.Noise;
using UnityEngine;

namespace BiomesIslands.GenSteps
{
    // from GenStep_ElevationFertility
    public class GenStep_IslandHills : GenStep
    {
        private class RoofThreshold
        {
            public RoofDef roofDef;

            public float minGridVal;
        }

        private float maxMineableValue = 3.40282347E+38f;

        private const int MinRoofedCellsPerGroup = 20;

        private float hillSetback = 1.5f;
        private float hillCenter = 16f;
        private float hillTuning = 0.1f;

        public override int SeedPart
        {
            get
            {
                return 1182952823;
            }
        }


        public override void Generate(Map map, GenStepParams parms)
        {
            if (!map.Biome.HasModExtension<IslandMap>())
            {
                return;
            }
            if (!map.Biome.GetModExtension<IslandMap>().hasHilliness)
            {
                return;
            }
            ElevationGrid(map);

            map.regionAndRoomUpdater.Enabled = false;
            float num = 0.7f;
            List<GenStep_IslandHills.RoofThreshold> list = new List<GenStep_IslandHills.RoofThreshold>();
            list.Add(new GenStep_IslandHills.RoofThreshold
            {
                roofDef = RoofDefOf.RoofRockThick,
                minGridVal = num * 1.14f
            });
            list.Add(new GenStep_IslandHills.RoofThreshold
            {
                roofDef = RoofDefOf.RoofRockThin,
                minGridVal = num * 1.04f
            });
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid caves = MapGenerator.Caves;
            foreach (IntVec3 current in map.AllCells)
            {
                float num2 = elevation[current];
                if (num2 > num)
                {
                    if (caves[current] <= 0f)
                    {
                        ThingDef def = GenStep_RocksFromGrid.RockDefAt(current);
                        GenSpawn.Spawn(def, current, map, WipeMode.Vanish);
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (num2 > list[i].minGridVal)
                        {
                            map.roofGrid.SetRoof(current, list[i].roofDef);
                            break;
                        }
                    }
                }
            }
            BoolGrid visited = new BoolGrid(map);
            List<IntVec3> toRemove = new List<IntVec3>();
            foreach (IntVec3 current2 in map.AllCells)
            {
                if (!visited[current2])
                {
                    if (this.IsNaturalRoofAt(current2, map))
                    {
                        toRemove.Clear();
                        map.floodFiller.FloodFill(current2, (IntVec3 x) => this.IsNaturalRoofAt(x, map), delegate (IntVec3 x)
                        {
                            visited[x] = true;
                            toRemove.Add(x);
                        }, 2147483647, false, null);
                        if (toRemove.Count < 20)
                        {
                            for (int j = 0; j < toRemove.Count; j++)
                            {
                                map.roofGrid.SetRoof(toRemove[j], null);
                            }
                        }
                    }
                }
            }
            GenStep_ScatterLumpsMineable genStep_ScatterLumpsMineable = new GenStep_ScatterLumpsMineable();
            genStep_ScatterLumpsMineable.maxValue = this.maxMineableValue;

            // TODO: Probably need to scale this down for smaller maps. Vanilla: num3 = 10f
            float num3 = 10f;
            switch (Find.WorldGrid[map.Tile].hilliness)
            {
                case Hilliness.Flat:
                    num3 = 4f;
                    break;
                case Hilliness.SmallHills:
                    num3 = 8f;
                    break;
                case Hilliness.LargeHills:
                    num3 = 11f;
                    break;
                case Hilliness.Mountainous:
                    num3 = 15f;
                    break;
                case Hilliness.Impassable:
                    num3 = 16f;
                    break;
            }
            genStep_ScatterLumpsMineable.countPer10kCellsRange = new FloatRange(num3, num3);
            genStep_ScatterLumpsMineable.Generate(map, parms);
            map.regionAndRoomUpdater.Enabled = true;
        }

        

        private void ElevationGrid(Map map)
        {
            ModuleBase moduleBase = new Perlin(0.020999999716877937, 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            moduleBase = new ScaleBias(0.5, 0.5, moduleBase);
            NoiseDebugUI.StoreNoiseRender(moduleBase, "elev base");
            float num = 1f;
            switch (map.TileInfo.hilliness)
            {
                case Hilliness.Flat:
                    num = MapGenTuning.ElevationFactorFlat;
                    break;
                case Hilliness.SmallHills:
                    num = MapGenTuning.ElevationFactorSmallHills;
                    break;
                case Hilliness.LargeHills:
                    num = MapGenTuning.ElevationFactorLargeHills;
                    break;
                case Hilliness.Mountainous:
                    num = MapGenTuning.ElevationFactorMountains;
                    break;
                case Hilliness.Impassable:
                    num = MapGenTuning.ElevationFactorImpassableMountains;
                    break;
            }

            num *= num;

            //num *= 1.2f;

            moduleBase = new Multiply(moduleBase, new Const((double)num));
            NoiseDebugUI.StoreNoiseRender(moduleBase, "elev world-factored");
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;


            foreach (IntVec3 current in map.AllCells)
            {
                //elevation[current] = moduleBase.GetValue(current);

                //elevation[current] = hillTuning * Mathf.Min(fertility[current] - hillSetback, hillCenter) * moduleBase.GetValue(current);     // round 1
                //elevation[current] = hillTuning * Mathf.Min(fertility[current] - hillSetback, hillCenter) + moduleBase.GetValue(current) - 1f;  // rouund 2
                //elevation[current] = (1 + hillTuning * Mathf.Min(fertility[current] - hillSetback, hillCenter)) * moduleBase.GetValue(current) - 0.5f;     // round 3

                elevation[current] = (1 + hillTuning * Mathf.Min(fertility[current], hillCenter)) * moduleBase.GetValue(current) - 0.5f;     // round 5

                //elevation[current] = (1 + hillTuning * Mathf.Min(fertility[current] - hillSetback, hillCenter)) * moduleBase.GetValue(current) - 0.5f;     // round 5


            }
        }


        private bool IsNaturalRoofAt(IntVec3 c, Map map)
        {
            return c.Roofed(map) && c.GetRoof(map).isNatural;
        }
    }
}
