using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkinType
{
    Color,
    Texture,
    Mixed
}

public enum ColorFadeType
{
    None,
    Basic,
    Special
}

public class NewPlayerCosmetics : MonoBehaviour {
    public Material[] originalPlayerMats = new Material[2];
    
    Material bodyMat;
    Material faceMat;

    CustomizableSockets customSockets;

    [SerializeField]
    int colorFadeValue;

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
    public bool applyOnStart = false;

    [SerializeField]
    int mustacheIndex;

    [SerializeField]
    int hatIndex;

    [SerializeField]
    int earsIndex;

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

    public int ColorFadeValue
    {
        get
        {
            return colorFadeValue;
        }

        set
        {
            if (!bodyMat)
                Init();

            colorFadeValue = value;

            bodyMat.SetInt("_ColorFade", colorFadeValue);
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

    public int MustacheIndex
    {
        get
        {
            return mustacheIndex;
        }

        set
        {
            mustacheIndex = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform mustacheTransform = customSockets.GetSocket(CustomizableType.Mustache);
            while (mustacheTransform.childCount > 0)
                DestroyImmediate(mustacheTransform.GetChild(0).gameObject);
            if (mustacheIndex > 0)
            {
                ICustomizable mustache = ((GameObject)Instantiate(Resources.Load((DatabaseManager.Db.mustaches[mustacheIndex-1].model)), mustacheTransform)).GetComponent<ICustomizable>();
                mustache.Init(GetComponentInParent<Rigidbody>());
            }
        }
    }

    public int HatIndex
    {
        get
        {
            return hatIndex;
        }

        set
        {
            hatIndex = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform hatTransform = customSockets.GetSocket(CustomizableType.Hat);
            while (hatTransform.childCount > 0)
                DestroyImmediate(hatTransform.GetChild(0).gameObject);
            if (hatIndex > 0)
            {
                ICustomizable hat = ((GameObject)Instantiate(Resources.Load((DatabaseManager.Db.hats[hatIndex - 1].model)), hatTransform)).GetComponent<ICustomizable>();
                hat.Init(GetComponentInParent<Rigidbody>());
            }
        }
    }

    public int EarsIndex
    {
        get
        {
            return earsIndex;
        }

        set
        {
            earsIndex = value;
            if (!customSockets)
                customSockets = GetComponent<CustomizableSockets>();
            Transform earsTransform = customSockets.GetSocket(CustomizableType.Hat);
            while (earsTransform.childCount > 0)
                DestroyImmediate(earsTransform.GetChild(0).gameObject);
            if (earsIndex > 0)
            {
                ICustomizable ear = ((GameObject)Instantiate(Resources.Load((DatabaseManager.Db.hats[earsIndex - 1].model)), earsTransform)).GetComponent<ICustomizable>();
                ear.Init(GetComponentInParent<Rigidbody>());
            }
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
        MustacheIndex = mustacheIndex;
    }

    void SetValuesFromMaterials()
    {
        skinType = SkinType.Mixed;
        bodyColor = bodyMat.color;
        bodyTexture = bodyMat.GetTexture("_MainTex");
    }
}
