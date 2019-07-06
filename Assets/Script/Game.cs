﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class BallData {
    public int Type;
    public Vector2 Pos;
    public bool Enable;
    public void SetRandomData() {
        float screenRate = Screen.width / 720f;
        float edge = screenRate * 150f;
        float randomX = Random.Range(edge, Screen.width - edge);
        float randomY = Random.Range(edge, Screen.height - edge);
        Vector2 touchPosition = new Vector2(randomX, randomY);
        this.Pos = touchPosition;
        this.Enable = true;
    }
}
public class Game : MonoBehaviour {
    public SpriteRenderer Tree;
    public Text Number;
    public AudioSource Audio;
    public List<AudioSource> Audio1;
    public List<AudioSource> Audio2;
    public List<AudioSource> Audio3;
    public Ball ResBall;
    public Bubble ResBubble;
    public Button Play;
    public Button BackGround;
    private List<BallData> ballDatas;
    private List<Ball> balls;
    private List<Ball> ballPool;
    private List<Bubble> bubbles;
    private List<Bubble> bubblePool;
    private int adthresholdNo = 300;
    private int numberOnce = 1;
    private bool showAd;
    private int audioIndex1;
    private int audioIndex2;
    private int audioIndex3;
    private float nextTime;
    private AudioSource nowAudio;
    private static string ruleIndex = "ruleIndex";
    private static string adShow = "adShow";
    private static string AndroidGameId = "3159824";
    private static string iOSGameId = "3159825";
    private static string AdName = "video";
    private string dataPath;

    private LSystem lTree;
    public void Start() {
#if UNITY_ANDROID
        Monetization.Initialize(AndroidGameId, true);
#else
        Monetization.Initialize(iOSGameId, true);
#endif
        this.balls = new List<Ball>();
        this.ballPool = new List<Ball>();
        this.bubbles = new List<Bubble>();
        this.bubblePool = new List<Bubble>();
        this.nowAudio = this.Audio;
        this.dataPath = Application.persistentDataPath + "/save.txt";
        this.LoadData();
        if (this.lTree == null) {
            this.lTree = new LSystem();
            this.lTree.Init();
        }
        this.lTree.Draw();
        if (this.ballDatas == null) {
            this.ballDatas = new List<BallData>();
            for (int i = 0; i < this.numberOnce; i++) {
                BallData data = new BallData();
                data.SetRandomData();
                this.ballDatas.Add(data);
            }
        }
        this.OnBackgroundClick();

        this.Tree.sprite.texture.SetPixels(GlobalValue.pixels);
        this.Tree.sprite.texture.Apply();
    }
    public void OnApplicationQuit() {
        this.SaveDate();
    }
    public void OnApplicationFocus(bool hasFocus) {
        if (!hasFocus) {
            this.SaveDate();
        }
    }
    void Update() {
        /*if (this.lTree.GrowNumber > 0 || Input.GetKeyUp(KeyCode.Mouse0)) {
            this.lTree.Grow();
            this.ClearTexture();
            this.lTree.Draw();
            this.ApplyTexture();
            this.number++;
            this.Score.text = this.lTree.GrowNumber.ToString();
        }*/
    }

