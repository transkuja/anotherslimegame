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

    }
    int curFaceType = 0;
    int curMustache = 0;

    public override void OnInspectorGUI()
    {
        //base.DrawDefaultInspector();


        if (facesList.Count > 0)
            curFaceType = EditorGUILayout.Popup(curFaceType, facesList.ToArray());

        if (cosmetics.FaceType != curFaceType)
            cosmetics.FaceType = curFaceType;

        if (mustachesList.Count > 0)
            curMustache = EditorGUILayout.Popup(curMustache, mustachesList.ToArray());

        if (cosmetics.MustacheIndex != curMustache)
            cosmetics.MustacheIndex = curMustache;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cosmetics);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

    }
}
