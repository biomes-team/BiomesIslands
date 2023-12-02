using System;
using BiomesCore.Planet;
using BiomesIslands.Planet;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesIslands.BiomeWorkers
{
	[HotSwappable]
	public class BiomeWorker_Atoll : BiomeWorker
	{
		public override float GetScore(Tile tile, int tileID)
		{
			if (tile.elevation > -100 || tile.temperature < 15 || tile.rainfall < 600)
			{
				// Atolls are islands. They require a minimum amount of temperature and rainfall.
				return BiomeWorker_Island.IgnoreMe;
			}

			const float atollFrequency = 0.005F;
			if (Rand.ChanceSeeded(1.0F - atollFrequency, Gen.HashCombineInt(WorldGenInfoHandler.WorldSeed, tileID)))
			{
				return BiomeWorker_Island.IgnoreMe;
			}

			float volcanicActivity = WorldGenInfoHandler.Get<WorldGenInfo_VolcanicActivity>().GetValue(tileID);
			// Atolls only appear in areas with low volcanic activity.
			volcanicActivity = Math.Min(0.4F, volcanicActivity);
			return 500.0F * volcanicActivity;
		}
	}
}