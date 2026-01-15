using BiomesCore.Planet;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace BiomesIslands.Planet
{
	/// <summary>
	/// Simulates areas with volcanic activity in the world. Negative values have no volcanic activity. Positive values
	/// indicate increasing amounts of activity.
	/// </summary>
	
	/*

	[HotSwappable]
	public class WorldGenInfo_VolcanicActivity : WorldGenInfo
	{
		private ModuleBase _volcanicActivityNoise;

		protected override void Setup()
		{
			int worldSeed = WorldGenInfoHandler.WorldSeed;

			// Defines volcanic areas which form long circular lines and smaller circles on the world map.
			ModuleBase volcanicArea = new RidgedMultifractal(0.02, 12.0, 18, Gen.HashCombineInt(worldSeed, 1786664917),
				QualityMode.High);
			_volcanicActivityNoise = new Multiply(volcanicArea, new Filter(volcanicArea, 0.04F, 1.0F));
			NoiseDebugUI.StorePlanetNoise(_volcanicActivityNoise, "BMT_Islands_VolcanicActivityNoise");
		}

		protected override float GenerateTileData(Tile tile, int tileID, Vector3 tileCenter)
		{
			return _volcanicActivityNoise.GetValue(tileCenter);
		}
	}

	*/
}