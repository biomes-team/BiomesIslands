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
            Map map = (Map)parms.target;
            GameConditionManager gameConditionManager = parms.target.GameConditionManager;
            GameCondition_GlowTide gameCondition_glowTide = (GameCondition_GlowTide)GameConditionMaker.MakeCondition(BiomesIslandsDefOf.GlowTide, 600000);

            gameConditionManager.RegisterCondition(gameCondition_glowTide);
            parms.letterHyperlinkThingDefs = gameCondition_glowTide.def.letterHyperlinks;
            SendStandardLetter(def.letterLabel, BiomesIslandsDefOf.GlowTide.letterText, def.letterDef, parms, new TargetInfo(gameCondition_glowTide.startingSpawn, map));
            SoundDefOf.PsychicSootheGlobal.PlayOneShotOnCamera((Map)parms.target);
            return true;
        }
    }


    class GameCondition_GlowTide : GameCondition
    {

        private IntRange radRange = new IntRange(5, 40);

        private List<IntVec3> validSpawns = new List<IntVec3>();

        private int curGeneration = 0;

        private List<CircleData> circleList = new List<CircleData> ();

        private int genLifespan = 4;

        public IntVec3 startingSpawn = IntVec3.Invalid;

        public override void Init()
        {
            base.Init();
            suppressEndMessage = true;
            SpawnInitialPlants();
        }

        public override void GameConditionTick()
        {
            if (TicksPassed % 128 == 0)
            {
                SpawnOnTick();
            }

            if (TicksPassed % 2560 == 0)
            {
                ExpandValidZone();
            }

        }


        private void SpawnInitialPlants()
        {
            Map map = base.SingleMap;
            curGeneration = 0;
            radRange = new IntRange(map.Size.x / 14, map.Size.x / 8);

            //radRange.min = Math.Max(radRange.min, 10);
            //radRange.max = Math.Min(radRange.max, 50);

            if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map), map, 0f, out IntVec3 initialSpawn))
            {
                GenSpawn.Spawn(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, initialSpawn, map, WipeMode.Vanish);
            }

            CircleData initCircle = new CircleData(initialSpawn, radRange.RandomInRange, curGeneration);

            circleList.Add(initCircle);

            startingSpawn = initialSpawn;

            //startingSpawn = new IntVec2(initialSpawn.x, initialSpawn.z)

            UpdateValidSpawns();
        }


        private void SpawnOnTick()
        {
            Map map = base.SingleMap;

            // spawn count should be proportional to valid spawn cells
            //int spawnCount = validSpawns.Count() / 200;
            //int spawnCount = validSpawns.Count() / 500;
            int spawnCount = validSpawns.Count() / 1200;
            spawnCount = Math.Max(spawnCount, 2);


            for (int j = 0; j < spawnCount; j++)
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
        }

        private void ExpandValidZone()
        {

            List<IntVec3> newGenCells = validSpawns;

            // new circles can spawn only in cells that are only in the newest circles (no overlap)
            foreach(CircleData c in circleList)
            {
                if(c.generation < curGeneration)
                {
                    newGenCells = newGenCells.Except(GenRadial.RadialCellsAround(c.center, c.radius, true)).ToList();
                }
                else if(curGeneration == 0)
                {
                    newGenCells = newGenCells.Except(GenRadial.RadialCellsAround(c.center, c.radius / 2, true)).ToList();
                }
            }

            curGeneration++;

            while (newGenCells.Count >= 4)
            {
                IntVec3 center = newGenCells.RandomElement();
                CircleData newCircle = new CircleData(center, radRange.RandomInRange, curGeneration);

                newGenCells = newGenCells.Except(GenRadial.RadialCellsAround(newCircle.center, newCircle.radius, true)).ToList();
                circleList.Add(newCircle);
            }

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
                    result = result.Distinct().ToList();
                }
            }
            //validSpawns = result;
            result = result.Where(c => c.InBounds(map)).ToList();
            validSpawns = result.Where(c => PlantUtility.CanEverPlantAt_NewTemp(BiomesIslandsDefOf.BiomesIslands_GlowPlankton, c, map)).ToList();

            if(validSpawns.Count() == 0)
            {
                base.End();
            }
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref curGeneration, "curGeneration", 0);
            Scribe_Collections.Look(ref circleList, "circleList", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit) 
            {
                UpdateValidSpawns();
            }
        }


        private int GetNextCirID()
        {
            return circleList.Count + 1;
        }


       
    }

    public class CircleData : IExposable
    {
        public IntVec3 center;
        public int radius;
        public int generation;

        public CircleData() { }
        public CircleData(IntVec3 cent, int rad, int gen)
        {
            center = cent;
            radius = rad;
            generation = gen;
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look<IntVec3>(ref center, "cent");
            Scribe_Values.Look(ref radius, "rad", 0);
            Scribe_Values.Look(ref generation, "gen", 0);
        }

    }

}
