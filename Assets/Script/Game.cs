using Newtonsoft.Json;
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
    public bool Enable;
    public void SetRandomData() {
        float screenRate = Screen.width / 720f;
        float edge = screenRate * 150f;
        float randomX = UnityEngine.Random.Range(edge, Screen.width - edge);
        float randomY = UnityEngine.Random.Range(edge, Screen.height - edge);
        Vector2 touchPosition = new Vector2(randomX, randomY);
        this.Pos = touchPosition;
        this.Enable = true;
    }
}
public class Game : MonoBehaviour {
    public BallManager BallManager;
    public List<GameObject> LineObjtects;
    public Text Number;
    public Text LeftTime;
    public AudioSource Audio;
    public List<AudioSource> Audio1;
    public List<AudioSource> Audio2;
    public List<AudioSource> Audio3;
    public Ball0 ResBall;
    public Bubble ResBubble;
    public Button Reset;
    public Button BackGround;
    public GameObject UIObj;
    private List<BallData> ballDatas;
    private List<BallBase> balls;
    private List<Bubble> bubbles;
    private List<Bubble> bubblePool;
    private int adthresholdNo = 300;
    private int numberOnce = 5;
    private int bubbleNumber = 50;
    private bool textureUpdate = false;
    
    private bool showAd;
    private int audioIndex1;
    private int audioIndex2;
    private int audioIndex3;
    private DateTime nextTime;
    private AudioSource nowAudio;
    private static string ruleIndex = "ruleIndex";
    private static string nextDateTime = "nextDateTime";
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
        this.balls = new List<BallBase>();
        this.bubbles = new List<Bubble>();
        this.bubblePool = new List<Bubble>();
        this.nowAudio = this.Audio;
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
            for (int i = 0; i < this.numberOnce; i++) {
                BallData data = new BallData();
                data.SetRandomData();
                this.ballDatas.Add(data);
            }
        }
        this.OnBackgroundClick();
        this.lTree.Draw();
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
        if (this.textureUpdate) {
            this.lTree.Draw();
            this.HideOrShowNumberAndReset();
            this.textureUpdate = false;
        }
    }
    public void OnBackgroundClick() {
        this.UIObj.gameObject.SetActive(true);
        this.BackGround.gameObject.SetActive(false);
        for (int i = 0; i < this.balls.Count; i++) {
            this.balls[i].gameObject.SetActive(true);
        }
    }
    public void OnResetClick() {
        this.lTree.Init();
        this.lTree.Draw();
        GlobalValue.TopLevel = 0;
        this.HideOrShowNumberAndReset();
    }
    public void OnHideClick() {
        this.UIObj.gameObject.SetActive(false);
        this.BackGround.gameObject.SetActive(true);
        for (int i = 0; i < this.balls.Count; i++) {
            this.balls[i].gameObject.SetActive(false);
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
        this.BackGround.gameObject.SetActive(false);
        this.HideOrShowNumberAndReset();
        for (int i = 0; i < this.ballDatas.Count; i++) {
            this.ShowBall(this.ballDatas[i]);
        }
        if (this.balls.Count < this.numberOnce) {
            this.StartCoroutine(this.CheckTime());
        }
        else {
            this.LeftTime.gameObject.SetActive(false);
        }
    }
    private IEnumerator CheckTime() {
        this.LeftTime.gameObject.SetActive(true);
        while (true) {
            DateTime time = DateTime.UtcNow;
            while (time >= this.nextTime) {
                this.RandomCreateBallData();
                if (this.balls.Count >= this.numberOnce) {
                    this.LeftTime.gameObject.SetActive(false);
                    yield break;
                }
                this.nextTime = this.nextTime.AddSeconds(GlobalDefine.TimeInterval);
            }
            TimeSpan span = this.nextTime - time;
            this.LeftTime.text = span.Minutes + ":" + (span.Seconds + 1).ToString("00");
            yield return new WaitForSeconds(1);
        }
    }
    public void ShowBall(BallData data) {
        if (!data.Enable) return;
        BallBase ball = this.BallManager.GetBall(data, this.Boo);
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
        PlayerPrefs.SetString(nextDateTime, this.nextTime.ToString());
        PlayerPrefs.SetInt(adShow, this.showAd ? 1 : 0);
        PlayerPrefs.SetInt(ruleIndex, GlobalValue.RuleIndex);
        using (StreamWriter sw = new StreamWriter(dataPath)) {
            string save = JsonConvert.SerializeObject(this.lTree);
            sw.WriteLine(save);
            save = JsonConvert.SerializeObject(GlobalValue.Rule);
            sw.WriteLine(save);
            save = JsonConvert.SerializeObject(this.ballDatas);
            sw.WriteLine(save);
        }
    }
    private void LoadData() {
        if (File.Exists(dataPath)) {
            this.nextTime = Convert.ToDateTime(PlayerPrefs.GetString(nextDateTime));
            this.showAd = PlayerPrefs.GetInt(adShow, 0) == 1 ? true : false;
            GlobalValue.RuleIndex = PlayerPrefs.GetInt(ruleIndex, 0);
            using (StreamReader sr = new StreamReader(dataPath)) {
                string data = sr.ReadLine();
                this.lTree = JsonConvert.DeserializeObject<LSystem>(data);
                data = sr.ReadLine();
                GlobalValue.Rule = RuleManager.Instance.JsonDeserialize(data, GlobalValue.RuleIndex);
                data = sr.ReadLine();
                this.ballDatas = JsonConvert.DeserializeObject<List<BallData>>(data);
            }
        }
    }
    private void Boo(BallBase ball) {
        this.BallManager.Recycle(ball);
        this.balls.Remove(ball);
        if (this.balls.Count == this.numberOnce - 1) {
            this.StopCoroutine(this.CheckTime());
            this.nextTime = DateTime.UtcNow.AddSeconds(GlobalDefine.TimeInterval);
            this.StartCoroutine(this.CheckTime());
        }
        this.StartCoroutine(this.CreateBubbles(ball.transform.position, this.bubbleNumber));
    }
    private IEnumerator CreateBubbles(Vector3 position, int number) {
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
            //yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.01f);
    }
    private void RecycleBubble(Bubble bubble) {
        this.bubblePool.Add(bubble);
        this.bubbles.Remove(bubble);
    }
    private void BubbleBoo(Bubble bubble) {
        if (this.lTree.GrowNumber > 0) {
            this.lTree.Grow();
            this.textureUpdate = true;
        }
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
