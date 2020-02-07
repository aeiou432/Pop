using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
public class ActionBaseSpecConcreteClassConverter : DefaultContractResolver
{
    protected override JsonConverter ResolveContractConverter(Type objectType) {
        if (typeof(ActionBase).IsAssignableFrom(objectType) && !objectType.IsAbstract) {
            return null;
        }
        return base.ResolveContractConverter(objectType);
    }
}
public class ActionBaseConverter : JsonConverter
{
    static JsonSerializerSettings specifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new ActionBaseSpecConcreteClassConverter() };
    public override bool CanConvert(Type objectType) {
        return (objectType == typeof(ActionBase));
    }
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        JObject jo = JObject.Load(reader);
        switch (jo["actionType"].Value<int>()) {
            case (int)ActionBase.ActionType.Left:
                return JsonConvert.DeserializeObject<Left>(jo.ToString(), specifiedSubclassConversion);
            case (int)ActionBase.ActionType.Down:
                return JsonConvert.DeserializeObject<Down>(jo.ToString(), specifiedSubclassConversion);
            case (int)ActionBase.ActionType.Divergence:
                return JsonConvert.DeserializeObject<Divergence>(jo.ToString(), specifiedSubclassConversion);
            case (int)ActionBase.ActionType.Roll:
                return JsonConvert.DeserializeObject<Roll>(jo.ToString(), specifiedSubclassConversion);
            case (int)ActionBase.ActionType.ToA:
                return JsonConvert.DeserializeObject<ToA>(jo.ToString(), specifiedSubclassConversion);
            case (int)ActionBase.ActionType.ToB:
                return JsonConvert.DeserializeObject<ToB>(jo.ToString(), specifiedSubclassConversion);
            case (int)ActionBase.ActionType.ToC:
                return JsonConvert.DeserializeObject<ToC>(jo.ToString(), specifiedSubclassConversion);
            default:
                throw new Exception();
        }
    }
    public override bool CanWrite {
        get { return false; }
    }
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        throw new NotImplementedException();
    }
}
public class ActionManager
{
    public static ActionManager Instance = new ActionManager();
    public delegate List<List<ActionBase>> Method();

    public List<List<ActionBase>> actionBases;
    public bool drawAxis;
    private List<Method> p;
    
    public ActionManager() {
        this.p = new List<Method>() { this.Rule1, this.Rule2 };
    }
    public void RandomSet() {
        this.actionBases = this.p[UnityEngine.Random.Range(0, this.p.Count)]();
        //this.actionBases = this.p[1]();
    }
    public void Grow(InterNode node) {
        InterNode newNode = null;
        for (int i = 0; i < this.actionBases[node.branchMethod].Count; i++) {
            this.actionBases[node.branchMethod][i].DoAction(node, ref newNode);
        }
    }
    /// <summary>
    /// array length is always 2
    /// </summary>
    public string[] Save() {
        string[] saveString = new string[2];
        saveString[0] = JsonConvert.SerializeObject(this.drawAxis);
        saveString[1] = JsonConvert.SerializeObject(this.actionBases);

        return saveString;
    }
    /// <summary>
    /// array length is always 2
    /// </summary>
    public void Load(string[] loadString) {
        this.drawAxis = JsonConvert.DeserializeObject<bool>(loadString[0]);
        this.actionBases = JsonConvert.DeserializeObject<List<List<ActionBase>>>(loadString[1]);
    }
    private List<List<ActionBase>> Rule1()
    {
        //List<int> r = new List<int>() { 90, 70 };
        //List<int> a = new List<int>() { 10, 70 - 10};
        //int divergencyA = 180;
        //this.drawAxis = false;
        List<int> r = new List<int>() { UnityEngine.Random.Range(90, 96), UnityEngine.Random.Range(70, 86) };
        int randA = UnityEngine.Random.Range(5, 66);
        List<int> a = new List<int>() { randA, 70 - randA};
        int divergencyA = UnityEngine.Random.Range(60, 301);
        this.drawAxis = (UnityEngine.Random.Range(0, 2) == 0) ? true : false;

        List<List<ActionBase>> actions = new List<List<ActionBase>>();
        List<ActionBase> action;

        action = new List<ActionBase>();
        action.Add(new ToB(r[0]));
        action.Add(new Down(a[0]));
        action.Add(new Divergence(divergencyA));
        action.Add(new ToB(r[1]));
        action.Add(new Down(a[1]));
        actions.Add(action);

        action = new List<ActionBase>();
        action.Add(new ToB(r[0]));
        action.Add(new Left(-a[0]));
        action.Add(new Roll());
        action.Add(new ToB(r[1]));
        action.Add(new Left(a[1]));
        action.Add(new Roll());
        actions.Add(action);

        return actions;
    }

