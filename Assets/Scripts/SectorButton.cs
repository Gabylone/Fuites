using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SectorButton : Displayable, IPointerClickHandler {
    public TextMeshProUGUI uiText;
    private Sector _sector;

    public void Display(Sector sector) {
        Show();
        _sector = sector;
        uiText.text = _sector.name;
    }

    public void OnPointerClick(PointerEventData eventData) {
        SectorSelection.Instance.FadeOut();
        SectorMap.Instance.Display(_sector);
    }

    public void Modify() {
        SectorSelection.Instance.FadeOut();
        SectorCreation.Instance.modify =true;
        SectorCreation.Instance.Display(_sector);
    }
}
