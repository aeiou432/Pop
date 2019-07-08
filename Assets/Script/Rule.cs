using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager {
    public static RuleManager Instance = new RuleManager();
    public int RuleIndex;
    private List<RuleBase> rulePool;
    public RuleManager() {
        this.rulePool = new List<RuleBase>();
        this.rulePool.Add(new Rule1());
        this.rulePool.Add(new Rule2());
    }
    public RuleBase RandomPickRule() {
        this.RuleIndex = UnityEngine.Random.Range(0, this.rulePool.Count);
        RuleBase ruleBase = this.rulePool[this.RuleIndex];
        ruleBase.Init();
        return ruleBase;
    }
    public RuleBase JsonDeserialize(string data, int ruleNumber) {
        switch(ruleNumber) {
            case 0:
                return JsonConvert.DeserializeObject<Rule1>(data);
            case 1:
                return JsonConvert.DeserializeObject<Rule2>(data);
            default:
                return null;
        }
    }
}
public abstract class RuleBase {
    [JsonIgnore] public List<Method> P;
    public List<float> R;
    public abstract void Init();
}

public delegate void Method(InterNode node);
public class Rule1 : RuleBase {
    private int branches = 2;
    public float DivergencyA = 180;
    public List<float> A = new List<float>() { 10, 60 };
    
    public Rule1() {
        this.P = new List<Method>() { this.P0, this.P1 };
    }
    public override void Init() {
        //this.R = new List<float>() { 0.9f, 0.7f };
        this.R = new List<float>() { UnityEngine.Random.Range(0.9f, 0.95f), UnityEngine.Random.Range(0.7f, 0.85f) };
        this.A[0] = UnityEngine.Random.Range(5, 66);
        this.A[1] = 70 - this.A[0];
        this.DivergencyA = UnityEngine.Random.Range(60, 300);
    }
    public void P0(InterNode node) {
        for (int i = 0; i < this.branches; i++) {
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
        for (int i = 0; i < this.branches; i++) {
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
        this.P = new List<Method>() { this.P0, this.P1, this.P2 };
    }
    public override void Init() {
        //this.R = new List<float>() { 0.9f, 0.6f };
        this.R = new List<float>() { 0.9f, UnityEngine.Random.Range(0.6f, 0.9f) };
        this.A[0] = UnityEngine.Random.Range(20, 65);
        if (UnityEngine.Random.Range(0, 2) == 0) {
            this.A[0] = -this.A[0];
        }
        this.A[1] = UnityEngine.Random.Range(20, 65);
        if (UnityEngine.Random.Range(0, 2) == 0) {
            this.A[1] = -this.A[1];
        }
        this.DivergencyA = UnityEngine.Random.Range(50, 170);
        if (UnityEngine.Random.Range(0, 2) == 0) {
            this.DivergencyA = -this.DivergencyA;
        }
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