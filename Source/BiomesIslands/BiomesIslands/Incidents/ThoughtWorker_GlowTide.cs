using RimWorld;
using Verse;

namespace BiomesIslands.Incidents
{
    public class ThoughtWorker_GlowTide : ThoughtWorker_GameCondition
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return base.CurrentStateInternal(p).Active && p.SpawnedOrAnyParentSpawned && p.GetRoom()?.PsychologicallyOutdoors == true && p.health.capacities.CapableOf(PawnCapacityDefOf.Sight);
		}
	}
}
