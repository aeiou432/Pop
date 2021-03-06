﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class BallData {
    public BallType Type;
    public Vector2 Pos;
    public void SetRandomData() {
        float screenRate = Screen.width / 720f;
        float edge = screenRate * 150f;
        float randomX = UnityEngine.Random.Range(edge, Screen.width - edge);
        float randomY = UnityEngine.Random.Range(edge, Screen.height - edge);
        Vector2 touchPosition = new Vector2(randomX, randomY);
        this.Pos = touchPosition;
    }
}
public class Game : MonoBehaviour {
    public BallManager BallManager;
    public List<GameObject> LineObjtects;
    public Text Number;
    public Text LeftTime;
    public Ball0 ResBall;
    public Bubble ResBubble;
    public Button Reset;
    public GameObject UIObj;
    public Gardener Gardener;
    public Image Container;
    private List<BallData> ballDatas;
    private List<BallBase> balls;
    private List<Bubble> bubbles;
    private List<Bubble> bubblePool;
    private int totalWater;
    private int waterQueueToTree;
    private int adthresholdNo = 300;
    
    private bool showAd;
    private int audioIndex1;
    private int audioIndex2;
    private int audioIndex3;
    private DateTime nextTime;
    private static string ruleIndex = "ruleIndex";
    private static string nextDateTime = "nextDateTime";
    private static string adShow = "adShow";
    private static string androidGameId = "3159824";
    private static string iOSGameId = "3159825";
    private static string adName = "video";
    private static string waterNumber = "waterNumber";
    private string dataPath;

