using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Locks2.Core;
using RimWorld;
using Verse;

namespace Locks2.Harmony
{
    [HarmonyPatch(typeof(Building_Door), nameof(Building_Door.PawnCanOpen))]
    internal static class Building_Door_PawnCanOpen_Patch
    {
        [HarmonyPriority(int.MaxValue)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Prefix(Building_Door __instance, Pawn p, ref bool __result)
        {
            if (__instance.Faction == null)
            {
                __result = true;
                return false;
            }
            if (!(__instance.Map?.IsPlayerHome ?? false) || p == null || (p.roping?.IsRopedByPawn ?? false)) return true;
            var config = Finder.currentConfig = __instance.GetConfig();
            if (config == null) return true;
            if (config.Allows(p))
                __result = true;
            else
                __result = false;
            return false;
        }
    }

    [HarmonyPatch]
    internal static class Building_Door_Expanded_PawnCanOpen_Patch
    {
        private static bool Prepare()
        {
            return AccessTools.Method("Building_DoorExpanded:PawnCanOpen") != null;
        }

        private static MethodBase TargetMethod()
        {
            return AccessTools.Method("Building_DoorExpanded:PawnCanOpen");
        }

        [HarmonyPriority(int.MaxValue)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Prefix(Building __instance, Pawn p, ref bool __result)
        {
            if (__instance.Faction == null)
            {
                __result = true;
                return false;
            }

            if (!(__instance.Map?.IsPlayerHome ?? false) || p == null) return true;
            var config = Finder.currentConfig = __instance.GetConfig();
            if (config == null) return true;
            if (config.Allows(p))
                __result = true;
            else
                __result = false;
            return false;
        }
    }
}