using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gardener : MonoBehaviour
{
    public GameObject Container;
    public UnityEvent OnFilled;
    public UnityEvent OnWatered;
    [NonSerialized]
    public int WaterNumber;
    [NonSerialized]
    public bool NoWater;
    private Dictionary<GardenerState, State> stateMapping;
    private Vector3 target;
    private float nextTime;
    private State state;
    // Start is called before the first frame update
    void Start()
    {
        stateMapping = new Dictionary<GardenerState, State>() {
            { GardenerState.Filling, new Filling() },
            { GardenerState.Watering, new Watering() },
            { GardenerState.WalkingToFill, new WalkingToFill() },
            { GardenerState.WalkingToWater, new WalkingToWater() }
        };
        this.target = new Vector3(Screen.width / 2, this.Container.transform.position.y, this.Container.transform.position.z);

        this.transform.position = this.target;
        this.ChangeState(GardenerState.Watering);
    }

    // Update is called once per frame
    void Update() {
        if (!this.NoWater) {
            this.state.Execut(this);
        }
    }

    public void ChangeState(GardenerState gardenerState) {
        this.stateMapping.TryGetValue(gardenerState, out this.state);
        this.nextTime = Time.realtimeSinceStartup + this.state.duration;
    }

    public enum GardenerState {
        Filling,
        Watering,
        WalkingToFill,
        WalkingToWater
    }
    abstract class State
    {
        public float duration;
        public GardenerState state;
        public void Execut(Gardener gardener) {
            this.DoAction(gardener);
            if (Time.realtimeSinceStartup > gardener.nextTime) {
                this.Next(gardener);
            }
        }
        protected abstract void Next(Gardener gardener);
        protected abstract void DoAction(Gardener gardener);
    }
    class Filling : State
    {
        public Filling() {
            this.duration = 1;
            this.state = GardenerState.Filling;
        }
        protected override void Next(Gardener gardener) {
            gardener.OnFilled.Invoke();
            gardener.ChangeState(GardenerState.WalkingToWater);
        }
        protected override void DoAction(Gardener gardener) {
        }
    }
    class Watering : State
    {
        public Watering() {
            this.duration = 1;
            this.state = GardenerState.Watering;
        }
        protected override void Next(Gardener gardener) {
            gardener.OnWatered.Invoke();
            if (!gardener.NoWater) {
                gardener.ChangeState(GardenerState.WalkingToFill);
            }
        }
        protected override void DoAction(Gardener gardener) {
        }
    }
    class WalkingToFill : State
    {
        public WalkingToFill() {
            this.duration = 2;
            this.state = GardenerState.WalkingToFill;
        }
        protected override void Next(Gardener gardener) {
            gardener.ChangeState(GardenerState.Filling);
        }
        protected override void DoAction(Gardener gardener) {
            float interpolation = Mathf.Clamp01((gardener.nextTime - Time.realtimeSinceStartup) / duration);
            gardener.transform.position = new Vector3(
                interpolation * gardener.target.x + (1 - interpolation) * gardener.Container.transform.position.x, 
                gardener.transform.position.y,
                gardener.transform.position.z);
        }
    }
    class WalkingToWater : State
    {
        public WalkingToWater() {
            this.duration = 2;
            this.state = GardenerState.WalkingToWater;
        }
        protected override void Next(Gardener gardener) {
            gardener.ChangeState(GardenerState.Watering);
        }
        protected override void DoAction(Gardener gardener) {
            float interpolation = Mathf.Clamp01((gardener.nextTime - Time.realtimeSinceStartup) / duration);
            gardener.transform.position = new Vector3(
                interpolation * gardener.Container.transform.position.x + (1 - interpolation) * gardener.target.x,
                gardener.transform.position.y,
                gardener.transform.position.z);
        }
    }
}
