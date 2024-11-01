using NBC.ActionEditor.Events;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    enum PointerDragType
    {
        None,
        Play,
        StartRange,
        EndRange
    }

    public class TimelinePointerView : ViewBase, IPointerClickHandler,
        IPointerDragHandler, IDragBeginHandler, IDragEndHandler
    {
        public DirectorAsset asset => App.AssetData;

        private Rect _playPointerHandler;
        private Rect _pointerTextRect;

        private PointerDragType _dragType = PointerDragType.None;

        protected override void OnInit()
        {
            base.OnInit();
        }


        public override void OnDraw()
        {
            if (asset == null) return;

            DrawTimeStep();
            DrawPointer();
            CheckDrag();
        }

        #region Time Step

        private void DrawTimeStep()
        {
            if (asset == null) return;
            var width = App.Width;

            var x = asset.TimeToPos(asset.Length, width);
            var stepRect = new Rect(Position.x, Position.y + (Styles.HeaderHeight - 4), x, 4);
            GUI.color = Styles.TimeStepRectColor;
            GUI.DrawTexture(stepRect, Styles.WhiteTexture);
            GUI.color = Color.white;


            var timeInfoInterval = 1000000f;
            var lowMod = 0.01f;
            var modulos = new[]
                { 0.01f, 0.1f, 0.5f, 1, 5, 10, 50, 100, 500, 1000, 5000, 10000, 50000, 100000, 250000, 500000 };
            for (var i = 0; i < modulos.Length; i++)
            {
                var count = asset.ViewTime / modulos[i];
                if (width / count > 50)
                {
                    timeInfoInterval = modulos[i];
                    lowMod = i > 0 ? modulos[i - 1] : lowMod;
                    break;
                }
            }

            var timeStep = lowMod;
            var timeInfoStart = Mathf.FloorToInt(asset.ViewTimeMin / timeInfoInterval) * timeInfoInterval;
            var timeInfoEnd = Mathf.CeilToInt(asset.ViewTimeMax / timeInfoInterval) * timeInfoInterval;

            //时间步长间隔 time step interval
            if (width / (asset.ViewTime / timeStep) > 6)
            {
                for (var i = timeInfoStart; i <= timeInfoEnd; i += timeStep)
                {
                    var posX = asset.TimeToPos(i, width);
                    if (posX > Position.width) continue;
                    GUI.DrawTexture(new Rect(posX, Position.y + (Styles.HeaderHeight - 4), 1, 4), Styles.WhiteTexture);
                }
            }

            //时间文字间隔 time text interval
            for (var i = timeInfoStart; i <= timeInfoEnd; i += timeInfoInterval)
            {
                var posX = asset.TimeToPos(i, width);
                var rounded = Mathf.Round(i * 10) / 10;
                if (posX > Position.width) continue;
                GUI.DrawTexture(new Rect(posX, Position.y + (Styles.HeaderHeight - 12), 1, 12), Styles.WhiteTexture);
                var text = rounded.ToString("0.00");

                var size = GUI.skin.label.CalcSize(new GUIContent(text));
                var stampRect = new Rect(posX, 0, size.x, size.y);
                GUI.Box(stampRect, rounded.ToString("0.00"), GUI.skin.label);
            }
        }

        #endregion

        #region Pointer

        private void DrawPointer()
        {
            var width = App.Width;
            var height = Styles.PlayControlHeight - Styles.HeaderHeight;
            var x = asset.TimeToPos(asset.Length, width);
            GUI.color = Styles.EndPointerColor;
            GUI.DrawTexture(new Rect(x - 2, Position.y, 5, height), Styles.WhiteTexture);
            GUI.DrawTexture(new Rect(x, Position.y, 1, Position.height), Styles.WhiteTexture);
            GUI.color = Color.white;

            var playX = asset.TimeToPos(App.CurrentTime, width);
            _playPointerHandler = new Rect(playX - 5, Position.y, 11, height);
            _pointerTextRect = new Rect(Position.x, Position.y, width, height);

            GUI.DrawTexture(_playPointerHandler, Styles.TimelineTimeCursorIcon);
            GUI.DrawTexture(new Rect(playX, Position.y, 1, Position.height), Styles.WhiteTexture);

            DrawRangeLine();
            DrawDragLine();
        }

        private void DrawRangeLine()
        {
            if (!App.IsRange) return;

            var startX = asset.TimeToPos(asset.RangeMin, App.Width);
            var rect1 = new Rect(startX - Styles.TimelineStartPlaybackIcon.width, Position.y,
                Styles.TimelineStartPlaybackIcon.width,
                Styles.TimelineStartPlaybackIcon.height);
            GUI.DrawTexture(rect1, Styles.TimelineStartPlaybackIcon);
            GUI.DrawTexture(new Rect(startX, Position.y, 1, Position.height), Styles.WhiteTexture);

            var endX = asset.TimeToPos(asset.RangeMax, App.Width);
            var rect2 = new Rect(endX, Position.y, Styles.TimelineEndPlaybackIcon.width,
                Styles.TimelineEndPlaybackIcon.height);
            GUI.DrawTexture(rect2, Styles.TimelineEndPlaybackIcon);
            GUI.DrawTexture(new Rect(endX, Position.y, 1, Position.height), Styles.WhiteTexture);
        }

        private void DrawDragLine()
        {
            if (App.CanMultipleSelect) return;
            if (ItemDragger.DragType > ItemDragType.None)
            {
                var items = ItemDragger.DragItems;
                var startTime = float.MaxValue;
                var endTime = 0f;
                foreach (var item in items)
                {
                    if (item.StartTime < startTime)
                    {
                        startTime = item.StartTime;
                    }

                    if (item.EndTime > endTime)
                    {
                        endTime = item.EndTime;
                    }
                }

                if (ItemDragger.DragType == ItemDragType.StretchStart)
                {
                    DrawDragLine(startTime);
                }
                else if (ItemDragger.DragType == ItemDragType.StretchEnd)
                {
                    DrawDragLine(endTime);
                }
                else
                {
                    DrawDragLine(startTime);
                    DrawDragLine(endTime);
                }
            }
        }

        private void DrawDragLine(float time)
        {
            var x = asset.TimeToPos(time, App.Width);
            var magnet = ItemDragger.HasMagnetSnapTime(time);
            if (magnet)
            {
                GUI.DrawTexture(new Rect(x, Position.y, 1, Position.height), Styles.WhiteTexture);
            }
            else
            {
                var color = new Color(0.48f, 0.48f, 0.48f);
                DrawTools.DrawDashedLine(x, Position.y, Position.y + Position.height, color);
            }

            var text = time.ToString("0.00");
            var size = GUI.skin.label.CalcSize(new GUIContent(text));
            var width = size.x + 5;

            var showX = x - width * 0.5f;

            GUI.color = Color.black.WithAlpha(0.8f);

            GUI.DrawTexture(new Rect(showX, Position.y, width, 18), Styles.BackgroundTexture);
            var stampRect = new Rect(showX + 2, 0, size.x, size.y);
            GUI.color = Color.white;
            GUI.Box(stampRect, text, GUI.skin.label);
        }

        #endregion

        #region Drag

        private void CheckDrag()
        {
            asset.PosToTime(0, App.Width);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_pointerTextRect.Contains(eventData.MousePosition))
            {
                ChangeCurrentTime(eventData.MousePosition);
            }
        }

        public void OnDragBegin(PointerEventData eventData)
        {
            if (_playPointerHandler.Contains(eventData.MousePosition))
            {
                Debug.Log("拖动当前播放指针");
                _dragType = PointerDragType.Play;
            }
            else
            {
                _dragType = PointerDragType.None;
            }
        }

        public void OnDragEnd(PointerEventData eventData)
        {
            _dragType = PointerDragType.None;
        }

        public void OnPointerDrag(PointerEventData eventData)
        {
            if (_dragType == PointerDragType.Play)
            {
                ChangeCurrentTime(eventData.MousePosition);
            }
        }

        private void ChangeCurrentTime(Vector2 mousePosition)
        {
            var time = asset.PosToTime(mousePosition.x, App.Width);
            App.CurrentTime = asset.SnapTime(time);
            App.CurrentTime =
                Mathf.Clamp(App.CurrentTime, 0 + float.Epsilon, asset.Length - float.Epsilon);

            Debug.Log($"time={time}");
            Window.Repaint();
        }

        #endregion
    }
}