using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuleBase {
    public List<Method> P;
    public List<float> R;
}

public delegate void Method(InterNode node);
public class Rule1 : RuleBase {
    private int Branches = 2;
    public float DivergencyA = 180;
    public List<float> A = new List<float>() { 10, 60 };
    
    public Rule1() {
        //this.R = new List<float>() { 0.9f, 0.7f };
        this.R = new List<float>() { Random.Range(0.9f, 0.95f), Random.Range(0.7f, 0.85f) };
        this.A[0] = Random.Range(5, 66);
        this.A[1] = 70 - this.A[0];
        this.DivergencyA = Random.Range(60, 300);
        this.P = new List<Method>() { this.P0, this.P1 };
    }
    public void P0(InterNode node) {
        for (int i = 0; i < this.Branches; i++) {
            InterNode tmp = new InterNode(node.maxHeight * this.R[i], node);
            tmp.Down(this.A[i], tmp);
            tmp.branchMethod = 1;
            node.subs.Add(tmp);
            if (i == 0) {
                node.Divergence(this.DivergencyA, node);
            }
        }
    }
    public void P1(InterNode node) {
        for (int i = 0; i < this.Branches; i++) {
            InterNode tmp = new InterNode(node.maxHeight * this.R[i], node);
            tmp.branchMethod = 1;
            if (i == 1) {
                tmp.Left(this.A[i], tmp);
            }
            else {
                tmp.Left(-this.A[i], tmp);
            }
            tmp.Roll(tmp);
            node.subs.Add(tmp);
        }
    }

}

public class Rule2 : RuleBase {
    public float DivergencyA = 137.5f;
    public List<float> A = new List<float>() { 45, 45 };
    public Rule2() {
        //this.R = new List<float>() { 0.9f, 0.6f };
        this.R = new List<float>() { 0.9f, Random.Range(0.6f, 0.9f) };
        this.A[0] = Random.Range(20, 65);
        if (Random.Range(0, 2) == 0) {
            this.A[0] = -this.A[0];
        }
        this.A[1] = Random.Range(20, 65);
        if (Random.Range(0, 2) == 0) {
            this.A[1] = -this.A[1];
        }
        this.DivergencyA = Random.Range(50, 170);
        if (Random.Range(0, 2) == 0) {
            this.DivergencyA = -this.DivergencyA;
        }
        this.P = new List<Method>() { this.P0, this.P1, this.P2 };
    }
    public void P0(InterNode node) {
        InterNode tmp = new InterNode(node.maxHeight * this.R[1], node);
        tmp.Down(-this.A[0], tmp);
        tmp.branchMethod = 1;
        node.subs.Add(tmp);

        node.Divergence(this.DivergencyA, node);
        tmp = new InterNode(node.maxHeight * this.R[0], node, true);
        tmp.branchMethod = 0;
        node.subs.Add(tmp);
    }
    public void P1(InterNode node) {
        InterNode tmp = new InterNode(node.maxHeight * this.R[1], node);
        tmp.Left(this.A[1], tmp);
        tmp.Roll(tmp);
        tmp.branchMethod = 2;
        node.subs.Add(tmp);

        tmp = new InterNode(node.maxHeight * this.R[0], node, true);
        tmp.branchMethod = 2;
        node.subs.Add(tmp);
    }
    public void P2(InterNode node) {
        InterNode tmp = new InterNode(node.maxHeight * this.R[1], node, true);
        tmp.Left(-this.A[1], tmp);
        tmp.Roll(tmp);
        tmp.branchMethod = 1;
        node.subs.Add(tmp);

        tmp = new InterNode(node.maxHeight * this.R[0], node, true);
        tmp.branchMethod = 1;
        node.subs.Add(tmp);
    }
}