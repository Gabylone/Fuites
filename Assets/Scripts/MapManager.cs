using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using TMPro;
using System.Runtime.CompilerServices;
using Mapbox.Unity.Map;
using Mapbox.Examples;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public RectTransform _mapTransform;
    public AbstractMap _map;
    public RawImage _image;


    private void Awake() {
        Instance = this;
    }
    public void LocateAndDisplay() {
        if (Application.isMobilePlatform) {
            StartCoroutine(LocationCoroutine());
        } else {
            
        }
    }

    public void ChangeView() {
        if (_map.ImageLayer.LayerSource == ImagerySourceType.MapboxSatellite) {
            _map.ImageLayer.SetLayerSource(Mapbox.Unity.Map.ImagerySourceType.MapboxStreets);
        } else {
            _map.ImageLayer.SetLayerSource(Mapbox.Unity.Map.ImagerySourceType.MapboxSatellite);
        }
        _map.UpdateMap(_map.CenterLatitudeLongitude, _map.Zoom);


    }

    public void TestLocation() {
        StartCoroutine(LocationCoroutine());
    }

    IEnumerator LocationCoroutine() {
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
            Debug.Log("Location not enabled on device or app does not have permission to access location");


        // Starts the location service.
        float desiredAccuracyInMeters = 10f;
        float updateDistanceInMeters = 10f;

        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1) {
            Debug.Log("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed) {
            Debug.LogError("Unable to determine device location");
            yield break;
        } else {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            var latlon = new Mapbox.Utils.Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);
            _map.UpdateMap(latlon, _map.Zoom);
            Debug.Log($"found location : {latlon.x}, {latlon.y}");
            yield return null;
        }

        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
    }

    public void EnableMovement() {
        MapManager.Instance._map.GetComponent<QuadTreeCameraMovement>().enabled = true;
    }
    public void DisableMovement() {
        MapManager.Instance._map.GetComponent<QuadTreeCameraMovement>().enabled = false;

    }


    public void ChangeParent(Transform parent) {
        Debug.Log($"changing parent");
        _mapTransform.SetParent(parent);
        _mapTransform.offsetMax = Vector3.zero;
        _mapTransform.offsetMin = Vector3.zero;
    }
}

