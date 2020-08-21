using HarmonyLib;
using System;
using System.Reflection;
using Verse;

namespace BiomesIslands
{
    [StaticConstructorOnStartup]
    public static class BiomesIslands
    {
        public const string Id = "rimworld.biomes.islands";
        public const string Name = "Biomes! Islands";
        public static string Version = (Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute).InformationalVersion;
    
        static BiomesIslands()
        {
            new Harmony(Id).PatchAll();
            //HarmonyInstance.Create(Id).PatchAll();
            Log("Initialized");
        }

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));

        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";

        public static bool IsAquatic(this PawnKindDef kind)
        {
            if (kind.race.HasModExtension<Swimming.AquaticExtension>())
            {
                return kind.race.GetModExtension<Swimming.AquaticExtension>().aquatic;
            }
            return false;
        }
    }
}
