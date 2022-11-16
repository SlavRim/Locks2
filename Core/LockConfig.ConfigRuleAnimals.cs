using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Locks2.Core
{
    public partial class LockConfig
    {
        public class ConfigRuleAnimals : IConfigRule
        {
            public int ageFilter = 1;
            public bool ageFilterEnabled;
            public Gender allowedGender = Gender.Female;

            private string buffer = "1";
            public bool enabled = true;
            public bool genderFilterEnabled;
            public bool disallowPen = true;

            public override float Height =>
                (enabled ? 25 + 25 + (genderFilterEnabled ? 50 : 25) + (ageFilterEnabled ? 50 : 25) : 54) + 15;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Allows(Pawn pawn)
            {
                if (!enabled) return false;
                if (pawn is not { Faction.IsPlayer: true }) return false;
                var isAnimal = pawn.RaceProps.Animal;
                if (disallowPen && AnimalPenUtility.GetCurrentPenOf(pawn, true) is not null)
                    return false;
                if (isAnimal)
                {
                    if (genderFilterEnabled && pawn.gender != allowedGender) return false;
                    if (ageFilterEnabled && pawn.ageTracker.AgeBiologicalYearsFloat > ageFilter) return false;
                    return true;
                }
                return false;
            }

            public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
                Action notifySelectionEnded)
            {
                var before = enabled;
                Widgets.CheckboxLabeled(rect.TopPartPixels(enabled ? 25 : 54), "Locks2Animals".Translate(), ref enabled);
                var offset = new Vector2(0, 25);
                if (enabled)
                {
                    rect = rect.TopPartPixels(20);
                    rect.position += offset;
                    rect.size = new Vector2(rect.size.x, 25);
                    Text.Font = GameFont.Tiny;
                    Widgets.CheckboxLabeled(rect, "Locks2AnimalsGenderFilter".Translate() + " " + allowedGender, ref genderFilterEnabled);

                    rect.position += offset;
                    Widgets.CheckboxLabeled(rect, "Locks2AnimalsDisallowPen".Translate(), ref disallowPen);

                    if (genderFilterEnabled)
                    {
                        Text.Font = GameFont.Small;
                        rect.position += offset;
                        if (Widgets.ButtonText(rect.RightHalf(), "Locks2AnimalsGenderMale".Translate()))
                        {
                            allowedGender = Gender.Male;
                            Notify_Dirty();
                        }

                        if (Widgets.ButtonText(rect.LeftHalf(), "Locks2AnimalsGenderFemale".Translate()))
                        {
                            allowedGender = Gender.Female;
                            Notify_Dirty();
                        }
                    }

                    Text.Font = GameFont.Tiny;
                    rect.position += offset;
                    Widgets.CheckboxLabeled(rect, "Locks2AnimalsAgeFilter".Translate(), ref ageFilterEnabled);
                    if (ageFilterEnabled)
                    {
                        Text.Font = GameFont.Small;
                        rect.position += offset;
                        Widgets.TextFieldNumeric(rect, ref ageFilter, ref buffer, 0, 20);
                    }
                }

                if (before != enabled) Notify_Dirty();
            }

            public override IConfigRule Duplicate()
            {
                return new ConfigRuleAnimals
                {
                    enabled = enabled,
                    ageFilter = ageFilter,
                    ageFilterEnabled = ageFilterEnabled,
                    genderFilterEnabled = genderFilterEnabled,
                    allowedGender = allowedGender,
                    disallowPen = disallowPen
                };
            }

            public override void ExposeData()
            {
                Scribe_Values.Look(ref enabled, "enabled", true);
                Scribe_Values.Look(ref genderFilterEnabled, "genderFilterEnabled");
                Scribe_Values.Look(ref allowedGender, "allowedGender");
                Scribe_Values.Look(ref ageFilterEnabled, "ageFilterEnabled");
                Scribe_Values.Look(ref ageFilter, "ageFilter", 1);
                Scribe_Values.Look(ref disallowPen, "disallowPen", true);
            }
        }
    }
}