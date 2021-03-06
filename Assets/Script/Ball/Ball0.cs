﻿using UnityEngine;

public class Ball0 : BallBase {
    const float SCALE_SPEED = 2;
    const float PRESS_TIME = 1;
    public Animator startFlash;
    private float time;
    private bool enable;
    private float stackTime;
    private bool playAudio;
    private float nextFlashTime;
    private float flashTime = -1;
    private float[] randomRange = { 2f, 5f};
    public void Start() {
        this.nextFlashTime = Time.time + Random.Range(randomRange[0], randomRange[1]);
        this.Button.onMouseDownEvent.AddListener(this.PressDown);
        this.Button.onMouseUpEvent.AddListener(this.PressUp);
    }
    public void Update() {
        if (Time.time <= this.flashTime - 0.5 ) {
            this.GetComponent<CanvasGroup>().alpha = this.flashTime - Time.time;
        }
        else if (Time.time <= this.flashTime) {
            this.GetComponent<CanvasGroup>().alpha = 1 - (this.flashTime - Time.time);
        }
        else {
            this.GetComponent<CanvasGroup>().alpha = 1;
        }
        if (Time.time > this.nextFlashTime) {
            this.flashTime = Time.time + 1;
            this.nextFlashTime = Time.time + Random.Range(randomRange[0], randomRange[1]);
        }
        if (!this.enable) {
            if (Time.time > this.time) return;
            float scale = (this.time - Time.time) * SCALE_SPEED + 1;
            this.transform.localScale = this.transform.localScale = new Vector3(scale, scale, 1);
        }
        else {
            float timePast = Time.time - this.time;
            float scale = timePast * SCALE_SPEED + 1;
            this.stackTime += Time.deltaTime;
            this.transform.localScale = this.transform.localScale = new Vector3(scale, scale, 1);
            if (timePast > PRESS_TIME) {
                if (this.playAudio) {
                    this.Audio.Play();
                    this.playAudio = false;
                }
                this.Boo.Invoke(this);
                this.End();
            }
        }
    }

    public override void Reset(BallData data) {
        base.Reset(data);
        this.time = Time.time;
        this.startFlash.Play("BallStart");
    }
    public void PressDown() {
        Vibration.Vibrate(100);
        this.time = Time.time - (this.transform.localScale.x - 1) / SCALE_SPEED;
        this.enable = true;
    }

    public void PressUp() {
        this.time = Time.time + (this.transform.localScale.x - 1) / SCALE_SPEED;
        this.enable = false;
    }
    public void End() {
        this.Button.gameObject.SetActive(false);
        this.enable = false;
    }
}
