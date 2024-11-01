using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class TimelineBottomView : ViewBase
    {
        public override void OnDraw()
        {
            ShowTimeSlider();
        }

        void ShowTimeSlider()
        {
            var asset = App.AssetData;
            if(asset == null) return;
            var left = Styles.TimelineLeftWidth + Styles.RightGapWidth;
            var rect = new Rect(Position.x + left, Position.y, Position.width - left, Position.height);
            GUILayout.BeginArea(rect);
            var _timeMin = asset.ViewTimeMin;
            var _timeMax = asset.ViewTimeMax;
            
            GUI.color = Color.gray;
            
            var sliderRect = new Rect(5, 0, rect.width - 10, 18);
            EditorGUI.MinMaxSlider(sliderRect, ref _timeMin, ref _timeMax, 0, asset.Length);
            asset.ViewTimeMin = _timeMin;
            asset.ViewTimeMax = _timeMax;
            if (sliderRect.Contains(Event.current.mousePosition) && Event.current.clickCount == 2)
            {
                asset.ViewTimeMin = 0;
                asset.ViewTimeMax = asset.Length;
            }

            GUILayout.EndArea();

            GUI.contentColor = Color.white;
        }
    }
}