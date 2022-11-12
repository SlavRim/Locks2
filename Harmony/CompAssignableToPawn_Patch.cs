using HarmonyLib;
using Locks2.Core;
using RimWorld;

namespace Locks2.Harmony
{
    [HarmonyPatch(typeof(CompAssignableToPawn), nameof(CompAssignableToPawn.TryAssignPawn))]
    public class CompAssignableToPawn_TryAssignPawn_Patch
    {
        public static void Postfix(CompAssignableToPawn __instance)
        {
            if (__instance?.parent?.def?.IsBed ?? false)
            {
                __instance.parent.Map?.reachability?.ClearCache();
                LockConfig.Notify_Dirty();
            }
        }
    }

    [HarmonyPatch(typeof(CompAssignableToPawn), nameof(CompAssignableToPawn.TryUnassignPawn))]
    public class CompAssignableToPawn_TryUnassignPawn_Patch
    {
        public static void Postfix(CompAssignableToPawn __instance)
        {
            if (__instance?.parent?.def?.IsBed ?? false)
            {
                __instance.parent.Map?.reachability?.ClearCache();
                LockConfig.Notify_Dirty();
            }
        }
    }

    [HarmonyPatch(typeof(CompAssignableToPawn), nameof(CompAssignableToPawn.ForceAddPawn))]
    public class CompAssignableToPawn_ForceAddPawn_Patch
    {
        public static void Postfix(CompAssignableToPawn __instance)
        {
            if (__instance?.parent?.def?.IsBed ?? false)
            {
                __instance.parent.Map?.reachability?.ClearCache();
                LockConfig.Notify_Dirty();
            }
        }
    }

    [HarmonyPatch(typeof(CompAssignableToPawn), nameof(CompAssignableToPawn.ForceRemovePawn))]
    public class CompAssignableToPawn_ForceRemovePawn_Patch
    {
        public static void Postfix(CompAssignableToPawn __instance)
        {
            if (__instance?.parent?.def?.IsBed ?? false)
            {
                __instance.parent.Map?.reachability?.ClearCache();
                LockConfig.Notify_Dirty();
            }
        }
    }
}