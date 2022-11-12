﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Locks2.Core
{
    public partial class LockConfig
    {
        public class ConfigRuleColonists : IConfigRule
        {
            public HashSet<Pawn> blackSet = new HashSet<Pawn>();
            public bool enabled = true;

            private readonly List<Pawn> removalPawns = new List<Pawn>();

            public override float Height => (enabled ? blackSet.Count * 25 + 75f : 54) + 15;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Allows(Pawn pawn)
            {
                return enabled && pawn.IsColonist && !blackSet.Contains(pawn) && !pawn.IsPrisoner;
            }

            public override IConfigRule Duplicate()
            {
                return new ConfigRuleColonists { enabled = enabled, blackSet = new HashSet<Pawn>(blackSet) };
            }

            public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
                Action notifySelectionEnded)
            {
                var before = enabled;
                Text.Font = GameFont.Small;
                Widgets.CheckboxLabeled(rect.TopPartPixels(25), "Locks2ColonistsFilter".Translate(), ref enabled);
                Text.Font = GameFont.Tiny;
                if (enabled)
                {
                    Widgets.Label(rect.TopPartPixels(50).BottomPartPixels(25),
                        "Locks2ColonistsFilterBlacklist".Translate());
                    var rowRect = rect.TopPartPixels(75).BottomPartPixels(25);
                    removalPawns.Clear();
                    foreach (var pawn in blackSet)
                    {
                        if (Widgets.ButtonText(rowRect, pawn.Name.ToString()))
                        {
                            Notify_Dirty();
                            removalPawns.Add(pawn);
                        }

                        rowRect.y += 25;
                    }
                    foreach (var pawn in removalPawns) blackSet.Remove(pawn);
                    if (Widgets.ButtonText(rowRect, "+"))
                    {
                        Find.CurrentMap.reachability.ClearCache();
                        notifySelectionBegan.Invoke();
                        DoExtraContent(p =>
                        {
                            blackSet.Add(p);
                            Notify_Dirty();
                        }, pawns.Where(p => !blackSet.Contains(p)), notifySelectionEnded);
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
                Scribe_Values.Look(ref enabled, "enabled", true);
                if (Scribe.mode == LoadSaveMode.Saving) blackSet.RemoveWhere(p => p == null || p.Destroyed || p.Dead);
                Scribe_Collections.Look(ref blackSet, "blackset", LookMode.Reference);
                if (blackSet == null) blackSet = new HashSet<Pawn>();
            }

            private void DoExtraContent(Action<Pawn> onSelection, IEnumerable<Pawn> pawns, Action notifySelectionEnded)
            {
                ITab_Lock.currentSelector = new Selector_PawnSelection(pawns, pawn =>
                {
                    Find.CurrentMap.reachability.ClearCache();
                    onSelection(pawn);
                }, true, notifySelectionEnded);
            }
        }
    }
}