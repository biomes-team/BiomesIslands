using System;
using System.Collections.Generic;
using System.Linq;
using BiomesCore.DefModExtensions;
using RimWorld;
using Verse;


namespace BiomesIslands.GenSteps
{
    public class GenStep_Island : GenStep
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

            if (!map.Biome.HasModExtension<BiomesMap>())
            {
                return;
            }
            if (!map.Biome.GetModExtension<BiomesMap>().isIsland)
            {
                return;
            }

            // Only make smooth atolls, or it won't look like an atoll
            if(map.Biome.defName == "BiomesIslands_Atoll")
            {
                (new GenStep_IslandSmooth()).Generate(map, parms);
            }

            // Other islands get randomized
            else
            {
                int islandType = Rand.Range(0, 3);
                Log.Message("Island selection " + islandType);
                switch (islandType)
                {
                    case 0:
                        (new GenStep_IslandSmooth()).Generate(map, parms);
                        break;
                    case 1:
                        (new GenStep_IslandRough()).Generate(map, parms);
                        break;
                    case 2:
                        Log.Message("Making crescent");
                        (new GenStep_IslandCrescent()).Generate(map, parms);
                        break;
                    default:
                        (new GenStep_IslandSmooth()).Generate(map, parms);
                        break;
                }
            }

        }
    }
}

