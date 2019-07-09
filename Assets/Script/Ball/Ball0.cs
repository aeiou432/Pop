using UnityEngine;

public class Ball0 : BallBase {
    private float time;
    private bool enable;
    private float stackTime;
    private bool playAudio;

    public void Start() {
        this.Button.onMouseDownEvent.AddListener(this.PressDown);
        this.Button.onMouseUpEvent.AddListener(this.PressUp);
    }
    public void Update() {
        if (!this.enable) {
            if (Time.time > this.time) return;
            float scale = (this.time - Time.time) * 0.25f + 1;
            //this.transform.Rotate(Vector3.forward, Time.deltaTime * scale * 1000);
            this.transform.localScale = this.transform.localScale = new Vector3(scale, scale, 1);
        }
        else {
            float scale = (Time.time - this.time) * 0.25f + 1;
            //this.transform.Rotate(Vector3.forward, Time.deltaTime * scale * 1000);
            this.stackTime += Time.deltaTime;
            if (this.stackTime > 0.1 / scale) {
                Vector2 vec2 = Random.insideUnitCircle * 1f;
                //this.transform.position = new Vector3(this.transform.position.x + vec2.x,
                //    this.transform.position.y + vec2.y, this.transform.position.z);
            }
            this.transform.localScale = this.transform.localScale = new Vector3(scale, scale, 1);
            if (scale > 2.95f && this.playAudio) {
                this.Audio.Play();
                this.playAudio = false;
            }
            if (scale > 3) {
                this.Boo.Invoke(this);
                this.End();
            }
        }
    }

    public override void Reset(BallData data) {
        base.Reset(data);
        this.time = Time.time;
        this.playAudio = true;
    }
    public void PressDown() {
        Vibration.Vibrate(100);
        this.Audio1.Play();
        this.time = Time.time - (this.transform.localScale.x - 1) * 4;
        this.enable = true;
    }

    public void PressUp() {
        //Vibration.Cancel();
        this.Audio1.Stop();
        this.time = Time.time + (this.transform.localScale.x - 1) * 4;
        this.enable = false;
    }
    public void End() {
        this.Audio1.Stop();
        this.Button.gameObject.SetActive(false);
        this.enable = false;
    }
}
