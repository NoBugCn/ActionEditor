using UnityEngine;

namespace NBC.ActionEditor.Events
{
    public interface IPointerClickHandler
    {
        void OnPointerClick(PointerEventData ev);
    }
}