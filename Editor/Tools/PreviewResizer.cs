using System;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    [Serializable]
    internal class PreviewResizer
    {
        private static float s_DraggedPreviewSize;
        private static float s_CachedPreviewSizeWhileDragging;
        private static float s_MouseDownLocation;
        private static float s_MouseDownValue;
        private static bool s_MouseDragged;
        [SerializeField] private float m_CachedPref;
        [SerializeField] private int m_ControlHash;
        [SerializeField] private string m_PrefName;
        private int m_Id;

        private int id
        {
            get
            {
                if (m_Id == 0)
                {
                    m_Id = GUIUtility.GetControlID(m_ControlHash, FocusType.Passive, default(Rect));
                }

                return m_Id;
            }
        }

        public void Init(string prefName)
        {
            if (m_ControlHash != 0 && !string.IsNullOrEmpty(m_PrefName))
            {
                return;
            }

            m_ControlHash = prefName.GetHashCode();
            m_PrefName = "Preview_" + prefName;
            m_CachedPref = EditorPrefs.GetFloat(m_PrefName, 1f);
        }

        public float ResizeHandle(Rect windowPosition, float minSize, float minRemainingSize, float resizerHeight)
        {
            return ResizeHandle(windowPosition, minSize, minRemainingSize, resizerHeight, default(Rect));
        }

        public float ResizeHandle(Rect windowPosition, float minSize, float minRemainingSize, float resizerHeight,
            Rect dragRect)
        {
            if (Mathf.Abs(m_CachedPref) < minSize)
            {
                m_CachedPref = minSize * Mathf.Sign(m_CachedPref);
            }

            float num = windowPosition.height - minRemainingSize;
            bool flag = GUIUtility.hotControl == id;
            float num2 = (!flag) ? Mathf.Max(0f, m_CachedPref) : s_DraggedPreviewSize;
            bool flag2 = m_CachedPref > 0f;
            float num3 = Mathf.Abs(m_CachedPref);
            Rect position = new Rect(0f, windowPosition.height - num2 - resizerHeight, windowPosition.width,
                resizerHeight);
            if (dragRect.width != 0f)
            {
                position.x = dragRect.x;
                position.width = dragRect.width;
            }

            bool flag3 = flag2;
            num2 = -PixelPreciseCollapsibleSlider(id, position, -num2, -num, 0f, ref flag2);
            num2 = Mathf.Min(num2, num);
            flag = (GUIUtility.hotControl == id);
            if (flag)
            {
                s_DraggedPreviewSize = num2;
            }

            if (num2 < minSize)
            {
                num2 = ((num2 >= minSize * 0.5f) ? minSize : 0f);
            }

            if (flag2 != flag3)
            {
                num2 = ((!flag2) ? 0f : num3);
            }

            flag2 = (num2 >= minSize / 2f);
            if (GUIUtility.hotControl == 0)
            {
                if (num2 > 0f)
                {
                    num3 = num2;
                }

                float num4 = num3 * (float)((!flag2) ? -1 : 1);
                if (num4 != m_CachedPref)
                {
                    m_CachedPref = num4;
                    EditorPrefs.SetFloat(m_PrefName, m_CachedPref);
                }
            }

            s_CachedPreviewSizeWhileDragging = num2;
            return num2;
        }

        public bool GetExpanded()
        {
            if (GUIUtility.hotControl == id)
            {
                return s_CachedPreviewSizeWhileDragging > 0f;
            }

            return m_CachedPref > 0f;
        }

        public float GetPreviewSize()
        {
            if (GUIUtility.hotControl == id)
            {
                return Mathf.Max(0f, s_CachedPreviewSizeWhileDragging);
            }

            return Mathf.Max(0f, m_CachedPref);
        }

        public bool GetExpandedBeforeDragging()
        {
            return m_CachedPref > 0f;
        }

        public void SetExpanded(bool expanded)
        {
            m_CachedPref = Mathf.Abs(m_CachedPref) * (float)((!expanded) ? -1 : 1);
            EditorPrefs.SetFloat(m_PrefName, m_CachedPref);
        }

        public void ToggleExpanded()
        {
            m_CachedPref = -m_CachedPref;
            EditorPrefs.SetFloat(m_PrefName, m_CachedPref);
        }

        public static float PixelPreciseCollapsibleSlider(int id, Rect position, float value, float min, float max,
            ref bool expanded)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (GUIUtility.hotControl == 0 && current.button == 0 && position.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = id;
                        s_MouseDownLocation = current.mousePosition.y;
                        s_MouseDownValue = value;
                        s_MouseDragged = false;
                        current.Use();
                    }

                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        GUIUtility.hotControl = 0;
                        if (!s_MouseDragged)
                        {
                            expanded = !expanded;
                        }

                        current.Use();
                    }

                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        value = Mathf.Clamp(
                            current.mousePosition.y - s_MouseDownLocation +
                            s_MouseDownValue, min, max - 1f);
                        GUI.changed = true;
                        s_MouseDragged = true;
                        current.Use();
                    }

                    break;
                case EventType.Repaint:
                    if (GUIUtility.hotControl == 0)
                    {
                        EditorGUIUtility.AddCursorRect(position, MouseCursor.SplitResizeUpDown);
                    }

                    if (GUIUtility.hotControl == id)
                    {
                        EditorGUIUtility.AddCursorRect(
                            new Rect(position.x, position.y - 100f, position.width, position.height + 200f),
                            MouseCursor.SplitResizeUpDown);
                    }

                    break;
            }

            return value;
        }
    }
}