    private List<List<ActionBase>> Rule2() {
        //List<int> r = new List<int>() { 90, 60 };
        //List<int> a = new List<int>() { 45, 45 };
        //int divergencyA = 137;
        //this.drawAxis = false;
        List<int> r = new List<int>() { 90, UnityEngine.Random.Range(60, 91) };
        List<int> a = new List<int>() { UnityEngine.Random.Range(20, 66), UnityEngine.Random.Range(20, 66) };
        if (UnityEngine.Random.Range(0, 2) == 0) {
            a[0] = -a[0];
        }
        if (UnityEngine.Random.Range(0, 2) == 0) {
            a[1] = -a[1];
        }
        int divergencyA = UnityEngine.Random.Range(50, 171);
        if (UnityEngine.Random.Range(0, 2) == 0) {
            divergencyA = -divergencyA;
        }
        this.drawAxis = (UnityEngine.Random.Range(0, 2) == 0) ? true : false;

        List<List<ActionBase>> actions = new List<List<ActionBase>>();
        List<ActionBase> action;

        action = new List<ActionBase>();
        action.Add(new ToB(r[1]));
        action.Add(new Down(-a[0]));
        action.Add(new Divergence(divergencyA));
        action.Add(new ToA(r[0]));
        actions.Add(action);

        action = new List<ActionBase>();
        action.Add(new ToC(r[1]));
        action.Add(new Left(a[1]));
        action.Add(new Roll());
        action.Add(new ToC(r[0]));
        actions.Add(action);

        action = new List<ActionBase>();
        action.Add(new ToB(r[1]));
        action.Add(new Left(-a[1]));
        action.Add(new Roll());
        action.Add(new ToB(r[0]));
        actions.Add(action);

        return actions;
    }
}
[JsonConverter(typeof(ActionBaseConverter))]
public abstract class ActionBase
{
    public enum ActionType {
        Left,
        Down,
        Divergence,
        Roll,
        ToA,
        ToB,
        ToC
    }
    public ActionType actionType;
    public virtual float Parameter { set; get; }
    public abstract void DoAction(InterNode from, ref InterNode to);
    public abstract ActionBase Clone();
}
public class Left : ActionBase
{
    public Left(float angle = 90) {
        this.actionType = ActionType.Left;
        this.Parameter = angle;
    }
    public override void DoAction(InterNode from, ref InterNode to) {
        Quaternion rotate = Quaternion.AngleAxis(this.Parameter, to.forward);
        to.angleVector = rotate * to.angleVector;
    }
    public override ActionBase Clone() {
        ActionBase action = new Left {
            Parameter = this.Parameter
        };
        return action;
    }
}
public class Down : ActionBase
{
    public Down(float angle = 90) {
        this.actionType = ActionType.Down;
        this.Parameter = angle;
    }
    public override void DoAction(InterNode from, ref InterNode to) {
        Vector3 right = Vector3.Cross(to.angleVector, to.forward);
        Quaternion Rotate = Quaternion.AngleAxis(this.Parameter, right);
        to.angleVector = Rotate * to.angleVector;
        to.forward = Rotate * to.forward;
    }
    public override ActionBase Clone() {
        ActionBase action = new Down {
            Parameter = this.Parameter
        };
        return action;
    }
}
public class Divergence : ActionBase
{
    public Divergence(float angle = 180) {
        this.actionType = ActionType.Divergence;
        this.Parameter = angle;
    }
    public override void DoAction(InterNode from, ref InterNode to) {
        Quaternion Rotate = Quaternion.AngleAxis(this.Parameter, from.angleVector);
        from.forward = Rotate * from.forward;
    }
    public override ActionBase Clone() {
        ActionBase action = new Divergence {
            Parameter = this.Parameter
        };
        return action;
    }
}
public class Roll : ActionBase
{
    public Roll() {
        this.actionType = ActionType.Roll;
        this.Parameter = float.NaN;
    }
    public override void DoAction(InterNode from, ref InterNode to) {
        to.forward = Vector3.Cross(to.angleVector, Vector3.Cross(Vector3.up, to.angleVector));
    }
    public override ActionBase Clone() {
        ActionBase action = new Roll {
            Parameter = this.Parameter
        };
        return action;
    }
}
public class ToA : ActionBase
{
    public override float Parameter {
        get => base.Parameter;
        set {
            base.Parameter = Mathf.Clamp(value, 1, 99);
        }
    }
    public ToA(float ratio = 90) {
        this.actionType = ActionType.ToA;
        this.Parameter = ratio;
    }
    public override void DoAction(InterNode from, ref InterNode to) {
        to = new InterNode(from.maxHeight * (this.Parameter / 100f), from);
        to.branchMethod = 0;
        from.subs.Add(to);
    }
    public override ActionBase Clone() {
        ActionBase action = new ToA {
            Parameter = this.Parameter
        };
        return action;
    }
}
public class ToB : ActionBase
{
    public override float Parameter {
        get => base.Parameter;
        set {
            base.Parameter = Mathf.Clamp(value, 1, 99);
        }
    }
    public ToB(float ratio = 90) {
        this.actionType = ActionType.ToB;
        this.Parameter = ratio;
    }
    public override void DoAction(InterNode from, ref InterNode to) {
        to = new InterNode(from.maxHeight * (this.Parameter / 100f), from);
        to.branchMethod = 1;
        from.subs.Add(to);
    }
    public override ActionBase Clone() {
        ActionBase action = new ToB {
            Parameter = this.Parameter
        };
        return action;
    }
}
public class ToC : ActionBase
{
    public override float Parameter {
        get => base.Parameter;
        set {
            base.Parameter = Mathf.Clamp(value, 1, 99);
        }
    }
    public ToC(float ratio = 90) {
        this.actionType = ActionType.ToC;
        this.Parameter = ratio;
    }
    public override void DoAction(InterNode from, ref InterNode to) {
        to = new InterNode(from.maxHeight * (this.Parameter / 100f), from);
        to.branchMethod = 2;
        from.subs.Add(to);
    }
    public override ActionBase Clone() {
        ActionBase action = new ToC {
            Parameter = this.Parameter
        };
        return action;
    }
}