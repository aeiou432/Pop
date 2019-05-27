using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    public SpriteRenderer Tree;
    public Text Score;
    public Text Top;
    public Text Sub;
    public Bubble ResBubble;
    public List<AudioSource> Audios;
    public List<Bubble> Bubbles;
    public List<Bubble> BubblePool;
    public float BubbleInverval = 2;
    public float LevelInterval = 10;
    public Texture2D Tex;
    public Button Play;
    public Button BackGround;
    private int adthresholdNo = 300;
    private int numberOnce = 1;
    private int nowLevelTimes = 0;
    private int number = 0;
    private int topNumber = 0;
    private Block treeStart;
    private bool showAd;
    private int soundIndex;
    private static string touchNumber = "number";
    private static string topHeight = "height";
    private static string subNumber = "subNumber";
    private static string adShow = "adShow";
    private static string AndroidGameId = "3159824";
    private static string iOSGameId = "3159825";
    private static string AdName = "video";
    private string dataPath;
    public void Start() {
#if UNITY_ANDROID
        Monetization.Initialize(AndroidGameId, true);
#else
        Monetization.Initialize(iOSGameId, true);
#endif

        this.dataPath = Application.persistentDataPath + "/save.txt";
        this.LoadData();

        if (this.treeStart == null) {
            this.treeStart = new Block(new Vector2(GrowDefine.LOCAL_DISPLAY_WIDTH / 2, 0), Vector2.up);
            this.treeStart.isTop = true;
            GlobalValue.StartBlock = this.treeStart;
        }
        this.Tex = new Texture2D(GrowDefine.LOCAL_DISPLAY_WIDTH, GrowDefine.LOCAL_DISPLAY_HEIGHT);
        this.Tex.filterMode = FilterMode.Trilinear;
        this.Tree.sprite = Sprite.Create(this.Tex, new Rect(0, 0, GrowDefine.LOCAL_DISPLAY_WIDTH, GrowDefine.LOCAL_DISPLAY_HEIGHT), new Vector2(0.5f, 0.5f));
        this.treeStart.Draw();
        this.Tree.sprite.texture.SetPixels(GlobalValue.pixels);
        this.Tree.sprite.texture.Apply();
        this.OnBackgroundClick();
    }
    public void OnApplicationQuit() {
        this.SaveDate();
    }
    public void OnApplicationFocus(bool hasFocus) {
        if (!hasFocus) {
            this.SaveDate();
        }
    }
    // Update is called once per frame
    void Update() {
        /*this.treeStart.Grow();
        this.treeStart.Draw();
        this.Tree.sprite.texture.SetPixels(GlobalValue.pixels);
        this.Tree.sprite.texture.Apply();*/
    }

    public void OnBackgroundClick() {
        bool active = !this.Play.gameObject.activeSelf;
        this.Play.gameObject.SetActive(active);
        if (this.topNumber > 0) {
            this.Score.text = this.topNumber.ToString();
            this.Score.gameObject.SetActive(active);
            this.Top.gameObject.SetActive(active);
        }
        if (GlobalValue.TreePixel > 0) {
            this.Sub.text = "Tree: " + GlobalValue.TreePixel.ToString();
            this.Sub.gameObject.SetActive(active);
        }
    }
    public void OnPlayClick() {
        if (this.showAd) {
            ShowAdPlacementContent adContent = Monetization.GetPlacementContent(AdName) as ShowAdPlacementContent;
            adContent.Show(PlayGame);
            this.showAd = false;
        }
        else {
            this.PlayGame(ShowResult.Finished);
        }
    }
    private void PlayGame(ShowResult finishState) {
        this.Play.gameObject.SetActive(false);
        this.BackGround.gameObject.SetActive(false);
        this.Score.gameObject.SetActive(true);
        this.Top.gameObject.SetActive(false);
        this.Sub.gameObject.SetActive(true);
        this.Sub.text = "Tree: " + GlobalValue.TreePixel.ToString();
        this.Score.text = this.number.ToString();
        this.RandomCreate();
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
        this.number++;
        this.Score.text = this.number.ToString();
        this.PlaySound();
        this.treeStart.Grow();
        this.treeStart.Draw();
        this.Tree.sprite.texture.SetPixels(GlobalValue.pixels);
        this.Tree.sprite.texture.Apply();
        this.Sub.text = "Tree: " + GlobalValue.TreePixel.ToString();
        this.BubblePool.Add(bubble);
        this.Bubbles.Remove(bubble);
        if (this.Bubbles.Count <= 0) {
            this.nowLevelTimes++;
            if (this.nowLevelTimes >= 10) {
                this.numberOnce++;
                this.nowLevelTimes = 0;
            }
            this.RandomCreate();
        }
    }
    private void PlaySound() {
        if (this.Audios[this.soundIndex].isPlaying) return;
        int chance = Random.Range(0, 100);
        if (this.number > 300 && chance < 10) {
            this.soundIndex = 3;
        }
        else if (this.number > 150 && chance < 25) {
            this.soundIndex = 2;
        }
        else if (this.number > 50 && chance < 50) {
            this.soundIndex = 1;
        }
        else {
            this.soundIndex = 0;
        }
        this.Audios[this.soundIndex].Play();
    }
    private void Failed(Bubble bubble) {
        for(int i = this.Bubbles.Count - 1; i >= 0 ; i--) {
            this.Bubbles[i].End();
            this.BubblePool.Add(this.Bubbles[i]);
            this.Bubbles.RemoveAt(i);
        }
        if(this.number > this.topNumber) {
            this.topNumber = this.number;
            this.Top.gameObject.SetActive(true);
        }
        if (this.number > this.adthresholdNo) {
            this.showAd = true;
        }
        this.number = 0;
        this.numberOnce = 1;
        this.nowLevelTimes = 0;
        this.BackGround.gameObject.SetActive(true);
    }
    private void SaveDate() {
        string save = JsonConvert.SerializeObject(this.treeStart);
        PlayerPrefs.SetInt(touchNumber, this.topNumber);
        PlayerPrefs.SetInt(topHeight, GlobalValue.MaxHeight);
        PlayerPrefs.SetInt(subNumber, GlobalValue.TreePixel);
        PlayerPrefs.SetInt(adShow, this.showAd ? 1 : 0);
        using (StreamWriter sw = new StreamWriter(dataPath)) {
            sw.WriteLine(save);
        }
    }
    private void LoadData() {
        if (File.Exists(dataPath)) {
            using (StreamReader sr = new StreamReader(dataPath)) {
                string data = sr.ReadLine();
                this.treeStart = JsonConvert.DeserializeObject<Block>(data);
            }
            if (this.treeStart != null) {
                this.treeStart.Load();
                GlobalValue.StartBlock = this.treeStart;
            }
            this.topNumber = PlayerPrefs.GetInt(touchNumber, this.topNumber);
            GlobalValue.MaxHeight = PlayerPrefs.GetInt(topHeight, GlobalValue.MaxHeight);
            GlobalValue.TreePixel = PlayerPrefs.GetInt(subNumber, GlobalValue.TreePixel);
            this.showAd = PlayerPrefs.GetInt(adShow, 0) == 1 ? true : false;
        }
    }
}
