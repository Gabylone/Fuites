using System.Collections;
using System.IO;
using UnityEngine;
using NativeShareNamespace;
using UnityEngine.UI;

public class PhotoManager : MonoBehaviour {
    WebCamTexture webCamTexture;
    public RenderTexture mapTexture;
    public RawImage rawImage;
    public static PhotoManager Instance;
    public Displayable photo_Display;
    public Material photo_Material;
    private bool takingPicture = false;

    public delegate void OnPictureSaved(Texture2D tex);
    public OnPictureSaved onPictureSaved;

    public Displayable photoButton;


    public Image testImage;

    public Leak _leak;

    public int currentPhotoID = 1;

    private void Awake() {
        Instance = this;
    }

    public void TakeScreenshot() {
        //DisplayMessage.instance.Display("WIP :\ncapture d'écran");
        Display_Form.Instance.SaveLeak();
        DisplayScreenShot.Instance.ScreenLeak(Leak.current);
    }

    public void OpenCamera(int photoID) {
        currentPhotoID = photoID;

        /*webCamTexture = new WebCamTexture();
        photo_Material.mainTexture = webCamTexture; //Add Mesh Renderer to the GameObject to which this script is attached to
        webCamTexture.Play();
        rawImage.uvRect = new Rect(1, 0, -1, 1);*/
        int maxSize = 1024;
        NativeCamera.TakePicture((path) => {
            Debug.Log("Image path: " + path);
            if (path != null) {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
                if (texture == null) {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                string photoPath = $"{Leak.current.GetPath()}/photo_{currentPhotoID}.png";
                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes(photoPath, bytes);

                Display_Form.Instance.LoadPictures();
            }
        }, maxSize);

        /*photoButton.FadeIn();
        photo_Display.FadeIn();*/
    }

    public void Close() {
        photo_Display.FadeOut();
        Display_Form.Instance.DisplayCurrentLeak();
    }
}
