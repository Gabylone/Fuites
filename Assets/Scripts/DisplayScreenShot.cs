using JetBrains.Annotations;
using System.IO;
using UnityEngine;

public class DisplayScreenShot : Displayable {
    public static DisplayScreenShot Instance;

    public Transform _mapParent;

    Leak _leak;
    public float screenshotzoom = 5f;

    public bool screenshot = false;

    public enum Type {
        leak,
        report,
    }
    public Type type;

    private void Awake() {
        Instance = this;
    }

    public void ScreenReport() {
        // display sans leak 
        type = Type.report;
        Debug.Log($"screenshot : sector");
        LeakPlacer.Instance.PlaceLeaks(Sector.current.leaks);
        Display_Report.Instance.CenterMap(Sector.current);

        Display();
    }

    public void ScreenLeak(Leak leak) {
        type = Type.leak;
        _leak = leak;

        Debug.Log($"screenshot : leak");
        LeakPlacer.Instance.PlaceUniqueLeak(leak);
        var latlon = new Mapbox.Utils.Vector2d(leak.latitude, leak.longitude);
        MapManager.Instance._map.UpdateMap(latlon, screenshotzoom);

        Display();
    }

    public void Display() {

        LeakPlacer.Instance.canPlacePin = false;
        FadeInInstant();
        MapManager.Instance.ChangeParent(_mapParent);
        Invoke("TakeScreenshot", 1f);
        MapManager.Instance.DisableMovement();
    }

    void TakeScreenshot() {

        if (type == Type.leak) {
            //string path = $"{Leak.Scurrent.GetPath()}/map.png";
            //string path = $"{Application.persistentDataPath}/saves/{Sector.current.id}/{_leak.id}/map.png";
            string path = $"saves/{Sector.current.id}/{_leak.id}/map.png";
            Debug.Log($"{path}");
            ScreenCapture.CaptureScreenshot(path);
        } else {
            string path = $"saves/{Sector.current.id}/{_leak.id}/sector_map.png";
            Debug.Log($"{path}");
            ScreenCapture.CaptureScreenshot(path);
        }

        Invoke("Close", 1f);
    }

    void Close() {
        MapManager.Instance.EnableMovement();   
        FadeOut();
        if (type == Type.leak) {
            Display_Form.Instance.DisplayCurrentLeak();
        } else {
            Display_Report.Instance.Display(Sector.current);
        }
        LeakPlacer.Instance.canPlacePin = false;
    }

}


