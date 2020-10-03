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
        private float movementDir = 0f;

        private float pctLeft
        {
            get
            {
               return (float)TicksLeft / (float)Duration;
            }
        }

        public override void Init()
        {
            base.Init();

            movementDir = Rand.Range(0, 360);
            // find out what else to init
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

                    int spawnCount = Rand.Range(1, 15);

                    for(int j = 0; j < spawnCount; j++)
                    {
                        IntVec3 spawnSpot = CellFinderLoose.RandomCellWith((IntVec3 c) => c.GetFirstBuilding(map) == null && PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map) && c.x < tideSize + pctLeft * mapSize && c.x > -tideSize + pctLeft * mapSize, map, 500);

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
