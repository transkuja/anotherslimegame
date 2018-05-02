using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum FaceEmotion
{
    Neutral,
    Attack,
    Hit,
    Winner,
    Loser
}

[System.Serializable]
public enum SkinType
{
    Color,
    Texture,
    Mixed
}

[System.Serializable]
public enum ColorFadeType
{
    None,
    Basic,
    Special
}
[ExecuteInEditMode]
public class NewPlayerCosmetics : MonoBehaviour {
    public Material[] originalPlayerMats = new Material[2];
    
    Material bodyMat;
    Material faceMat;

    CustomizableSockets customSockets;

    [SerializeField]
    Color bodyColor;

    [SerializeField]
    Texture bodyTexture; // TEMP : Needs to be a choice field according to data from db

    [SerializeField]
    int faceType;

    [SerializeField]
    FaceEmotion faceEmotion;

    [SerializeField]
    SkinType skinType;

    [SerializeField]
    ColorFadeType colorFadeType = ColorFadeType.None;

    [SerializeField]
    public bool applyOnStart = false;

    [SerializeField]
    string mustache = "None";

    [SerializeField]
    string hat = "None";

    [SerializeField]
    string ears = "None";

    private void Awake()
    {
        Init(true);
    }

    public Color BodyColor
    {
        get
        {
            return bodyColor;
        }

        set
        {
            if (!bodyMat)
                Init();

            bodyColor = value;
            if (skinType == SkinType.Color || skinType == SkinType.Mixed)
                bodyMat.color = bodyColor;
        }
    }

    void ApplyTextureOnly()
    {
        if (!bodyMat)
            Init();

        bodyMat.color = Color.white;
        bodyMat.SetTexture("_MainTex", bodyTexture);
    }

    void ApplyColorOnly()
    {
        if (!bodyMat)
            Init();

        bodyMat.SetTexture("_MainTex", null);
        BodyColor = bodyColor;
    }

    void ApplyMixed()
    {
        if (!bodyMat)
            Init();

        bodyMat.SetTexture("_MainTex", bodyTexture);
        BodyColor = bodyColor;
    }

    public int FaceType
    {
        get
        {
            return faceType;
        }

        set
        {
            if (!faceMat)
                Init();
            faceType = value;
            faceMat.SetTextureOffset("_MainTex", new Vector2((faceType-1)/8.0f, 1 - (float)faceEmotion/8.0f)); // faceType-1 due to wrong uv config?
        }
    }

    public FaceEmotion FaceEmotion
    {
        get
        {
            return faceEmotion;
        }

        set
        {
            if (!faceMat)
                Init();
            faceEmotion = value;
            faceMat.SetTextureOffset("_MainTex", new Vector2((faceType - 1) / 8.0f, 1 - (float)faceEmotion / 8.0f));
        }
    }

    public SkinType SkinType
    {
        get
        {
            return skinType;
        }

        set
        {
            if (!faceMat || !bodyMat)
                Init();

            skinType = value;
            switch (skinType)
            {
                case SkinType.Color:
                    ApplyColorOnly();
                    break;
                case SkinType.Texture:
                    ApplyTextureOnly();
                    break;
                case SkinType.Mixed:
                    ApplyMixed();
                    break;
            }
        }
    }

    public Texture BodyTexture
    {
        get
        {
            return bodyTexture;
        }

        set
        {
            if (!bodyMat)
                Init();

            bodyTexture = value;
            if(skinType == SkinType.Texture || skinType == SkinType.Mixed)
            {
                bodyMat.SetTexture("_MainTex", bodyTexture);
            }
        }
    }

    public string Mustache
    {
        get
        {
            return mustache;
        }

        set
        {
            mustache = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform mustacheTransform = customSockets.GetSocket(CustomizableType.Mustache);
            while (mustacheTransform.childCount > 0)
                DestroyImmediate(mustacheTransform.GetChild(0).gameObject);
            if (mustache != "None" && mustache != string.Empty)
            {
                if (DatabaseManager.Db == null)
                    DatabaseManager.LoadDb();
                DatabaseClass.MustacheData data = ((DatabaseClass.MustacheData)DatabaseManager.Db.GetDataFromId<DatabaseClass.MustacheData>(mustache));
                
                ICustomizable mustacheCustom = ((GameObject)Instantiate(Resources.Load(data.model), mustacheTransform)).GetComponent<ICustomizable>();
                mustacheCustom.Init(GetComponentInParent<Rigidbody>());
            }
        }
    }

