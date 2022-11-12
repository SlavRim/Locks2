using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Locks2.Core
{
    public partial class LockConfig
    {
        public class ConfigRuleIgnorDrafted : IConfigRule
        {
            public bool enabled = true;

            public override float Height => 54;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Allows(Pawn pawn)
            {
                if (enabled && pawn.Drafted) return true;
                return false;
            }

            public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
                Action notifySelectionEnded)
            {
                var before = enabled;
                Widgets.CheckboxLabeled(rect, "Locks2DraftedPawns".Translate(), ref enabled);
                if (enabled != before) Notify_Dirty();
            }

            public override IConfigRule Duplicate()
            {
                return new ConfigRuleIgnorDrafted { enabled = enabled };
            }

            public override void ExposeData()
            {
                Scribe_Values.Look(ref enabled, "enabled", true);
            }
        }
    }
}