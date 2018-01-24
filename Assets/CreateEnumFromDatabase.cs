using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CreateEnumFromDatabase : MonoBehaviour {

    [SerializeField]
    public System.Enum enumeration;

    [SerializeField, HideInInspector]
    public string HideString;

    [SerializeField, HideInInspector]
    public int HideInt = 0;

    List<string> runesList;
    DatabaseClass.Database db;

    public void OnEnable()
    {
        db = Resources.Load("Database") as DatabaseClass.Database;
        runesList = new List<string>();
        foreach (DatabaseClass.RuneData s in db.runes)
        {
            runesList.Add(s.Id);
        }
        if(HideInt != 0)
            enumeration = EnumUtils.CreateEnumFromArrays(runesList, HideInt);
        else
            enumeration = EnumUtils.CreateEnumFromArrays(runesList, 0);
    }

}

[CustomEditor(typeof(CreateEnumFromDatabase))]
[CanEditMultipleObjects]
public class MyCustomEditor : Editor
{

    CreateEnumFromDatabase test;

    public void OnEnable()
    {
        test = (CreateEnumFromDatabase)target;

    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        test.enumeration = EditorGUILayout.EnumPopup(test.enumeration);
        test.HideString = test.enumeration.ToString();
        test.HideInt = (int)test.enumeration.GetType().GetField(test.HideString).GetValue(test.enumeration);

        EditorUtility.SetDirty(test);
    }
}