    private LSystem lTree;
    public void Start() {
#if UNITY_ANDROID
        //Monetization.Initialize(androidGameId, true);
#else
        //Monetization.Initialize(iOSGameId, true);
#endif
        this.balls = new List<BallBase>();
        this.bubbles = new List<Bubble>();
        this.bubblePool = new List<Bubble>();
        this.dataPath = Application.persistentDataPath + "/save.txt";
        GlobalValue.LineObjects = this.LineObjtects;
        this.LoadData();
        if (this.lTree == null) {
            this.lTree = new LSystem();
            this.lTree.Init();
        }
        else {
            this.lTree.SetTopLevel();
        }
        if (this.ballDatas == null) {
            this.ballDatas = new List<BallData>();
        }
        this.lTree.Draw();
        this.Container.fillAmount = (float)this.totalWater / (float)GlobalDefine.MaxWaterNumber;
        if (this.totalWater <= 0) {
            this.Gardener.NoWater = true;
        }
        this.PlayGame();
    }
    public void OnApplicationQuit() {
        this.SaveDate();
    }
    public void OnApplicationFocus(bool hasFocus) {
        if (!hasFocus) {
            this.SaveDate();
        }
    }
    public void FixedUpdate() {
        if (this.waterQueueToTree > 0) {
            this.waterQueueToTree--;
            if (this.lTree.GrowNumber > 0) {
                this.lTree.Grow();
                this.lTree.Draw();
                this.HideOrShowNumberAndReset();
            }
        }
    }
    public void OnResetClick() {
        this.lTree.Init();
        this.lTree.Draw();
        GlobalValue.TopLevel = 0;
        this.HideOrShowNumberAndReset();
    }
    public void OnGardenerFilled() {
        this.Gardener.WaterNumber =
            this.totalWater < GlobalDefine.WaterPerFill ? this.totalWater : GlobalDefine.WaterPerFill;
        this.totalWater -= this.Gardener.WaterNumber;
        this.Container.fillAmount = (float)this.totalWater / (float)GlobalDefine.MaxWaterNumber ;
    }
    public void OnGardenerWatered() {
        this.waterQueueToTree += this.Gardener.WaterNumber;
        this.Gardener.WaterNumber = 0;
        if (this.totalWater <= 0) {
            this.Gardener.NoWater = true;
        }
    }
    private void ShowAd() {
        /*if (this.showAd && Monetization.IsReady(AdName)) {
            ShowAdPlacementContent adContent = Monetization.GetPlacementContent(AdName) as ShowAdPlacementContent;
            adContent.Show(this.BackGroundCB);
            this.showAd = false;
        }*/
    }
    private void PlayGame() {
        this.HideOrShowNumberAndReset();
        if (this.balls.Count < GlobalDefine.BallNumberOnce) {
            this.StartCoroutine(this.CheckTime());
        }
        else {
            this.LeftTime.gameObject.SetActive(false);
        }
    }
    private IEnumerator CheckTime() {
        while (true) {
            DateTime time = DateTime.Now;
            if (this.ballDatas.Count < GlobalDefine.BallNumberOnce) {
                this.LeftTime.gameObject.SetActive(true);
                while (time >= this.nextTime) {
                    BallData data = new BallData();
                    data.SetRandomData();
                    this.ballDatas.Add(data);
                    if (this.ballDatas.Count >= GlobalDefine.BallNumberOnce) {
                        this.LeftTime.gameObject.SetActive(false);
                        break;
                    }
                    this.nextTime = this.nextTime.AddSeconds(GlobalDefine.TimeInterval);
                }
                TimeSpan span = this.nextTime - time;
                this.LeftTime.text = span.Minutes + ":" + (span.Seconds + 1).ToString("00");
            }

            if (this.balls.Count < this.ballDatas.Count) {
                ShowBall();
            }
            yield return new WaitForSeconds(1);
        }
    }
    private void ShowBall() {
        this.StopCoroutine(this.StartShowBall());
        this.StartCoroutine(this.StartShowBall());
    }
    private IEnumerator StartShowBall() {
        for (int i = this.balls.Count; i < this.ballDatas.Count; i++) {
            BallData data = this.ballDatas[i];
            BallBase ball = this.BallManager.GetBall(data, this.Boo);
            this.balls.Add(ball);
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void SaveDate() {
        PlayerPrefs.SetString(nextDateTime, this.nextTime.ToString("o")) ;
        PlayerPrefs.SetInt(adShow, this.showAd ? 1 : 0);
        PlayerPrefs.SetInt(ruleIndex, GlobalValue.RuleIndex);
        PlayerPrefs.SetInt(waterNumber, this.totalWater);
        PlayerPrefs.Save();
        using (StreamWriter sw = new StreamWriter(dataPath)) {
            string save = JsonConvert.SerializeObject(this.lTree);
            sw.WriteLine(save);

            string[] actionStrings = ActionManager.Instance.Save();
            sw.WriteLine(actionStrings[0]);
            sw.WriteLine(actionStrings[1]);

            save = JsonConvert.SerializeObject(this.ballDatas);
            sw.WriteLine(save);
        }
    }
    private void LoadData() {
        if (File.Exists(dataPath)) {
            this.nextTime = Convert.ToDateTime(PlayerPrefs.GetString(nextDateTime));
            this.showAd = PlayerPrefs.GetInt(adShow, 0) == 1 ? true : false;
            GlobalValue.RuleIndex = PlayerPrefs.GetInt(ruleIndex, 0);
            this.totalWater = PlayerPrefs.GetInt(waterNumber, 0);
            using (StreamReader sr = new StreamReader(dataPath)) {
                string data = sr.ReadLine();
                this.lTree = JsonConvert.DeserializeObject<LSystem>(data);

                string[] actionStrings = new string[2];
                actionStrings[0] = sr.ReadLine();
                actionStrings[1] = sr.ReadLine();
                ActionManager.Instance.Load(actionStrings);

                data = sr.ReadLine();
                this.ballDatas = JsonConvert.DeserializeObject<List<BallData>>(data);
            }
        }
    }
    private void Boo(BallBase ball) {
        this.ballDatas.Remove(ball.data);
        this.BallManager.Recycle(ball);
        this.balls.Remove(ball);
        if (this.ballDatas.Count == GlobalDefine.BallNumberOnce - 1) {
            this.StopCoroutine(this.CheckTime());
            this.nextTime = DateTime.Now.AddSeconds(GlobalDefine.TimeInterval);
            this.StartCoroutine(this.CheckTime());
        }
        this.CreateBubbles(ball.transform.position, GlobalDefine.BubbleNumber);
    }
    private void CreateBubbles(Vector3 position, int number) {
        Bubble bubble;
        for (int i = 0; i < number; i++) {
            if (this.bubblePool.Count <= 0) {
                bubble = GameObject.Instantiate<Bubble>(this.ResBubble, this.transform);
                bubble.OnTouch.AddListener(this.BubbleBoo);
                bubble.OnMiss.AddListener(this.RecycleBubble);
            }
            else {
                bubble = this.bubblePool[0];
                this.bubblePool.RemoveAt(0);
            }
            bubble.Begin(position);
            this.bubbles.Add(bubble);
        }
    }
    private void RecycleBubble(Bubble bubble) {
        this.bubblePool.Add(bubble);
        this.bubbles.Remove(bubble);
    }
    private void BubbleBoo(Bubble bubble) {
        this.totalWater++;
        this.Gardener.NoWater = false;
        this.Container.fillAmount = (float)this.totalWater / (float)GlobalDefine.MaxWaterNumber;
        this.RecycleBubble(bubble);
    }
    private void HideOrShowNumberAndReset() {
        if (this.lTree.GrowNumber > 0) {
            //this.Reset.gameObject.SetActive(false);
            this.Number.gameObject.SetActive(true);
            this.Number.text = this.lTree.GrowNumber.ToString();
        }
        else {
            this.Reset.gameObject.SetActive(true);
            this.Number.gameObject.SetActive(false);
        }
    }
}
