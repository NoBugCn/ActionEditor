using System;
using NBC.ActionEditor.Events;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public abstract class ViewBase
    {
        protected EditorWindow Window;

        public Rect Position;

        private bool _visible;

        public bool Visible
        {
            get => _visible;
            set { _visible = value; }
        }

        public void Init(EditorWindow window)
        {
            Window = window;
            _visible = true;
            OnInit();
        }

        public void Update()
        {
            if (Position.width <= 0 && Position.height <= 0) return;
            OnUpdate();
        }


        protected virtual void OnInit()
        {
        }


        public virtual void OnGUI(Rect rect)
        {
            Position = rect;
            try
            {
                OnDraw();
                CheckPointerEvent();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        
        public abstract void OnDraw();

        protected virtual void OnUpdate()
        {
        }

        #region Event Handler

        private bool _havePointerDown;
        private bool _isDragging;
        private bool _isPointerOver;
        private PointerEventData _eventData = new PointerEventData();
        private const float DragThreshold = 1f;
        private Vector2 _dragStartPos;

        protected void CheckPointerEvent()
        {
            var ev = Event.current;
            _eventData.SetEvent(ev);
            // Debug.Log("CheckPointerEvent");
            var hasRect = Position.Contains(ev.mousePosition);

            _eventData.HasRect = hasRect;
            if (ev.type == EventType.MouseDown)
            {
                if (hasRect)
                {
                    _havePointerDown = true;
                    if (this is IPointerDownHandler pointerDownHandler)
                    {
                        pointerDownHandler.OnPointerDown(_eventData);
                    }
                }
                else
                {
                    _havePointerDown = false;
                }
            }
            else if (ev.type == EventType.MouseUp)
            {
                if (this is IPointerUpHandler pointerUpHandler)
                {
                    pointerUpHandler.OnPointerUp(_eventData);
                }

                if (_isDragging && this is IDragEndHandler dragEndHandler)
                {
                    _isDragging = false;
                    dragEndHandler.OnDragEnd(_eventData);
                }

                if (hasRect)
                {
                    if (_havePointerDown && this is IPointerClickHandler pointerClickHandler)
                    {
                        pointerClickHandler.OnPointerClick(_eventData);
                    }
                }
                else
                {
                    _havePointerDown = false;
                }
            }
            else if (ev.type == EventType.MouseDrag)
            {
                if (hasRect)
                {
                    if (!_isDragging && Vector2.Distance(_dragStartPos, ev.mousePosition) > DragThreshold)
                    {
                        _isDragging = true; // 达到阈值，开始拖动 When the threshold is reached, drag begins
                        if (this is IDragBeginHandler dragBeginHandler)
                        {
                            dragBeginHandler.OnDragBegin(_eventData);
                        }
                    }

                    if (_isDragging && this is IPointerDragHandler dragHandler)
                    {
                        dragHandler.OnPointerDrag(_eventData);
                    }
                }
            }

            // if (ev.type == EventType.MouseMove)
            // {
            //     // if (_havePointerDown)
            //     {
            //         if (this is IPointerMoveHandler pointerMoveHandler)
            //         {
            //             pointerMoveHandler.OnPointerMove(_eventData);
            //         }
            //     }
            // }

            if (this is IPointerEnterHandler pointerEnterHandler)
            {
                if (hasRect && !_isPointerOver)
                {
                    _isPointerOver = true;
                    pointerEnterHandler.OnPointerEnter(_eventData);
                }
            }

            if (this is IPointerExitHandler pointerExitHandler)
            {
                if (!hasRect && _isPointerOver)
                {
                    _isPointerOver = false;
                    pointerExitHandler.OnPointerExit(_eventData);
                }
            }
        }

        #endregion
    }
}