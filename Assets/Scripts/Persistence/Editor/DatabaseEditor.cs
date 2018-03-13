using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using DatabaseClass;
using UnityEngine;

[CustomEditor(typeof(Database))]
public class DatabaseEditor : Editor
{

    Database comp;

    public void OnEnable()
    {
        comp = (Database)target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Reset Database"))
            comp.ResetAll();

        if (GUILayout.Button("Unlock All"))
            comp.UnlockedAll();

        if (GUILayout.Button("All Cost to Zero"))
            comp.AllCostToZero();

        if (GUILayout.Button("All rune to -1 cost to 100"))
            comp.TestCost();

        base.OnInspectorGUI();

        EditorUtility.SetDirty(comp);
    }

}
