using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkinType
{
    Color,
    Texture,
    Mixed
}

public class NewPlayerCosmetics : MonoBehaviour {
    Material bodyMat;
    Material faceMat;

    [SerializeField]
    bool useColorFade;

    [SerializeField]
    Color bodyColor;

    [SerializeField]
    Texture bodyTexture; // TEMP : Needs to be a choice field according to data from db

    [SerializeField]
    FaceType faceType;

    [SerializeField]
    FaceEmotion faceEmotion;

    [SerializeField]
    SkinType skinType;

    [SerializeField]
    bool applyOnStart = false;


    public Color BodyColor
    {
        get
        {
            return bodyColor;
        }

        set
        {
            bodyColor = value;
            if (skinType == SkinType.Color || skinType == SkinType.Mixed)
                bodyMat.color = bodyColor;
        }
    }

    public bool UseColorFade
    {
        get
        {
            return useColorFade;
        }

        set
        {

            int toAssign = value ? 1 : 0;
            bodyMat.SetInt("_ColorFade", toAssign);

            useColorFade = value;
        }
    }

    void ApplyTextureOnly()
    {
        bodyMat.color = Color.white;
        bodyMat.SetTexture("_MainTex", bodyTexture);
    }

    void ApplyColorOnly()
    {
        bodyMat.SetTexture("_MainTex", null);
        BodyColor = bodyColor;
    }

    void ApplyMixed()
    {
        bodyMat.SetTexture("_MainTex", bodyTexture);
        BodyColor = bodyColor;
    }

    public FaceType FaceType
    {
        get
        {
            return faceType;
        }

        set
        {
            faceType = value;
            faceMat.SetTextureOffset("_MainTex", new Vector2((float)faceType/8.0f, 1 - (float)faceEmotion/8.0f));
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
            faceEmotion = value;
            faceMat.SetTextureOffset("_MainTex", new Vector2((float)faceType / 8.0f, 1 - (float)faceEmotion / 8.0f));
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
            bodyTexture = value;
            if(skinType == SkinType.Texture || skinType == SkinType.Mixed)
            {
                bodyMat.SetTexture("_MainTex", bodyTexture);
            }
        }
    }

    public void SetUniqueColor(Color _color)
    {
        BodyColor = _color;
    }

    public void Init()
    {
        bodyMat = GetComponentInChildren<Renderer>().materials[0];
        faceMat = GetComponentInChildren<Renderer>().materials[1];

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
        faceType = (FaceType)bodyMat.GetFloat("_FaceType");
        faceEmotion = (FaceEmotion)bodyMat.GetFloat("_FaceEmotion");
    }

    void Awake () {
        if (bodyMat == null)
            Init();
	}

}
