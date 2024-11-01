using UnityEngine;

namespace NBC.ActionEditor
{
    public abstract class ItemViewBase : ViewBase
    {
        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            CheckEvent();
        }


        #region Event

        private void CheckEvent()
        {
            // 检测鼠标事件是否发生在 itemRect 内
            if (Event.current.type == EventType.MouseDown && Position.Contains(Event.current.mousePosition))
            {
                // Event.current.Use();
                OnClick(Event.current);
            }
        }

        protected virtual void OnClick(Event ev)
        {
        }

        protected void StopPropagation()
        {
            Event.current.Use();
        }

        #endregion
    }
}