    public string Hat
    {
        get
        {
            return hat;
        }

        set
        {
            hat = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform hatTransform = customSockets.GetSocket(CustomizableType.Hat);
            while (hatTransform.childCount > 0)
                DestroyImmediate(hatTransform.GetChild(0).gameObject);
            
            if (hat != "None" && hat != string.Empty)
            {
                if (DatabaseManager.Db == null)
                    DatabaseManager.LoadDb();

                DatabaseClass.HatData data = ((DatabaseClass.HatData)DatabaseManager.Db.GetDataFromId<DatabaseClass.HatData>(hat));
                ICustomizable hatCustom = ((GameObject)Instantiate(Resources.Load(data.model), hatTransform)).GetComponent<ICustomizable>();
                hatCustom.Init(GetComponentInParent<Rigidbody>());
                
                Transform earsTransform = customSockets.GetSocket(CustomizableType.Ears);
                
                if (data.shouldHideEars)
                {
                    while (earsTransform.childCount > 0)
                        DestroyImmediate(earsTransform.GetChild(0).gameObject);
                }
                else if(earsTransform.childCount == 0)
                {
                    Ears = ears;
                }
            }
                
        }
    }

    public string Ears
    {
        get
        {
            return ears;
        }

        set
        {
            ears = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform earsTransform = customSockets.GetSocket(CustomizableType.Ears);
            while (earsTransform.childCount > 0)
                DestroyImmediate(earsTransform.GetChild(0).gameObject);
            if (ears != "None" && ears != string.Empty)
            {
                if (DatabaseManager.Db == null)
                    DatabaseManager.LoadDb();

                DatabaseClass.EarsData data = ((DatabaseClass.EarsData)DatabaseManager.Db.GetDataFromId<DatabaseClass.EarsData>(ears));
                ICustomizable earCustom = ((GameObject)Instantiate(Resources.Load(data.model), earsTransform)).GetComponent<ICustomizable>();
                earCustom.Init(GetComponentInParent<Rigidbody>());
            }
        }
    }

    public ColorFadeType ColorFadeType
    {
        get
        {
            return colorFadeType;
        }

        set
        {
            if (!bodyMat)
                Init();

            colorFadeType = value;
            bodyMat.SetInt("_ColorFade", (int)colorFadeType);
        }
    }

    public void SetUniqueColor(Color _color)
    {
        BodyColor = _color;
    }

    public void Init(bool _applyOnStart)
    {
        bool prevBool = applyOnStart;
        applyOnStart = _applyOnStart;
        Init();
        applyOnStart = prevBool;
    }

    public void Init()
    {
        customSockets = GetComponent<CustomizableSockets>();
        Renderer r = GetComponentInChildren<Renderer>();
        Material[] originals = r.sharedMaterials;
        Material[] newMaterials = new Material[originals.Length];

        if (originals == null || originals.Length < 1 || originals[0] == null)
        {
            originals = originalPlayerMats;
        }

        for(int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = new Material(originals[i]);
        }


        r.sharedMaterials = newMaterials;

        bodyMat = r.sharedMaterials[0];
        faceMat = r.sharedMaterials[1];

        if (applyOnStart)
            SetValuesFromEditor();
        else
            SetValuesFromMaterials();
    }

    void SetValuesFromEditor()
    {
        SkinType = skinType;
        BodyColor = bodyColor;
        BodyTexture = bodyTexture;
        FaceType = faceType;
        FaceEmotion = faceEmotion;
        Mustache = mustache;
        ColorFadeType = colorFadeType;
    }

    void SetValuesFromMaterials()
    {
        skinType = SkinType.Mixed;
        bodyColor = bodyMat.color;
        bodyTexture = bodyMat.GetTexture("_MainTex");
        ColorFadeType = (ColorFadeType)bodyMat.GetInt("_ColorFade");
    }
}
