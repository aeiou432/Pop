using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public static Color32[] pixels = new Color32[500 * 1000];
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
    public void Draw() {
       
    }

    public void line(int x0, int x1, int y0, int y1, Color32[] pixels) {
        bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
        int tmp;
        if (steep) {
            tmp = x0;
            x0 = y0;
            y0 = tmp;
            tmp = x1;
            x1 = y1;
            y1 = tmp;
        }
        if (x0 > x1) {
            tmp = x0;
            x0 = x1;
            x1 = tmp;
            tmp = y0;
            y0 = y1;
            y1 = tmp;
        }
        int deltax = x1 - x0;
        int deltay = Mathf.Abs(y1 - y0);
        float error = 0;
        float delttaerr = deltay / deltax;
        int ystep;
        int y = y0;
        if (y0 < y1) ystep = 1; else ystep = -1;
        for (int x = x0; x <= x1; x++) {
            if (steep) pixels[y + x * 500] = Color.white; else pixels[x + y * 500] = Color.white;
            error = error + delttaerr;
            if (error >= 0.5) {
                y = y + ystep;
                error = error - 1.0f;
            }
        }
    }
}
