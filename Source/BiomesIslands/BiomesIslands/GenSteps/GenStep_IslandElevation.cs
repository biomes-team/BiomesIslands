using System.Collections.Generic;
using Verse;
using RimWorld;
using RimWorld.Planet;
using Verse.Noise;
using UnityEngine;
using BiomesCore.DefModExtensions;
using MapDesigner;

namespace BiomesIslands.GenSteps
{
    // from GenStep_ElevationFertility
    // this adds island-appropriate hilliness to island maps

    public class GenStep_IslandElevation : GenStep
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
            SetElevationGrid(map);
        }

        

        private void SetElevationGrid(Map map)
        {
            float freq = 0.025f;
            float lacun = 2.0f;
            if(ModsConfig.IsActive("zylle.MapDesigner"))
            {
                freq = 1.2f * LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().hillSize;
                lacun = LoadedModManager.GetMod<MapDesigner_Mod>().GetSettings<MapDesignerSettings>().hillSmoothness;
            }


            ModuleBase moduleBase = new Perlin(freq, lacun, 0.5, 6, Rand.Range(0, 2147483647), QualityMode.High);

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
                elevation[current] = (1 + islandHillTuning * Mathf.Min(fertility[current], islandHillCenter)) * moduleBase.GetValue(current) - 0.5f;

            }


        }


        private bool IsNaturalRoofAt(IntVec3 c, Map map)
        {
            return c.Roofed(map) && c.GetRoof(map).isNatural;
        }
    }
}
