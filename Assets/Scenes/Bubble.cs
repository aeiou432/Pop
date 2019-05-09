using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BubbleEvent : UnityEvent<Bubble> { }
public class Bubble : MonoBehaviour {
    public BubbleEvent Boo = new BubbleEvent();
    public BubbleEvent OnTouch = new BubbleEvent();
    public AudioSource Audio;
    public CustomButton Button;
    private float time;
    private bool enable;

    public void Start() {
        this.Button.onMouseDownEvent.AddListener(this.Touch);
    }
    public void Update() {
        if (!this.enable) return;
        float scale = Time.time - this.time + 1;
        this.transform.localScale = this.transform.localScale = new Vector3(scale, scale, 1);
        if (scale > 7) {
            this.End();
            this.Boo.Invoke(this);
        }
    }

    public void Touch() {
        this.Audio.Play();
        this.End();
        this.OnTouch.Invoke(this);
    }

    public void Reset(Vector2 position) {
        this.GetComponent<RectTransform>().anchoredPosition = position;
        this.transform.localScale = Vector3.one;
        this.time = Time.time;
        this.transform.position = position;
        this.Button.gameObject.SetActive(true);
        this.enable = true;
    }

    public void End() {
        this.Button.gameObject.SetActive(false);
        this.enable = false;
    }

    public void Stop() {
        this.Button.enabled = false;
    }
}
