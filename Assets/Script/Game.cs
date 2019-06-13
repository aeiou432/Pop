using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    public SpriteRenderer Tree;
    public Text Score;
    public Text Sub;
    public AudioSource Audio;
    public List<AudioSource> Audio1;
    public List<AudioSource> Audio2;
    public List<AudioSource> Audio3;
    public Ball ResBall;
    public Bubble ResBubble;
    public float LevelInterval = 10;
    public Texture2D Tex;
    public Button Play;
    public Button BackGround;
    private List<Ball> Balls;
    private List<Ball> BallPool;
    private List<Bubble> Bubbles;
    private List<Bubble> BubblePool;
    private int maxLevel = 50;
    private int adthresholdNo = 300;
    private int numberOnce = 1;
    private int nowLevelTimes = 0;
    private int number = 0;
    private int topNumber = 0;
    private Block treeStart;
    private bool showAd;
    private int audioIndex1;
    private int audioIndex2;
    private int audioIndex3;
    private float nextTime;
    private AudioSource nowAudio;
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
        this.Balls = new List<Ball>();
        this.BallPool = new List<Ball>();
        this.Bubbles = new List<Bubble>();
        this.BubblePool = new List<Bubble>();
        this.nowAudio = this.Audio;
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
        /*this.treeStart.Draw();
        this.Tree.sprite.texture.SetPixels(GlobalValue.pixels);
        this.Tree.sprite.texture.Apply();
        this.OnBackgroundClick();*/
        this.DrawLsystem(this.oldString);
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
    // Update is called once per frame
    void Update() {
        /*if(this.nextTime != 0 && Time.time > this.nextTime) {
            this.RandomCreate();
            this.nextTime = 0;
        }*/
        /*this.treeStart.Grow();
        this.treeStart.Draw();
        this.Tree.sprite.texture.SetPixels(GlobalValue.pixels);
        this.Tree.sprite.texture.Apply();*/

        if(Input.GetKeyUp(KeyCode.Mouse0)) {
            this.LSystemTest();
            this.DrawLsystem(this.oldString);
            this.Tree.sprite.texture.SetPixels(GlobalValue.pixels);
            this.Tree.sprite.texture.Apply();
            this.number++;
            this.Score.text = this.number.ToString();
        }
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
        if (GlobalValue.TreePixel > 0) {
            this.Sub.text = "Tree: " + GlobalValue.TreePixel.ToString();
            this.Sub.gameObject.SetActive(active);
        }
    }
    private void PlayGame(ShowResult finishState) {
        this.Play.gameObject.SetActive(false);
        this.BackGround.gameObject.SetActive(false);
        this.Score.gameObject.SetActive(true);
        this.Sub.gameObject.SetActive(true);
        this.Sub.text = GlobalValue.TreePixel.ToString();
        this.Score.text = this.number.ToString();
        this.RandomCreate();
    }
    private void RandomCreate() {
        Ball ball;
        for (int i = 0; i < this.numberOnce; i++) {
            if (this.BallPool.Count <= 0) {
                ball = GameObject.Instantiate<Ball>(this.ResBall, this.transform);
                ball.OnTouch.AddListener(this.Recycle);
                ball.Boo.AddListener(this.Boo);
            }
            else {
                ball = this.BallPool[0];
                this.BallPool.RemoveAt(0);
            }
            ball.transform.SetAsFirstSibling();
            float screenRate = Screen.width / 720f;
            float edge = screenRate * 150f;
            float randomX = Random.Range(edge, Screen.width - edge);
            float randomY = Random.Range(edge, Screen.height - edge);
            Vector2 touchPosition = new Vector2(randomX, randomY);
            ball.Reset(touchPosition);
            this.Balls.Add(ball);
        }
    }
    private void Recycle(Ball ball) {
        this.number++;
        this.Score.text = this.number.ToString();
        this.PlaySound();
        this.treeStart.Grow();
        this.treeStart.Draw();
        this.Tree.sprite.texture.SetPixels(GlobalValue.pixels);
        this.Tree.sprite.texture.Apply();
        this.Sub.text = GlobalValue.TreePixel.ToString();
        this.BallPool.Add(ball);
        this.Balls.Remove(ball);
        if (this.Balls.Count <= 0) {
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
        for(int i = this.Balls.Count - 1; i >= 0 ; i--) {
            this.Balls[i].End();
            this.BallPool.Add(this.Balls[i]);
            this.Balls.RemoveAt(i);
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
    private void Boo(Ball ball) {
        this.BallPool.Add(ball);
        this.Balls.Remove(ball);
        this.StartCoroutine(this.CreateBubbles(ball.transform.position, 10));
    }
    private IEnumerator CreateBubbles(Vector3 position, int number) {
        Bubble bubble;
        for (int i = 0; i < number; i++) {
            if (this.BubblePool.Count <= 0) {
                bubble = GameObject.Instantiate<Bubble>(this.ResBubble, this.transform);
                bubble.OnTouch.AddListener(this.RecycleBubble);
            }
            else {
                bubble = this.BubblePool[0];
                this.BubblePool.RemoveAt(0);
            }
            bubble.Begin(position);
            this.Bubbles.Add(bubble);
            
                yield return new WaitForSeconds(0.01f);

        }
    }
    private void RecycleBubble(Bubble bubble) {
        this.BubblePool.Add(bubble);
        this.Bubbles.Remove(bubble);
        if(this.Bubbles.Count == 0) {
            this.RandomCreate();
        }
    }
    string oldString = "X";
    //string newString = string.Empty;
    StringBuilder newString = new StringBuilder();
    public void LSystemTest() {
        for (int i = 0; i < this.oldString.Length; i++) {
            char now = this.oldString[i];
            if (now == 'X') {
                //this.newString.Append("F+[[X]-X]-F[-FX]+X");
                //this.newString.Append("F[+X]F[-X]+X");
                //this.newString.Append("F[+X]X[-X]");
                this.newString.Append("F[+X][-X]");
            }
            else if (now == 'F') {
                this.newString.Append("FF");
            }
            else {
                this.newString.Append(now);
            }
        }
        this.oldString = this.newString.ToString();
        this.newString.Clear();
        /*this.oldString = this.oldString.Remove(0, 1);
        if(this.oldString.Length == 0) {
            this.oldString = this.newString;
            this.newString = string.Empty;
        }*/
    }
    struct node {
        public Vector2 angle;
        public Vector2 point;
    }
    Stack<node> nodes = new Stack<node>();
    public void DrawLsystem(string treeString) {
        for (int i = 0; i < GlobalValue.pixels.Length; i++) {
            GlobalValue.pixels[i] = Color.clear;
        }
        Quaternion leftRotate = Quaternion.AngleAxis(-25, Vector3.forward);
        Quaternion rightRotate = Quaternion.AngleAxis(25, Vector3.forward);
        Vector2 angleVector = new Vector2(0, 3);
        Vector2 point = new Vector2(360, 0);
        for (int i = 0; i < treeString.Length; i++) {
            switch (treeString[i]) {
                case 'F': {
                        Vector2 end = point + angleVector;
                        this.DrawLineOverlap(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y),
                            Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y), Overlap.LINE_OVERLAP_NONE, Color.white);
                        point = end;
                        break;
                    }
                case '-': {
                        angleVector = leftRotate * angleVector;
                        break;
                    }
                case '+': {
                        angleVector = rightRotate * angleVector;
                        break;
                    }
                case '[': {
                        nodes.Push(new node() { angle = angleVector, point = point });
                        break;
                    }
                case ']': {
                        node tmp = nodes.Pop();
                        angleVector = tmp.angle;
                        point = tmp.point;
                        break;
                    }
                default:
                    break;
            }
        }
    }
    public void DrawLineOverlap(int aXStart, int aYStart, int aXEnd, int aYEnd, Overlap aOverlap,
        Color aColor) {
        int tDeltaX, tDeltaY, tDeltaXTimes2, tDeltaYTimes2, tError, tStepX, tStepY;

        /*
         * Clip to display size
         */
        if (aXStart >= GrowDefine.LOCAL_DISPLAY_WIDTH) {
            return;
        }
        if (aXStart < 0) {
            return;
        }
        if (aXEnd >= GrowDefine.LOCAL_DISPLAY_WIDTH) {
            aXEnd = GrowDefine.LOCAL_DISPLAY_WIDTH - 1;
        }
        if (aXEnd < 0) {
            aXEnd = 0;
        }
        if (aYStart >= GrowDefine.LOCAL_DISPLAY_HEIGHT) {
            return;
        }
        if (aYStart < 0) {
            return;
        }
        if (aYEnd >= GrowDefine.LOCAL_DISPLAY_HEIGHT) {
            aYEnd = GrowDefine.LOCAL_DISPLAY_HEIGHT - 1;
        }
        if (aYEnd < 0) {
            aYEnd = 0;
        }

        if ((aXStart == aXEnd) || (aYStart == aYEnd)) {
            //horizontal or vertical line -> fillRect() is faster
            this.FillRect(aXStart, aYStart, aXEnd, aYEnd, aColor);
        }
        else {
            //calculate direction
            tDeltaX = aXEnd - aXStart;
            tDeltaY = aYEnd - aYStart;
            if (tDeltaX < 0) {
                tDeltaX = -tDeltaX;
                tStepX = -1;
            }
            else {
                tStepX = +1;
            }
            if (tDeltaY < 0) {
                tDeltaY = -tDeltaY;
                tStepY = -1;
            }
            else {
                tStepY = +1;
            }
            tDeltaXTimes2 = tDeltaX << 1;
            tDeltaYTimes2 = tDeltaY << 1;
            //draw start pixel
            this.DrawPixel(aXStart, aYStart, aColor);
            if (tDeltaX > tDeltaY) {
                // start value represents a half step in Y direction
                tError = tDeltaYTimes2 - tDeltaX;
                while (aXStart != aXEnd) {
                    // step in main direction
                    aXStart += tStepX;
                    if (tError >= 0) {
                        if ((aOverlap & Overlap.LINE_OVERLAP_MAJOR) == Overlap.LINE_OVERLAP_MAJOR) {
                            // draw pixel in main direction before changing
                            this.DrawPixel(aXStart, aYStart, aColor);
                        }
                        // change Y
                        aYStart += tStepY;
                        if ((aOverlap & Overlap.LINE_OVERLAP_MINOR) == Overlap.LINE_OVERLAP_MINOR) {
                            // draw pixel in minor direction before changing
                            this.DrawPixel(aXStart - tStepX, aYStart, aColor);
                        }
                        tError -= tDeltaXTimes2;
                    }
                    tError += tDeltaYTimes2;
                    this.DrawPixel(aXStart, aYStart, aColor);
                }
            }
            else {
                tError = tDeltaXTimes2 - tDeltaY;
                while (aYStart != aYEnd) {
                    aYStart += tStepY;
                    if (tError >= 0) {
                        if ((aOverlap & Overlap.LINE_OVERLAP_MAJOR) == Overlap.LINE_OVERLAP_MAJOR) {
                            // draw pixel in main direction before changing
                            this.DrawPixel(aXStart, aYStart, aColor);
                        }
                        aXStart += tStepX;
                        if ((aOverlap & Overlap.LINE_OVERLAP_MINOR) == Overlap.LINE_OVERLAP_MINOR) {
                            // draw pixel in minor direction before changing
                            this.DrawPixel(aXStart, aYStart - tStepY, aColor);
                        }
                        tError -= tDeltaYTimes2;
                    }
                    tError += tDeltaXTimes2;
                    this.DrawPixel(aXStart, aYStart, aColor);
                }
            }
        }
    }
    public void DrawPixel(int x, int y, Color color) {
        GlobalValue.pixels[x + y * GrowDefine.LOCAL_DISPLAY_WIDTH] = color;
    }
    public void FillRect(int aXStart, int aYStart, int aXEnd, int aYEnd, Color aColor) {
        for (int x = aXStart; x <= aXEnd; x++) {
            for (int y = aYStart; y <= aYEnd; y++) {
                this.DrawPixel(x, y, aColor);
            }
        }
    }
}
