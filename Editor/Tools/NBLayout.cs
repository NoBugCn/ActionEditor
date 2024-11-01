using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NBC.ActionEditor
{
    class LayoutHorizontalInfo
    {
        public Rect NowRect;
        public bool Inverse = false;
        public float X;
    }

    public class NBLayout
    {
        #region Horizontal

        private static List<LayoutHorizontalInfo> _horizontalInfos = new List<LayoutHorizontalInfo>();


        public static void BeginHorizontal(Rect rect, params GUILayoutOption[] options)
        {
            BeginHorizontal(rect, false, options);
        }

        public static void BeginHorizontal(Rect rect, bool inverse = false, params GUILayoutOption[] options)
        {
            if (!inverse)
            {
                GUILayout.BeginHorizontal(options);
            }

            var info = new LayoutHorizontalInfo
            {
                NowRect = rect,
                Inverse = inverse,
                X = inverse ? rect.x + rect.width : rect.x
            };
            _horizontalInfos.Add(info);
        }

        public static Rect GetHRect(float width, float height = 0, bool center = true)
        {
            if (_horizontalInfos.Count <= 0) return Rect.zero;
            var last = _horizontalInfos.Last();
            if (height <= 0)
            {
                height = last.NowRect.height;
            }

            var ret = new Rect(last.Inverse ? last.X - width : last.X, last.NowRect.y, width, height);
            if (center)
            {
                ret.y = last.NowRect.GetY(height);
            }

            Space(width);
            return ret;
        }


        public static void EndHorizontal()
        {
            if (_horizontalInfos.Count <= 0) return;
            var last = _horizontalInfos.Last();
            if (!last.Inverse)
            {
                GUILayout.EndHorizontal();
            }

            _horizontalInfos.Remove(last);
        }

        public static void Space(float pixels)
        {
            if (_horizontalInfos.Count > 0)
            {
                var last = _horizontalInfos.Last();
                last.X += last.Inverse ? pixels * -1 : pixels;
                if (!last.Inverse)
                {
                    GUILayout.Space(pixels);
                }
            }
        }

        public static float ExpandWidth()
        {
            if (_horizontalInfos.Count <= 0) return 0;
            var last = _horizontalInfos.Last();
            return last.NowRect.width - last.X;
        }

        #endregion
    }
}