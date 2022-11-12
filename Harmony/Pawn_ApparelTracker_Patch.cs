using HarmonyLib;
using Locks2.Core;
using RimWorld;
using Verse;

namespace Locks2.Harmony
{
    public static class Pawn_ApparelTracker_Nofity_Changed
    {
        public static void ApparelChanged(Pawn pawn)
        {
            pawn.Notify_Dirty();
        }
    }

    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_ApparelAdded))]
    public static class Pawn_ApparelTracker_Notify_ApparelAdded_Patch
    {
        public static void Postfix(Pawn_ApparelTracker __instance)
        {
            Pawn_ApparelTracker_Nofity_Changed.ApparelChanged(__instance.pawn);
        }
    }

    [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_ApparelRemoved))]
    public static class Pawn_ApparelTracker_Notify_ApparelRemoved_Patch
    {
        public static void Postfix(Pawn_ApparelTracker __instance)
        {
            Pawn_ApparelTracker_Nofity_Changed.ApparelChanged(__instance.pawn);
        }
    }
}