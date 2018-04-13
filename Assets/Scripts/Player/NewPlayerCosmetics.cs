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
    Material bodyMat;
    Material faceMat;

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
    bool applyOnStart = false;

    int mustacheIndex;
    int hatIndex;
    int earsIndex;

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
            Debug.Log(DatabaseManager.Db.mustaches[value].Id + " selected !");
            mustacheIndex = value;
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
        }
    }

    public void SetUniqueColor(Color _color)
    {
        BodyColor = _color;
    }

    public void Init()
    {
        Renderer r = GetComponentInChildren<Renderer>();
        Material[] originals = r.sharedMaterials;
        r.materials = new Material[2];
        bodyMat = r.materials[0] = new Material(originals[0]);
        faceMat = r.materials[1] = new Material(originals[1]);

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
    }

    void SetValuesFromMaterials()
    {
        skinType = SkinType.Mixed;
        bodyColor = bodyMat.color;
        bodyTexture = bodyMat.GetTexture("_MainTex");
        faceType = (int)bodyMat.GetFloat("_FaceType");
        faceEmotion = (FaceEmotion)bodyMat.GetFloat("_FaceEmotion");
    }
}
