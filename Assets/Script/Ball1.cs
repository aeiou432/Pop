using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball1 : MonoBehaviour {
    public CustomButton Button;
    public Animator Vibrate;
    private float vibrateTime;
    public void Start() {
        this.Button.onPointerEnter.AddListener(this.HoverIn);
        this.Button.onPointerExit.AddListener(this.HoverOut);
    }
    public void Update() {
        if (this.vibrateTime != 0 && Time.time > this.vibrateTime) {
            Vibration.Cancel();
            this.vibrateTime = 0;
            this.Vibrate.Play("Idle");
        }
    }
    public void HoverIn() {
        this.vibrateTime = Time.time + 1f;
        Vibration.Vibrate(1000);
        this.Vibrate.Play("Vibrate", 0, 0);
    }
    public void HoverOut() {
        this.vibrateTime = Time.time + 0.1f;
    }
}
