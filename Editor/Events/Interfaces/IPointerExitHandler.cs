using UnityEngine;

namespace NBC.ActionEditor.Events
{
    public interface IPointerExitHandler
    {
        void OnPointerExit(PointerEventData ev);
    }
}