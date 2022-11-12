using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Locks2.Core
{
    public class LockComp : ThingComp
    {
        public LockConfig config;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref config, "conifg");
            if (config != null && config.door == null) config.door = parent;
            if (config?.rules?.Count == 0) config.Initailize();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (config.door != parent)
            {
                config = new LockConfig { door = parent };
                config.Initailize();
            }
            if (parent.Faction != Faction.OfPlayer)
            {
            }
            else
            {
                if (Find.Selector.SelectedObjects.FirstOrDefault() is Building building)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "copy",
                        defaultDesc = "Copy current lock configuration",
                        activateSound = SoundDefOf.Designate_Claim,
                        icon = TexButton.Copy,
                        action = () =>
                        {
                            var config = building?.GetConfig();
                            Finder.clip = config;
                            Finder.clipThingId = parent.thingIDNumber;
                        }
                    };
                }
                if (Finder.clip != null && Finder.clip != config && Finder.clipThingId != parent.thingIDNumber)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Paste",
                        defaultDesc = "Paste lock configuration",
                        activateSound = SoundDefOf.Designate_Claim,
                        icon = TexButton.Paste,
                        action = () =>
                        {
                            foreach (Thing thing in Find.Selector.SelectedObjects)
                            {
                                if (!(thing is Building door)) continue;
                                var config = door.GetConfig();
                                if (config == null || config == Finder.clip) continue;
                                config.rules.Clear();
                                foreach (var rule in Finder.clip.rules) config.rules.Add(rule.Duplicate());
                            }
                        }
                    };
                }
            }
        }
    }
}