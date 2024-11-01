using UnityEngine;

namespace NBC.ActionEditor.Events
{
    public class PointerEventData
    {
        private Event _event;

        public bool HasRect;

        public Vector2 MousePosition => _event != null ? _event.mousePosition : Vector2.zero;

        public PointerEventData()
        {
        }

        public PointerEventData(Event ev)
        {
            SetEvent(ev);
        }

        public void SetEvent(Event ev)
        {
            _event = ev;
        }

        public void StopPropagation()
        {
            _event.Use();
        }

        public bool IsRight()
        {
            return _event.button == 1;
        }

        public bool IsMiddle()
        {
            return _event.button == 2;
        }

        public bool IsLeft()
        {
            return _event.button == 0;
        }
    }
}