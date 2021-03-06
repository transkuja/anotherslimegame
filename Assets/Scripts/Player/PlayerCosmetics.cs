﻿using System.Collections;
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
    Texture bodyTexture;

    [SerializeField]
    string earsTexture;

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

    [SerializeField]
    float metallic = 0f;
    [SerializeField]
    float smoothness = 0f;

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
                SetEarsColor(bodyColor);
            }
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

        SetSkinTextures("Skins/None");
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
                if (data == null)
                {
                    Debug.LogWarning("Can't find data of " + skin + " in the database");
                    skin = "None";
                    metallic = 0f;
                    smoothness = 0f;
                    return;
                }
                metallic = data.metallic;
                smoothness = data.smoothness;
                SkinType = data.skinType;
                SetSkinTextures(data.texture);
            }
            else
            {
                BodyTexture = null;
                EarsTexture = "None";
                SkinType = SkinType.Color;
            }
        }
    }

    void SetSkinTextures(string baseTexturePath)

    {
        BodyTexture = Resources.Load(baseTexturePath) as Texture;
        Texture tex = Resources.Load(baseTexturePath + "_Metallic") as Texture;
        bodyMat.SetFloat("_Metallic", tex == null? 0 : metallic);
        bodyMat.SetTexture("_MetallicTex", tex);
        tex = Resources.Load(baseTexturePath + "_Smoothness") as Texture;
        bodyMat.SetFloat("_Smoothness", tex == null ? 0 : smoothness);
        bodyMat.SetTexture("_SmoothnessTex", tex);
        //bodyMat.SetTexture("_Normal", Resources.Load(baseTexturePath + "_Normal") as Texture);
        bodyMat.SetTexture("_Height", Resources.Load(baseTexturePath + "_Height") as Texture);
        EarsTexture = baseTexturePath;

        if (skinType == SkinType.Texture)
            SetEarsColor(Color.white);
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

    public string EarsTexture
    {
        get
        {
            return earsTexture;
        }

        set
        {
            if (!bodyMat)
                Init();

            earsTexture = value;
            if (ears == "None" || earsMats == null || earsMats.Length == 0)
                return;

            foreach (Material mat in earsMats)
            {
                mat.SetTexture("_MainTex", Resources.Load(earsTexture + "_Ears") as Texture);
                bodyMat.SetFloat("_Metallic", .05f);
                mat.SetTexture("_MetallicTex", Resources.Load(earsTexture + "_Ears_Metallic") as Texture);
                bodyMat.SetFloat("_Smoothness", .4f);
                mat.SetTexture("_SmoothnessTex", Resources.Load(earsTexture + "_Ears_Smoothness") as Texture);

                if(earsTexture == "Skins/None" || earsTexture == "Skins/Base1" || earsTexture == "Skins/Base2")
                {
                    mat.SetFloat("_Metallic", .05f);
                    mat.SetFloat("_Smoothness", .4f);
                }
                else
                {
                    mat.SetFloat("_Metallic", 0f);
                    mat.SetFloat("_Smoothness", 0f);
                }
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
            if (Mustache != "None" && Mustache != string.Empty)
            {
                if (DatabaseManager.Db == null)
                    DatabaseManager.LoadDb();
                DatabaseClass.MustacheData data = ((DatabaseClass.MustacheData)DatabaseManager.Db.GetDataFromId<DatabaseClass.MustacheData>(mustache));
                if(data == null)
                {
                    Debug.LogWarning("Can't find data of " + mustache + " in the database");
                    mustache = "None";
                    return;
                }
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
                if (data == null)
                {
                    Debug.LogWarning("Can't find data of " + hat + " in the database");
                    hat = "None";
                    return;
                }

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

                if (data == null)
                {
                    Debug.LogWarning("Can't find data of " + ears + " in the database");
                    ears = "None";
                    return;
                }

                ICustomizable earCustom = ((GameObject)Instantiate(Resources.Load(data.model), earsTransform)).GetComponent<ICustomizable>();
                earCustom.Init(GetComponentInParent<Rigidbody>());
                InitEarsMats();
                if(EarsTexture != null)
                {
                    EarsTexture = earsTexture;
                }
                else
                {
                    EarsTexture = "None";
                }
                if (skinType == SkinType.Color || skinType == SkinType.Mixed)
                    SetEarsColor(BodyColor);
                else
                    SetEarsColor(Color.white);
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
                if (data == null)
                {
                    Debug.LogWarning("Can't find data of " + accessory + " in the database");
                    accessory = "None";
                    return;
                }

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
                if (data == null)
                {
                    Debug.LogWarning("Can't find data of " + forehead + " in the database");
                    forehead = "None";
                    return;
                }
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
                    Debug.LogWarning("Can't find data of " + chin + " in the database");
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
        EarsTexture = earsTexture;
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

    public void RandomSelection()
    {
        // Clean up
        Skin = "None";
        Ears = "None";
        Hat = "None";
        Mustache = "None";
        Accessory = "None";
        Chin = "None";
        Forehead = "None";

        // Random
        bool hasSkin = (Random.Range(0, 2) == 1);

        if (hasSkin)
        {
            Skin = DatabaseManager.Db.skins[Random.Range(0, DatabaseManager.Db.skins.Count)].Id;
        }
        else
        {
            SetUniqueColor(DatabaseManager.Db.colors[Random.Range(0, DatabaseManager.Db.colors.Count)].color);
        }

        FaceType = DatabaseManager.Db.faces[Random.Range(0, DatabaseManager.Db.faces.Count)].indiceForShader;
        bool hasEars = (Random.Range(0, 2) == 1);
        bool hasHat = (Random.Range(0, 2) == 1);
        bool hasMustache = (Random.Range(0, 2) == 1);
        bool hasAccessory = (Random.Range(0, 2) == 1);
        bool hasChin = (Random.Range(0, 2) == 1);
        bool hasForehead = (Random.Range(0, 4) == 1);

        if (hasHat)
        {
            Hat = DatabaseManager.Db.hats[Random.Range(0, DatabaseManager.Db.hats.Count)].Id;
        }
        else
        {
            if (hasEars)
            {
                Ears = DatabaseManager.Db.ears[Random.Range(0, DatabaseManager.Db.ears.Count)].Id;
            }
        }

        if (hasMustache)
        {
            Mustache = DatabaseManager.Db.mustaches[Random.Range(0, DatabaseManager.Db.mustaches.Count)].Id;
        }
        if (hasAccessory)
        {
            Accessory = DatabaseManager.Db.accessories[Random.Range(0, DatabaseManager.Db.accessories.Count)].Id;
        }
        if (hasChin)
        {
            Chin = DatabaseManager.Db.chins[Random.Range(0, DatabaseManager.Db.chins.Count)].Id;
        }
        if (hasForehead)
        {
            Forehead = DatabaseManager.Db.foreheads[Random.Range(0, DatabaseManager.Db.foreheads.Count)].Id;
        }
    }

    public void RandomSelectionUnlocked()
    {
        // Clean up
        Skin = "None";
        Ears = "None";
        Hat = "None";
        Mustache = "None";
        Accessory = "None";
        Chin = "None";
        Forehead = "None";

        // Random
        bool hasSkin = (Random.Range(0, 2) == 1);

        if (hasSkin)
        {
            List<DatabaseClass.SkinData> skins = new List<DatabaseClass.SkinData>();
            skins.AddRange(DatabaseManager.Db.skins);
            foreach (DatabaseClass.SkinData data in DatabaseManager.Db.skins)
            {
                if (!data.isUnlocked)
                    skins.Remove(data);
            }
            if (skins.Count > 0)
                Skin = skins[Random.Range(0, skins.Count)].Id;
        }
        else
        {
            SetUniqueColor(DatabaseManager.Db.colors[Random.Range(0, DatabaseManager.Db.colors.Count)].color);
        }

        FaceType = DatabaseManager.Db.faces[Random.Range(0, DatabaseManager.Db.faces.Count)].indiceForShader;
        bool hasEars = (Random.Range(0, 2) == 1);
        bool hasHat = (Random.Range(0, 2) == 1);
        bool hasMustache = (Random.Range(0, 2) == 1);
        bool hasAccessory = (Random.Range(0, 2) == 1);
        bool hasChin = (Random.Range(0, 2) == 1);
        bool hasForehead = (Random.Range(0, 4) == 1);

        if (hasHat)
        {
            List<DatabaseClass.HatData> hats = new List<DatabaseClass.HatData>();
            hats.AddRange(DatabaseManager.Db.hats);
            foreach (DatabaseClass.HatData data in DatabaseManager.Db.hats)
            {
                if (!data.isUnlocked)
                    hats.Remove(data);
            }
            if (hats.Count > 0)
                Hat = hats[Random.Range(0, hats.Count)].Id;
        }
        else
        {
            if (hasEars)
            {
                List<DatabaseClass.EarsData> ears = new List<DatabaseClass.EarsData>();
                ears.AddRange(DatabaseManager.Db.ears);
                foreach (DatabaseClass.EarsData data in DatabaseManager.Db.ears)
                {
                    if (!data.isUnlocked)
                        ears.Remove(data);
                }
                if (ears.Count > 0)
                    Ears = ears[Random.Range(0, ears.Count)].Id;
            }
        }

        if (hasMustache)
        {
            List<DatabaseClass.MustacheData> mustaches = new List<DatabaseClass.MustacheData>();
            mustaches.AddRange(DatabaseManager.Db.mustaches);
            foreach (DatabaseClass.MustacheData data in DatabaseManager.Db.mustaches)
            {
                if (!data.isUnlocked)
                    mustaches.Remove(data);
            }
            if (mustaches.Count > 0)
                Mustache = mustaches[Random.Range(0, mustaches.Count)].Id;
        }
        if (hasAccessory)
        {
            List<DatabaseClass.AccessoryData> accessories = new List<DatabaseClass.AccessoryData>();
            accessories.AddRange(DatabaseManager.Db.accessories);
            foreach (DatabaseClass.AccessoryData data in DatabaseManager.Db.accessories)
            {
                if (!data.isUnlocked)
                    accessories.Remove(data);
            }
            if (accessories.Count > 0)
                Accessory = accessories[Random.Range(0, accessories.Count)].Id;
        }
        if (hasChin)
        {
            List<DatabaseClass.ChinData> chins = new List<DatabaseClass.ChinData>();
            chins.AddRange(DatabaseManager.Db.chins);
            foreach (DatabaseClass.ChinData data in DatabaseManager.Db.chins)
            {
                if (!data.isUnlocked)
                    chins.Remove(data);
            }
            if (chins.Count > 0)
                Chin = chins[Random.Range(0, chins.Count)].Id;
        }
        if (hasForehead)
        {
            List<DatabaseClass.ForeheadData> foreheads = new List<DatabaseClass.ForeheadData>();
            foreheads.AddRange(DatabaseManager.Db.foreheads);
            foreach (DatabaseClass.ForeheadData data in DatabaseManager.Db.foreheads)
            {
                if (!data.isUnlocked)
                    foreheads.Remove(data);
            }
            if (foreheads.Count > 0)
                Forehead = foreheads[Random.Range(0, foreheads.Count)].Id;
        }
    }
}
