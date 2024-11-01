using UnityEngine;

namespace NBC.ActionEditor.Events
{
    public interface IPointerEnterHandler
    {
        void OnPointerEnter(PointerEventData ev);
    }
}