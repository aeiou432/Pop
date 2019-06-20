using UnityEngine;

public class GrowDefine {
    public static int beginHeightRate = 1;
    public static int endHeightRate = 199;
    public static int lightLimit = 1200;
    public static int minHeight = 50;
    public static int subChance = 10;
    public static int[] angles = new int[] { -60, 0, 60 };
    public static int angleRange = 20;
    public static int LOCAL_DISPLAY_WIDTH = 600;
    public static int LOCAL_DISPLAY_HEIGHT = 1000;
    public static Color TreeColor = new Color(1f, 1f, 1f, 1);
    public static Color ColorGrad = new Color(0.03f, 0.03f, 0.03f, 0);
}

public static class GlobalValue {
    public static int MaxHeight;
    public static int TreePixel;
    public static Block StartBlock;
    public static Color[] pixels = new Color[GrowDefine.LOCAL_DISPLAY_WIDTH * GrowDefine.LOCAL_DISPLAY_HEIGHT];
}
public enum Overlap {
    LINE_OVERLAP_NONE = 0x000,
    LINE_OVERLAP_MAJOR = 0x001,
    LINE_OVERLAP_MINOR = 0x002,
}
public enum ThicknessMod {
    LINE_THICKNESS_MIDDLE,
    LINE_THICKNESS_DRAW_CLOCKWISE,
    LINE_THICKNESS_DRAW_COUNTERCLOCKWISE
}
public class Block {
    public int height = 0;
    public int width = 3;
    public int light;
    public bool end = false;
    public float growChanceRate;
    public int level = 0;
    public int depth = 0;
    public Vector2 angleVector;
    public bool isTop = false;
    public Vector2 start;
    public Block[] subs = new Block[3];
    Block root;

    public Block(Vector2 start, Vector2 angleVector, float growChanceRate = 1f) {
        this.start = start;
        this.growChanceRate = growChanceRate;
        this.angleVector = angleVector;
    }
    public bool Grow() {
        if (this.light > GrowDefine.lightLimit && (this.subs[0] != null || this.subs[2] != null)) {
            if (this.subs[0] != null) {
                this.subs[0].Die();
                this.subs[0] = null;
            }
            if (this.subs[2] != null) {
                this.subs[2].Die();
                this.subs[2] = null;
            }
            for (int i = 0; i < GlobalValue.pixels.Length; i++) {
                GlobalValue.pixels[i] = Color.clear;
            }
        }
        if (this.end) return false;
        Vector2 endPoint = this.start + this.angleVector * this.height;
        if (endPoint.x < 0 || endPoint.x >= GrowDefine.LOCAL_DISPLAY_WIDTH ||
            endPoint.y < 0 || endPoint.y >= GrowDefine.LOCAL_DISPLAY_HEIGHT) {
            this.end = true;
            return false;
        }
        bool grow = false;
        for (int i = 0; i < this.subs.Length; i++) {
            if (this.subs[i] != null) {
                if(this.subs[i].Grow()) {
                    grow = true;
                }
            }
        }
        if (this.subs[1] != null) return grow;
        int height = GlobalValue.MaxHeight >= 150 ? GlobalValue.MaxHeight - 150 : 0;
        float growChance = (height * 0.1f + 1) * this.growChanceRate;
        //float growChance = this.growChanceRate;
        if (Random.Range(0, growChance) >= 1) return grow;
        this.height++;
        GlobalValue.TreePixel++;
        if (this.isTop) {
            GlobalValue.MaxHeight++;
            GlobalValue.StartBlock.SetLight();
        }
        if (this.height < GrowDefine.minHeight) return true;
        if (Random.Range(0, 100) >= GrowDefine.subChance) return true;

        for (int i = 0; i < this.subs.Length; i++) {
            int leftAngle = GrowDefine.angles[i] - GrowDefine.angleRange;
            int rightAngle = GrowDefine.angles[i] + GrowDefine.angleRange + 1;
            if (this.isTop && this.angleVector.x > 0) {
                leftAngle = GrowDefine.angles[i] - GrowDefine.angleRange / 2;
            }
            else if (this.isTop && this.angleVector.x < 0) {
                rightAngle = GrowDefine.angles[i] + GrowDefine.angleRange / 2 + 1;
            }

            int angle = Random.Range(leftAngle, rightAngle);
            Quaternion rotate = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector2 angleVector = rotate * this.angleVector;
            if (angleVector.y < -0.5f) continue;
            Vector2 end = this.start + this.angleVector * this.height;
            float chance = this.growChanceRate;
            if (i != 1) {
                chance = growChanceRate * 2;
            }
            Block newBlock = new Block(end, angleVector, chance);
            newBlock.level = this.level + 1;
            newBlock.root = this;
            this.subs[i] = newBlock;
        }
        this.height++;
        if (this.isTop) {
            this.isTop = false;
            this.subs[1].isTop = true;
        }
        this.depth++;
        this.width = depth * 2 + 3;
        if (this.root != null) {
            this.root.ReCalDepth();
        }
        return true;
    }
    public void ReCalDepth() {
        for (int i = 0; i < this.subs.Length; i++) {
            if (this.subs[i] != null) {
                if (this.subs[i].depth >= this.depth) {
                    this.depth = this.subs[i].depth + 1;
                }
            }
        }
        this.width = depth * 2 + 3;
        if (this.root != null) {
            this.root.ReCalDepth();
        }
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
        for (int i = 0; i < this.subs.Length; i++) {
            if (this.subs[i] != null) {
                this.subs[i].Die();
                this.subs[i] = null;
            }
        }
        this.root = null;
    }

    public void Draw() {
        for (int i = 0; i < this.subs.Length; i++) {
            if (this.subs[i] != null) {
                this.subs[i].Draw();
            }
        }
        if (this.height <= 0) return;
        Vector2 end = this.start + this.angleVector * this.height;
        Color color = GrowDefine.TreeColor - GrowDefine.ColorGrad * this.level;
        DrawLine.DrawThickLine(Mathf.RoundToInt(this.start.x), Mathf.RoundToInt(this.start.y), Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y), this.width, ThicknessMod.LINE_THICKNESS_MIDDLE, color);
    }
    public void Load() {
        if(this.subs == null) {
            this.subs = new Block[3];
        }
        for (int i = 0; i < this.subs.Length; i++) {
            if (this.subs[i] != null) {
                this.subs[i].root = this;
                this.subs[i].Load();
            }
        }
    }
}
