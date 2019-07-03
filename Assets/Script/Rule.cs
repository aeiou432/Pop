using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuleBase {
    public abstract void Order0(InterNode node);
    public abstract void OrderOthers(InterNode node);
}

public class Rule1 : RuleBase {
    private int Branches = 2;
    public List<float> R = new List<float>() { 0.9f, 0.7f };
    public float DivergencyA = 120;
    public List<float> A = new List<float>() { 10, 60 };
    public Rule1() {

    }
    public override void Order0(InterNode node) {
        for (int i = 0; i < this.Branches; i++) {
            InterNode tmp = new InterNode(node.maxHeight * this.R[i], node);
            tmp.order++;
            tmp.level++;
            tmp.Down(this.A[i], tmp);
            node.subs.Add(tmp);
            if (i == 0) {
                node.Divergence(this.DivergencyA, node);
            }
        }
    }
    public override void OrderOthers(InterNode node) {
        for (int i = 0; i < this.Branches; i++) {
            InterNode tmp = new InterNode(node.maxHeight * this.R[i], node);
            tmp.order++;
            tmp.level++;
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