using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleText : MonoBehaviour
{
    public Button ruleButton;
    public List<Text> actionTexts;

    public List<ActionBase> actions;
    public void SetDatas(List<ActionBase> actions) {
        this.actions = actions;
        this.Refresh();
    }
    public void Refresh() {
        for (int i = 0; i < this.actions.Count; i++) {
            if (this.actionTexts.Count <= i) {
                this.actionTexts.Add(Instantiate<Text>(this.actionTexts[0], this.actionTexts[0].transform.parent));
            }
            this.actionTexts[i].text = actions[i].ToString() + " " + actions[i].Parameter;
            this.actionTexts[i].gameObject.SetActive(true);
        }
        for (int i = actions.Count; i < this.actionTexts.Count; i++) {
            this.actionTexts[i].gameObject.SetActive(false);
        }
    }
}
