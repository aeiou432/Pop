using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalDefine {
    public static float W = 0.707f;
    public static int MaxLevel = 9;
    public static int TimeInterval = 20;
    public static float DrawPoint = 300;
    public static int LOCAL_DISPLAY_WIDTH = 600;
    public static int LOCAL_DISPLAY_HEIGHT = 1000;
    public static string ResBallPath = "Assets/Prefabs/";
}
public static class GlobalValue {
    public static int TopLevel;
    public static int RuleIndex;
    public static RuleBase Rule;
    public static Color[] pixels = new Color[GlobalDefine.LOCAL_DISPLAY_WIDTH * GlobalDefine.LOCAL_DISPLAY_HEIGHT];
    public static bool[] fillPixels = new bool[GlobalDefine.LOCAL_DISPLAY_WIDTH * GlobalDefine.LOCAL_DISPLAY_HEIGHT];
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
        float startPointX, endPointX;
        if (GlobalValue.Rule.DrawAxis) {
            startPointX = startPoint.x;
            endPointX = endPoint.x;
        }
        else {
            startPointX = startPoint.z;
            endPointX = endPoint.z;
        }
        DrawLine.DrawThickLine(Mathf.RoundToInt(startPointX + GlobalDefine.DrawPoint), Mathf.RoundToInt(-startPoint.y),
                Mathf.RoundToInt(endPointX + GlobalDefine.DrawPoint), Mathf.RoundToInt(-endPoint.y), Mathf.RoundToInt(width),
                ThicknessMod.LINE_THICKNESS_MIDDLE, Color.white);
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
}
public class LSystem {
    [JsonProperty] private InterNode node;
    private Vector3 nodeStart = Vector3.zero;
    private int length = 150;
    public int GrowNumber;
    public void Init() {
        GlobalValue.Rule = RuleManager.Instance.RandomPickRule();
        GlobalValue.RuleIndex = RuleManager.Instance.RuleIndex;
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
