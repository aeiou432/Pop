using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystem {
    public int period = 10;
    private string OldString;
    private float growChance = 100f;
    private float deathChance = 10f;
    private float mainOrderGrow = 100;
    private float mainOrderDeath = 0;
    private int maxOrder = 2;

    private float divergence = 180f;
    private int branches = 2;
    private bool monopod = true;

    private int growTime = 0;
    private struct Node {
        public Vector2 angle;
        public Vector2 point;
    }
    private Stack<Node> nodes = new Stack<Node>();
    private StringBuilder newString = new StringBuilder();
    public void Init() {
        this.OldString = "AFT";
    }

    public void Grow() {
        int order = 0;
        for (int i = 0; i < this.OldString.Length; i++) {
            this.growTime++;
            switch (this.OldString[i]) {
                case 'A':
                    this.newString.Append("AF");
                    break;
                case 'T':
                    if (this.growTime < this.period) {
                        this.newString.Append("FT");
                        break;
                    }
                    this.growTime = 0;
                    if (order >= this.maxOrder) {
                        this.newString.Append("FT");
                    }
                    float chance = Random.Range(0f, 100f);
                    if (order == 0) {
                        if (chance < this.mainOrderGrow) {
                            this.newString.Append("W");
                            for (int j = 0; j < this.branches; j++) {
                                this.newString.Append("&[+FT]");
                            }
                            if (this.monopod) {
                                this.newString.Append("FT");
                            }
                        }
                        else {
                            this.newString.Append("FT");
                        }
                    }
                    else {
                        if (chance > this.growChance) {
                            this.newString.Append("W");
                            for (int j = 0; j < this.branches; j++) {
                                this.newString.Append("&[+FT]");
                            }
                            if (this.monopod) {
                                this.newString.Append("FT");
                            }
                        }
                        else {
                            this.newString.Append("FT");
                        }
                    }
                    break;
                default:
                    this.newString.Append(this.OldString[i]);
                    break;
            }
        }
        this.OldString = this.newString.ToString();
        this.newString.Clear();
    }
    public void Draw() {
        for (int i = 0; i < GlobalValue.pixels.Length; i++) {
            GlobalValue.pixels[i] = Color.clear;
        }
        Quaternion leftRotate = Quaternion.AngleAxis(-25.75f, Vector3.forward);
        Quaternion rightRotate = Quaternion.AngleAxis(25.75f, Vector3.forward);
        Vector2 angleVector = new Vector2(0, 6);
        Vector2 point = new Vector2(360, 0);
        for (int i = 0; i < this.OldString.Length; i++) {
            switch (this.OldString[i]) {
                case 'F': {
                        Vector2 end = point + angleVector;
                        DrawLine.DrawLineOverlap(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y),
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
                        nodes.Push(new Node() { angle = angleVector, point = point });
                        break;
                    }
                case ']': {
                        Node tmp = nodes.Pop();
                        angleVector = tmp.angle;
                        point = tmp.point;
                        break;
                    }
                default:
                    break;
            }
        }
    }
    private char GetLeftSymbol(string context, int index) {
        int bracketCount = 0;
        for (int i = index - 1; i >= 0; i--) {
            if (context[i] == ']') {
                bracketCount++;
            }
            else if (bracketCount > 0 && context[i] == '[') {
                bracketCount--;
            }
            else if (bracketCount == 0 && (context[i] == '0' || context[i] == '1')) {
                return context[i];
            }
        }
        return ' ';
    }
    private char GetRightSymbol(string context, int index) {
        int bracketCount = 0;
        for (int i = index + 1; i < context.Length; i++) {
            if (bracketCount < 0) {
                return ' ';
            }
            if (context[i] == '[') {
                bracketCount++;
            }
            else if (context[i] == ']') {
                bracketCount--;
            }
            else if (bracketCount == 0 && (context[i] == '0' || context[i] == '1')) {
                return context[i];
            }
        }
        return ' ';
    }
}
