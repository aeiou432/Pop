using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public static class GlobalDefine {
    public static float W = 0.707f;
    public static int MaxLevel = 9;
    public static int TimeInterval = 10;
    public static int MaxWaterNumber = 100;
    public static int WaterPerFill = 10;
    public static int BallNumberOnce = 5;
    public static int BubbleNumber = 50;
}
public static class GlobalValue {
    public static int TopLevel;
    public static int RuleIndex;
    public static RuleBase Rule;
    public static List<GameObject> LineObjects;
}
public class InterNode {
    public float height = 0;
    public float maxHeight = 0;
    public float growHeight = 0;
    public int order = 0;
    public Vector3 angleVector = Vector3.down;
    public Vector3 forward = Vector3.forward;
    public List<InterNode> subs;
    public int level = 0;
    public int branchMethod = 0;
    private GameObject line;

    public InterNode(float maxHeight, InterNode from = null, bool mainOrder = false) {
        if (from != null) {
            this.angleVector = from.angleVector;
            this.forward = from.forward;
            this.level = from.level + 1;
            if (!mainOrder) {
                this.order = from.order + 1;
            }
        }
        this.maxHeight = maxHeight;
        if (this.level <= 1) {
            this.growHeight = maxHeight / 10;
        }
        else {
            this.growHeight = maxHeight;
        }
        if (GlobalValue.LineObjects.Count <= 1) {
            this.line = GameObject.Instantiate<GameObject>(GlobalValue.LineObjects[0], GlobalValue.LineObjects[0].transform.parent);
        }
        else {
            this.line = GlobalValue.LineObjects[GlobalValue.LineObjects.Count - 1];
            GlobalValue.LineObjects.RemoveAt(GlobalValue.LineObjects.Count -1 );
        }
    }
    public void Grow() {
        if (this.height < this.maxHeight) {
            this.height++;
        }
        if (this.subs != null) {
            for (int i = 0; i < this.subs.Count; i++) {
                this.subs[i].Grow();
            }
        }
        if (this.height >= this.growHeight && this.subs == null && this.level < GlobalDefine.MaxLevel) {
            int level = this.level + 1;
            if (level > GlobalValue.TopLevel) {
                GlobalValue.TopLevel = level;
            }
            this.subs = new List<InterNode>();
            GlobalValue.Rule.P[this.branchMethod](this);
        }
    }
    public void Left(float angle, InterNode from) {
        Quaternion rotate = Quaternion.AngleAxis(angle, from.forward);
        this.angleVector = rotate * from.angleVector;
    }
    public void Down(float angle, InterNode from) {
        Vector3 right = Vector3.Cross(from.angleVector, from.forward);
        Quaternion Rotate = Quaternion.AngleAxis(angle, right);
        this.angleVector = Rotate * from.angleVector;
        this.forward = Rotate * from.forward;
    }
    public void Divergence(float angle, InterNode from) {
        Quaternion Rotate = Quaternion.AngleAxis(angle, from.angleVector);
        this.forward = Rotate * from.forward;
    }
    public void Roll(InterNode from) {
        this.forward = Vector3.Cross(from.angleVector, Vector3.Cross(Vector3.up, from.angleVector));
    }
    public void Draw(Vector3 startPoint, float width) {
        Vector3 endPoint = startPoint + this.angleVector * this.height;
        Vector3 drawStartPoint = startPoint;
        Vector3 drawVector = this.angleVector;
        drawStartPoint.y = -startPoint.y - 500;
        drawVector.y = -this.angleVector.y;
        if (!GlobalValue.Rule.DrawAxis) {
            drawStartPoint.x = startPoint.z;
            drawVector.x = this.angleVector.z;
            drawStartPoint.z = startPoint.x;
            drawVector.z = this.angleVector.x;
        }
        this.line.gameObject.SetActive(true);
        this.line.transform.localPosition = drawStartPoint / 100;
        this.line.transform.localScale = new Vector3((width + 1) / 100, height / 100, 1);
        Quaternion rotate = Quaternion.FromToRotation(Vector3.up, drawVector);
        this.line.transform.localRotation = rotate;

        float subWidth = width * GlobalDefine.W;
        if (this.subs == null) {
            return;
        }
        for (int i = 0; i < this.subs.Count - 1; i++) {
            this.subs[i].Draw(endPoint, subWidth);
        }
        this.subs[this.subs.Count - 1].Draw(endPoint, subWidth);
    }
    public void SetTopLevel() {
        if (this.level > GlobalValue.TopLevel) {
            GlobalValue.TopLevel = this.level;
        }
        if (this.subs != null) {
            for (int i = 0; i < this.subs.Count; i++) {
                this.subs[i].SetTopLevel();
            }
        }
    }
    public void Destroy() {
        if(this.subs != null) {
            for(int i = 0; i < this.subs.Count; i++) {
                this.subs[i].Destroy();
            }
            this.subs.Clear();
            this.subs = null;
        }
        this.line.gameObject.SetActive(false);
        GlobalValue.LineObjects.Add(this.line);
        this.line = null;
    }
}
public class LSystem {
    [JsonProperty] private InterNode node;
    private Vector3 nodeStart = Vector3.zero;
    private int length = 150;
    public int GrowNumber;
    public void Init() {
        GlobalValue.Rule = RuleManager.Instance.RandomPickRule();
        GlobalValue.RuleIndex = RuleManager.Instance.RuleIndex;
        if (this.node != null) {
            this.node.Destroy();
        }
        this.node = new InterNode(this.length);
        this.CountGrowNumber();
    }
    public void Grow() {
        this.GrowNumber--;
        this.node.Grow();
    }
    public void Draw() {
        this.node.Draw(this.nodeStart, GlobalValue.TopLevel * 1.5f + 1);
    }
    public void SetTopLevel() {
        this.node.SetTopLevel();
    }
    private void CountGrowNumber() {
        int total = 0;
        float lenth = this.length;
        float maxR = 0;
        for (int i = 0; i < GlobalValue.Rule.R.Count; i++) {
            if (GlobalValue.Rule.R[i] > maxR) {
                maxR = GlobalValue.Rule.R[i];
            }
        }
        for (int i = 0; i <= GlobalDefine.MaxLevel; i++) {
            if (i <= 1) {
                total += ((int)lenth / 10);
            }
            else {
                total += Mathf.CeilToInt(lenth);
            }
            lenth *= maxR;
        }
        this.GrowNumber = total + 1;
    }
}
