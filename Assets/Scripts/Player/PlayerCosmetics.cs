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
public class PlayerCosmetics : MonoBehaviour {
    public Material[] originalPlayerMats = new Material[2];
    
    Material bodyMat;
    Material faceMat;
    Material[] earsMats;

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

    [SerializeField]
    string accessory = "None";

    [SerializeField]
    string forehead = "None";

    [SerializeField]
    string skin = "None";

    [SerializeField]
    string chin = "None";

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
            {
                bodyMat.color = bodyColor;
            }
            SetEarsColor(bodyColor);
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

    public string Skin
    {
        get
        {
            return skin;
        }

        set
        {
            skin = value;
            
            if (skin != "None" && skin != string.Empty)
            {
                if (DatabaseManager.Db == null)
                    DatabaseManager.LoadDb();

                DatabaseClass.SkinData data = ((DatabaseClass.SkinData)DatabaseManager.Db.GetDataFromId<DatabaseClass.SkinData>(skin));
                bodyTexture = Resources.Load(data.texture) as Texture;
                SkinType = SkinType.Texture;
            }
            else
            {
                bodyTexture = null;
                SkinType = SkinType.Color;
            }
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
            if (mustache == string.Empty)
                mustache = "None";
            return mustache;
        }

        set
        {
            mustache = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform mustacheTransform = customSockets.GetSocket(CustomizableType.Mustache);
            if (mustacheTransform == null)
            {
                Debug.LogWarning("Mustache Socket not found");
                return;
            }
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
            if (hat == string.Empty)
                hat = "None";
            return hat;
        }

        set
        {
            hat = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform hatTransform = customSockets.GetSocket(CustomizableType.Hat);
            if (hatTransform == null)
            {
                Debug.LogWarning("Hat Socket not found");
                return;
            }
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
            if (ears == string.Empty)
                ears = "None";
            return ears;
        }

        set
        {
            ears = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform earsTransform = customSockets.GetSocket(CustomizableType.Ears);
            if (earsTransform == null)
            {
                Debug.LogWarning("Ears Socket not found");
                return;
            }
            while (earsTransform.childCount > 0)
                DestroyImmediate(earsTransform.GetChild(0).gameObject);
            if (ears != "None" && ears != string.Empty)
            {
                if (DatabaseManager.Db == null)
                    DatabaseManager.LoadDb();

                DatabaseClass.EarsData data = ((DatabaseClass.EarsData)DatabaseManager.Db.GetDataFromId<DatabaseClass.EarsData>(ears));
                ICustomizable earCustom = ((GameObject)Instantiate(Resources.Load(data.model), earsTransform)).GetComponent<ICustomizable>();
                earCustom.Init(GetComponentInParent<Rigidbody>());
                InitEarsMats();
                SetEarsColor(BodyColor);
            }
        }
    }

    void SetEarsColor(Color color)
    {
        if(earsMats == null || earsMats.Length == 0)
            return;

        foreach(Material m in earsMats)
        {
            m.color = color;
        }
    }

    public string Accessory
    {
        get
        {
            if (accessory == string.Empty)
                accessory = "None";
            return accessory;
        }

        set
        {
            accessory = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform accessoryTransform = customSockets.GetSocket(CustomizableType.Accessory);
            if (accessoryTransform == null)
            {
                Debug.LogWarning("Accessory Socket not found");
                return;
            }
            while (accessoryTransform.childCount > 0)
                DestroyImmediate(accessoryTransform.GetChild(0).gameObject);
            if (accessory != "None" && accessory != string.Empty)
            {
                if (DatabaseManager.Db == null)
                    DatabaseManager.LoadDb();

                DatabaseClass.AccessoryData data = ((DatabaseClass.AccessoryData)DatabaseManager.Db.GetDataFromId<DatabaseClass.AccessoryData>(accessory));
                ICustomizable accessoryCustom = ((GameObject)Instantiate(Resources.Load(data.model), accessoryTransform)).GetComponent<ICustomizable>();
                accessoryCustom.Init(GetComponentInParent<Rigidbody>());
            }
        }
    }

    public string Forehead
    {
        get
        {
            if (forehead == string.Empty)
                forehead = "None";
            return forehead;
        }

        set
        {
            forehead = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform foreheadTransform = customSockets.GetSocket(CustomizableType.Forehead);
            if (foreheadTransform == null)
            {
                Debug.LogWarning("Forehead Socket not found");
                return;
            }
            while (foreheadTransform.childCount > 0)
                DestroyImmediate(foreheadTransform.GetChild(0).gameObject);
            if (forehead != "None" && forehead != string.Empty)
            {
                if (DatabaseManager.Db == null)
                    DatabaseManager.LoadDb();

                DatabaseClass.ForeheadData data = ((DatabaseClass.ForeheadData)DatabaseManager.Db.GetDataFromId<DatabaseClass.ForeheadData>(forehead));
                ICustomizable foreheadCustom = ((GameObject)Instantiate(Resources.Load(data.model), foreheadTransform)).GetComponent<ICustomizable>();
                foreheadCustom.Init(GetComponentInParent<Rigidbody>());
            }
        }
    }

    public string Chin
    {
        get
        {
            if (chin == string.Empty)
                chin = "None";
            return chin;
        }

        set
        {
            chin = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform chinTransform = customSockets.GetSocket(CustomizableType.Chin);
            if (chinTransform == null)
            {
                Debug.LogWarning("Chin Socket not found");
                return;
            }
            while (chinTransform.childCount > 0)
                DestroyImmediate(chinTransform.GetChild(0).gameObject);
            if (chin != "None" && chin != string.Empty)
            {
                if (DatabaseManager.Db == null)
                    DatabaseManager.LoadDb();

                DatabaseClass.ChinData data = ((DatabaseClass.ChinData)DatabaseManager.Db.GetDataFromId<DatabaseClass.ChinData>(chin));
                if(data == null)
                {
                    chin = "None";
                    return;
                }
                ICustomizable chinCustom = ((GameObject)Instantiate(Resources.Load(data.model), chinTransform)).GetComponent<ICustomizable>();
                chinCustom.Init(GetComponentInParent<Rigidbody>());
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

    void InitEarsMats()
    {
        Renderer[] earsRenderers = customSockets.GetSocket(CustomizableType.Ears).GetComponentsInChildren<Renderer>();
        if(earsRenderers == null || earsRenderers.Length == 0)
        {
            earsMats = null;
            return;
        }
        Material[] newEarsMaterials = new Material[earsRenderers.Length];

        for (int i = 0; i < newEarsMaterials.Length; i++)
        {
            newEarsMaterials[i] = new Material(originalPlayerMats[0]);
        }

        for (int i = 0; i < earsRenderers.Length; i++)
        {
            earsRenderers[i].sharedMaterial = newEarsMaterials[i];
        }
        earsMats = newEarsMaterials;
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
        Ears = ears;
        Accessory = accessory;
        Chin = chin;
        Forehead = forehead;
        Skin = skin;
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
