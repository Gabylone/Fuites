using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorSelection : Displayable
{
    public static SectorSelection Instance;

    public List<SectorButton> buttons = new List<SectorButton>();
    public SectorButton prefab;

    public Transform parent;

    public Transform addSectorButton;

    private void Awake() {
        Instance = this;
    }

    public override void Start() {
        base.Start();
    }
    public void Display() {
        FadeInInstant();

        foreach (var item in buttons)
            item.Hide();
        for (int i = 0; i < Sector.s_sectors.Count; i++) {
            if (buttons.Count <= i)
                buttons.Add(Instantiate(prefab, parent));
            buttons[i].Display(Sector.s_sectors[i]);
        }

        addSectorButton.SetAsLastSibling();
    }
}
