using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[System.Serializable]
public class Leak {
    public static Leak current;

    public static string[] leaksTypes_text = new string[6] {
        "canalisation",
        "branchement",
        "prise en charge",
        "après compteur",
        "poteau incendie",
        "autres"
    };

    public string id = "";
    public string name = "";
    public string date;
    public double latitude;
    public double longitude;
    public string adress;
    public int leakType = 5;
    public string GetLeakTypeText() {
        return leaksTypes_text[leakType];
    }
    public string material;
    public int urgency = 1;
    public string infos;


    public string GetPath() {
        return Path.Combine(Sector.current.GetPath(), id);
    }

    public string GetFolderName() {
        return id;
    }

    public string GetUrgency_text() {
        switch (urgency) {
            case 0:
                return "faible";
            case 1:
                return "moyenne";
            case 2:
                return "élévée";
            default:
                return "nn";
        }
    }
}