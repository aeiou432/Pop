using System;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public Button action;
    public Text actionName;
    public Text numberText;
    public ActionBase actionData;
    public GameObject operationBoard;
    public Button up;
    public Button down;
    public Button remove;
    public Button confirm;
    public Button exit;
    public InputField numberInput;
    public void Awake() {
        if (this.action != null) this.action.onClick.AddListener(this.OnActionClick);
    }
    public void OnDestroy() {
        if (this.action != null) this.action.onClick.RemoveListener(this.OnActionClick);
    }
    public void SetActionData(ActionBase actionData) {
        this.actionData = actionData;
        this.Refresh();
    }
    public void Refresh() {
        this.actionName.text = actionData.ToString();
        if (!float.IsNaN(actionData.Parameter)) {
            this.numberText.gameObject.SetActive(true);
            this.numberText.text = actionData.Parameter.ToString();
        }
        else {
            this.numberText.gameObject.SetActive(false);
        }
    }
    private void OnActionClick() {
        this.operationBoard.SetActive(true);
        this.operationBoard.transform.position = this.transform.position;
        this.exit.onClick.AddListener(this.ExitAndRemoveListeners);
        if (!float.IsNaN(actionData.Parameter)) {
            this.up.gameObject.SetActive(true);
            this.down.gameObject.SetActive(true);
            this.numberInput.gameObject.SetActive(true);
            this.up.onClick.AddListener(this.Up);
            this.down.onClick.AddListener(this.Down);
            this.numberInput.text = this.actionData.Parameter.ToString();
            this.numberInput.onEndEdit.AddListener(this.OnInpuValueChange);
        }
        else {
            this.up.gameObject.SetActive(false);
            this.down.gameObject.SetActive(false);
            this.numberInput.gameObject.SetActive(false);
        }
    }
    private void OnInpuValueChange(string text) {
        this.actionData.Parameter = Convert.ToInt32(text);
        this.Refresh();
    }
    public void Up() {
        this.actionData.Parameter = this.actionData.Parameter + 1;
        this.numberText.text = actionData.Parameter.ToString();
        this.numberInput.text = actionData.Parameter.ToString();
    }
    public void Down() {
        this.actionData.Parameter = this.actionData.Parameter - 1;
        this.numberText.text = actionData.Parameter.ToString();
        this.numberInput.text = actionData.Parameter.ToString();
    }
    public void ExitAndRemoveListeners() {
        this.up.onClick.RemoveAllListeners();
        this.down.onClick.RemoveAllListeners();
        this.exit.onClick.RemoveAllListeners();
        this.confirm.onClick.RemoveAllListeners();
        this.remove.onClick.RemoveAllListeners();
        this.numberInput.onEndEdit.RemoveListener(this.OnInpuValueChange);
        this.operationBoard.SetActive(false);
    }
}
