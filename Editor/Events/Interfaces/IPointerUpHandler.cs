using UnityEngine;

namespace NBC.ActionEditor.Events
{
    public interface IPointerUpHandler
    {
        void OnPointerUp(PointerEventData ev);
    }
}