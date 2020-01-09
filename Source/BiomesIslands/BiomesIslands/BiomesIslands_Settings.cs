using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;


namespace BiomesIslands
{
    public class BiomesIslands_Settings : ModSettings
    {
        public static bool flagSauronAtoll = false;


        public override void ExposeData()
        {
            Scribe_Values.Look(ref flagSauronAtoll, "flagSauronAtoll", false);
        }
    }
}

