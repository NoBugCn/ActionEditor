using System;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class G
    {
        public static string SearchString;

        public static float timeInfoInterval = 1000000f;
        public static float timeInfoHighMod = timeInfoInterval;
        public static float timeInfoStart;
        public static float timeInfoEnd;

        public static float SnapTime(float time)
        {
            if (Event.current.control)
            {
                return time;
            }

            return (Mathf.Round(time / Prefs.snapInterval) * Prefs.snapInterval);
        }

        #region Size

        public static float TotalHeight;

        public static float ScreenWidth => Screen.width / EditorGUIUtility.pixelsPerPoint;

        public static float ScreenHeight => Screen.height / EditorGUIUtility.pixelsPerPoint;

        public static Vector2 ScrollPos;

        public static readonly float BottomHeight = Styles.BOTTOM_HEIGHT * 2;

        public static Rect TopLeftRect;

        public static Rect LeftRect;

        public static Rect TopMiddleRect;

        public static Rect CenterRect;

        public static void Reset()
        {
            TopLeftRect = new Rect(0, Styles.TOOLBAR_HEIGHT, Styles.LEFT_MARGIN, Styles.TOP_MARGIN);


            var centerHeight = ScreenHeight - Styles.TOOLBAR_HEIGHT - Styles.TOP_MARGIN + ScrollPos.y;
            var centerWidth = ScreenWidth - Styles.LEFT_MARGIN - Styles.RIGHT_MARGIN;

            TopMiddleRect = new Rect(Styles.LEFT_MARGIN, Styles.TOOLBAR_HEIGHT, centerWidth, Styles.TOP_MARGIN);

            LeftRect = new Rect(0, Styles.TOOLBAR_HEIGHT + Styles.TOP_MARGIN, Styles.LEFT_MARGIN, centerHeight);

            CenterRect = new Rect(Styles.LEFT_MARGIN, Styles.TOP_MARGIN + Styles.TOOLBAR_HEIGHT, centerWidth,
                centerHeight - BottomHeight);
        }

        #endregion
        
        
        internal static bool IsFilteredOutBySearch(ScriptableObject direct)
        {
            if (string.IsNullOrEmpty(SearchString))
            {
                return false;
            }

            if (string.IsNullOrEmpty(direct.name))
            {
                return true;
            }

            return !direct.name.ToLower().Contains(SearchString.ToLower());
        }
    }
}