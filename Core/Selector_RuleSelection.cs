using System;
using System.Linq;
using UnityEngine;
using Verse;
using static Locks2.Core.LockConfig;

namespace Locks2.Core
{
    public class Selector_RuleSelection : ISelector
    {
        public Action<IConfigRule> onSelect;

        public Type[] rulesTypes;

        private Vector2 scrollPosition = Vector2.zero;

        private string searchString = "";

        public Selector_RuleSelection(Action<IConfigRule> onSelect, bool integrated = false, Action closeAction = null)
            : base(integrated, closeAction)
        {
            this.onSelect = onSelect;
            rulesTypes = typeof(IConfigRule).AllSubclasses().ToArray();
        }

        public override void FillContents(Rect inRect)
        {
            GameFont font = Text.Font;
            TextAnchor anchor = Text.Anchor;
            try
            {
                Rect searchRect = inRect.TopPartPixels(20);
                if (Widgets.ButtonImage(searchRect.LeftPartPixels(20), TexButton.CloseXSmall))
                {
                    Close();
                }
                Text.Font = GameFont.Tiny;
                string searchBuffer = Widgets.TextField(new Rect(searchRect.position + new Vector2(25, 0), searchRect.size - new Vector2(55, 0)),
                        searchString).ToLower().Trim();
                if (searchBuffer != searchString)
                {
                    scrollPosition = Vector2.zero;
                    searchString = searchBuffer;
                }
                inRect.yMin += 25;
                Rect contentRect = new Rect(0, 0, inRect.width - 20, rulesTypes.Count() * 40);
                Widgets.DrawMenuSection(inRect);
                Widgets.BeginScrollView(inRect, ref scrollPosition, contentRect);
                Rect currentRect = contentRect.TopPartPixels(40);
                currentRect.xMin += 15;
                Text.Font = GameFont.Tiny;
                foreach (var type in rulesTypes)
                {
                    string name = type.Name.Translate().ToLower();
                    if (searchString.Length > 0 && !name.Contains(searchString))
                    {
                        continue;
                    }
                    Widgets.DrawHighlightIfMouseover(currentRect);
                    Widgets.Label(currentRect, name);
                    if (Widgets.ButtonInvisible(currentRect))
                    {
                        onSelect(Activator.CreateInstance(type) as IConfigRule);
                        Close();
                    }
                    currentRect.y += currentRect.height;
                }
                Widgets.EndScrollView();
            }
            finally
            {
                Text.Anchor = anchor;
                Text.Font = font;
            }
        }
    }
}