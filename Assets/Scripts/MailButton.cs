using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailButton : Displayable
{
    private MailInfo _mailInfo;

    public Image image;
    public TMP_InputField inputField;

    public GameObject deleteObj;

    public void Display(MailInfo mailInfo) {
        Show();
        _mailInfo = mailInfo;
        UpdateDisplay();
    }

    void UpdateDisplay() {
        inputField.text = _mailInfo.adress;
        deleteObj.SetActive(_mailInfo.assigned);
        image.sprite = _mailInfo.assigned ? SectorCreation.Instance.assigned_Sprite : SectorCreation.Instance.unassigned_Sprite;
    }

    public void Delete() {
        SectorCreation.Instance.RemoveMail(_mailInfo);
    }

    public void OnEndEdit() {
        _mailInfo.assigned = true;
        _mailInfo.adress = inputField.text;
        UpdateDisplay();
        SectorCreation.Instance.UpdateDisplay();
    }
}
