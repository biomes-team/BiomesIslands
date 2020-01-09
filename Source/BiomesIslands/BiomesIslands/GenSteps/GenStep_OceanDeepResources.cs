using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;


namespace BiomesIslands.GenSteps
{
    public class GenStep_OceanDeepResources : GenStep_Scatterer
    {
        public override int SeedPart
        {
            get
            {
                return 1712041303;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            if (!map.TileInfo.WaterCovered)
            {
                return;
            }
            int num = base.CalculateFinalCount(map);

            num = CountFromPer10kCells(14, map);

            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec;
                if (!this.TryFindScatterCell(map, out intVec))
                {
                    return;
                }
                this.ScatterAt(intVec, map, 1);
                this.usedSpots.Add(intVec);
            }
            this.usedSpots.Clear();
        }

        protected ThingDef ChooseThingDef()
        {
            return DefDatabase<ThingDef>.AllDefs.RandomElementByWeight((ThingDef def) => def.deepCommonality);
        }

        protected override bool CanScatterAt(IntVec3 c, Map map)
        {
            return !base.NearUsedSpot(c, this.minSpacing);
        }

        protected override void ScatterAt(IntVec3 c, Map map, int stackCount = 1)
        {
            ThingDef thingDef = this.ChooseThingDef();
            int numCells = Mathf.CeilToInt((float)thingDef.deepLumpSizeRange.RandomInRange);
            foreach (IntVec3 current in GridShapeMaker.IrregularLump(c, map, numCells))
            {
                if (!current.InNoBuildEdgeArea(map))
                {
                    map.deepResourceGrid.SetAt(current, thingDef, thingDef.deepCountPerCell);
                }
            }
        }
    }
}

