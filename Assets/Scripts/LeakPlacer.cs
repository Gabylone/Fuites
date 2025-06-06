using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeakPlacer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public static LeakPlacer Instance;

    public Transform _worldTarget;
    public RectTransform _UITarget;
    public RectTransform _rectTransform;

    public bool canPlacePin = false;

    public List<DisplayPin> pins = new List<DisplayPin>();
    public DisplayPin prefab;
    public Transform parent;

    public DisplayPin placeholderPin;

    public Camera _camera;
    public float depth = 200f;

    private List<Leak> _leaks = new List<Leak>();

    public float clickDelay = 0.2f;
    public float maxDistanceClick = 1f;

    double pendingLat = 0f;
    double pendingLon = 0f;

    Vector2 _targetPos = Vector2.zero;
    float timer = 0f;

    Vector2 _exitPos = Vector2.zero;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        timer += Time.deltaTime;
    }

    private void LateUpdate() {
        UpdateLeaks();
    }
    public Image test;

    public void PlaceUniqueLeak(Leak leak) {
        PlaceLeaks(new List<Leak> { leak });
    }

    public void PlaceLeaks(List<Leak> leaks) {

        // reset //
        canPlacePin = true;
        MapManager.Instance.EnableMovement();
        placeholderPin.Hide();
        //

        _leaks = leaks;

        foreach (var item in pins) {
            item.Hide();
        }

        for (int i = 0; i < leaks.Count; i++) {
            var leak = leaks[i];
            if (pins.Count <= i)
                pins.Add(Instantiate(prefab, parent));
            pins[i].Display(leak);
        }

        UpdateLeaks();
    }



    void UpdateLeaks() {
        var x = 0f;
        var y = 0f;
        for (int i = 0; i < _leaks.Count; i++) {
            var leak = _leaks[i];
            var v = new Mapbox.Utils.Vector2d(leak.latitude, leak.longitude);
            var worldPos = MapManager.Instance._map.GeoToWorldPosition(v);
            var viewPort = _camera.WorldToViewportPoint(worldPos);
            var anchoredPos = viewPort * _rectTransform.rect.size;
            pins[i].GetRectTranform.anchoredPosition = anchoredPos;
            x+= pins[i].GetRectTranform.anchoredPosition.x;
            y+= pins[i].GetRectTranform.anchoredPosition.y;
        }

        //test.rectTransform.anchoredPosition = new Vector2(x, y) / _leaks.Count;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (!canPlacePin) return;
        // Reset timer
        timer = 0f;
        _targetPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData) {

        if (timer >= clickDelay) {
            //Debug.Log("touch : too late");
            return;
        }
        if (SectorMap.Instance.state != Displayable.State.visible) {
            return;
        }

        if ( Display_Report.Instance.state == Displayable.State.visible ) {
            //Debug.Log($"touch : rapport ouvert");
            return;
        }

        var dis = (_targetPos - eventData.position).magnitude;
        if (dis > maxDistanceClick) {
            //Debug.Log($"touch : too far ({dis}/{maxDistanceClick})");
            return;
        }

        _targetPos = eventData.position;
        //Debug.Log($"leak : pointer up");

        CancelInvoke("Delay");
        Invoke("Delay", 0.1f);
    }

    public Vector2d PositonTest (Vector2 targetpos) {

        _UITarget.position = targetpos;
        var viewportPos = _UITarget.anchoredPosition / _rectTransform.rect.size;
        // set world pos
        var worldpos = _camera.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, depth));
        _worldTarget.position = worldpos;

        // get lat lon
        var v = MapManager.Instance._map.WorldToGeoPosition(_worldTarget.position);
        pendingLat = v.x;
        pendingLon = v.y;
        return new Vector2d(pendingLat, pendingLon);
    }
    void Delay() {
        if (DisplayPin.pressedPin)
            return;

        canPlacePin = false;
        MapManager.Instance.DisableMovement();

        // set 2D target
        _UITarget.position = _targetPos;

        // get viewport
        var viewportPos = _UITarget.anchoredPosition / _rectTransform.rect.size;
        // set world pos
        var worldpos = _camera.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, depth));
        _worldTarget.position = worldpos;

        // get lat lon
        var v = MapManager.Instance._map.WorldToGeoPosition(_worldTarget.position);
        pendingLat = v.x;
        pendingLon = v.y;

        placeholderPin.Show();
        var v2 = new Mapbox.Utils.Vector2d(pendingLat, pendingLon);
        var worldPos = MapManager.Instance._map.GeoToWorldPosition(v2);
        var viewPort = _camera.WorldToViewportPoint(worldPos);
        var anchoredPos = viewPort * _rectTransform.rect.size;
        placeholderPin.GetRectTranform.anchoredPosition = anchoredPos;

        CancelInvoke("ShowMessage");
        Invoke("ShowMessage", 1f);
    }

    void ShowMessage() {
        DisplayMessage.instance.Display("Créer une nouvelle fuite ? ", HandleOnConfirmPinPlacement, CancelCallBack);
    }

    public void CancelCallBack() {
        MapManager.Instance.EnableMovement();
        canPlacePin = true;
        placeholderPin.FadeOut();
    }

    public void HandleOnConfirmPinPlacement() {
        var newLeak = new Leak();
        string id = StringUtils.SanitizeFileName(DateTime.Now.ToString());
        newLeak.id = id;
        newLeak.latitude = pendingLat;
        newLeak.longitude = pendingLon;
        newLeak.name = (Sector.current.leaks.Count+1).ToString();
        Display_Form.Instance.Display(newLeak);
        SectorMap.Instance.FadeOut();
    }
}
