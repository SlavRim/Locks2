using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using static Locks2.Core.LockConfig;

namespace Locks2.Core
{
    public class ITab_Lock : ITab
    {
        private const int debugInfoSize = 100;

        private static LockConfig currentClip;
        private static readonly Vector2 offset = new Vector2(0, 25);
        private static readonly Color selectionOverlay = new Color(0, 0, 0, 0.4f);

        public static ISelector currentSelector;
        public static Rect currentRightRect;

        private LockConfig config;
        private Building door;
        private bool expanded;
        private IConfigRule expandedRule;
        private Rect inRect;
        private readonly HashSet<IConfigRule> removalSet = new HashSet<IConfigRule>();
        private Vector2 scrollPosition = Vector2.zero;
        private Rect viewRect = Rect.zero;

        public ITab_Lock()
        {
            size = new Vector2(Finder.settings.tabSizeX, Finder.settings.tabSizeY);
            inRect = new Rect(offset, size - offset);
            inRect = inRect.ContractedBy(5);
            labelKey = "lockTab";
            tutorTag = "locks2Tab";
        }

        public override bool IsVisible => (SelThing as Building).GetConfig() != null && SelThing.Map.IsPlayerHome &&
                                          (SelThing.Faction?.IsPlayer ?? false);

        protected override bool StillValid => base.StillValid;

        public float PawnSectionHeight
        {
            get
            {
                var height = 0f;
                foreach (var rule in config.rules) height += rule.Height;
                return height + 100 + 54 * config.rules.Count + 30;
            }
        }

