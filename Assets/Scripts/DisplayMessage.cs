using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;

public class DisplayMessage : Displayable
{
    public static DisplayMessage instance;
    public TextMeshProUGUI uiText;

    public Action OnConfirm;
    public Action OnCancel;

    void Awake() {
        instance = this;
    }

    public void Display(string message) {
        FadeInInstant();
        uiText.text = message;
    }

    public void Display(string message, Action callBack) {
        FadeInInstant();
        uiText.text = message;
        OnConfirm = callBack;
    }

    public void Display(string message, Action callBack, Action cancelCAllback) {
        FadeInInstant();
        uiText.text = message;
        OnConfirm = callBack;
        OnCancel = cancelCAllback;
    }

    public void Confirm() {
        FadeOut();
            OnCancel = null;
        if (OnConfirm != null) {
            OnConfirm();
            OnConfirm = null;
        }
    }

    public override void Hide() {
        base.Hide();
        OnConfirm =null;

        if (OnCancel != null) {
            OnCancel();
            OnCancel = null;
        }
    }
}
