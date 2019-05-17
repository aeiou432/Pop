using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    public SpriteRenderer Tree;
    public Text Score;
    public Text TimeLeft;
    public Bubble ResBubble;
    public List<AudioSource> Audios;
    public List<Bubble> Bubbles;
    public List<Bubble> BubblePool;
    public float BubbleInverval = 2;
    public float LevelInterval = 10;
    public float EndTime = 150;
    public Texture2D tex;

    private float nextTime;
    private float levelTime;
    private float endTime;
    private int numberOnce = 1;
    private int nowLevelTimes = 0;
    private int number = 100000;
    private Block treeStart;

    private static string prefsKey = "number";
    public void Start() {
        this.number = PlayerPrefs.GetInt(prefsKey, this.number);
        this.levelTime = Time.time + this.LevelInterval;
        this.nextTime = Time.time;
        this.endTime = Time.time + this.EndTime;
        this.Score.text = this.number.ToString();
        this.RandomCreate();
        this.treeStart = new Block(new Vector2(GrowDefine.LOCAL_DISPLAY_WIDTH / 2, 0), Vector2.up);
        this.treeStart.isTop = true;
        GlobalValue.StartBlock = this.treeStart;
        this.tex = new Texture2D(GrowDefine.LOCAL_DISPLAY_WIDTH, GrowDefine.LOCAL_DISPLAY_HEIGHT);
        this.Tree.sprite = Sprite.Create(this.tex, new Rect(0, 0, GrowDefine.LOCAL_DISPLAY_WIDTH, GrowDefine.LOCAL_DISPLAY_HEIGHT), new Vector2(0.5f, 0.5f));
        this.Tree.sprite.texture.SetPixels32(GlobalValue.pixels);
        this.Tree.sprite.texture.Apply();
    }
    public void OnApplicationPause() {
        PlayerPrefs.SetInt(prefsKey, this.number);
    }
    // Update is called once per frame
    void Update() {
        /*if (Time.time > this.nextTime) {
            this.RandomCreate();
            this.nextTime += this.BubbleInverval;
        }
        if(this.endTime - Time.time < 30) {
            this.LevelInterval = 2;
        }
        if (Time.time > this.levelTime) {
            this.numberOnce++;
            this.levelTime += this.LevelInterval;
        }*/
        this.TimeLeft.text = (this.endTime - Time.time).ToString("0.00");

        this.treeStart.Grow();
        this.treeStart.Draw();
        this.Tree.sprite.texture.SetPixels32(GlobalValue.pixels);
        this.Tree.sprite.texture.Apply();
    }
    private void RandomCreate() {
        Bubble bubble;
        for (int i = 0; i < this.numberOnce; i++) {
            if (this.BubblePool.Count <= 0) {
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
    }
    private void Recycle(Bubble bubble) {
        this.PlaySound();
        this.number--;
        if (this.nowLevelTimes >= 10) {
            this.numberOnce++;
            this.nowLevelTimes = 0;
        }
        this.Score.text = this.number.ToString();
        this.BubblePool.Add(bubble);
        this.Bubbles.Remove(bubble);
        if (this.Bubbles.Count <= 0) {
            this.nowLevelTimes++;
            this.RandomCreate();
        }
    }
    private void PlaySound() {
        for (int i = 0; i < this.Audios.Count; i++) {
            if (!this.Audios[i].isPlaying) {
                this.Audios[i].Play();
                return;
            }
        }
        AudioSource audio = GameObject.Instantiate<AudioSource>(this.Audios[0], this.Audios[0].transform.parent);
        audio.Play();
        this.Audios.Add(audio);
    }
    private void Failed(Bubble bubble) {
        for(int i = this.Bubbles.Count - 1; i >= 0 ; i--) {
            this.Bubbles[i].End();
            this.BubblePool.Add(this.Bubbles[i]);
            this.Bubbles.RemoveAt(i);
        }
        this.numberOnce = 1;
        this.nowLevelTimes = 0;
        this.RandomCreate();
    }
}
