using System;
using UnityEngine;
using Verse;

namespace Locks2.Core
{
    public class Window_Confirm : Window
    {
        private static readonly Vector2 size = new Vector2(400, 100);
        private bool actionPreformed;
        private readonly Action confirmedAction;
        private readonly Action notConfirmedAction;

        public Window_Confirm(Action confirmedAction, Action notConfirmedAction)
        {
            this.confirmedAction = confirmedAction;
            this.notConfirmedAction = notConfirmedAction;
        }

        public override Vector2 InitialSize => size;

        public override void DoWindowContents(Rect inRect)
        {
            var anchor = Text.Anchor;
            var font = Text.Font;
            DoContent(inRect);
            Text.Anchor = anchor;
            Text.Font = font;
        }

        public override void Close(bool doCloseSound = true)
        {
            if (!actionPreformed) notConfirmedAction.Invoke();
            base.Close(doCloseSound);
        }

        private void DoContent(Rect inRect)
        {
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                var topRect = inRect.TopHalf();
                Widgets.Label(topRect, "Locks2Confirm".Translate());
            }
            {
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.MiddleCenter;
                var bottomRect = inRect.BottomHalf();
                if (Widgets.ButtonText(bottomRect.RightHalf(), "Locks2ConfirmNo".Translate()))
                {
                    actionPreformed = true;
                    notConfirmedAction.Invoke();
                    Close();
                }

                if (Widgets.ButtonText(bottomRect.LeftHalf(), "Locks2ConfirmYes".Translate()))
                {
                    actionPreformed = true;
                    confirmedAction.Invoke();
                    Close();
                }
            }
        }
    }
}