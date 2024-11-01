using UnityEngine;

namespace NBC.ActionEditor.Events
{
    public interface IPointerDownHandler
    {
        void OnPointerDown(PointerEventData ev);
    }
}