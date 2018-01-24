using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CreateRuneEnumFromDatabase : MonoBehaviour {

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

[CustomEditor(typeof(CreateRuneEnumFromDatabase))]
public class MyCustomEditor : Editor
{

    CreateRuneEnumFromDatabase runeEnum;

    public void OnEnable()
    {
        runeEnum = (CreateRuneEnumFromDatabase)target;

    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        runeEnum.enumeration = EditorGUILayout.EnumPopup(runeEnum.enumeration);
        runeEnum.HideString = runeEnum.enumeration.ToString();
        runeEnum.HideInt = (int)runeEnum.enumeration.GetType().GetField(runeEnum.HideString).GetValue(runeEnum.enumeration);

        EditorUtility.SetDirty(runeEnum);
    }
}