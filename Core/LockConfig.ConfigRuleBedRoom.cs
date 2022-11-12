using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace Locks2.Core
{
    public partial class LockConfig
    {
        public class ConfigRuleBedRoom : IConfigRule
        {
            public bool enabled = true;

            public override float Height => 64;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Allows(Pawn pawn)
            {
                if (!enabled) return false;
                var door = Finder.currentConfig?.door;
                if (door == null) return false;
                var region = door.GetRegion();
                var pawnRoom = pawn.GetRoom();
                foreach (var other in region.Neighbors)
                {
                    var room = other.Room;
                    if (room == null) continue;
                    if (room.Role != RoomRoleDefOf.Bedroom && room.Role != RoomRoleDefOf.Barracks) continue;
                    var beds = room.ContainedBeds;
                    if (beds.Count() > 0 && pawnRoom == room) return true;
                    foreach (var bed in beds)
                    {
                        var owners = bed.OwnersForReading;
                        if (owners.Contains(pawn)) return true;
                        if (owners.Count < bed.SleepingSlotsCount) return true;
                    }
                }

                return false;
            }

            public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
                Action notifySelectionEnded)
            {
                var before = enabled;
                Widgets.CheckboxLabeled(rect.TopPartPixels(25), "Locks2BedRoomFilter".Translate(), ref enabled);
                var font = Text.Font;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect.BottomPartPixels(35), "Locks2BedRoomFilterHint".Translate());
                if (before != enabled) Notify_Dirty();

                Text.Font = font;
            }

            public override IConfigRule Duplicate()
            {
                return new ConfigRuleBedRoom { enabled = enabled };
            }

            public override void ExposeData()
            {
                Scribe_Values.Look(ref enabled, "enabled", true);
            }
        }
    }
}