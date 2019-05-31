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
    public void Draw() {
        for (int i = 0; i < this.subs.Length; i++) {
            if (this.subs[i] != null) {
                this.subs[i].Draw();
            }
        }
        if (this.height <= 0) return;
        Vector2 end = this.start + this.angleVector * this.height;
        Color color = GrowDefine.TreeColor - GrowDefine.ColorGrad * this.level;
        this.DrawThickLine(Mathf.RoundToInt(this.start.x), Mathf.RoundToInt(this.start.y), Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y), this.width, ThicknessMod.LINE_THICKNESS_MIDDLE, color);
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

    public void DrawThickLine(int aXStart, int aYStart, int aXEnd, int aYEnd, int aThickness,
        ThicknessMod aThicknessMode, Color aColor) {
        int i, tDeltaX, tDeltaY, tDeltaXTimes2, tDeltaYTimes2, tError, tStepX, tStepY;

        if (aThickness <= 1) {
            this.DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, Overlap.LINE_OVERLAP_NONE, aColor);
        }
        /*
         * Clip to display size
         */
        if (aXStart >= GrowDefine.LOCAL_DISPLAY_WIDTH) {
            aXStart = GrowDefine.LOCAL_DISPLAY_WIDTH - 1;
        }
        if (aXStart < 0) {
            aXStart = 0;
        }
        if (aXEnd >= GrowDefine.LOCAL_DISPLAY_WIDTH) {
            aXEnd = GrowDefine.LOCAL_DISPLAY_WIDTH - 1;
        }
        if (aXEnd < 0) {
            aXEnd = 0;
        }
        if (aYStart >= GrowDefine.LOCAL_DISPLAY_HEIGHT) {
            aYStart = GrowDefine.LOCAL_DISPLAY_HEIGHT - 1;
        }
        if (aYStart < 0) {
            aYStart = 0;
        }
        if (aYEnd >= GrowDefine.LOCAL_DISPLAY_HEIGHT) {
            aYEnd = GrowDefine.LOCAL_DISPLAY_HEIGHT - 1;
        }
        if (aYEnd < 0) {
            aYEnd = 0;
        }

        /**
         * For coordinate system with 0.0 top left
         * Swap X and Y delta and calculate clockwise (new delta X inverted)
         * or counterclockwise (new delta Y inverted) rectangular direction.
         * The right rectangular direction for LINE_OVERLAP_MAJOR toggles with each octant
         */
        tDeltaY = aXEnd - aXStart;
        tDeltaX = aYEnd - aYStart;
        // mirror 4 quadrants to one and adjust deltas and stepping direction
        bool tSwap = true; // count effective mirroring
        if (tDeltaX < 0) {
            tDeltaX = -tDeltaX;
            tStepX = -1;
            tSwap = !tSwap;
        }
        else {
            tStepX = +1;
        }
        if (tDeltaY < 0) {
            tDeltaY = -tDeltaY;
            tStepY = -1;
            tSwap = !tSwap;
        }
        else {
            tStepY = +1;
        }
        tDeltaXTimes2 = tDeltaX << 1;
        tDeltaYTimes2 = tDeltaY << 1;
        Overlap tOverlap;
        // adjust for right direction of thickness from line origin
        int tDrawStartAdjustCount = aThickness / 2;
        if (aThicknessMode == ThicknessMod.LINE_THICKNESS_DRAW_COUNTERCLOCKWISE) {
            tDrawStartAdjustCount = aThickness - 1;
        }
        else if (aThicknessMode == ThicknessMod.LINE_THICKNESS_DRAW_CLOCKWISE) {
            tDrawStartAdjustCount = 0;
        }

        // which octant are we now
        if (tDeltaX >= tDeltaY) {
            if (tSwap) {
                tDrawStartAdjustCount = (aThickness - 1) - tDrawStartAdjustCount;
                tStepY = -tStepY;
            }
            else {
                tStepX = -tStepX;
            }
            /*
             * Vector for draw direction of lines is rectangular and counterclockwise to original line
             * Therefore no pixel will be missed if LINE_OVERLAP_MAJOR is used
             * on changing in minor rectangular direction
             */
            // adjust draw start point
            tError = tDeltaYTimes2 - tDeltaX;
            for (i = tDrawStartAdjustCount; i > 0; i--) {
                // change X (main direction here)
                aXStart -= tStepX;
                aXEnd -= tStepX;
                if (tError >= 0) {
                    // change Y
                    aYStart -= tStepY;
                    aYEnd -= tStepY;
                    tError -= tDeltaXTimes2;
                }
                tError += tDeltaYTimes2;
            }
            //draw start line
            this.DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, Overlap.LINE_OVERLAP_NONE, aColor);
            // draw aThickness lines
            tError = tDeltaYTimes2 - tDeltaX;
            for (i = aThickness; i > 1; i--) {
                // change X (main direction here)
                aXStart += tStepX;
                aXEnd += tStepX;
                tOverlap = Overlap.LINE_OVERLAP_NONE;
                if (tError >= 0) {
                    // change Y
                    aYStart += tStepY;
                    aYEnd += tStepY;
                    tError -= tDeltaXTimes2;
                    /*
                     * change in minor direction reverse to line (main) direction
                     * because of choosing the right (counter)clockwise draw vector
                     * use LINE_OVERLAP_MAJOR to fill all pixel
                     *
                     * EXAMPLE:
                     * 1,2 = Pixel of first lines
                     * 3 = Pixel of third line in normal line mode
                     * - = Pixel which will additionally be drawn in LINE_OVERLAP_MAJOR mode
                     *           33
                     *       3333-22
                     *   3333-222211
                     * 33-22221111
                     *  221111                     /\
                     *  11                          Main direction of draw vector
                     *  -> Line main direction
                     *  <- Minor direction of counterclockwise draw vector
                     */
                    tOverlap = Overlap.LINE_OVERLAP_MAJOR;
                }
                tError += tDeltaYTimes2;
                this.DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, tOverlap, aColor);
            }
        }
        else {
            // the other octant
            if (tSwap) {
                tStepX = -tStepX;
            }
            else {
                tDrawStartAdjustCount = (aThickness - 1) - tDrawStartAdjustCount;
                tStepY = -tStepY;
            }
            // adjust draw start point
            tError = tDeltaXTimes2 - tDeltaY;
            for (i = tDrawStartAdjustCount; i > 0; i--) {
                aYStart -= tStepY;
                aYEnd -= tStepY;
                if (tError >= 0) {
                    aXStart -= tStepX;
                    aXEnd -= tStepX;
                    tError -= tDeltaYTimes2;
                }
                tError += tDeltaXTimes2;
            }
            //draw start line
            this.DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, Overlap.LINE_OVERLAP_NONE, aColor);
            tError = tDeltaXTimes2 - tDeltaY;
            for (i = aThickness; i > 1; i--) {
                aYStart += tStepY;
                aYEnd += tStepY;
                tOverlap = Overlap.LINE_OVERLAP_NONE;
                if (tError >= 0) {
                    aXStart += tStepX;
                    aXEnd += tStepX;
                    tError -= tDeltaYTimes2;
                    tOverlap = Overlap.LINE_OVERLAP_MAJOR;
                }
                tError += tDeltaXTimes2;
                this.DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, tOverlap, aColor);
            }
        }
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
