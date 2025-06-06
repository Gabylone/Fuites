using Mapbox.Examples;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Display_Report : Displayable
{
    public static Display_Report Instance;

    public ScrollRect scrollRect;

    public TMP_InputField inputField_location;
    public TMP_InputField inputField_sumUp;
    public TMP_InputField inputField_results;

    public int report_zoom = 14;

    public Image _screenshot;
    public GameObject _screenshot_icon;

    public Transform _mapParent;

    private Sector _sector;


    public float maxZoom = 16f;
    public float minZoom = 11f;
    public float maxDelta = 120f;

    private void Awake() {
        Instance = this;
    }

    public void Display(Sector sector) {

        _sector = sector;
        FadeIn();

        MapManager.Instance._map.GetComponent<QuadTreeCameraMovement>().enabled = false;
        SectorMap.Instance._mapTransform.SetParent(_mapParent);
        SectorMap.Instance._mapTransform.offsetMax = Vector3.zero;
        SectorMap.Instance._mapTransform.offsetMin = Vector3.zero;

        scrollRect.verticalNormalizedPosition = 1f;

        CenterMap(_sector);

        if (string.IsNullOrEmpty(_sector.location)) {

        }

        string text = "";
        for (int leakType = 0; leakType < 6; leakType++) {
            var leaks = _sector.leaks.FindAll(x => x.leakType == leakType);
            if (leaks.Count == 0)
                continue;
            Debug.Log($"trouvé un truc");
            string word = leaks.Count > 1 ? "fuites" : "fuite";
            text += $"{leaks.Count} {word} {Leak.leaksTypes_text[leakType]} ; ";
        }

        string str = "";
        if (_sector.leaks.Count == 0)
            str = "Aucune fuites réalisées à ce jour";
        else
            str = $"Les investigations réalisées ont permis de localiser {_sector.leaks.Count} fuites, dont {text}";
        _sector.sumUp = str;
        _sector.results = "Contrôle du réseau par écoutes systématiques, localisation des fuites par corrélations acoustiques et écoutes au sol.";

        inputField_location.text = _sector.location;
        inputField_results.text = _sector.results;
        inputField_sumUp.text = _sector.sumUp;

        string path = $"{Sector.current.GetPath()}/sector_map.png";
        CheckPath(path);
    }

    bool CheckPath(string path) {
        bool b = File.Exists(path);
        _screenshot.gameObject.SetActive(b);
        _screenshot_icon.gameObject.SetActive(!b);
        if (b) {
            var rawData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(0, 0);
            tex.LoadImage(rawData);
            tex = PDFExport.CropTo16by9(tex);
            _screenshot.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        }
        return b;
    }

    public static Vector2d GetSphericalGeographicCenter(List<Vector2d> coords) {
        if (coords == null || coords.Count == 0)
            return new Vector2d(0, 0);

        double x = 0, y = 0, z = 0;

        foreach (var coord in coords) {
            double lat = coord.x * Mathf.Deg2Rad;
            double lon = coord.y * Mathf.Deg2Rad;

            x += Math.Cos(lat) * Math.Cos(lon);
            y += Math.Cos(lat) * Math.Sin(lon);
            z += Math.Sin(lat);
        }

        int total = coords.Count;
        x /= total;
        y /= total;
        z /= total;

        double centralLon = Math.Atan2(y, x);
        double hyp = Math.Sqrt(x * x + y * y);
        double centralLat = Math.Atan2(z, hyp);

        return new Vector2d(centralLat * Mathf.Rad2Deg, centralLon * Mathf.Rad2Deg);
    }

    public void Close() {
        FadeOut();
        SectorMap.Instance.Display(_sector);
    }
    public void CenterMap(Sector sector) {
        float totalX = 0, totalY = 0;
        Vector2d smallest = new Vector2d(double.MaxValue, double.MaxValue);
        Vector2d biggest = new Vector2d();
        Vector2d center = new Vector2d();
        List<Vector2d> coords = new List<Vector2d>();
        foreach (var leak in sector.leaks) {
            center += new Vector2d(leak.latitude, leak.longitude);
            totalX += (float)leak.latitude;
            totalY += (float)leak.longitude;
            coords.Add(new Vector2d(leak.latitude, leak.longitude));
            if (leak.latitude < smallest.x)
                smallest.x = leak.latitude;
            if (leak.latitude > biggest.x)
                biggest.x = leak.latitude;
            if (leak.longitude < smallest.y)
                smallest.y = leak.longitude;
            if (leak.longitude > biggest.y)
                biggest.y = leak.longitude;
        }
        center /= sector.leaks.Count;
        float centerX = totalX / sector.leaks.Count;
        float centerY = totalY / sector.leaks.Count;

        var span = biggest - smallest;
        Debug.Log($"small : {smallest.x}/{smallest.y}");
        Debug.Log($"big : {biggest.x}/{biggest.y}");
        Debug.Log($"span : {span.x}/{span.y} (m:{span.magnitude})");
        var d = span.magnitude * 1000;
        Debug.Log($"delta : {d}");

        float dZoom = maxZoom;
        if (d > 0) {
            float lerp = (float)d / maxDelta;
            Debug.Log($"lerp : {lerp}");
            dZoom = Mathf.Lerp(maxZoom, minZoom, lerp);
            Debug.Log($"target zoo : {dZoom}");
        }
        
        // get middle of all leaks
        var latlon = new Mapbox.Utils.Vector2d(centerX, centerY);
        center = GetSphericalGeographicCenter(coords);
        MapManager.Instance._map.UpdateMap(center, dZoom);
    }

    public void SaveReport() {
        LocationConfirm();
        ResultsConfirm();
        SumUpConfirm();
        Close();
    }


    public int ZoomToBounds(List<Vector2d> coords) {
        if (coords.Count < 2)
            return report_zoom;
        var bounds = new LatLonBounds(coords[0]);
        for (int i = 1; i < coords.Count; i++)
            bounds.Encapsulate(coords[i]);
        return EstimateZoomLevel(bounds);

    }
    int EstimateZoomLevel(LatLonBounds bounds) {
        double maxSpan = Math.Max(bounds.LatSpan, bounds.LonSpan);
        double zoom = Math.Log(360 / maxSpan, 2);
        return Mathf.Clamp((int)zoom, 0, 22);
    }

    public void LocationConfirm() {
        _sector.location = inputField_location.text;
    }

    public void ResultsConfirm() {
        _sector.results = inputField_results.text;
    }

    public void SumUpConfirm() {
        _sector.sumUp = inputField_sumUp.text;
    }

    public void SendReport() {
        MailManager.instance.SendSectorReport();
    }

    void SendReportConfirm() {
        PDFExport.Instance.GenerateReport_Sector(Sector.current);
    }



    public struct LatLonBounds {
        public double North;
        public double South;
        public double East;
        public double West;

        public LatLonBounds(Vector2d firstCoord) {
            North = South = firstCoord.x; // latitude
            East = West = firstCoord.y;   // longitude
        }

        public void Encapsulate(Vector2d coord) {
            North = Math.Max(North, coord.x);
            South = Math.Min(South, coord.x);
            East = Math.Max(East, coord.y);
            West = Math.Min(West, coord.y);
        }

        public Vector2d Center => new Vector2d(
            (North + South) / 2.0,
            (East + West) / 2.0
        );

        public double LatSpan => North - South;
        public double LonSpan => East - West;
    }

}
