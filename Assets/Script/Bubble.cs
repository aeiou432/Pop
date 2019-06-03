using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BubbleEvent : UnityEvent<Bubble> { }
public class Bubble : MonoBehaviour {
    public BubbleEvent OnTouch = new BubbleEvent();
    public CustomButton Button;
    public AudioSource Audio;
    public AudioSource Audio1;
    public GameObject Light;
    private Vector3 target;
    private float smooth = 0.5f;
    private float enableTime;
    private float endTime;
    private float lightingTime;
    private Vector3 velocity;
    void Start() {
        //this.target = this.transform.localPosition;
        this.Button.onPointerEnter.AddListener(this.Hover);
    }
    public void Begin(Vector3 start) {
        this.gameObject.SetActive(true);
        this.Button.interactable = true;
        this.Button.transform.localScale = Vector3.one;
        this.enableTime = Time.time + 0.1f;
        this.Button.gameObject.SetActive(true);
        this.Button.image.raycastTarget = true;
        this.transform.position = start;
        float x = Random.Range(-150f, 150f);
        float y = Random.Range(0, 50f);
        this.target = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        this.target.x += x;
        this.target.y += y;
        //this.Audio1.PlayDelayed(Random.Range(0, 0.1f));
        this.Audio1.Play();
        this.Light.SetActive(false);
    }
    public void Update() {
        if (this.Button.IsInteractable()) {
            this.transform.position = Vector3.Lerp(this.transform.position, this.target, this.smooth * Time.deltaTime);
        }
        if (this.endTime != 0 && Time.time > this.endTime) {
            this.endTime = 0;
            this.Button.gameObject.SetActive(false);
        }
        if (Time.time < this.lightingTime) {
            this.Light.transform.position = this.Light.transform.position + this.velocity * Time.deltaTime;
        }
        else if(this.lightingTime != 0) {
            this.lightingTime = 0;
            this.OnTouch.Invoke(this);
            this.gameObject.SetActive(false);
        }
    }
    public void Hover() {
        if (!this.Button.IsInteractable()) return;
        if (Time.time < this.enableTime) return;
        this.Button.image.raycastTarget = false;
        Vibration.Vibrate(80);
        this.Audio.Play();
        this.Button.interactable = false;
        this.Button.transform.localScale = this.Button.transform.localScale * 1.2f;
        this.endTime = Time.time + 0.05f;

        this.Light.SetActive(true);
        this.Light.transform.localPosition = Vector3.zero;
        this.lightingTime = Time.time + 1f;
        this.target = new Vector3(Screen.width / 2, 0, 0);
        this.velocity = this.target - this.Light.transform.position;
    }

}
