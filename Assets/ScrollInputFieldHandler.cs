using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollInputFieldHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private ScrollRect scrollRect;
    private ScrollRect GetScrollRect {
        get {
            if ( scrollRect == null)
                scrollRect = GetComponentInParent<ScrollRect>(true);
            return scrollRect;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (GetScrollRect != null)
            GetScrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData) {
        if (GetScrollRect != null)
            GetScrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (GetScrollRect != null)
            GetScrollRect.OnEndDrag(eventData);
    }
}
