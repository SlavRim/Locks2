using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace Locks2.Core
{
    public partial class LockConfig
    {
        public class ConfigRuleHediff : IConfigRule
        {
            public bool enabled = true, allow = false;
            public HashSet<HediffDef> hediffs = new();
            readonly List<HediffDef> removal = new();

            public override float Height => (enabled ? hediffs.Count * 25 + 75f : 54) + 15 + 25;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Allows(Pawn pawn, ref bool force)
            {
                if (!enabled) return false;
                force = true;
                foreach (var hediff in hediffs)
                {
                    var has = pawn.health.hediffSet.HasHediff(hediff);
                    if (allow == has)
                        return true;
                }
                return false;
            }

            public override IConfigRule Duplicate()
            {
                return new ConfigRuleHediff { enabled = enabled, allow = allow, hediffs = hediffs };
            }
            static IEnumerable<HediffDef> HediffDefs => DefDatabase<HediffDef>.defsList;
            public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
               Action notifySelectionEnded)
            {
                var before = enabled;
                var defs = 
                Text.Font = GameFont.Small;
                Widgets.CheckboxLabeled(rect.TopPartPixels(25), "Locks2HediffsFilter".Translate(), ref enabled);
                Text.Font = GameFont.Tiny;
                if (enabled)
                {
                    rect.y += 25;
                    Widgets.CheckboxLabeled(rect.TopPartPixels(25), "Locks2HediffsFilterAllow".Translate(), ref allow);
                    Widgets.Label(rect.TopPartPixels(50).BottomPartPixels(25),
                        "Locks2HediffsFilterList".Translate());
                    var rowRect = rect.TopPartPixels(75).BottomPartPixels(25);
                    removal.Clear();
                    foreach (var hediff in hediffs)
                    {
                        if (Widgets.ButtonText(rowRect, hediff.LabelCap))
                        {
                            Notify_Dirty();
                            removal.Add(hediff);
                        }

                        rowRect.y += 25;
                    }
                    foreach (var hediff in removal) hediffs.Remove(hediff);

                    if (Widgets.ButtonText(rowRect, "+"))
                    {
                        Find.CurrentMap.reachability.ClearCache();
                        notifySelectionBegan.Invoke();
                        DoExtraContent(p =>
                        {
                            hediffs.Add(p);
                            Notify_Dirty();
                        }, HediffDefs.Where(p => !hediffs.Contains(p)), notifySelectionEnded);
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
                Scribe_Values.Look(ref enabled, nameof(enabled), true);
                Scribe_Values.Look(ref allow, nameof(allow), false);
                Scribe_Collections.Look(ref hediffs, nameof(hediffs));
                hediffs ??= new();
            }

            private void DoExtraContent(Action<HediffDef> onSelection, IEnumerable<HediffDef> defs, Action notifySelectionEnded)
            {
                ITab_Lock.currentSelector = new Selector_DefSelection(defs, def =>
                {
                    Find.CurrentMap.reachability.ClearCache();
                    onSelection(def as HediffDef);
                }, true, notifySelectionEnded);
            }
        }
    }
}