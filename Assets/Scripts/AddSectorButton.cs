using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddSectorButton : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        SectorSelection.Instance.FadeOut();
        SectorCreation.Instance.modify =false;
        SectorCreation.Instance.Display(new Sector());
    }
}
