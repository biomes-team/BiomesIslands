using HarmonyLib;
using System;
using BiomesCore.Planet;
using BiomesIslands.Planet;
using Verse;

namespace BiomesIslands
{
	public class BiomesIslands : Mod
	{
		public const string Id = "rimworld.biomes.islands";
		public const string Name = "Biomes! Islands";
		private static readonly Version Version = typeof(BiomesIslands).Assembly.GetName().Version;

		public BiomesIslands(ModContentPack content) : base(content)
		{
			new Harmony(Id).PatchAll();
			//HarmonyInstance.Create(Id).PatchAll();
			// Register WorldGenInfos in Biomes! Core.
			//WorldGenInfoHandler.Register<WorldGenInfo_IslandPresence>();
			//WorldGenInfoHandler.Register<WorldGenInfo_VolcanicActivity>();
			Log("Initialized");
		}

		public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));

		private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
	}
}