using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;

namespace BiomesIslands.Incidents
{
    class Incident_GlowTide : IncidentWorker_MakeGameCondition
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            return map.mapTemperature.SeasonalTemp >= 5f && map.mapTemperature.SeasonalTemp <= 50f;
        }
    }


    class GameCondition_GlowTide : GameCondition
    {
        // direction the tide is approaching from
        private Rot4 tideDir;

        private float pctRemaining
        {
            get
            {
               return (float)TicksLeft / (float)Duration;
            }
        }

        public override void Init()
        {
            base.Init();
            tideDir = Rot4.Random;
        }

        public override void GameConditionTick()
        {
            // spawn plants
            List<Map> affectedMaps = base.AffectedMaps;
            
            if (TicksPassed % 128 == 0)
            {
                for (int i = 0; i < affectedMaps.Count; i++)
                {
                    Map map = affectedMaps[i];
                    int mapSize = map.Size.x;
                    int tideSize = mapSize / 10;
                    int spawnCount = 7;

                    for(int j = 0; j < spawnCount; j++)
                    {
                        IntVec3 spawnSpot;
                        if(tideDir == Rot4.East)
                        {
                            spawnSpot = CellFinderLoose.RandomCellWith((IntVec3 c) => c.GetFirstBuilding(map) == null
                                        && PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map)
                                        && c.x < tideSize + pctRemaining * mapSize
                                        && c.x > -tideSize + pctRemaining * mapSize, map, 500);
                        }
                        else if (tideDir == Rot4.West)
                        {
                            spawnSpot = CellFinderLoose.RandomCellWith((IntVec3 c) => c.GetFirstBuilding(map) == null
                                        && PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map)
                                        && c.x < tideSize + (1 - pctRemaining) * mapSize
                                        && c.x > -tideSize + (1 - pctRemaining) * mapSize, map, 500);
                        }
                        else if (tideDir == Rot4.South)
                        {
                            spawnSpot = CellFinderLoose.RandomCellWith((IntVec3 c) => c.GetFirstBuilding(map) == null
                                        && PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map)
                                        && c.z < tideSize + (1 - pctRemaining) * mapSize
                                        && c.z > -tideSize + (1 - pctRemaining) * mapSize, map, 500);
                        }
                        else
                        {
                            spawnSpot = CellFinderLoose.RandomCellWith((IntVec3 c) => c.GetFirstBuilding(map) == null
                                        && PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map)
                                        && c.z < tideSize + pctRemaining * mapSize
                                        && c.z > -tideSize + pctRemaining * mapSize, map, 500);
                        }

                        if (spawnSpot != null)
                        {
                            GenSpawn.Spawn(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, spawnSpot, map, WipeMode.Vanish);
                        }
                    }
                   
                }
            }

        }

    }



}
