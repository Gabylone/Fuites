using UnityEngine;
using UnityEngine.UI;

public class cameraTets : MonoBehaviour
{
    public RawImage rawImage;
    private void Start() {
        TakePicture(1024);
    }
    private void TakePicture(int maxSize) {
        NativeCamera.TakePicture((path) => {
            Debug.Log("Image path: " + path);
            if (path != null) {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                if (texture == null) {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                rawImage.texture = texture;

                //Destroy(texture, 5f);
            }
        }, maxSize);
    }
}
