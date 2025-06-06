using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplayPin : Displayable, IPointerClickHandler {
    public bool leurre = false;
    private Leak _leak;
    private Image image;

    public TextMeshProUGUI uiText;

    static bool _overing = false;

    public void Display(Leak leak) {
        if ( image == null)
            image = GetComponentInChildren<Image>();
        _leak = leak;
        image.color = GameManager.Instance.urgencyColors[leak.urgency];

        uiText.text = leak.name;
        FadeIn();
    }

    public static bool pressedPin = false;

    public void OnPointerClick(PointerEventData eventData) {
        if (Display_Report.Instance.state == State.visible)
            return;
        if ( leurre)
            return;

        Tween.Bounce(GetTransform);

        pressedPin = true;

        Debug.Log($"pressed pin");

        SectorMap.Instance.FadeOut();
        Display_Form.Instance.Display(_leak);

        CancelInvoke("Delay");
        Invoke("Delay", 1f);
    }

    void Delay() {
        pressedPin = false;
    }
}
