using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    public SpriteRenderer Tree;
    public Text Score;
    public AudioSource Audio;
    public List<AudioSource> Audio1;
    public List<AudioSource> Audio2;
    public List<AudioSource> Audio3;
    public Ball ResBall;
    public Bubble ResBubble;
    public float LevelInterval = 10;
    public Button Play;
    public Button BackGround;
    private List<Ball> balls;
    private List<Ball> ballPool;
    private List<Bubble> bubbles;
    private List<Bubble> bubblePool;
    private int maxLevel = 50;
    private int adthresholdNo = 300;
    private int numberOnce = 1;
    private int nowLevelTimes = 0;
    private int number = 0;
    private int topNumber = 0;
    private bool showAd;
    private int audioIndex1;
    private int audioIndex2;
    private int audioIndex3;
    private float nextTime;
    private AudioSource nowAudio;
    private static string touchNumber = "number";
    private static string topHeight = "height";
    private static string subNumber = "subNumber";
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
        if (this.topNumber > 0) {
            this.Score.text = "Top: " + this.topNumber.ToString();
            this.Score.gameObject.SetActive(active);
        }
    }
    private void PlayGame(ShowResult finishState) {
        this.Play.gameObject.SetActive(false);
        this.BackGround.gameObject.SetActive(false);
        this.Score.gameObject.SetActive(true);
        this.Score.text = this.lTree.GrowNumber.ToString();
        this.RandomCreate();
    }
    private void RandomCreate() {
        Ball ball;
        for (int i = 0; i < this.numberOnce; i++) {
            if (this.ballPool.Count <= 0) {
                ball = GameObject.Instantiate<Ball>(this.ResBall, this.transform);
                ball.OnTouch.AddListener(this.Recycle);
                ball.Boo.AddListener(this.Boo);
            }
            else {
                ball = this.ballPool[0];
                this.ballPool.RemoveAt(0);
            }
            ball.transform.SetAsFirstSibling();
            float screenRate = Screen.width / 720f;
            float edge = screenRate * 150f;
            float randomX = Random.Range(edge, Screen.width - edge);
            float randomY = Random.Range(edge, Screen.height - edge);
            Vector2 touchPosition = new Vector2(randomX, randomY);
            ball.Reset(touchPosition);
            this.balls.Add(ball);
        }
    }
    private void Recycle(Ball ball) {
        this.PlaySound();
        this.ballPool.Add(ball);
        this.balls.Remove(ball);
        if (this.balls.Count <= 0) {
            this.nowLevelTimes++;
            if (this.nowLevelTimes >= 10 && this.numberOnce < this.maxLevel) {
                this.numberOnce++;
                this.nowLevelTimes = 0;
            }
            this.nextTime = Time.time + (this.numberOnce - 1) * 0.1f;
        }
    }
    private void PlaySound() {
        if (this.nowAudio.isPlaying) return;
        int chance = Random.Range(0, 100);
        if (this.number > 300 && chance < 10) {
            this.nowAudio = this.Audio3[this.audioIndex3];
            this.audioIndex3 = (this.audioIndex3 == 0) ? 1 : 0;
        }
        else if (this.number > 150 && chance < 25) {
            this.nowAudio = this.Audio2[this.audioIndex2];
            this.audioIndex2 = (this.audioIndex2 == 0) ? 1 : 0;
        }
        else if (this.number > 50 && chance < 50) {
            this.nowAudio = this.Audio1[this.audioIndex1];
            this.audioIndex1 = (this.audioIndex1 == 0) ? 1 : 0;
        }
        else {
            this.nowAudio = this.Audio;
        }
        this.nowAudio.Play();
    }
    private void Failed(Ball ball) {
        for(int i = this.balls.Count - 1; i >= 0 ; i--) {
            this.balls[i].End();
            this.ballPool.Add(this.balls[i]);
            this.balls.RemoveAt(i);
        }
        if(this.number > this.topNumber) {
            this.topNumber = this.number;
        }
        if (this.number > this.adthresholdNo) {
            this.showAd = true;
        }
        this.number = 0;
        this.numberOnce = 1;
        this.nowLevelTimes = 0;
        //this.BackGround.gameObject.SetActive(true);
    }
    private void SaveDate() {
        string save = JsonConvert.SerializeObject(this.lTree);
        string save1 = JsonConvert.SerializeObject(GlobalValue.Rule);
        PlayerPrefs.SetInt(touchNumber, this.topNumber);
        PlayerPrefs.SetInt(adShow, this.showAd ? 1 : 0);
        PlayerPrefs.SetInt(ruleIndex, GlobalValue.RuleIndex);
        using (StreamWriter sw = new StreamWriter(dataPath)) {
            sw.WriteLine(save);
            sw.WriteLine(save1);
        }
    }
    private void LoadData() {
        if (File.Exists(dataPath)) {
            this.topNumber = PlayerPrefs.GetInt(touchNumber, this.topNumber);
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
        this.number++;
        this.lTree.Grow();
        this.ClearTexture();
        this.lTree.Draw();
        this.ApplyTexture();
        this.Score.text = this.lTree.GrowNumber.ToString();
        this.bubblePool.Add(bubble);
        this.bubbles.Remove(bubble);
        if(this.bubbles.Count == 0) {
            this.RandomCreate();
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
