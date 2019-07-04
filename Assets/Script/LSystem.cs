﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class TreeParam {
    public static float W = 0.707f;
    public static int MaxLevel = 9;
    public static int TopLevel;
    public static RuleBase Rule;
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
        if (this.height >= this.growHeight && this.subs == null && this.level < TreeParam.MaxLevel) {
            int level = this.level + 1;
            if (level > TreeParam.TopLevel) {
                TreeParam.TopLevel = level;
            }
            this.subs = new List<InterNode>();
            TreeParam.Rule.P[this.branchMethod](this);
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
        DrawLine.DrawThickLine(Mathf.RoundToInt(startPoint.x), Mathf.RoundToInt(-startPoint.y), Mathf.RoundToInt(endPoint.x), Mathf.RoundToInt(-endPoint.y), Mathf.RoundToInt(width), ThicknessMod.LINE_THICKNESS_MIDDLE, Color.white);
        float subWidth = width * TreeParam.W;
        if (this.subs == null) {
            return;
        }
        for (int i = 0; i < this.subs.Count - 1; i++) {
            this.subs[i].Draw(endPoint, subWidth);
        }
        this.subs[this.subs.Count - 1].Draw(endPoint, subWidth);
    }
}
public class LSystem {
    private InterNode node;
    private Vector3 nodeStart = new Vector3(300, 0, 0);
    private int length = 150;
    public int GrowNumber;
    public void Init() {
        TreeParam.Rule = new Rule2();
        this.node = new InterNode(this.length);
        this.CountGrowNumber();
        this.node.Draw(this.nodeStart, TreeParam.TopLevel * 1.5f + 1);
    }
    public void Grow() {
        this.GrowNumber--;
        this.node.Grow();
    }
    public void Draw() {
        this.node.Draw(this.nodeStart, TreeParam.TopLevel * 1.5f + 1);
    }
    private void CountGrowNumber() {
        int total = 0;
        float lenth = this.length;
        float maxR = 0;
        for (int i = 0; i < TreeParam.Rule.R.Count; i++) {
            if (TreeParam.Rule.R[i] > maxR) {
                maxR = TreeParam.Rule.R[i];
            }
        }
        for (int i = 0; i <= TreeParam.MaxLevel; i++) {
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
