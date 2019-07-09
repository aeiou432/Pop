using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallManager : MonoBehaviour {
    public List<BallBase> resBall;
    private Dictionary<BallType, List<BallBase>> ballPool;
    
    public void Start() {
        this.ballPool = new Dictionary<BallType, List<BallBase>>();
        foreach (BallType type in Enum.GetValues(typeof(BallType))) {
            this.ballPool.Add(type, new List<BallBase>());
        }
    }
    public void OnDestroy() {
        foreach (List<BallBase> balls in this.ballPool.Values) {
            for (int i = 0; i < balls.Count; i++) {
                GameObject.Destroy(balls[i]);
                balls[i] = null;
            }
        }
    }
    public BallBase GetBall(BallData data, UnityAction<BallBase> booCb) {
        BallType type = data.Type;
        BallBase ball;
        if (this.ballPool[type].Count <= 0) {
            ball = GameObject.Instantiate<BallBase>(this.resBall[(int)type], this.transform);   
        }
        else {
            ball = this.ballPool[type][0];
            this.ballPool[type].RemoveAt(0);
        }
        ball.Boo.AddListener(booCb);
        ball.transform.SetAsFirstSibling();
        ball.Reset(data);
        return ball;
    }
    public void Recycle(BallBase ball) {
        if (ball == null) {
            return;
        }
        BallType type = ball.data.Type;
        ball.Boo.RemoveAllListeners();
        ball.data.Enable = false;
        ball.data = null;
        this.ballPool[type].Add(ball);
    }
}
