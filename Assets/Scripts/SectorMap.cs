using Mapbox.Examples;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SectorMap : Displayable
{
    public static SectorMap Instance;
    public Sector _sector;
    
    public TextMeshProUGUI uiText;

    public RectTransform _mapTransform;
    public Transform _mapParent;

    public float formZoom = 16;


    private void Awake() {
        Instance = this;
    }

    public void Display() {
        Display(_sector);
    }

    public void Display(Sector sector) {
        _sector = sector;
        Sector.current = _sector;

        // nittemor
        var dirPath = $"{Application.persistentDataPath}/TMP";
        if (Directory.Exists(dirPath)) {
            Directory.Delete(dirPath, true);
        }
        Directory.CreateDirectory(dirPath);

        // reset map
        MapManager.Instance.ChangeParent(_mapParent);

        MapManager.Instance.EnableMovement();

        uiText.text = _sector.name;

        FadeIn();

        if (sector.leaks.Count == 0) {
            // no leaks, display location test.
            MapManager.Instance.LocateAndDisplay();
            Debug.Log($"no leaks : target user loc");
        } else {
            // centre sur le milieu de toutes les fuites et calcule le niveau de zoom
            Display_Report.Instance.CenterMap(sector);
        }
        
        Invoke("DisplayPins", 0f);
    }

    void DisplayPins() {
        LeakPlacer.Instance.PlaceLeaks(_sector.leaks);
    }

    public void OpenRapport() {
        _sector.Save();
        FadeOut();
        Display_Report.Instance.Display(_sector);
    }

    public override void Show() {
        base.Show();
        MapManager.Instance.EnableMovement();
        LeakPlacer.Instance.placeholderPin.Hide();
        LeakPlacer.Instance.canPlacePin = true;
    }

    public override void Hide() {
        base.Hide();
        MapManager.Instance.DisableMovement();
        LeakPlacer.Instance.canPlacePin = false;
    }


}
