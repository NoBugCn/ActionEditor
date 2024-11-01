using UnityEngine;

namespace NBC.ActionEditor
{
    public class SimpleClipDraw : ClipDrawBase
    {
        protected override void OnDraw()
        {
            GUI.color = Styles.ClipBackColor.WithAlpha(0.5f);
            if (ClipRect.width < 1)
            {
                ClipRect.width = 5;
                ClipRect.x -= 2;
            }

            GUI.DrawTexture(ClipRect, Styles.WhiteTexture);
        }
    }
}