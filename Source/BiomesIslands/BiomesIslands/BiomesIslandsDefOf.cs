using Verse;
using RimWorld;

namespace BiomesIslands
{
	[DefOf]
	public static class BiomesIslandsDefOf
	{
		public static BiomeDef BiomesIslands_Atoll;
		public static GameConditionDef GlowTide;
		public static PawnKindDef BiomesIslands_RimCrab;
		public static SongDef Biomes_LobsterRave;
		public static ThingDef BiomesIslands_CoralRock;
		public static ThingDef BiomesIslands_GlowPlankton;
		public static ThoughtDef BiomesIslands_Crab;

		static BiomesIslandsDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(BiomesIslandsDefOf));
		}
	}
}