using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(CustomizableSockets))]
public class CustomizableSocketsEditor : Editor {
    CustomizableSockets customizableSockets;
    private void Awake()
    {
        customizableSockets = (CustomizableSockets)target;
    }

    public void OnEnable()
    {
        if (customizableSockets.sockets == null || customizableSockets.sockets.Length != (int)CustomizableType.Size - 2)
            customizableSockets.sockets = new Transform[(int)CustomizableType.Size - 2];
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Sockets", EditorStyles.boldLabel);
        for(int i = 0; i < (int)CustomizableType.Size - 2; i++)
        {
            customizableSockets.sockets[i] = EditorGUILayout.ObjectField(((CustomizableType)i+2).ToString(), customizableSockets.sockets[i], typeof(Transform), true) as Transform;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(customizableSockets);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

    }
}
