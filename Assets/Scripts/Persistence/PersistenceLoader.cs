using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Boomlagoon.JSON;

public class PersistenceData
{
    public Dictionary<string, bool> minigames;
    public Dictionary<string, bool> colors;
    public Dictionary<string, bool> faces;
    public List<bool> options;

    public PersistenceData()
    {
        minigames = new Dictionary<string, bool>();
        colors = new Dictionary<string, bool>();
        faces = new Dictionary<string, bool>();
        options = new List<bool>();
    }
}

public class PersistenceLoader {

    private PersistenceData data;

    private JSONObject json;

    private bool isOverrided;

    public PersistenceLoader()
    {
        data = new PersistenceData();
        isOverrided = true;
    }

    #region Accessors
    public PersistenceData Data
    {
        get
        {
            return data;
        }

        set
        {
            data = value;
        }
    }
    #endregion
    public void Load()
    {
        // Create Local Savegame
        string pathBase = Application.persistentDataPath;
        string fileContents;
        
        if (isOverrided || !File.Exists(pathBase + "/sauvegarde.json"))
        {
            string path = Path.Combine(Application.streamingAssetsPath, "sauvegardeReset.json");

            if (path.Contains("://"))
            {
                WWW www = new WWW(path);
                fileContents = www.text;
            }
            else
                fileContents = File.ReadAllText(path);

            File.WriteAllText(pathBase + "/sauvegarde.json", fileContents);
        }
        else
            fileContents = File.ReadAllText(pathBase + "/sauvegarde.json");

        // Load 
        json = JSONObject.Parse(fileContents);

        JSONArray minigamesArray = json["Minigames"].Array;
        foreach (JSONValue value in minigamesArray)
        {
            string id = String.Empty;
            bool isUnlocked = false;
            foreach (KeyValuePair<string, JSONValue> minigameEntry in value.Obj)
            {
                switch (minigameEntry.Key)
                {
                    // ROOT DATA
                    case "id":
                        id = minigameEntry.Value.Str;
                        break;
                    case "unlocked":
                        isUnlocked = minigameEntry.Value.Boolean;
                        break;
                }
            }
            if (!data.minigames.ContainsKey(id)) { }
                data.minigames.Add(id, isUnlocked);
        }

        JSONArray colorsArray = json["Colors"].Array;
        foreach (JSONValue value in colorsArray)
        {
            string id = String.Empty;
            bool isUnlocked = false;
            foreach (KeyValuePair<string, JSONValue> colorEntry in value.Obj)
            {
                switch (colorEntry.Key)
                {
                    // ROOT DATA
                    case "id":
                        id = colorEntry.Value.Str;
                        break;
                    case "unlocked":
                        isUnlocked = colorEntry.Value.Boolean;
                        break;
                }
            }
            if (!data.colors.ContainsKey(id))
            {
                data.colors.Add(id, isUnlocked);
            }

        }

        JSONArray facesArray = json["Faces"].Array;
        foreach (JSONValue value in facesArray)
        {
            string id = String.Empty;
            bool isUnlocked = false;
            foreach (KeyValuePair<string, JSONValue> faceEntry in value.Obj)
            {
                switch (faceEntry.Key)
                {
                    // ROOT DATA
                    case "id":
                        id = faceEntry.Value.Str;
                        break;
                    case "unlocked":
                        isUnlocked = faceEntry.Value.Boolean;
                        break;
                }
            }
            if (!data.faces.ContainsKey(id))
                data.faces.Add(id, isUnlocked);
        }

        //JSONObject PreferenceObject = json["Preference"].Obj;
        //foreach (KeyValuePair<string, JSONValue> optionEntry in PreferenceObject)
        //{
        //    switch (optionEntry.Key)
        //    {
        //        // ROOT DATA
        //        case "testForOption":
        //            if (!data.options.Contains(optionEntry.Value.Boolean))
        //            {
        //                data.options.Add(optionEntry.Value.Boolean);
        //            }
        //            break;
        //    }
        //}
    }

    public void SetMinigameUnlocked(string key, bool isUnlocked)
    {
        SetUnlocked("Minigames", key, "unlocked", isUnlocked);
    }

    public void SetColorUnlocked(string key, bool isUnlocked)
    {
        SetUnlocked("Colors", key, "unlocked", isUnlocked);
    }

    public void SetFaceUnlocked(string key, bool isUnlocked)
    {
        SetUnlocked("Faces", key, "unlocked", isUnlocked);
    }

    //public void SetOptionUnlocked(bool isActive)
    //{
    //    SetOption("Preference", "testForOption", isActive);
    //}

    private void SetUnlocked(string what, string id, string secondKey, bool isUnlocked)
    {
        JSONArray array = json[what].Array;
        bool isRightKey;
        foreach (JSONValue value in array)
        {
            isRightKey = false;
            foreach (KeyValuePair<string, JSONValue> persistedEntry in value.Obj)
            {
                if (persistedEntry.Key == "id")
                {
                    if (persistedEntry.Value.Str == id)
                        isRightKey = true;
                }

                if (isRightKey == true && persistedEntry.Key == secondKey)
                {
                     persistedEntry.Value.Boolean = isUnlocked;
                     break;
                }
            }
        }
        string pathBase = Application.persistentDataPath;
        File.WriteAllText(pathBase + "/sauvegarde.json", json.ToString());

        return;
    }

    //private void SetOption(string what, string id, bool isUnlocked)
    //{

    //    JSONObject CameraObject = json[what].Obj;
    //    foreach (KeyValuePair<string, JSONValue> optionEntry in CameraObject)
    //    {
    //        if (optionEntry.Key == id)
    //        {
    //            optionEntry.Value.Boolean = isUnlocked;
    //            break;
    //        }
    //    }

    //    string pathBase = Application.persistentDataPath;
    //    File.WriteAllText(pathBase + "/sauvegarde.json", json.ToString());

    //    return;
    //}
}



