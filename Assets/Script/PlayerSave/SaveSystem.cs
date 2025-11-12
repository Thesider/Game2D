using UnityEngine;
using System.IO;
using System;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/save.json";

    public static void Save(PlayerData data)
    {
        try
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string json = JsonUtility.ToJson(data, true);
            Debug.Log("Saving data: " + json);
            File.WriteAllText(path, json);
            Debug.Log("Saved to: " + path);
        }
        catch (Exception ex)
        {
            Debug.LogError("Save Failed: " + ex.Message);
        }
    }

    public static void Load(PlayerData data)
    {
        if (!File.Exists(path)) return;
        string json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, data);
    }
}
