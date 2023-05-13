using UnityEngine;

namespace NBC.ActionEditor
{
    public static class ColorUtility
    {
        public static Color Grey(float value) {
            return new Color(value, value, value);
        }

        public static Color WithAlpha(this Color color, float alpha) {
            color.a = alpha;
            return color;
        }
    }
}