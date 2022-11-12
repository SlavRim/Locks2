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
        public class ConfigRuleGameCondition : IConfigRule
        {
            public bool enabled;

            public HashSet<GameConditionDef> conditionsSet = new HashSet<GameConditionDef>();

            public List<GameConditionDef> removalSet = new List<GameConditionDef>();

            public override float Height => (enabled ? 75 + conditionsSet.Count * 25 : 54) + 15;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Allows(Pawn pawn)
            {
                if (enabled && !AnyConditionActive(pawn.Map) && !pawn.IsPrisoner &&
                    !(pawn.IsWildMan() && pawn.Faction == null)) return true;
                return false;
            }

            public override IConfigRule Duplicate()
            {
                return new ConfigRuleGameCondition { enabled = enabled, conditionsSet = new HashSet<GameConditionDef>(conditionsSet) };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool AnyConditionActive(Map map)
            {
                var conditionManager = map.gameConditionManager;
                foreach (var def in conditionsSet)
                {
                    if (conditionManager.ConditionIsActive(def))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
                Action notifySelectionEnded)
            {
                var before = enabled;
                Text.Font = GameFont.Small;
                Widgets.CheckboxLabeled(rect.TopPartPixels(25), "Locks2GameConditionFilter".Translate(), ref enabled);
                Text.Font = GameFont.Tiny;
                if (enabled)
                {
                    Widgets.Label(rect.TopPartPixels(50).BottomPartPixels(25), "Locks2DenyIf".Translate());
                    var rowRect = rect.TopPartPixels(75).BottomPartPixels(25);
                    removalSet.Clear();
                    foreach (var def in conditionsSet)
                    {
                        if (Widgets.ButtonText(rowRect, def.label))
                        {
                            Notify_Dirty();
                            removalSet.Add(def);
                        }

                        rowRect.y += 25;
                    }

                    foreach (var def in removalSet)
                    {
                        Find.CurrentMap.reachability.ClearCache();
                        conditionsSet.Remove(def);
                    }

                    if (Widgets.ButtonText(rowRect, "+"))
                    {
                        notifySelectionBegan();
                        DoExtraContent(def =>
                            {
                                Notify_Dirty();
                                conditionsSet.Add(def);
                                Find.CurrentMap.reachability.ClearCache();
                            }, DefDatabase<GameConditionDef>.AllDefs.Where(def => !conditionsSet.Contains(def)),
                            notifySelectionEnded);
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
                Scribe_Collections.Look(ref conditionsSet, "conditionsSet", LookMode.Def);
            }

            private void DoExtraContent(Action<GameConditionDef> onSelection, IEnumerable<GameConditionDef> conditions,
                Action notifySelectionEnded)
            {
                ITab_Lock.currentSelector = new Selector_DefSelection(conditions,
                    def => onSelection(def as GameConditionDef), true, notifySelectionEnded);
            }
        }
    }
}