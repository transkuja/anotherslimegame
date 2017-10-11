using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(JumpManager))]
public class ParabolaInspector : Editor {

    public JumpManager pm;
    public bool toggleParabola;
    SerializedProperty[] parabolaPropTab;
    private void OnEnable()
    {
        pm = (target as JumpManager);
        parabolaPropTab = new SerializedProperty[3];
        parabolaPropTab[0] = serializedObject.FindProperty("jumpToTest.upParabola");
        parabolaPropTab[1] = serializedObject.FindProperty("jumpToTest.fallingParabola");
        parabolaPropTab[2] = serializedObject.FindProperty("jumpToTest.minJumpParabola");

    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        if (pm.jumpTab != null)
            for (int i = 0; i < pm.jumpTab.Length;i++)
            {
                string name = pm.jumpTab[i].Name != string.Empty ? pm.jumpTab[i].Name : "Parabola "+ i.ToString();
                if (GUILayout.Button("Show "+ name))
                {
                    ParabolaGraph.CreateGraph(this, pm.jumpTab[i]);
                }
            }

        serializedObject.ApplyModifiedProperties();
    }
}
