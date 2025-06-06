using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    private CanvasGroup[] groups;

    private int currentSelection = -1;
    public int CurrentSelection {
        get {
            return currentSelection;
        }
    }

    CanvasGroup[] Groups {
        get {
            if ( groups == null)
                groups = GetComponentsInChildren<CanvasGroup>(true);
            return groups;
        }
    }

    public void Select(int i) {
        foreach (var group in Groups)
            group.alpha = 0.5f;

        Groups[i].alpha = 1f;
        currentSelection = i;

        Tween.Bounce(Groups[i].transform);
    }
}
