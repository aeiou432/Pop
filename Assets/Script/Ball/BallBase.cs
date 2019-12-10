using UnityEngine;
using UnityEngine.Events;

public class BallEvent : UnityEvent<BallBase> { }
public enum BallType {
    Balloon = 0,
    Cut = 1,
    Click = 2,
    //自體旋轉
    //彈射
    //放大
}
public class BallBase : MonoBehaviour {
    public BallEvent Boo = new BallEvent();
    public CustomButton Button;
    public AudioSource Audio;
    public BallData data;

    public virtual void Reset(BallData data) {
        this.data = data;
        this.GetComponent<RectTransform>().anchoredPosition = data.Pos;
        this.transform.localScale = Vector3.one;
        this.transform.position = data.Pos;
        this.Button.gameObject.SetActive(true);
    }
}
