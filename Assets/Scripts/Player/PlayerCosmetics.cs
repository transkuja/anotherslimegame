using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FaceType
{
    Face0,
    Face1,
    Face2,
    Face3,
    Face4,
    Face5,
    Face6
}



public class PlayerCosmetics : MonoBehaviour {
    Material bodyMat;
    Material[] handMats;
    Material[] earMats;

    [SerializeField]
    bool useColorFade;

    [SerializeField]
    Color bodyColor;

    [SerializeField]
    Color handsColor;

    [SerializeField]
    Color earColor;

    [SerializeField]
    FaceType faceType;

    [SerializeField]
    FaceEmotion faceEmotion;

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
            bodyMat.color = bodyColor;
        }
    }

    public Color HandsColor
    {
        get
        {
            return handsColor;
        }

        set
        {
            handsColor = value;
            foreach (Material m in handMats)
                m.color = handsColor;
        }
    }

    public Color EarColor
    {
        get
        {
            return earColor;
        }

        set
        {
            earColor = value;
            foreach (Material m in earMats)
                m.color = earColor;
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

            foreach (Material m in handMats)
                m.SetInt("_ColorFade", toAssign);

            foreach (Material m in earMats)
                m.SetInt("_ColorFade", toAssign);

            useColorFade = value;
        }
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
            bodyMat.SetFloat("_FaceType", (float)value);
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
            bodyMat.SetFloat("_FaceEmotion", (float)value);
        }
    }

    public void SetUniqueColor(Color _color)
    {
        BodyColor = _color;
        EarColor = _color;
        HandsColor = _color;
    }

    public void Init()
    {
        bodyMat = GetComponent<MeshRenderer>().material;

        handMats = new Material[2];
        earMats = new Material[2];

        for (int i = 0; i < 2; i++)
        {
            handMats[i] = transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().material;
            earMats[i] = transform.GetChild(1).GetChild(i).GetComponent<MeshRenderer>().material;
        }

        if (applyOnStart)
            SetValuesFromEditor();
        else
            SetValuesFromMaterials();
    }

    void SetValuesFromEditor()
    {
        BodyColor = bodyColor;
        HandsColor = handsColor;
        EarColor = earColor;

        FaceType = faceType;
        FaceEmotion = faceEmotion;
    }

    void SetValuesFromMaterials()
    {
        bodyColor = bodyMat.color;
        handsColor = handMats[0].color;
        earColor = earMats[0].color;

        faceType = (FaceType)bodyMat.GetFloat("_FaceType");
        faceEmotion = (FaceEmotion)bodyMat.GetFloat("_FaceEmotion");
    }

    void Awake () {
        if (bodyMat == null || handMats == null || earMats == null)
            Init();
	}

}
