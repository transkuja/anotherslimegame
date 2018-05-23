using System.Collections;
using System.Collections.Generic;
using DatabaseClass;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DatabaseManager : MonoBehaviour {

    public static DatabaseManager instance;
    private static string saveDirectory;
    private static string saveFilePath;


    private static Database db;

    public static Database Db
    {
        get
        {
            return db;
        }

        set
        {
            db = value;
        }
    }

    // Use this for initialization
    public void Awake () {
        saveDirectory = Application.persistentDataPath + "/Saves";
        saveFilePath = saveDirectory + "/saves.json";
        if (LoadDb())
        {
            instance = this;
        }
        else
            Destroy(this);
    }

    public static bool LoadDb()
    {
        if (instance == null)
        {
            if(!LoadData())
                Db = Resources.Load("Database") as Database;
            return true;
        }
        return false;
    }

    public void SaveData()
    {
        #if UNITY_EDITOR
            return;
        #endif
        // save
        #pragma warning disable CS0162 // Impossible d'atteindre le code détecté
        try
        #pragma warning restore CS0162 // Impossible d'atteindre le code détecté
        {
            // if folder doesn't exist
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            // if file doesn't exist
            if (!File.Exists(saveFilePath))
            {
                FileStream file = File.Create(saveFilePath);
                file.Close();
            }

            // write
            string data = JsonUtility.ToJson(db);
            File.WriteAllText(saveFilePath, data);
        }
        catch
        {
            Debug.Log("couldn't write in file");
        }
    }

    public static bool LoadData()
    {
        #if UNITY_EDITOR
            return false;
        #endif

        // load
        #pragma warning disable CS0162 // Impossible d'atteindre le code détecté
        try
        #pragma warning restore CS0162 // Impossible d'atteindre le code détecté
        {
            if (!File.Exists(saveFilePath))
                return false;

            // load
            string dataAsJson = File.ReadAllText(saveFilePath);
            db = new Database();
            JsonUtility.FromJsonOverwrite(dataAsJson, db);
            return true;
        }
        catch
        {
            // will scriptable object in resource if it doesn't exist
            return false;
        }
    }
}
