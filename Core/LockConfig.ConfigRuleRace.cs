using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Locks2.Core
{
    public partial class LockConfig
    {
        public class ConfigRuleRace : ConfigRuleGuests
        {
            private static IEnumerable<ThingDef> racesDefs;

            private readonly List<ThingDef> removalKinds = new List<ThingDef>();
            public HashSet<ThingDef> whiteSet = new HashSet<ThingDef>();

            public override float Height => (enabled ? whiteSet.Count * 25 + 75f : 54) + 15;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Allows(Pawn pawn)
            {
                if (enabled && whiteSet.Contains(pawn.def) && (pawn.IsColonist || base.Allows(pawn))) return true;
                return false;
            }

            public override IConfigRule Duplicate()
            {
                return new ConfigRuleRace { enabled = enabled, whiteSet = new HashSet<ThingDef>(whiteSet) };
            }

            public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
                Action notifySelectionEnded)
            {
                var before = enabled;
                if (racesDefs == null) racesDefs = DefDatabase<ThingDef>.AllDefs.Where(def => def.race != null);
                Text.Font = GameFont.Small;
                Widgets.CheckboxLabeled(rect.TopPartPixels(25), "Locks2RaceFilter".Translate(), ref enabled);
                Text.Font = GameFont.Tiny;
                if (enabled)
                {
                    Widgets.Label(rect.TopPartPixels(50).BottomPartPixels(25), "Locks2RaceFilterWhitelist".Translate());
                    var rowRect = rect.TopPartPixels(75).BottomPartPixels(25);
                    removalKinds.Clear();
                    foreach (var def in whiteSet)
                    {
                        if (Widgets.ButtonText(rowRect, def.label))
                        {
                            Notify_Dirty();
                            removalKinds.Add(def);
                        }

                        rowRect.y += 25;
                    }

                    foreach (var def in removalKinds) whiteSet.Remove(def);
                    if (Widgets.ButtonText(rowRect, "+"))
                    {
                        notifySelectionBegan();
                        DoExtraContent(def =>
                        {
                            Notify_Dirty();
                            whiteSet.Add(def as ThingDef);
                        }, racesDefs.Where(def => !whiteSet.Contains(def)), notifySelectionEnded);
                    }
                }

                if (before != enabled)
                {
                    Notify_Dirty();
                    Find.CurrentMap.reachability.ClearCache();
                }
            }

            public override void ExposeData()
            {
                base.ExposeData();
                Scribe_Collections.Look(ref whiteSet, "whiteset", LookMode.Def);
                if (whiteSet == null) whiteSet = new HashSet<ThingDef>();
            }

            private void DoExtraContent(Action<Def> onSelection, IEnumerable<ThingDef> defs,
                Action notifySelectionEnded)
            {
                ITab_Lock.currentSelector = new Selector_DefSelection(defs, onSelection, true, notifySelectionEnded);
            }
        }
    }
}