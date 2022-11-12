using Locks2.Core;
using Verse;

namespace Locks2
{
    [StaticConstructorOnStartup]
    public static class Finder
    {
        public const string PackageID = "krkr.locks2";
        public static bool debug = false;

        public static HarmonyLib.Harmony harmony = new HarmonyLib.Harmony(PackageID);

        public static Settings settings;
        public static int clipThingId = -1;
        public static LockConfig clip;
        public static LockConfig currentConfig;

        static Finder()
        {
            harmony.PatchAll();
        }
    }
}