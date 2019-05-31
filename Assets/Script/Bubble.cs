using UnityEngine;
using UnityEngine.Events;

public class BubbleEvent : UnityEvent<Bubble> { }
public class Bubble : MonoBehaviour {
    public BubbleEvent OnTouch = new BubbleEvent();
    public CustomButton Button;
    public AudioSource Audio;
    public AudioSource Audio1;
    public Vector3 target;
    public Texture Splatter;
    private float smooth = 0.5f;
    private float enableTime;
    private float endTime;
    void Start() {
        //this.target = this.transform.localPosition;
        this.Button.onPointerEnter.AddListener(this.Hover);
    }
    public void Begin(Vector3 start) {
        this.Button.interactable = true;
        this.Button.transform.localScale = Vector3.one;
        this.enableTime = Time.time + 1;
        this.Button.gameObject.SetActive(true);
        this.transform.position = start;
        float x = Random.Range(-150f, 150f);
        float y = Random.Range(-150f, 150f);
        this.target = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        this.target.x += x;
        this.target.y += y;
        //this.Audio1.PlayDelayed(Random.Range(0, 0.1f));
        this.Audio1.Play();
    }
    public void Update() {
        this.transform.position = Vector3.Lerp(this.transform.position, this.target, this.smooth * Time.deltaTime);
        if (this.endTime != 0 && Time.time > this.endTime) {
            this.Button.gameObject.SetActive(false);
            this.endTime = 0;
            this.OnTouch.Invoke(this);
        }
    }
    public void Hover() {
        if (!this.Button.IsInteractable()) return;
        if (Time.time < this.enableTime) return;
        Vibration.Vibrate(80);
        this.Audio.Play();
        this.Button.interactable = false;
        this.Button.transform.localScale = this.Button.transform.localScale * 1.2f;
        this.endTime = Time.time + 0.05f;
    }

}
