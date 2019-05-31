using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button {
    public UnityEvent onMouseDownEvent;
    public UnityEvent onMouseUpEvent;
    public UnityEvent onPointerEnter;
    public override void OnPointerDown(PointerEventData data) {
        base.OnPointerDown(data);
        this.onMouseDownEvent.Invoke();
    }
    public override void OnPointerUp(PointerEventData data) {
        base.OnPointerUp(data);
        this.onMouseUpEvent.Invoke();
    }
    public override void OnPointerEnter(PointerEventData data) {
        base.OnPointerEnter(data);
        this.onPointerEnter.Invoke();
    }
}
