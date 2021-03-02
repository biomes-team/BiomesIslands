using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;

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


        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (base.TryExecuteWorker(parms))
            {
                SoundDefOf.PsychicSootheGlobal.PlayOneShotOnCamera((Map)parms.target);
                return true;
            }
            return false;
        }
    }


    class GameCondition_GlowTide : GameCondition
    {

        private IntRange radRange = new IntRange(5, 40);

        private List<IntVec3> validSpawns = new List<IntVec3>();

        private int curGeneration;

        private List<CircleData> circleList = new List<CircleData> ();

        private int genLifespan = 4;


        // direction the tide is approaching from
        //private Rot4 tideDir;

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
            //tideDir = Rot4.Random;

            SpawnInitialPlants();
        }

        public override void GameConditionTick()
        {
            // spawn plants
            //List<Map> affectedMaps = base.AffectedMaps;
            
            if (TicksPassed % 128 == 0)
            {
                SpawnOnTick();
            }

            if(TicksPassed % 2560 == 0)
            {
                ExpandValidZone();
            }

        }



        private void SpawnInitialPlants()
        {
            Map map = base.SingleMap;
            curGeneration = 0;
            radRange = new IntRange(map.Size.x / 15, map.Size.x / 7);

            //radRange.min = Math.Max(radRange.min, 10);
            //radRange.max = Math.Min(radRange.max, 50);

            if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map), map, 0f, out IntVec3 initialSpawn))
            {
                GenSpawn.Spawn(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, initialSpawn, map, WipeMode.Vanish);
            }

            CircleData initCircle = new CircleData(initialSpawn, radRange.RandomInRange, curGeneration);

            circleList.Add(initCircle);

            //validSpawns = GenRadial.RadialCellsAround(initialSpawn, radRange.RandomInRange, true).ToList();
            UpdateValidSpawns();

        }

        private void SpawnOnTick()
        {
            Map map = base.SingleMap;

            // spawn count should be proportional to valid spawn cells
            int spawnCount = validSpawns.Count() / 200;

            for(int j = 0; j < spawnCount; j++)
            {
                IntVec3 spawnSpot = CellFinderLoose.RandomCellWith((IntVec3 c) => c.GetFirstBuilding(map) == null
                    && PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map)
                    && validSpawns.Contains(c), map, 500);

                if (spawnSpot != null)
                {
                    if (spawnSpot.InBounds(map))
                    {
                        GenSpawn.Spawn(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, spawnSpot, map, WipeMode.Vanish);

                    }
                }
            }

            //PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map)

        }

        private void ExpandValidZone()
        {
            //Log.Message(String.Format("----- EXPANDING BLOOM. Generation: {0}", curGeneration));

            List<IntVec3> newGenCells = validSpawns;

            // new circles can spawn only in cells that are only in the newest circles (no overlap)
            foreach(CircleData c in circleList)
            {
                if(c.generation < curGeneration)
                {
                    newGenCells = newGenCells.Except(GenRadial.RadialCellsAround(c.center, c.radius, true)).ToList();
                }
                else 
                {
                    newGenCells = newGenCells.Except(GenRadial.RadialCellsAround(c.center, c.radius / 2, true)).ToList();
                }
                //Log.Message(String.Format("Testing circle, gen {0}. cells remaining: {1}", c.generation, newGenCells.Count));
            }

            curGeneration++;

            int newCount = 0;   //for testing only

            //Log.Message("valid new cells: " + newGenCells.Count());
            //while(newGenCells.Count >= radRange.min * radRange.min / 4)
            while (newGenCells.Count >= 4)

            {
                IntVec3 center = newGenCells.RandomElement();
                CircleData newCircle = new CircleData(center, radRange.RandomInRange, curGeneration);

                newGenCells = newGenCells.Except(GenRadial.RadialCellsAround(newCircle.center, newCircle.radius, true)).ToList();
                circleList.Add(newCircle);
                newCount++;
            }
            //Log.Message(String.Format("Added {0} new spawn spots", newCount));

            UpdateValidSpawns();
        }

        private void UpdateValidSpawns()
        {
            Map map = base.SingleMap;
            List<IntVec3> result = new List<IntVec3>();
            foreach (CircleData c in circleList)
            {
                if (c.generation >= curGeneration - genLifespan)
                {
                    result.AddRange(GenRadial.RadialCellsAround(c.center, c.radius, true));

                    // TODO: Test if this line is useful
                    // removing it may result in a more natural "flow"
                    result = result.Distinct().ToList();
                }
            }
            //validSpawns = result;
            result = result.Where(c => c.InBounds(map)).ToList();
            validSpawns = result.Where(c => PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map)).ToList();

            //Log.Message(String.Format("Spawns updated. {0} valid tiles"));

        }


        private class CircleData
        {
            public IntVec3 center;
            public int radius;

            // spawn time?
            public int generation;

            public CircleData(IntVec3 c, int r, int g)
            {
                center = c;
                radius = r;
                generation = g;
            }
        }
    }



}
