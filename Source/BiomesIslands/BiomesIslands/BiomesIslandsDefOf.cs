using Verse;
using RimWorld;

namespace BiomesIslands
{
	[DefOf]
	public static class BiomesIslandsDefOf
	{
		public static BiomeDef BMT_Atoll;
		public static GameConditionDef GlowTide;
		public static PawnKindDef BMT_RimCrab;
		public static SongDef Biomes_LobsterRave;
		public static ThingDef BMT_CoralRock;
		public static ThingDef BMT_GlowPlankton;
		public static ThoughtDef BiomesIslands_Crab;

		static BiomesIslandsDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(BiomesIslandsDefOf));
		}
	}
}