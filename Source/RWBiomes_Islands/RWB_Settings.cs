using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;


namespace RWBiomes_Islands
{
    public class RWBIslands_Settings : ModSettings
    {
        public static bool flagSauronAtoll = false;


        public override void ExposeData()
        {
            Scribe_Values.Look(ref flagSauronAtoll, "flagSauronAtoll", false);
        }
    }

    public class RWB_IslandsMod : Mod
    {
        RWBIslands_Settings settings;

        public RWB_IslandsMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<RWBIslands_Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {

            inRect.width = 450f;
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("RWBIslands_SauronAtollLabel".Translate(), ref RWBIslands_Settings.flagSauronAtoll);

            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "RWBIslands_ModName".Translate();
        }

    }
}

