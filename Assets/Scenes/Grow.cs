using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowDefine {
    public static int beginHeightRate = 1;
    public static int endHeightRate = 199;
    public static int lightLimit = 600;
    public static int subChance = 10;
    public static int[] angles = new int[] { -60, 0, 60 };
    public static int angleRange = 10;}

public static class GlobalValue {
    public static int MaxHeight;
    public static Block StartBlock;
}
public class Block {
    int height = 0;
    int width = 0;
    int light;
    float growChance = 1;
    public int depth = 0;
    public Vector2 angleVector;
    public bool isTop = false;
    Vector2 start;
    Block[] subs = new Block[3];
    Block root;

    public Block(Vector2 start, float growChance, Vector2 angleVector) {
        this.start = start;
        this.growChance = growChance;
        this.angleVector = angleVector;
    }
    public void Grow() {
        if(this.light > GrowDefine.lightLimit) {
            if(this.subs[0] != null) {
                this.subs[0].Die();
            }
            if (this.subs[2] != null) {
                this.subs[2].Die();
            }
        }
        for(int i = 0; i < this.subs.Length; i++) {
            if (this.subs[i] != null) {
                this.subs[i].Grow();
            }
        }
        if (Random.Range(0, this.growChance) >= 1) return;
        this.height++;
        if(this.isTop) {
            GlobalValue.MaxHeight++;
            GlobalValue.StartBlock.SetLight();
        }
        this.growChance += 0.2f;
        if (Random.Range(0, 100) >= GrowDefine.subChance) return;

        for(int i = 0; i < this.subs.Length; i++) {
            int angle = Random.Range(GrowDefine.angles[i] - GrowDefine.angleRange, GrowDefine.angles[i] + GrowDefine.angleRange + 1);
            Quaternion rotate = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector2 angleVector = rotate * this.angleVector;
            Vector2 end = this.start + this.angleVector * this.height;
            Block newBlock = new Block(end, this.growChance, angleVector);
            newBlock.root = this;
            this.subs[i] = newBlock;
        }
        if(this.isTop) {
            this.subs[1].isTop = true;
        }
        this.depth++;
        this.width = depth * 2 + 1;
        this.root.ReCalDepth();
    }
    public void ReCalDepth() {
        for (int i = 0; i < this.subs.Length; i++) {
            if (this.subs[i] != null) {
                if (this.subs[i].depth >= this.depth) {
                    this.depth = this.subs[i].depth + 1;
                }
            }
        }
        this.width = depth * 2 + 1;
        this.root.ReCalDepth();
    }
    public void SetLight() {
        this.light = GlobalValue.MaxHeight - (int)this.start.y;
        for (int i = 0; i < this.subs.Length; i++) {
            if (this.subs[i] != null) {
                this.subs[i].SetLight();
            }
        }
    }
    public void Die() {
        for(int i = 0; i < this.subs.Length; i++) {
            if(this.subs[i] != null) {
                this.subs[i].Die();
                this.subs[i] = null;
            }
        }
        this.root = null;
    }
}
