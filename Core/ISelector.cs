using System;
using UnityEngine;
using Verse;

namespace Locks2.Core
{
    public abstract class ISelector : Window
    {
        private readonly Action closeAction;
        private readonly bool integrated;

        public ISelector(bool integrated = false, Action closeAction = null)
        {
            this.integrated = integrated;
            if (this.integrated && closeAction == null)
                throw new InvalidOperationException(
                    "In intergrated mod you must pass a listing_standard and an onclose action");
            this.closeAction = closeAction;
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (integrated) throw new InvalidOperationException("Called do DoWindowContents in integrated mod");
            var font = Text.Font;
            var anchor = Text.Anchor;
            inRect.height -= 30;
            FillContents(inRect);
            inRect.height += 30;
            if (Widgets.ButtonText(inRect.BottomPartPixels(30), "Locks2Close".Translate())) Close();
            Text.Font = font;
            Text.Anchor = anchor;
        }

        public void DoIntegratedContents(Rect rect)
        {
            var font = Text.Font;
            var anchor = Text.Anchor;
            FillContents(rect.ContractedBy(3));
            Text.Font = font;
            Text.Anchor = anchor;
        }

        public abstract void FillContents(Rect inRect);

        public override void Close(bool doCloseSound = true)
        {
            if (!integrated)
                base.Close(doCloseSound);
            else
                closeAction.Invoke();
        }
    }
}