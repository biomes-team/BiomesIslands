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
using BiomesCore_BiomeControl;

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

        private float islandHillSetback = 1.5f;
        private float islandHillCenter = 16f;
        private float islandHillTuning = 0.1f;

        public override int SeedPart
        {
            get
            {
                return 1182952823;
            }
        }


        public override void Generate(Map map, GenStepParams parms)
        {
            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return;
            }
            if (!map.Biome.GetModExtension<BiomesMap>().hasHilliness)
            {
                return;
            }
            ElevationGrid(map);

            map.regionAndRoomUpdater.Enabled = false;
            float roofThreshhold = 0.7f;
            List<GenStep_IslandHills.RoofThreshold> list = new List<GenStep_IslandHills.RoofThreshold>();
            list.Add(new GenStep_IslandHills.RoofThreshold
            {
                roofDef = RoofDefOf.RoofRockThick,
                minGridVal = roofThreshhold * 1.14f
            });
            list.Add(new GenStep_IslandHills.RoofThreshold
            {
                roofDef = RoofDefOf.RoofRockThin,
                minGridVal = roofThreshhold * 1.04f
            });
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid caves = MapGenerator.Caves;
            foreach (IntVec3 current in map.AllCells)
            {
                float curElev = elevation[current];
                if (curElev > roofThreshhold)
                {
                    if (caves[current] <= 0f)
                    {
                        ThingDef def = GenStep_RocksFromGrid.RockDefAt(current);
                        GenSpawn.Spawn(def, current, map, WipeMode.Vanish);
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (curElev > list[i].minGridVal)
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

            float oreTuning = 10f;
            switch (Find.WorldGrid[map.Tile].hilliness)
            {
                case Hilliness.Flat:
                    oreTuning = 4f;
                    break;
                case Hilliness.SmallHills:
                    oreTuning = 8f;
                    break;
                case Hilliness.LargeHills:
                    oreTuning = 11f;
                    break;
                case Hilliness.Mountainous:
                    oreTuning = 15f;
                    break;
                case Hilliness.Impassable:
                    oreTuning = 16f;
                    break;
            }

            // This scales the amount of available ore for islands
            oreTuning *= 0.8f;

            genStep_ScatterLumpsMineable.countPer10kCellsRange = new FloatRange(oreTuning, oreTuning);
            genStep_ScatterLumpsMineable.Generate(map, parms);
            map.regionAndRoomUpdater.Enabled = true;
        }

        

        private void ElevationGrid(Map map)
        {
            //ModuleBase moduleBase = new Perlin(0.020999999716877937, 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);
            ModuleBase moduleBase = new Perlin(0.025, 2.0, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

            moduleBase = new ScaleBias(0.5, 0.5, moduleBase);
            NoiseDebugUI.StoreNoiseRender(moduleBase, "elev base");
            float elevScaling = 1f;
            switch (map.TileInfo.hilliness)
            {
                case Hilliness.Flat:
                    elevScaling = MapGenTuning.ElevationFactorFlat;
                    break;
                case Hilliness.SmallHills:
                    elevScaling = MapGenTuning.ElevationFactorSmallHills;
                    break;
                case Hilliness.LargeHills:
                    elevScaling = MapGenTuning.ElevationFactorLargeHills;
                    break;
                case Hilliness.Mountainous:
                    elevScaling = MapGenTuning.ElevationFactorMountains;
                    break;
                case Hilliness.Impassable:
                    elevScaling = MapGenTuning.ElevationFactorImpassableMountains;
                    break;
            }

            elevScaling *= elevScaling;

            //elevScaling *= 1.2f;

            moduleBase = new Multiply(moduleBase, new Const((double)elevScaling));
            NoiseDebugUI.StoreNoiseRender(moduleBase, "elev world-factored");
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;


            foreach (IntVec3 current in map.AllCells)
            {
                //elevation[current] = moduleBase.GetValue(current);

                //elevation[current] = hillTuning * Mathf.Min(fertility[current] - hillSetback, hillCenter) * moduleBase.GetValue(current);     // round 1
                //elevation[current] = hillTuning * Mathf.Min(fertility[current] - hillSetback, hillCenter) + moduleBase.GetValue(current) - 1f;  // rouund 2
                //elevation[current] = (1 + hillTuning * Mathf.Min(fertility[current] - hillSetback, hillCenter)) * moduleBase.GetValue(current) - 0.5f;     // round 3

                elevation[current] = (1 + islandHillTuning * Mathf.Min(fertility[current], islandHillCenter)) * moduleBase.GetValue(current) - 0.5f;     // round 5

                //elevation[current] = (1 + hillTuning * Mathf.Min(fertility[current] - hillSetback, hillCenter)) * moduleBase.GetValue(current) - 0.5f;     // round 5


            }
        }


        private bool IsNaturalRoofAt(IntVec3 c, Map map)
        {
            return c.Roofed(map) && c.GetRoof(map).isNatural;
        }
    }
}
