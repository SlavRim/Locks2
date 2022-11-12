using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Locks2.Core
{
    public class Selector_PawnSelection : ISelector
    {
        public Action<Pawn> onSelect;

        public IEnumerable<Pawn> pawns;
        private Vector2 scrollPosition = Vector2.zero;
        private string searchString = "";
        private Rect viewRect = Rect.zero;

        public Selector_PawnSelection(IEnumerable<Pawn> pawns, Action<Pawn> onSelect, bool integrated = false,
            Action closeAction = null) : base(integrated, closeAction)
        {
            this.pawns = pawns;
            this.onSelect = onSelect;
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
                Rect contentRect = new Rect(0, 0, inRect.width - 20, pawns.Count() * 40);
                Widgets.DrawMenuSection(inRect);
                Widgets.BeginScrollView(inRect, ref scrollPosition, contentRect);
                Rect currentRect = contentRect.TopPartPixels(40);
                currentRect.xMin += 15;
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.MiddleLeft;
                foreach (var pawn in pawns)
                {
                    string name = pawn.Name.ToStringFull;
                    if (searchString.Length > 0 && !name.ToLower().Contains(searchString))
                    {
                        continue;
                    }
                    Widgets.DrawHighlightIfMouseover(currentRect);
                    Widgets.Label(currentRect, name);
                    if (Widgets.ButtonInvisible(currentRect))
                    {
                        onSelect(pawn);
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