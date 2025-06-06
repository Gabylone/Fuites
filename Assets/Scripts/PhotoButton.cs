using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhotoButton : Displayable, IPointerClickHandler {

    public int currentPhotoID = 1;
    public Image _image;
    public GameObject _icon;

    public void Display() {
        CanvasGroup.alpha = 1;
        CheckPath($"{Leak.current.GetPath()}/photo_{currentPhotoID}.png");
    }

    bool CheckPath(string path) {
        bool b = File.Exists(path);
        _image.gameObject.SetActive(b);
        _icon.SetActive(!b);
        if (b) {
            var rawData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(0, 0);
            tex.LoadImage(rawData);
            _image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        }
        return b;
    }

    public void NoPictures() {
        CanvasGroup.alpha = 0.5f;
    }

    public void OnPointerClick(PointerEventData eventData) {
        Tween.Bounce(GetTransform);
        PhotoManager.Instance.OpenCamera(currentPhotoID);
    }
}
