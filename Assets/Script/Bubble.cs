using UnityEngine;
using UnityEngine.Events;

public class BubbleEvent : UnityEvent<Bubble> { }
public class Bubble : MonoBehaviour {
    public BubbleEvent OnTouch = new BubbleEvent();
    public BubbleEvent OnMiss = new BubbleEvent();
    public CustomButton Button;
    public AudioSource Audio;
    public AudioSource Audio1;
    public GameObject Light;
    public Sprite Bubble2;
    public GameObject Container;
    private Vector3 target;
    private float enableTime;
    private float endTime;
    private float lightingTime;
    private Vector3 velocity;
    private float floatTime;
    private Vector2 floatVelocity;
    private float missTime;
    void Start() {
        Animator bubbleRotate = this.Button.GetComponent<Animator>();
        bubbleRotate.speed = UnityEngine.Random.Range(0f, 1f);
        this.Button.onPointerEnter.AddListener(this.Hover);
        if (UnityEngine.Random.Range(0, 2) == 1) {
            this.Button.image.sprite = this.Bubble2;
            bubbleRotate.Play("bubble1");
        }
    }
    public void Begin(Vector3 start) {
        this.gameObject.SetActive(true);
        this.Button.interactable = true;
        float scale = UnityEngine.Random.Range(1f, 1.5f);
        this.Button.transform.localScale = new Vector3(scale, scale, 1);
        this.enableTime = Time.time + 0.1f;
        this.Button.gameObject.SetActive(true);
        this.Button.image.raycastTarget = true;
        this.transform.position = start;
        float x = UnityEngine.Random.Range(-150f, 150f);
        float y = UnityEngine.Random.Range(-50, 150f);
        this.target = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z);
        this.target.x += x;
        this.target.y += y;
        this.Audio1.PlayDelayed(UnityEngine.Random.Range(0, 0.1f));
        //this.Audio1.Play();
        this.Light.SetActive(true);
        this.Light.transform.localPosition = Vector3.zero;
        this.missTime = Time.time + 30;
        Rigidbody2D rigidbody = this.GetComponent<Rigidbody2D>();
        rigidbody.AddForce(new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)), ForceMode2D.Impulse);
    }
    public void Update() {
        if (this.Button.IsInteractable()) {
            if (Time.time > this.missTime) {
                this.gameObject.SetActive(false);
                this.OnMiss.Invoke(this);
            }
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
        this.Audio.Play();
        this.Button.interactable = false;
        this.Button.transform.localScale = this.Button.transform.localScale * 1.2f;
        this.endTime = Time.time + 0.05f;

        this.Light.SetActive(true);
        this.Light.transform.localPosition = Vector3.zero;
        this.lightingTime = Time.time + 1f;
        this.target = this.Container.transform.position;
        this.velocity = this.target - this.Light.transform.position;
    }
}
