using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using UnityEngine;

[System.Serializable]
public class Sector
{
    // sectors
    public static Sector current;
    public static List<Sector> s_sectors = new List<Sector>();

    // prout
    public string id;
    public string name;
    public List<MailInfo> mailInfos = new List<MailInfo>();
    public List<Leak> leaks = new List<Leak>();
    public string location = "";
    public string results = "";
    public string sumUp = "";

    public Sector() {

    }

    /// <summary>
    /// SAVE / LOAD
    /// </summary>
    public void CreateDirectories() {
        if (Directory.Exists(name)) {
            Debug.LogError($"save file {name} already exists");
        } else {
            string root = GetPath();
            Directory.CreateDirectory(root);
        }
    }

    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        string path = $"{GetPath()}/save.dat";
        if (File.Exists(path)) {
            file = File.Open(path, FileMode.Open);
        } else {
            file = File.Create(path);
        }
        bf.Serialize(file, this);
        file.Close();
    }

    public static List<Sector> LoadAll() {
        string path = $"{Application.persistentDataPath}/saves";
        if (!Directory.Exists(path)) {
            Debug.Log($"creating saves directory");
            Directory.CreateDirectory(path);
        }
        Debug.Log("save path : " + path);

        string[] dirs = Directory.GetDirectories(path);
        var sectors = new List<Sector>();

        foreach (var directoryPath in dirs) {
            var saveFilePath = $"{directoryPath}/save.dat";
            if ( !File.Exists(saveFilePath) )
                continue;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveFilePath, FileMode.Open);
            sectors.Add((Sector)bf.Deserialize(file));
            file.Close();
        }

        return sectors;
    }

    public string GetPath() {
        return Path.Combine(Application.persistentDataPath, "saves", id);
    }

}
