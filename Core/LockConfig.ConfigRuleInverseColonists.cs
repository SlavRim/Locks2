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
        public class ConfigRuleInverseColonists : IConfigRule
        {
            public bool enabled;

            private readonly List<Pawn> removalPawns = new List<Pawn>();

            public HashSet<Pawn> whiteSet = new HashSet<Pawn>();

            public override float Height => (enabled ? whiteSet.Count * 25 + 75f : 54) + 15;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Allows(Pawn pawn)
            {
                if (enabled && (pawn?.RaceProps?.Humanlike ?? false) && (pawn.Faction?.IsPlayer ?? false) &&
                    whiteSet.Contains(pawn) && !pawn.IsPrisoner) return true;
                return false;
            }

            public override IConfigRule Duplicate()
            {
                return new ConfigRuleInverseColonists { enabled = enabled, whiteSet = new HashSet<Pawn>(whiteSet) };
            }

            public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
                Action notifySelectionEnded)
            {
                var before = enabled;
                Text.Font = GameFont.Small;
                Widgets.CheckboxLabeled(rect.TopPartPixels(25), "Locks2ColonistInvertedFilter".Translate(),
                    ref enabled);
                Text.Font = GameFont.Tiny;
                if (enabled)
                {
                    Widgets.Label(rect.TopPartPixels(50).BottomPartPixels(25),
                        "Locks2ColonistInvertedFilterWhitelist".Translate());
                    var rowRect = rect.TopPartPixels(75).BottomPartPixels(25);
                    removalPawns.Clear();
                    foreach (var pawn in whiteSet)
                    {
                        if (Widgets.ButtonText(rowRect, pawn.Name.ToString()))
                        {
                            Find.CurrentMap.reachability.ClearCache();
                            removalPawns.Add(pawn);
                        }

                        rowRect.y += 25;
                    }

                    foreach (var pawn in removalPawns)
                    {
                        Find.CurrentMap.reachability.ClearCache();
                        whiteSet.Remove(pawn);
                    }

                    if (Widgets.ButtonText(rowRect, "+"))
                    {
                        Find.CurrentMap.reachability.ClearCache();
                        notifySelectionBegan();
                        DoExtraContent(p =>
                        {
                            whiteSet.Add(p);
                            Find.CurrentMap.reachability.ClearCache();
                        }, pawns.Where(p => !whiteSet.Contains(p)), notifySelectionEnded);
                    }
                }

                if (before != enabled) Notify_Dirty();
            }

            public override void ExposeData()
            {
                Scribe_Values.Look(ref enabled, "enabled", true);
                if (Scribe.mode == LoadSaveMode.Saving) whiteSet.RemoveWhere(p => p == null || p.Destroyed || p.Dead);
                Scribe_Collections.Look(ref whiteSet, "whitset", LookMode.Reference);
                if (whiteSet == null) whiteSet = new HashSet<Pawn>();
            }

            private void DoExtraContent(Action<Pawn> onSelection, IEnumerable<Pawn> pawns, Action notifySelectionEnded)
            {
                ITab_Lock.currentSelector = new Selector_PawnSelection(pawns, onSelection, true, notifySelectionEnded);
            }
        }
    }
}