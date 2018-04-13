using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DatabaseClass;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(NewPlayerCosmetics))]
public class NewPlayerCosmeticsEditor : Editor
{
    NewPlayerCosmetics cosmetics;
    private void Awake()
    {
        cosmetics = (NewPlayerCosmetics)target;
    }
    List<string> facesList;
    List<string> mustachesList;
    List<string> hatsList;
    List<string> earsList;

    public void OnEnable()
    {
        DatabaseManager.LoadDb();

        facesList = new List<string>();
        mustachesList = new List<string>();
        mustachesList.Add("None");
        foreach (FaceData s in DatabaseManager.Db.faces)
        {
            facesList.Add(s.Id);
        }

        foreach (MustacheData s in DatabaseManager.Db.mustaches)
        {
            mustachesList.Add(s.Id);
        }
        curFaceType = cosmetics.FaceType;
        curMustache = cosmetics.MustacheIndex;
        curBodyColor = cosmetics.BodyColor;
        curTexture = cosmetics.BodyTexture;
        curSkinType = cosmetics.SkinType;
    }

    int curFaceType = 0;
    int curMustache = 0;
    Color curBodyColor;
    Texture curTexture;
    SkinType curSkinType;


    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Reset"))
            cosmetics.Init();
        if (GUILayout.Button("ResetAndApply"))
        {
            cosmetics.applyOnStart = true;
            cosmetics.Init();
            cosmetics.applyOnStart = false;
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

        if (facesList.Count > 0)
        {
            curFaceType = EditorGUILayout.Popup("Face Type", curFaceType, facesList.ToArray());
            if (cosmetics.FaceType != curFaceType)
                cosmetics.FaceType = curFaceType;
        }

        if (mustachesList.Count > 0)
        {
            curMustache = EditorGUILayout.Popup("Mustache", curMustache, mustachesList.ToArray());

            if (cosmetics.MustacheIndex != curMustache)
                cosmetics.MustacheIndex = curMustache;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cosmetics);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

    }
}
