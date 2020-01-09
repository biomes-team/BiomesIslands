using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BiomesIslands
{
    public class BiomesIslands_IslandsMod : Mod
    {
        BiomesIslands_Settings settings;

        public BiomesIslands_IslandsMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<BiomesIslands_Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            inRect.width = 450f;
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("BiomesIslands_SauronAtollLabel".Translate(), ref BiomesIslands_Settings.flagSauronAtoll);

            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "BiomesIslands_ModName".Translate();
        }

    }
}
