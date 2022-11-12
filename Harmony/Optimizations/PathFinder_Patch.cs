using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Locks2.Core;
using Verse;
using Verse.AI;

namespace Locks2.Harmony
{
    [HarmonyPatch(typeof(PathFinder), nameof(PathFinder.FindPath), typeof(IntVec3), typeof(LocalTargetInfo),
        typeof(TraverseParms), typeof(PathEndMode), typeof(PathFinderCostTuning))]
    public class PathFinder_FindPath_Patch
    {
        private const int MAX_FAILS = 3;
        private static readonly Dictionary<int, Pair<int, int>> cache = new Dictionary<int, Pair<int, int>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetKey(TraverseParms traverseParms, LocalTargetInfo dest)
        {
            int hash;
            unchecked
            {
                hash = traverseParms.pawn?.thingIDNumber.GetHashCode() ?? 0;
                hash = hash ^ (dest.Cell.GetHashCode() << 1);
            }
            return hash;
        }

        public static void Postfix(PawnPath __result, TraverseParms traverseParms, LocalTargetInfo dest)
        {
            if (__result != PawnPath.NotFound) return;
            if (traverseParms.pawn == null) return;
            var key = GetKey(traverseParms, dest);
            if (cache.TryGetValue(key, out var store) && GenTicks.TicksGame - store.Second < 2500)
            {
                if (store.First > MAX_FAILS)
                {
                    cache.Remove(key);
                    traverseParms.pawn.Map?.reachability?.ClearCacheFor(traverseParms.pawn);
                    LockConfig.Notify_Dirty();
                }
                else
                {
                    cache[key] = store = new(store.First + 1, GenTicks.TicksGame);
                }

                return;
            }
            cache[key] = new Pair<int, int>(1, GenTicks.TicksGame);
        }
    }
}