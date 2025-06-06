using UnityEngine;

[ExecuteInEditMode]
public class PDFTest : MonoBehaviour
{
    private void Update() {
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.F) && Input.GetKeyDown(KeyCode.P)) {
            GameObject.FindAnyObjectByType<PDFExport>().Test();
        }
    }
}
