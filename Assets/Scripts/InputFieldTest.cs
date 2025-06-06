using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFieldTest : MonoBehaviour
{
    public TMP_InputField inputField; // Reference to the InputField
    public RectTransform inputFieldRectTransform; // Reference to the RectTransform
    public float padding = 10f; // Padding to adjust the height

    void Start() {
        // Add a listener to adjust height when the text changes
        inputField.onValueChanged.AddListener(AdjustHeight);
    }

    void AdjustHeight(string text) {
        // Calculate the required size for the text
        Vector2 preferredValues = inputField.textComponent.GetPreferredValues();

        // Adjust the height of the RectTransform (keeping the width the same)
        inputFieldRectTransform.sizeDelta = new Vector2(inputFieldRectTransform.sizeDelta.x, preferredValues.y + padding);
    }

}
