using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    public Text Score;
    public Text TimeLeft;
    public Bubble ResBubble;
    public List<Bubble> Bubbles;
    public List<Bubble> BubblePool;
    public float BubbleInverval = 2;
    public float LevelInterval = 10;
    public float EndTime = 150;
    private float nextTime;
    private float levelTime;
    private float endTime;
    private int numberOnce = 1;
    private int score = 0;
    public void Start() {
        this.levelTime = Time.time + this.LevelInterval;
        this.nextTime = Time.time;
        this.endTime = Time.time + this.EndTime;
        this.Score.text = this.score.ToString();
    }

    // Update is called once per frame
    void Update() {
        if (Time.time > this.nextTime) {
            Bubble bubble;
            for (int i = 0; i < this.numberOnce; i++) {
                if (BubblePool.Count <= 0) {
                    bubble = GameObject.Instantiate<Bubble>(this.ResBubble, this.transform);
                    bubble.OnTouch.AddListener(this.Recycle);
                    bubble.Boo.AddListener(this.Failed);
                }
                else {
                    bubble = this.BubblePool[0];
                    this.BubblePool.RemoveAt(0);
                }
                bubble.transform.SetAsFirstSibling();
                float randomX = Random.Range(50f, Screen.width - 50f);
                float randomY = Random.Range(50, Screen.height - 50f);
                Vector2 touchPosition = new Vector2(randomX, randomY);
                bubble.Reset(touchPosition);
                this.Bubbles.Add(bubble);
            }
            this.nextTime += this.BubbleInverval;
        }
        if(this.endTime - Time.time < 30) {
            this.LevelInterval = 2;
        }
        if (Time.time > this.levelTime) {
            //this.BubbleInverval = this.BubbleInverval * 0.8f;
            this.numberOnce++;
            this.levelTime += this.LevelInterval;
        }
        this.TimeLeft.text = (this.endTime - Time.time).ToString("0.00");
        //for (var i = 0; i < Input.touchCount; ++i) {
        //    var wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
        //    var touchPosition = new Vector2(wp.x, wp.y);

        //    if (Physics2D.OverlapPoint(touchPosition) != null) {
        //        Debug.Log("HIT!");
        //    }
        //    else {
        //        Debug.Log("MISS");
        //    }
        //}
    }

    private void Recycle(Bubble bubble) {
        this.score += 10;
        this.Score.text = this.score.ToString();
        this.BubblePool.Add(bubble);
        this.Bubbles.Remove(bubble);
    }

    private void Failed(Bubble bubble) {
        Time.timeScale = 0;
        for(int i = 0; i < this.Bubbles.Count; i++) {
            this.Bubbles[i].Stop();
        }
    }
}
