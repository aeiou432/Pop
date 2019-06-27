using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class TreeParam {
    public static int Branches = 2;
    public static bool Monopod = true;
    public static float R1;
    public static float R2;
    public static List<float> A;
 
}
public class InterNode {
    public float height = 0;
    public float maxHeight = 0;
    public float growHeight = 0;
    public int order = 0;
    public Vector3 angleVector;
    public Vector3 forward;
    public List<InterNode> subs;

    public InterNode(float maxHeight) {
        this.maxHeight = maxHeight;
        this.growHeight = maxHeight / 2;
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
        if (this.height == this.growHeight) {
            if (this.order == 0) {
                this.subs = new List<InterNode>();
                for (int i = 0; i < TreeParam.Branches; i++) {
                    InterNode tmp = new InterNode(this.maxHeight * TreeParam.R1);
                    tmp.order = this.order + 1;
                    tmp.Left(TreeParam.A[i], this);
                    this.subs.Add(tmp);
                }
                if(TreeParam.Monopod) {
                    InterNode tmp = new InterNode(this.maxHeight * TreeParam.R2);
                    tmp.order = this.order;
                    this.subs.Add(tmp);
                }
            }
            else {

            }
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
    public void P1() {

    }
    public void P2() {

    }
}
public class LSystem {
    public int period = 50;
    private string OldString;
    private float growChance = 100f;
    private float deathChance = 10f;
    private float mainOrderGrow = 100;
    private float mainOrderDeath = 0;
    private int maxOrder = 5000;
    private float leftAngle = -45f;
    private float rightAngle = 45f;
    private float divergenceAngle = 137.5f;
    private int branches = 2;
    private bool monopod = false;
    private int age;

    private int growTime = 0;
    private struct Node {
        public Vector3 angle;
        public Vector3 point;
        public Vector3 divergence;
        public float width;
        public Vector3 up;
        public Vector3 right;
        public Vector3 forward;
    }
    private Stack<Node> nodes = new Stack<Node>();
    private StringBuilder newString = new StringBuilder();

    private InterNode StartBlock;
 

    public void Grow1() {
        if (StartBlock == null) {
            this.StartBlock = new InterNode();
        }
        InterNode nowBlock = this.StartBlock;
       
    }
    public void Grow() {
        int order = 0;
        this.age++;
        for (int i = 0; i < this.OldString.Length; i++) {
            switch (this.OldString[i]) {
                case 'F':
                    
                    break;
                case 'T':
                    if (this.growTime < this.period) {
                        this.newString.Append("FT");
                        break;
                    }
                    if (order >= this.maxOrder) {
                        this.newString.Append("FT");
                        break;
                    }
                    float chance = Random.Range(0f, 100f);
                    if (order == 0) {
                        if (chance < this.mainOrderGrow) {
                            this.newString.Append("W");
                            for (int j = 0; j < this.branches; j++) {
                                this.newString.Append("/[+$FT]");
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
                        if (chance < this.growChance) {
                            this.newString.Append("W");
                            for (int j = 0; j < this.branches; j++) {
                                this.newString.Append("/[+$FT]");
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
                case '[':
                    order++;
                    this.newString.Append(this.OldString[i]);
                    break;
                case ']':
                    order--;
                    this.newString.Append(this.OldString[i]);
                    break;
                default:
                    this.newString.Append(this.OldString[i]);
                    break;
            }
        }
        if (this.growTime >= this.period) {
            this.growTime = 0;
        }
        this.OldString = this.newString.ToString();
        this.newString.Clear();
    }

    public void Draw() {
        for (int i = 0; i < GlobalValue.pixels.Length; i++) {
            GlobalValue.pixels[i] = Color.clear;
        }
        Vector3 angleVector = new Vector3(0, 1, 0);
        Vector3 forward = Vector3.forward;
        Vector3 point = new Vector3(350, 0, 0);
        float width = this.age / 50 + 1;
        for (int i = 0; i < this.OldString.Length; i++) {
            switch (this.OldString[i]) {
                case 'F': {
                        Vector3 end = point + angleVector;
                        DrawLine.DrawThickLine(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y),
                            Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y), Mathf.RoundToInt(width), ThicknessMod.LINE_THICKNESS_MIDDLE, Color.white);
                        point = end;
                        break;
                    }
                case '-': {
                        Quaternion leftRotate = Quaternion.AngleAxis(this.leftAngle, forward);
                        angleVector = leftRotate * angleVector;
                        break;
                    }
                case '+': {
                        Quaternion rightRotate = Quaternion.AngleAxis(this.rightAngle, forward);
                        angleVector = rightRotate * angleVector;
                        break;
                    }
                case '[': {
                        nodes.Push(new Node() {
                            angle = angleVector,
                            point = point,
                            forward = forward,
                            width = width });
                        break;
                    }
                case ']': {
                        Node tmp = nodes.Pop();
                        angleVector = tmp.angle;
                        point = tmp.point;
                        forward = tmp.forward;
                        width = tmp.width;
                        break;
                    }
                case '/': {
                        Quaternion divergenceRotate = Quaternion.AngleAxis(this.divergenceAngle, angleVector);
                        forward = divergenceRotate * forward;
                        break;
                    }
                case '$': {
                        forward = Vector3.Cross(angleVector, Vector3.Cross(Vector3.up, angleVector));
                        break;
                    }
                case 'W': {
                        width = width * 0.707f;
                        break;
                    }
                default:
                    break;
            }
        }
    }

    private float r1 = 0.9f;
    private float r2 = 0.6f;
    private float a0 = 45;
    private float a1 = 10;
    private float a2 = 45;
    private float d = 137.5f;
    private float w = 0.707f;
    //private float w = 1f;

    private Vector3 startPoint = new Vector3(350, 0, 0);
    private Vector3 nowAngle = Vector3.up;
    private Vector3 up = new Vector3(0, 0, 1);
    public void Reset() {
        this.startPoint = new Vector3(350, 0, 0);
        this.nowAngle = Vector3.up;
        this.up = new Vector3(0, 0, 1);
    }
    public void A(float length, float width, int depth) {
        if (depth == 0) return;
        int nextDepth = depth - 1;
        F(length, width);

        this.Push();
        this.Down(this.a0);
        B(length * r2, width * w, nextDepth);
        this.Pop();
        this.Divergency(this.d);
        A(length * r1, width * w, nextDepth);
        /*F(length, width);
        this.Push();
        this.Down(this.a1);
        B(length * this.r1, width * this.w, nextDepth);
        this.Pop();
        this.Divergency(180);
        this.Push();
        this.Down(this.a2);
        B(length * this.r2, width * this.w, nextDepth);
        this.Pop();*/
    }
    public void B(float length, float width, int depth) {
        if (depth == 0) return;
        int nextDepth = depth - 1;
        F(length, width);
        this.Push();
        this.Right(this.a2);
        this.Roll();
        C(length * r2, width * w, nextDepth);
        this.Pop();
        C(length * r1, width * w, nextDepth);
        /*F(length, width);
        this.Push();
        this.Left(this.a1);
        this.Roll();
        B(length * this.r1, width * w, nextDepth);
        this.Pop();
        this.Push();
        this.Right(this.a2);
        this.Roll();
        B(length * this.r2, width * w, nextDepth);
        this.Pop();*/
    }
    public void C(float length, float width, int depth) {
        if (depth == 0) return;
        int nextDepth = depth - 1;
        F(length, width);
        this.Push();
        this.Left(this.a2);
        this.Roll();
        B(length * r2, width * w, nextDepth);
        this.Pop();
        B(length * r1, width * w, nextDepth);
    }
    public void Push() {
        nodes.Push(new Node() {
            angle = this.nowAngle,
            point = this.startPoint,
            forward = this.up
        });
    }
    public void Pop() {
        Node tmp = nodes.Pop();
        this.nowAngle = tmp.angle;
        this.startPoint = tmp.point;
        this.up = tmp.forward;
    }
    public void Left(float angle) {
        Quaternion Rotate = Quaternion.AngleAxis(angle, this.up);
        this.nowAngle = Rotate * this.nowAngle;
        this.up = Rotate * this.up;
        return;
    }
    public void Right(float angle) {
        Quaternion Rotate = Quaternion.AngleAxis(-angle, this.up);
        this.nowAngle = Rotate * this.nowAngle;
        this.up = Rotate * this.up;
        return;
    }
    public void Down(float angle) {
        Vector3 right = Vector3.Cross(this.nowAngle, this.up);
        Quaternion Rotate = Quaternion.AngleAxis(angle, right);
        this.nowAngle = Rotate * this.nowAngle;
        this.up = Rotate * this.up;
    }
    public void Divergency(float angle) {
        Quaternion Rotate = Quaternion.AngleAxis(angle, this.nowAngle);
        this.nowAngle = Rotate * this.nowAngle;
        this.up = Rotate * this.up;
        return;
    }
    public void Roll() {
        this.up = Vector3.Cross(this.nowAngle, Vector3.Cross(Vector3.up, this.nowAngle));
    }
    public void F(float length, float width) {
        Vector3 end = this.startPoint + (this.nowAngle * length);
        DrawLine.DrawThickLine(Mathf.RoundToInt(this.startPoint.x), Mathf.RoundToInt(this.startPoint.y),
                            Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y), Mathf.RoundToInt(width), ThicknessMod.LINE_THICKNESS_MIDDLE, Color.white);
        this.startPoint = end;
    }
}
