using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public sealed class BottomGUI : ICustomized
    {
        public void OnGUI()
        {
            var sliderRect = new Rect(G.CenterRect.x, G.ScreenHeight - G.BottomHeight, G.CenterRect.width,
                Styles.BOTTOM_HEIGHT);
            ShowTimeSlider(sliderRect);
        }

        /// <summary>
        /// 显示底部滚动信息
        /// </summary>
        void ShowTimeSlider(Rect rect)
        {
            GUILayout.BeginArea(rect);
            var asset = App.AssetData;
            //最小最大值滑块
            var _timeMin = asset.ViewTimeMin;
            var _timeMax = asset.ViewTimeMax;
            var sliderRect = new Rect(5, 0, G.TopMiddleRect.width - 10, 18);
            EditorGUI.MinMaxSlider(sliderRect, ref _timeMin, ref _timeMax, 0, asset.MaxTime);
            asset.ViewTimeMin = _timeMin;
            asset.ViewTimeMax = _timeMax;
            if (sliderRect.Contains(Event.current.mousePosition) && Event.current.clickCount == 2)
            {
                asset.ViewTimeMin = 0;
                asset.ViewTimeMax = asset.Length;
            }

            GUI.color = Color.white.WithAlpha(0.1f);
            GUI.DrawTexture(Rect.MinMaxRect(0, Styles.TOP_MARGIN - 1, G.TopMiddleRect.xMax, Styles.TOP_MARGIN),
                Styles.whiteTexture);
            GUI.color = Color.white;

            GUILayout.EndArea();

            GUI.contentColor = Color.white;
        }
    }
}