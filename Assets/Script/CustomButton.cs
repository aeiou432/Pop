using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button {
    public UnityEvent onMouseDownEvent;
    public override void OnPointerDown(PointerEventData data) {
        base.OnPointerDown(data);
        this.onMouseDownEvent.Invoke();
    }
}