    public void OnBackgroundClick() {
        if (this.showAd && Monetization.IsReady(AdName)) {
            ShowAdPlacementContent adContent = Monetization.GetPlacementContent(AdName) as ShowAdPlacementContent;
            adContent.Show(this.BackGroundCB);
            this.showAd = false;
        }
        else {
            this.BackGroundCB(ShowResult.Finished);
        }
    }
    public void OnPlayClick() {
        this.PlayGame(ShowResult.Finished);
    }
    private void BackGroundCB(ShowResult finishState) {
        bool active = !this.Play.gameObject.activeSelf;
        this.Play.gameObject.SetActive(active);
    }
    private void PlayGame(ShowResult finishState) {
        this.Play.gameObject.SetActive(false);
        this.BackGround.gameObject.SetActive(false);
        this.Number.gameObject.SetActive(true);
        this.Number.text = this.lTree.GrowNumber.ToString();
        for (int i = 0; i < this.ballDatas.Count; i++) {
            this.ShowBall(this.ballDatas[i]);
        } 
    }
    public void ShowBall(BallData data) {
        if (!data.Enable) return;
        Ball ball;
        if (this.ballPool.Count <= 0) {
            ball = GameObject.Instantiate<Ball>(this.ResBall, this.transform);
            ball.Boo.AddListener(this.Boo);
        }
        else {
            ball = this.ballPool[0];
            this.ballPool.RemoveAt(0);
        }
        ball.transform.SetAsFirstSibling();
        ball.Reset(data);
        this.balls.Add(ball);
    }
    private void RandomCreateBallData() {
        BallData data = this.ballDatas.Find(this.FindUnusedBallData);
        data.SetRandomData();
        this.ShowBall(data);
    }
    private bool FindUnusedBallData(BallData data) {
        return !data.Enable;
    }
    private void PlaySound() {
        if (this.nowAudio.isPlaying) return;
        this.nowAudio.Play();
    }
    private void SaveDate() {
        string save = JsonConvert.SerializeObject(this.lTree);
        string save1 = JsonConvert.SerializeObject(GlobalValue.Rule);
        PlayerPrefs.SetInt(adShow, this.showAd ? 1 : 0);
        PlayerPrefs.SetInt(ruleIndex, GlobalValue.RuleIndex);
        using (StreamWriter sw = new StreamWriter(dataPath)) {
            sw.WriteLine(save);
            sw.WriteLine(save1);
        }
    }
    private void LoadData() {
        if (File.Exists(dataPath)) {
            this.showAd = PlayerPrefs.GetInt(adShow, 0) == 1 ? true : false;
            GlobalValue.RuleIndex = PlayerPrefs.GetInt(ruleIndex, 0);
            using (StreamReader sr = new StreamReader(dataPath)) {
                string data = sr.ReadLine();
                string data1 = sr.ReadLine();
                this.lTree = JsonConvert.DeserializeObject<LSystem>(data);
                GlobalValue.Rule = RuleManager.Instance.JsonDeserialize(data1, GlobalValue.RuleIndex);
            }

        }
    }
    private void Boo(Ball ball) {
        ball.data.Enable = false;
        ball.data = null;
        this.ballPool.Add(ball);
        this.balls.Remove(ball);
        this.StartCoroutine(this.CreateBubbles(ball.transform.position, 10));
    }
    private IEnumerator CreateBubbles(Vector3 position, int number) {
        Bubble bubble;
        for (int i = 0; i < number; i++) {
            if (this.bubblePool.Count <= 0) {
                bubble = GameObject.Instantiate<Bubble>(this.ResBubble, this.transform);
                bubble.OnTouch.AddListener(this.RecycleBubble);
            }
            else {
                bubble = this.bubblePool[0];
                this.bubblePool.RemoveAt(0);
            }
            bubble.Begin(position);
            this.bubbles.Add(bubble);
            yield return new WaitForSeconds(0.01f);
        }
    }
    private void RecycleBubble(Bubble bubble) {
        this.lTree.Grow();
        this.ClearTexture();
        this.lTree.Draw();
        this.ApplyTexture();
        this.Number.text = this.lTree.GrowNumber.ToString();
        this.bubblePool.Add(bubble);
        this.bubbles.Remove(bubble);
        if(this.bubbles.Count == 0) {
            this.RandomCreateBallData();
        }
    }
    private void ClearTexture() {
        for (int i = 0; i < GlobalValue.fillPixels.Length; i++) {
            if (GlobalValue.fillPixels[i]) {
                GlobalValue.fillPixels[i] = false;
                GlobalValue.pixels[i] = Color.clear;
                this.Tree.sprite.texture.SetPixel(i % GlobalDefine.LOCAL_DISPLAY_WIDTH, i / GlobalDefine.LOCAL_DISPLAY_WIDTH, Color.clear);
            }
        }
    }
    private void ApplyTexture() {
        for (int i = 0; i < GlobalValue.fillPixels.Length; i++) {
            if (GlobalValue.fillPixels[i]) {
                this.Tree.sprite.texture.SetPixel(i % GlobalDefine.LOCAL_DISPLAY_WIDTH, i / GlobalDefine.LOCAL_DISPLAY_WIDTH, GlobalValue.pixels[i]);
                
            }
        }
        this.Tree.sprite.texture.Apply(true);
    }
}