        public IEnumerable<Pawn> Pawns
        {
            get
            {
                var pawns = door.Map.mapPawns.FreeColonists;
                pawns.AddRange(door.Map.mapPawns.PrisonersOfColony.AsEnumerable());
                return new HashSet<Pawn>(pawns).AsEnumerable();
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            door = SelThing as Building;
            config = door.GetConfig();
            ResetRightPanel();
        }

        protected override void CloseTab()
        {
            base.CloseTab();
            var map = SelThing.Map;
            currentSelector = null;
            map.reachability.ClearCache();
            config.Dirty();
            ResetRightPanel();
        }

        protected override void UpdateSize()
        {
            base.UpdateSize();
            size = new Vector2(Finder.settings.tabSizeX, Finder.settings.tabSizeY);
            inRect = new Rect(offset, size - offset);
            inRect = inRect.ContractedBy(5);
            if (expanded) size.x *= 2;
            currentRightRect = inRect;
            if (expanded)
            {
                currentRightRect.x += inRect.width;
                currentRightRect.width = inRect.width;
                currentRightRect.height = inRect.height;
            }
        }

        protected override void FillTab()
        {
            GameFont font = Text.Font;
            UpdateSize();
            if (!expanded) ResetRightPanel();
            if (SelThing is Building temp && (temp.GetConfig() != config || !Find.Selector.SelectedObjects.Contains(door)))
            {
                CloseTab();
                ResetRightPanel();
                foreach (var window in Find.WindowStack.Windows)
                {
                    var type = window.GetType();
                    if (false
                        || type == typeof(Selector_DefSelection)
                        || type == typeof(Selector_PawnSelection)
                        || type == typeof(Selector_RuleSelection))
                        window.Close();
                }
                return;
            }

            config.Dirty();
            inRect.yMin += 10;
            Rect contentRect = inRect.AtZero();
            Rect headerRect = inRect.TopPartPixels(30);
            contentRect.width -= 20;
            contentRect.height = config.rules.Sum(t => t.Height) + (config.rules.Count() - 1) * 25;

            Text.Font = GameFont.Medium;
            Widgets.Label(headerRect, "Locks2DoorSettings".Translate());
            Text.Font = GameFont.Tiny;
            Rect sRect = headerRect.RightPartPixels(18);
            if (Widgets.ButtonImageFitted(sRect, TexButton.Plus))
            {
                ResetRightPanel();
                expanded = true;
                currentSelector = new Selector_RuleSelection(rule => config.rules.Add(rule), true, () => { expanded = false; });
            }
            sRect.x -= 18;
            if (Widgets.ButtonImageFitted(sRect, TexButton.Copy))
            {
                ResetRightPanel();
                Finder.clip = config;
                Finder.clipThingId = this.SelThing?.thingIDNumber ?? -1;
                currentClip = config;
            }
            if (currentClip != config && currentClip != null)
            {
                sRect.x -= 18;
                if (Widgets.ButtonImageFitted(sRect, TexButton.Paste))
                {
                    config.rules.Clear();
                    foreach (var rule in currentClip.rules)
                        config.rules.Add(rule.Duplicate());
                }
            }
            sRect.x -= 36;
            sRect.width = 36;
            if (Widgets.ButtonText(sRect.TopPartPixels(25).BottomPartPixels(20), "Locks2SettingReset".Translate()))
                Find.WindowStack.Add(new Window_Confirm(() =>
                {
                    ResetRightPanel();
                    config.rules.Clear();
                    config.Initailize();
                }, () =>
                {
                    ResetRightPanel();
                }));
            inRect.yMin += 40;
            Widgets.BeginScrollView(inRect, ref scrollPosition, contentRect);

            FillRules(contentRect);

            Widgets.EndScrollView();
            if (expanded)
            {
                currentSelector.DoIntegratedContents(currentRightRect);
            }
            Text.Font = font;
        }

        private void FillRules(Rect inRect)
        {
            Text.Font = GameFont.Tiny;
            float currentY = 0f;
            int counter = 0;
            bool moveUp = false;
            bool moveDown = false;
            removalSet.Clear();
            foreach (IConfigRule rule in config.rules)
            {
                Rect currentRect = new Rect(0, currentY, inRect.width, rule.Height);
                Widgets.DrawMenuSection(currentRect);
                currentRect.xMin += 5;
                currentRect.xMax -= 5;
                currentY += rule.Height;
                if (counter < config.rules.Count - 1)
                {
                    FillSeperator(new Rect(0, currentY, inRect.width, 20));
                }
                currentY += 20;

                FillRow(rule, () =>
                {
                    Rect leftPart = currentRect.LeftPartPixels(18);
                    if (counter != 0 && Widgets.ButtonImageFitted(leftPart.TopPartPixels(18), TexButton.ReorderUp))
                    {
                        moveUp = true;
                    }
                    else if (counter != config.rules.Count - 1 && Widgets.ButtonImageFitted(leftPart.TopPartPixels(54).BottomPartPixels(18), TexButton.ReorderDown))
                    {
                        moveDown = true;
                    }
                    if (Widgets.ButtonImageFitted(leftPart.TopPartPixels(36).BottomPartPixels(18), TexButton.Minus))
                    {
                        removalSet.Add(rule);
                        ResetRightPanel();
                        Find.CurrentMap.reachability.ClearCache();
                    }

                    rule.DoContent(Pawns, currentRect.RightPart(0.90f), () =>
                    {
                        expanded = true;
                        expandedRule = rule;
                    }, () =>
                    {
                        expanded = false;
                        ResetRightPanel();
                    });
                    if (expandedRule != null && rule != expandedRule)
                    {
                        Widgets.DrawBoxSolid(currentRect, selectionOverlay);
                    }
                });
                if (moveUp || moveDown) break;
                counter++;
            }

            if (moveUp)
            {
                var temp = config.rules[counter];
                config.rules[counter] = config.rules[counter - 1];
                config.rules[counter - 1] = temp;
            }

            if (moveDown)
            {
                var temp = config.rules[counter];
                config.rules[counter] = config.rules[counter + 1];
                config.rules[counter + 1] = temp;
            }

            foreach (var rule in removalSet)
            {
                config.rules.Remove(rule);
            }
        }

        private void FillSeperator(Rect rect)
        {
            var font = Text.Font;
            var anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect, "Locks2Or".Translate());
            Text.Anchor = anchor;
            Text.Font = font;
        }

        private void FillRow(IConfigRule rule, Action doContent)
        {
            var font = Text.Font;
            var anchor = Text.Anchor;
            doContent.Invoke();
            Text.Font = font;
            Text.Anchor = anchor;
        }

        private void ResetRightPanel()
        {
            currentSelector = null;
            expanded = false;
            expandedRule = null;
        }
    }
}