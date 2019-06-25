using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DrawLine
{
    public static void DrawThickLine(int aXStart, int aYStart, int aXEnd, int aYEnd, int aThickness,
        ThicknessMod aThicknessMode, Color aColor) {
        int i, tDeltaX, tDeltaY, tDeltaXTimes2, tDeltaYTimes2, tError, tStepX, tStepY;

        if (aThickness <= 1) {
            DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, Overlap.LINE_OVERLAP_NONE, aColor);
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
            DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, Overlap.LINE_OVERLAP_NONE, aColor);
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
                DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, tOverlap, aColor);
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
            DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, Overlap.LINE_OVERLAP_NONE, aColor);
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
                DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, tOverlap, aColor);
            }
        }
    }
    public static void DrawLineOverlap(int aXStart, int aYStart, int aXEnd, int aYEnd, Overlap aOverlap,
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
            FillRect(aXStart, aYStart, aXEnd, aYEnd, aColor);
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
            DrawPixel(aXStart, aYStart, aColor);
            if (tDeltaX > tDeltaY) {
                // start value represents a half step in Y direction
                tError = tDeltaYTimes2 - tDeltaX;
                while (aXStart != aXEnd) {
                    // step in main direction
                    aXStart += tStepX;
                    if (tError >= 0) {
                        if ((aOverlap & Overlap.LINE_OVERLAP_MAJOR) == Overlap.LINE_OVERLAP_MAJOR) {
                            // draw pixel in main direction before changing
                            DrawPixel(aXStart, aYStart, aColor);
                        }
                        // change Y
                        aYStart += tStepY;
                        if ((aOverlap & Overlap.LINE_OVERLAP_MINOR) == Overlap.LINE_OVERLAP_MINOR) {
                            // draw pixel in minor direction before changing
                            DrawPixel(aXStart - tStepX, aYStart, aColor);
                        }
                        tError -= tDeltaXTimes2;
                    }
                    tError += tDeltaYTimes2;
                    DrawPixel(aXStart, aYStart, aColor);
                }
            }
            else {
                tError = tDeltaXTimes2 - tDeltaY;
                while (aYStart != aYEnd) {
                    aYStart += tStepY;
                    if (tError >= 0) {
                        if ((aOverlap & Overlap.LINE_OVERLAP_MAJOR) == Overlap.LINE_OVERLAP_MAJOR) {
                            // draw pixel in main direction before changing
                            DrawPixel(aXStart, aYStart, aColor);
                        }
                        aXStart += tStepX;
                        if ((aOverlap & Overlap.LINE_OVERLAP_MINOR) == Overlap.LINE_OVERLAP_MINOR) {
                            // draw pixel in minor direction before changing
                            DrawPixel(aXStart, aYStart - tStepY, aColor);
                        }
                        tError -= tDeltaYTimes2;
                    }
                    tError += tDeltaXTimes2;
                    DrawPixel(aXStart, aYStart, aColor);
                }
            }
        }
    }
    public static void DrawPixel(int x, int y, Color color) {
        GlobalValue.pixels[x + y * GrowDefine.LOCAL_DISPLAY_WIDTH] = color;
    }
    public static void FillRect(int aXStart, int aYStart, int aXEnd, int aYEnd, Color aColor) {
        if (aXStart > aXEnd) {
            int tmp = aXStart;
            aXStart = aXEnd;
            aXEnd = tmp;
        }
        if(aYStart > aYEnd) {
            int tmp = aYStart;
            aYStart = aYEnd;
            aYEnd = tmp;
        }
        for (int x = aXStart; x <= aXEnd; x++) {
            for (int y = aYStart; y <= aYEnd; y++) {
                DrawPixel(x, y, aColor);
            }
        }
    }
}
