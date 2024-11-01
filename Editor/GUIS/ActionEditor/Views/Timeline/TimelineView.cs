using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class TimelineView : ViewBase
    {
        private SplitterView _splitterView;

        private TimelineHeaderView _headerView;
        private TimelineMiddleView _middleView;
        private TimelinePointerView _pointerView;
        private TimelineBottomView _bottomView;

        public Asset asset => App.AssetData;

        private Rect _pointerRect;

        protected override void OnInit()
        {
            _splitterView = Window.CreateView<SplitterView>();

            _headerView = Window.CreateView<TimelineHeaderView>();
            _middleView = Window.CreateView<TimelineMiddleView>();
            _pointerView = Window.CreateView<TimelinePointerView>();
            _bottomView = Window.CreateView<TimelineBottomView>();

            Prefs.SnapInterval = 0.01f;
        }

        public override void OnDraw()
        {
            asset.UpdateMaxTime();

            var headRect = new Rect(0, 0, Position.width, Styles.PlayControlHeight);
            GUILayout.BeginArea(headRect, EditorStyles.helpBox);
            _headerView.OnGUI(new Rect(0, 0, headRect.width, headRect.height));
            GUILayout.EndArea();


            App.Width = Styles.TimelineRightWidth;
            DoZoomAndPan();
            ItemDragger.OnCheck();

            var middleRect = new Rect(0, Styles.PlayControlHeight, Position.width,
                Position.height - Styles.PlayControlHeight - Styles.BottomHeight);

            //groups and tracks
            GUILayout.BeginArea(middleRect);
            // _middleView.OnGUI(middleRect);
            _middleView.OnGUI(new Rect(middleRect.x, middleRect.y - Styles.PlayControlHeight, middleRect.width,
                middleRect.height));
            GUILayout.EndArea();

            //bottom
            var bottomRect = new Rect(0, middleRect.y + middleRect.height, Position.width, Styles.BottomHeight);
            GUILayout.BeginArea(bottomRect, EditorStyles.helpBox);
            _bottomView.OnGUI(new Rect(0, 0, bottomRect.width, bottomRect.height));
            GUILayout.EndArea();

            // var leftOffset = Styles.TimelineLeftWidth + Styles.SplitterWidth;
            // var pointerWidth = Position.width - leftOffset;
            // Styles.TimelineRightWidth = pointerWidth - Styles.RightGapWidth;
            // var pointerRect = new Rect(leftOffset, Styles.HeaderHeight, pointerWidth,
            //     Position.height - Styles.BottomHeight - Styles.HeaderHeight);
            // _pointerRect = pointerRect;
            // GUILayout.BeginArea(pointerRect);
            // _pointerView.OnGUI(new Rect(0, 0, pointerRect.width, pointerRect.height));
            // GUILayout.EndArea();

            var leftOffset = Styles.TimelineLeftWidth + Styles.SplitterWidth + Styles.RightGapWidth;
            Styles.TimelineRightWidth = Position.width - leftOffset;
            var pointerRect = new Rect(leftOffset, Styles.HeaderHeight, Styles.TimelineRightWidth,
                Position.height - Styles.BottomHeight - Styles.HeaderHeight);
            _pointerRect = pointerRect;
            GUILayout.BeginArea(pointerRect);
            _pointerView.OnGUI(new Rect(0, 0, pointerRect.width, pointerRect.height));
            GUILayout.EndArea();

            var leftWidth = Styles.TimelineLeftWidth;
            leftWidth = _splitterView.OnSplit(Position, leftWidth);
            if (!leftWidth.Equals(Styles.TimelineLeftWidth))
            {
                Styles.TimelineLeftWidth = leftWidth;
            }
        }

        #region Zoom & Pan

        private bool _isMouseButton2Down;
        private float _lastZoomX;

        public void DoZoomAndPan()
        {
            var e = Event.current;

            if (!_pointerRect.Contains(e.mousePosition)) return;

            // var ev = Event.current;
            // if (ev.button == 2)
            // {
            //     Debug.LogError("修改拖动光标===22=");
            //     EditorGUIUtility.AddCursorRect(new Rect(_pointerRect), MouseCursor.Zoom);
            //     // ev.Use();
            // }

            if (e.button == 2 && e.type == EventType.MouseDown)
            {
                _isMouseButton2Down = true;
                _lastZoomX = e.mousePosition.x;
                Window.Repaint();
            }

            if (e.button == 2 && e.rawType == EventType.MouseUp)
            {
                _isMouseButton2Down = false;
                _lastZoomX = e.mousePosition.x;
                Window.Repaint();
            }

            if (e.type == EventType.ScrollWheel)
            {
                var delta = e.delta.y;
                if (delta > 0)
                {
                    delta = 3;
                }
                else if (delta < 0)
                {
                    delta = -3;
                }

                var t = Mathf.Abs(delta * 25) / Position.width * asset.ViewTime;

                var maxAdd = delta > 0 ? t : -t;

                if (maxAdd > 0 && asset.ViewTimeMax - asset.ViewTimeMin > 240)
                {
                    Debug.Log("Exceed maximum range!");
                    return;
                }

                asset.ViewTimeMax += maxAdd;

                Window.Repaint();

                e.Use();
            }

            if (_isMouseButton2Down)
            {
                var rect = new Rect(_pointerRect);
                rect.y += Styles.HeaderHeight;
                rect.height -= Styles.HeaderHeight;
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Pan);

                if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown || e.type == EventType.MouseUp)
                {
                    var offset = e.mousePosition.x - _lastZoomX;
                    var t = Mathf.Abs(offset) / App.Width * asset.ViewTime;

                    var min = asset.ViewTimeMin + (offset > 0 ? -t : t);
                    var max = asset.ViewTimeMax + (offset > 0 ? -t : t);
                    // var minTime = 0;
                    // var minTime = asset.PosToTime(4, App.Width) * -1;
                    if (min >= 0)
                    {
                        asset.ViewTimeMin = min;
                        asset.ViewTimeMax = max;
                    }
                    else if (min < 0 && asset.ViewTimeMin > 0)
                    {
                        asset.ViewTimeMin = 0;
                        asset.ViewTimeMax = max + 0;
                    }

                    _lastZoomX = e.mousePosition.x;

                    e.Use();
                    Window.Repaint();
                }
            }
        }

        #endregion
    }
}