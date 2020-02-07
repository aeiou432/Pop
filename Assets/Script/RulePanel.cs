using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulePanel : MonoBehaviour
{
    private List<List<ActionBase>> actionBases;
    private int nowRuleSet;
    public List<RuleText> ruleSets;
    public List<ActionButton> nowActionButtons;
    public Button finishNowRuleSet;
    public Button allFinish;
    public Toggle turnOver;
    public GameObject nowRuleSetBoard;
    public GameObject choosableBoard;
    public ActionButton leftAction;
    public ActionButton downAction;
    public ActionButton divergenceAction;
    public ActionButton rollAction;
    public ActionButton toA;
    public ActionButton toB;
    public ActionButton toC;

    private void Awake()
    {
        this.actionBases = new List<List<ActionBase>>();
        for (int i = 0; i < this.ruleSets.Count; i++) {
            this.actionBases.Add(new List<ActionBase>());
            this.ruleSets[i].SetDatas(this.actionBases[i]);
            int tempi = i;
            ruleSets[i].ruleButton.onClick.AddListener(() => this.OnRuleSetClick(tempi));
        }
        for (int i = 0; i < this.nowActionButtons.Count; i++) {
            int temp = i;
            this.nowActionButtons[temp].action.onClick.AddListener(() => this.NowActionClick(this.nowActionButtons[temp]));
        }
        this.allFinish.onClick.AddListener(this.OK);
        this.finishNowRuleSet.onClick.AddListener(this.FinishOneRuleSet);
        this.leftAction.SetActionData(new Left());
        this.downAction.SetActionData(new Down());
        this.divergenceAction.SetActionData(new Divergence());
        this.rollAction.SetActionData(new Roll());
        this.toA.SetActionData(new ToA());
        this.toB.SetActionData(new ToB());
        this.toC.SetActionData(new ToC());
        this.leftAction.action.onClick.AddListener(() => this.AddActionClick(this.leftAction));
        this.downAction.action.onClick.AddListener(() => this.AddActionClick(this.downAction));
        this.divergenceAction.action.onClick.AddListener(() => this.AddActionClick(this.divergenceAction));
        this.rollAction.action.onClick.AddListener(() => this.AddActionClick(this.rollAction));
        this.toA.action.onClick.AddListener(() => this.AddActionClick(this.toA));
        this.toB.action.onClick.AddListener(() => this.AddActionClick(this.toB));
        this.toC.action.onClick.AddListener(() => this.AddActionClick(this.toC));
    }
    private void OnEnable() {
        this.nowRuleSetBoard.SetActive(false);
    }
    private void OnRuleSetClick(int index) {
        this.nowRuleSet = index;
        this.nowRuleSetBoard.SetActive(true);
        this.RefreshNowRuleSet();
        this.choosableBoard.SetActive(true);
    }
    private void FinishOneRuleSet() {
        this.nowRuleSetBoard.SetActive(false);
        this.ruleSets[this.nowRuleSet].Refresh();
    }
    private void RefreshNowRuleSet() {
        for (int i = 0; i < this.actionBases[this.nowRuleSet].Count; i++) {
            if (i >= this.nowActionButtons.Count) {
                this.nowActionButtons.Add(Instantiate<ActionButton>(nowActionButtons[0], nowActionButtons[0].transform.parent));
                int temp = i;
                this.nowActionButtons[i].action.onClick.AddListener(() => this.NowActionClick(this.nowActionButtons[temp]));
            }
            this.nowActionButtons[i].gameObject.SetActive(true);
            this.nowActionButtons[i].SetActionData(this.actionBases[this.nowRuleSet][i]);
        }
        for (int i = actionBases[this.nowRuleSet].Count; i < this.nowActionButtons.Count; i++) {
            this.nowActionButtons[i].gameObject.SetActive(false);
        }
    }
    private void AddActionClick(ActionButton actionButton) {
        actionButton.remove.gameObject.SetActive(false);
        actionButton.confirm.gameObject.SetActive(true);
        actionButton.confirm.onClick.AddListener(() => this.ConfirmAddAction(actionButton));
    }
    private void NowActionClick(ActionButton actionButton) {
        actionButton.remove.gameObject.SetActive(true);
        actionButton.confirm.gameObject.SetActive(false);
        actionButton.remove.onClick.AddListener(() => this.RemoveNowAction(actionButton));
    }
    private void ConfirmAddAction(ActionButton actionButton) {
        actionButton.confirm.onClick.RemoveAllListeners();
        actionButton.ExitAndRemoveListeners();
        this.actionBases[this.nowRuleSet].Add(actionButton.actionData.Clone());
        this.RefreshNowRuleSet();
    }
    private void RemoveNowAction(ActionButton actionButton) {
        actionButton.remove.onClick.RemoveAllListeners();
        actionButton.ExitAndRemoveListeners();
        this.actionBases[this.nowRuleSet].Remove(actionButton.actionData);
        this.RefreshNowRuleSet();
    }
    public void OK() {
        ActionManager.Instance.actionBases = this.actionBases;
        ActionManager.Instance.drawAxis = this.turnOver.isOn;
        this.gameObject.SetActive(false);
    }
}
