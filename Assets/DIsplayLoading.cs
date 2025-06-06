using TMPro;
using UnityEngine;

public class DIsplayLoading : Displayable
{
    public static DIsplayLoading instance;

    public TextMeshProUGUI uiText;

    public float rate = 0.5f;
    public RectTransform loading_RectTransform;
    public float amount = 22.5f;
    float timer = 0f;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        if (state == State.visible) {
            timer += Time.deltaTime;
            if (timer >= rate) {
                timer = 0f;
                loading_RectTransform.Rotate(Vector3.forward * amount);
            }
        }
    }

    public void Display(string text, bool instant = false) {
        if (instant)
            Show();
        else
            FadeInInstant();
        UpdateText(text);
    }

    public void UpdateText(string text) {
        uiText.text = text;
    }

}
