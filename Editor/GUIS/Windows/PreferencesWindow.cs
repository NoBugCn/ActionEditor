using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class PreferencesWindow : PopupWindowContent
    {
        private static Rect myRect;
        private bool firstPass = true;

        public static void Show(Rect rect)
        {
            myRect = rect;
            PopupWindow.Show(new Rect(rect.x, rect.y, 0, 0), new PreferencesWindow());
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(myRect.width, myRect.height);
        }
        
        public override void OnGUI(Rect rect)
        {
            DrawTools.Draw<PreferencesGUI>();
            
            if (firstPass || Event.current.type == EventType.Repaint)
            {
                firstPass = false;
                myRect.height = GUILayoutUtility.GetLastRect().yMax + 5;
            }
        }
    }
}