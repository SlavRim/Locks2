using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace Locks2.Core
{
    public partial class LockConfig
    {
        public class ConfigRuleMechs : IConfigRule
        {
            public bool enabled = true;

            public override float Height => 54;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Allows(Pawn pawn)
            {
                if (!enabled) return false;
#if v1_4
                if (pawn.IsColonyMech) return true;
#endif
                return pawn.RaceProps.IsMechanoid && pawn.Faction.IsPlayer;
            }

            public override IConfigRule Duplicate()
            {
                return new ConfigRuleMechs { enabled = enabled };
            }

            public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
                Action notifySelectionEnded)
            {
                var before = enabled;
                Widgets.CheckboxLabeled(rect, "Locks2MechsFilter".Translate(), ref enabled);
                if (before != enabled) Notify_Dirty();
            }

            public override void ExposeData()
            {
                Scribe_Values.Look(ref enabled, "enabled", true);
            }
        }
    }
}