using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class SplitterView : ViewBase
    {
        private bool isResizing = false;

        public float Width;
        private float _minWidth;

        public float OnSplit(Rect rect, float width, float minWidth = 220)
        {
            Width = width;
            _minWidth = minWidth;
            OnGUI(rect);
            return Width;
        }

        public override void OnDraw()
        {
            // 绘制分隔条，可以拖动改变左右区域比例
            GUILayout.BeginArea(new Rect(Width, 0, Styles.SplitterWidth, Position.height),
                EditorStyles.helpBox);
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, Styles.SplitterWidth, Position.height),
                MouseCursor.ResizeHorizontal);
            GUILayout.EndArea();

            // 检测鼠标事件，拖动分隔条时调整左右区域的比例
            HandleSplitterResize(Width, Position);
        }

        private void HandleSplitterResize(float leftPanelWidth, Rect rect)
        {
            Rect splitterRect = new Rect(leftPanelWidth, 0, Styles.SplitterWidth, rect.height);

            // 检查鼠标事件
            if (Event.current.type == EventType.MouseDown && splitterRect.Contains(Event.current.mousePosition))
            {
                isResizing = true;
                Event.current.Use();
            }

            if (isResizing && Event.current.type == EventType.MouseDrag)
            {
                var leftPanelWidthPercent = Mathf.Clamp(Event.current.mousePosition.x / rect.width, 0.1f, 0.9f);
                Width = Mathf.Clamp(rect.width * leftPanelWidthPercent, _minWidth, Position.width);
                Window.Repaint(); // 刷新窗口
            }

            if (Event.current.type == EventType.MouseUp)
            {
                isResizing = false;
            }
        }
    }
}