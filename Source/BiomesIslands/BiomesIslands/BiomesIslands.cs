using HarmonyLib;
using System;
using Verse;

namespace BiomesIslands
{
	[StaticConstructorOnStartup]
	public static class BiomesIslands
	{
		public const string Id = "rimworld.biomes.islands";
		public const string Name = "Biomes! Islands";
		private static readonly Version Version = typeof(BiomesIslands).Assembly.GetName().Version;

		static BiomesIslands()
		{
			new Harmony(Id).PatchAll();
			//HarmonyInstance.Create(Id).PatchAll();
			Log("Initialized");
		}

		public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));

		private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
	}
}