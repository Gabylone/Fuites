using TMPro;
using UnityEngine;

public class DIsplayDate : MonoBehaviour
{
    public TextMeshProUGUI uiText;

    private void OnEnable() {
        uiText.text = System.DateTime.Now.ToLongDateString();
    }
}
