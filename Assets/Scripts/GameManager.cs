using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float delay = 2f;
    public string text = "Chargement des données";
    public Color[] urgencyColors;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Sector.s_sectors = Sector.LoadAll();
        DIsplayLoading.instance.Display(text, true);
        Invoke("Delay", 1f);
    }

    void Delay() {
        SectorSelection.Instance.Display();
        DIsplayLoading.instance.FadeOut();
    }
}
