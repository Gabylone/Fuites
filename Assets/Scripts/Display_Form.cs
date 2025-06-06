using JetBrains.Annotations;
using Mapbox.Geocoding;
using Mapbox.Unity;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class Display_Form : Displayable
{
    public static Display_Form Instance;

    private Leak _leak;

    public TMP_InputField field_id;
    public TMP_InputField field_Date;
    public TMP_InputField field_GPS;
    public TMP_InputField field_Adress;
    public Selector field_Type;
    public TMP_InputField field_Material;
    public Selector field_Urgency;
    public TMP_InputField field_Infos;
    public ScrollRect scrollRect;

    public PhotoButton[] photoButtons;
    public Image _screenshot;
    public GameObject _screenshot_icon;
    
    ReverseGeocodeResource _resource;
    Geocoder _geocoder;
    public ReverseGeocodeResponse _response;

    private void Awake() {
        Instance = this;
    }

    void HandleGeoCoderResponse(ReverseGeocodeResponse res) {
        field_Adress.text = res.Features[0].PlaceName;
    }

    public void DisplayCurrentLeak() {
        Display(_leak);
    }

    public void Display(Leak leak) {

        Leak.current = leak;

        // handling leak folder
        string folderPath = $"{Leak.current.GetPath()}";
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }

        _leak = leak;

        field_id.text = leak.name;
        field_Date.text = leak.date;

        if (string.IsNullOrEmpty(leak.date)) {
            string date = DateTime.Now.ToString("dd/MM/yyyy");
            field_Date.text = date;
        }

        field_GPS.text = $"{leak.latitude}\n{leak.longitude}";
        field_Adress.text = leak.adress;
        field_Type.Select(leak.leakType);

        field_Material.text = leak.material;

        field_Urgency.Select(leak.urgency);
        field_Infos.text = leak.infos;

        LoadPictures();

        FadeIn();
        scrollRect.verticalNormalizedPosition = 1f;

        // get adress
        _resource = new ReverseGeocodeResource(MapManager.Instance._map.CenterLatitudeLongitude);
        _resource.Query = MapManager.Instance._map.CenterLatitudeLongitude;
        _geocoder = MapboxAccess.Instance.Geocoder;
        _geocoder.Geocode(_resource, HandleGeoCoderResponse);
    }

    public void LoadPictures() {
        foreach (var item in photoButtons)
            item.Display();

        string path = $"{Leak.current.GetPath()}/map.png";
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

    public void SaveLeakButton() {
        SaveLeak();
        Close();
    }

    public void Close() {
        FadeOut();
        Sector.current.Save();
        SectorMap.Instance.Display();
    }

    public void SaveLeak() {

        // assign all data to leak 
        _leak.name = field_id.text;
        _leak.date = field_Date.text;
        _leak.adress = field_Adress.text;
        _leak.leakType = field_Type.CurrentSelection;
        _leak.material = field_Material.text;
        _leak.urgency= field_Urgency.CurrentSelection;
        _leak.infos = field_Infos.text;

        if (Sector.current.leaks.Find(x => x.id == _leak.id) == null)
            SectorMap.Instance._sector.leaks.Add(_leak);
    }

    public void SendMail() {
        SendFormConfirm();

        /*string path = $"{Leak.current.GetPath()}/map.png";
        if (!File.Exists(path)) {
            DisplayMessage.instance.Display("Voulez vous envoyer le rapport de fuite sans capture d'écran ?", SendFormConfirm);
        } else {
        }*/
    }

    public void SendFormConfirm() {
        SaveLeak();
        MailManager.instance.SendLeakReport(_leak);
    }
}
