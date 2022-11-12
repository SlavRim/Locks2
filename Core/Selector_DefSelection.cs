using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Locks2.Core
{
    public class Selector_DefSelection : ISelector
    {
        public IEnumerable<Def> defs;
        public Action<Def> onSelect;
        private Vector2 scrollPosition = Vector2.zero;
        private string searchString = "";
        private Rect viewRect = Rect.zero;

        public Selector_DefSelection(IEnumerable<Def> defs, Action<Def> onSelect, bool integrated = false,
            Action closeAction = null) : base(integrated, closeAction)
        {
            this.defs = defs;
            this.onSelect = onSelect;
        }

        public override void FillContents(Rect inRect)
        {
            GameFont font = Text.Font;
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
                Rect contentRect = new Rect(0, 0, inRect.width - 20, defs.Count() * 40);
                Widgets.DrawMenuSection(inRect);
                Widgets.BeginScrollView(inRect, ref scrollPosition, contentRect);
                Rect currentRect = contentRect.TopPartPixels(40);
                Text.Font = GameFont.Tiny;
                foreach (var def in defs)
                {
                    if (searchString.Length > 0 && !def.label.ToLower().Contains(searchString))
                    {
                        continue;
                    }
                    Widgets.DefLabelWithIcon(currentRect, def);
                    if (Widgets.ButtonInvisible(currentRect))
                    {
                        onSelect(def);
                        Close();
                    }
                    currentRect.y += currentRect.height;
                }
                Widgets.EndScrollView();
            }
            finally
            {
                Text.Font = font;
            }
        }
    }
}