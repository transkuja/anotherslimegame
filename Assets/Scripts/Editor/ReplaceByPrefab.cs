using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReplaceByPrefab : EditorWindow {

    GameObject prefabModel;
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/ReplacePrefab")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ReplaceByPrefab window = (ReplaceByPrefab)EditorWindow.GetWindow(typeof(ReplaceByPrefab));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Replace all selected scene gameobject by \n this prefab model :", EditorStyles.boldLabel);
        prefabModel =(GameObject) EditorGUILayout.ObjectField(prefabModel,typeof(GameObject),true);
        if (GUILayout.Button("Replace all (DESTRUCTIVE ACTION)"))
        {
            if (prefabModel == null)
            {
                return;
            }
            else
                ReplaceAll();
        }
    }
    void ReplaceAll()
    {
        GameObject[] selectedGo = Selection.gameObjects;
        foreach(GameObject go in selectedGo)
        {
            //go.SetActive(false);
            Instantiate(prefabModel, go.transform.position, go.transform.rotation, go.transform.parent);
            DestroyImmediate(go);
        }
    }
}
