using BiomesCore.Planet;
using BiomesIslands.BiomeWorkers;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace BiomesIslands.Planet
{
	/// <summary>
	/// Random value for each world map tile. The perlin noise generating the value is distributed to have solitary
	/// peaks always surrounded by valleys. Peaks tend to appear in clusters. This value is either 0 or 1.
	/// </summary>
	[HotSwappable]
	public class WorldGenInfo_IslandPresence : WorldGenInfo
	{
		/// <summary>
		/// Responsible for clustering islands together.
		/// </summary>
		private ModuleBase _clusterNoise;

		/// <summary>
		/// Determines island positions near the mainland.
		/// </summary>
		private ModuleBase _elevationNoise;

		/// <summary>
		/// Determines positions of distant islands.
		/// </summary>
		private ModuleBase _distantNoise;

		protected override void Setup()
		{
			int worldSeed = WorldGenInfoHandler.WorldSeed;
			_clusterNoise = new Perlin(0.55, 2.0, 0.4, 6, Gen.HashCombineInt(worldSeed, -1214663760), QualityMode.High);
			_elevationNoise = new Perlin(0.15, 2.0, 0.4, 6, Gen.HashCombineInt(worldSeed, -935172814), QualityMode.High);
			_distantNoise = new Perlin(0.55, 2.0, 0.4, 6, Gen.HashCombineInt(worldSeed, 1011617332), QualityMode.High);
		}

		private bool CanHaveIsland(Tile tile, int tileID, Vector3 tileCenter)
		{
			// Determines how many islands end up being present on the map.
			const float islandFrequency = 0.2F;
			// Makes islands cluster together. Also affects island frequency.
			const float clusterThreshold = 0.4F;

			if (
				!tile.WaterCovered ||
				// Random chance used to prevent islands from spawning next to each other.
				!Rand.ChanceSeeded(islandFrequency, Gen.HashCombineInt(WorldGenInfoHandler.WorldSeed, tileID)) ||
				// Prefer spawning on clusters.
				_clusterNoise.GetValue(tileCenter) < clusterThreshold ||
				// Filter out any tiles in which Sea Ice could potentially spawn. See BiomeWorker_SeaIce for details.
				BiomeWorkerUtil.IsPermanentIce(tile))
			{
				return false;
			}

			const float elevationMean = -150.0F;
			const float elevationVariance = 40.0F;
			const float elevationWidth = 25.0F;

			float chainElev = elevationMean + _elevationNoise.GetValue(tileCenter) * elevationVariance;
			float elevation = WorldGenInfoHandler.NoiseElevation.GetValue(tileCenter);

			if (elevation > chainElev - elevationWidth && elevation < chainElev + elevationWidth)
			{
				return true;
			}

			// Islands that can appear far away from the mainland.
			const float distantThreshold = 0.95F;
			return _distantNoise.GetValue(tileCenter) > distantThreshold;
		}

		protected override float GenerateTileData(Tile tile, int tileID, Vector3 tileCenter)
		{
			if (!CanHaveIsland(tile, tileID, tileCenter))
			{
				return -1.0F;
			}

			const float archipelagoFrequency = 0.1F;
			return Rand.ChanceSeeded(archipelagoFrequency, Gen.HashCombineInt(WorldGenInfoHandler.WorldSeed, tileID))
				? 1.0F
				: 0.0F;
		}

		public static bool AllowsIsland(float value)
		{
			return value > -0.5F;
		}

		public static bool IsArchipelago(float value)
		{
			return value > 0.5F;
		}
	}
}