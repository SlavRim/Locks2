using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace Locks2.Core
{
    public partial class LockConfig
    {
        public class ConfigRulePrisoners : IConfigRule
        {
            public bool enabled;

            public override float Height => 54;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Allows(Pawn pawn)
            {
                if (enabled && pawn.IsPrisoner && pawn.HostFaction == Faction.OfPlayer) return true;
                return false;
            }

            public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
                Action notifySelectionEnded)
            {
                var before = enabled;
                Widgets.CheckboxLabeled(rect, "Locks2PrisonersFilter".Translate(), ref enabled);
                if (before != enabled) Notify_Dirty();
            }

            public override IConfigRule Duplicate()
            {
                return new ConfigRulePrisoners { enabled = enabled };
            }

            public override void ExposeData()
            {
                Scribe_Values.Look(ref enabled, "enabled", true);
            }
        }
    }
}