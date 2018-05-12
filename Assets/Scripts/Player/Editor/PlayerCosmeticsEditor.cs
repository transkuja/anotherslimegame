using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DatabaseClass;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(PlayerCosmetics))]
public class PlayerCosmeticsEditor : Editor
{
    PlayerCosmetics cosmetics;
    private void Awake()
    {
        cosmetics = (PlayerCosmetics)target;
    }
    List<string> facesList;
    List<string> mustachesList;
    List<string> hatsList;
    List<string> earsList;
    List<string> accessoriesList;
    List<string> chinsList;
    List<string> skinsList;
    List<string> foreheadsList;

    public void OnEnable()
    {
        DatabaseManager.LoadDb();

        facesList = new List<string>();
        mustachesList = new List<string>();
        hatsList = new List<string>();
        earsList = new List<string>();
        accessoriesList = new List<string>();
        chinsList = new List<string>();
        skinsList = new List<string>();
        foreheadsList = new List<string>();

        mustachesList.Add("None");
        hatsList.Add("None");
        earsList.Add("None");
        accessoriesList.Add("None");
        chinsList.Add("None");
        skinsList.Add("None");
        foreheadsList.Add("None");

        foreach (FaceData s in DatabaseManager.Db.faces)
        {
            facesList.Add(s.Id);
        }

        foreach (MustacheData s in DatabaseManager.Db.mustaches)
        {
            mustachesList.Add(s.Id);
        }

        foreach (HatData s in DatabaseManager.Db.hats)
        {
            hatsList.Add(s.Id);
        }

        foreach (EarsData s in DatabaseManager.Db.ears)
        {
            earsList.Add(s.Id);
        }

        foreach (AccessoryData s in DatabaseManager.Db.accessories)
        {
            accessoriesList.Add(s.Id);
        }

        foreach (ChinData s in DatabaseManager.Db.chins)
        {
            chinsList.Add(s.Id);
        }

        foreach (SkinData s in DatabaseManager.Db.skins)
        {
            skinsList.Add(s.Id);
        }

        foreach (ForeheadData s in DatabaseManager.Db.foreheads)
        {
            foreheadsList.Add(s.Id);
        }

        curFaceType = cosmetics.FaceType;
        curFaceEmotion = cosmetics.FaceEmotion;
        curMustache = GetIndexFromString(cosmetics.Mustache, CustomizableType.Mustache);
        curHat = GetIndexFromString(cosmetics.Hat, CustomizableType.Hat);
        curEars = GetIndexFromString(cosmetics.Ears, CustomizableType.Ears);
        curChin = GetIndexFromString(cosmetics.Ears, CustomizableType.Chin);
        curForehead = GetIndexFromString(cosmetics.Ears, CustomizableType.Forehead);
        curAccessory = GetIndexFromString(cosmetics.Ears, CustomizableType.Accessory);
        curBodyColor = cosmetics.BodyColor;
        curTexture = cosmetics.BodyTexture;
        curSkinType = cosmetics.SkinType;
        curColorFadeType = cosmetics.ColorFadeType;

        if(cosmetics.originalPlayerMats == null || cosmetics.originalPlayerMats.Length < 1)
        {
            cosmetics.originalPlayerMats = new Material[2];
        }
    }
    
    int curFaceType = 0;
    FaceEmotion curFaceEmotion = FaceEmotion.Neutral;
    int curMustache = 0;
    int curHat = 0;
    int curEars = 0;
    int curChin = 0;
    int curForehead = 0;
    int curAccessory = 0;
    ColorFadeType curColorFadeType = 0;

    Color curBodyColor;
    Texture curTexture;
    SkinType curSkinType;


    public override void OnInspectorGUI()
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty("originalPlayerMats");
        EditorGUILayout.PropertyField(property, true);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Reset"))
            cosmetics.Init();
        if (GUILayout.Button("ResetAndApply"))
        {
            bool previousBool = cosmetics.applyOnStart;
            cosmetics.applyOnStart = true;
            cosmetics.Init();
            cosmetics.applyOnStart = previousBool;
        }
        GUILayout.Space(20);
        //base.DrawDefaultInspector();
        curSkinType = (SkinType)EditorGUILayout.EnumPopup("Skin Type", curSkinType);

        if (curSkinType != cosmetics.SkinType)
            cosmetics.SkinType = curSkinType;

        if(curSkinType == SkinType.Color || curSkinType == SkinType.Mixed)
        {
            curBodyColor = EditorGUILayout.ColorField("Color", curBodyColor);
            if (curBodyColor != cosmetics.BodyColor)
                cosmetics.BodyColor = curBodyColor;
        }

        if (curSkinType == SkinType.Texture || curSkinType == SkinType.Mixed)
        {
            curTexture = EditorGUILayout.ObjectField("Texture", curTexture, typeof(Texture), false) as Texture;
            if (curTexture != cosmetics.BodyTexture)
                cosmetics.BodyTexture = curTexture;
        }

        curColorFadeType = (ColorFadeType)EditorGUILayout.EnumPopup("Color Fade", curColorFadeType);
        if (curColorFadeType != cosmetics.ColorFadeType)
            cosmetics.ColorFadeType = curColorFadeType;

        if (facesList.Count > 0)
        {
            curFaceType = EditorGUILayout.Popup("Face Type", curFaceType, facesList.ToArray());
            if (cosmetics.FaceType != curFaceType)
                cosmetics.FaceType = curFaceType;
        }

        curFaceEmotion = (FaceEmotion)EditorGUILayout.EnumPopup("Face Emotion", curFaceEmotion);
        if (curFaceEmotion != cosmetics.FaceEmotion)
            cosmetics.FaceEmotion = curFaceEmotion;

        if (mustachesList.Count > 0)
        {
            curMustache = EditorGUILayout.Popup("Mustache", curMustache, mustachesList.ToArray());

            if (cosmetics.Mustache != mustachesList[curMustache])
                cosmetics.Mustache = mustachesList[curMustache];
        }

        if (hatsList.Count > 0)
        {
            curHat = EditorGUILayout.Popup("Hat", curHat, hatsList.ToArray());

            if (cosmetics.Hat != hatsList[curHat])
                cosmetics.Hat = hatsList[curHat];
        }

        if (earsList.Count > 0)
        {
            curEars = EditorGUILayout.Popup("Ears", curEars, earsList.ToArray());

            if (cosmetics.Ears != earsList[curEars])
                cosmetics.Ears = earsList[curEars];
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cosmetics);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
#endif
        }

    }

    int GetIndexFromString(string customizableName, CustomizableType type)
    {
        int toReturn = 0;
        switch(type)
        {
            case CustomizableType.Mustache:
                toReturn = mustachesList.FindIndex(x => x.Equals(customizableName));
                break;
            case CustomizableType.Hat:
                toReturn = hatsList.FindIndex(x => x.Equals(customizableName));
                break;
            case CustomizableType.Ears:
                toReturn = earsList.FindIndex(x => x.Equals(customizableName));
                break;
        }
        toReturn = (toReturn < 0) ? 0 : toReturn;

        return toReturn;
    }
}
