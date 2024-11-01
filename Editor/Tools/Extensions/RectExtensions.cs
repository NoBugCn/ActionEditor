using UnityEngine;

namespace NBC.ActionEditor
{
    public static class RectExtensions
    {
        public static float GetY(this Rect rect, float height, float lineHeight = Styles.LineHeight)
        {
            return rect.y + (lineHeight - height) * 0.5f;
        }
    }
}