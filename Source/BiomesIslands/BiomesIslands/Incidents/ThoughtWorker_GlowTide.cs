using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
