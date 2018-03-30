using System.Collections;
using System.Collections.Generic;
using DatabaseClass;
using UnityEngine;

public class DatabaseManager : MonoBehaviour {

    public static DatabaseManager instance;

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
        if (LoadDb())
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this);
	}


    public static bool LoadDb()

    {
        if (instance == null)
        {
            Db = Resources.Load("Database") as Database;
            return true;
        }
        return false;
    }
}
