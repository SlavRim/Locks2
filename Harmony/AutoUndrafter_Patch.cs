using HarmonyLib;
using Locks2.Core;
using RimWorld;

namespace Locks2.Harmony
{
    [HarmonyPatch(typeof(AutoUndrafter), nameof(AutoUndrafter.Notify_Drafted))]
    public static class AutoUndrafter_Notify_Drafted_Patch
    {
        public static void Postfix(AutoUndrafter __instance)
        {
            Traverse.Create(__instance).Field<Pawn>("pawn").Value.Notify_Dirty();
        }
    }
}