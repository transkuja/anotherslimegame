using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CreateMinigameEnumFromDatabase : MonoBehaviour {

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
        foreach (DatabaseClass.MinigameData s in db.minigames)
        {
            runesList.Add(s.Id);
        }
        if(HideInt != 0)
            enumeration = EnumUtils.CreateEnumFromArrays(runesList, HideInt);
        else
            enumeration = EnumUtils.CreateEnumFromArrays(runesList, 0);
    }

}

[CustomEditor(typeof(CreateMinigameEnumFromDatabase))]
public class MyCustomEditor2 : Editor
{

    CreateMinigameEnumFromDatabase minigameEnum;

    public void OnEnable()
    {
        minigameEnum = (CreateMinigameEnumFromDatabase)target;

    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        minigameEnum.enumeration = EditorGUILayout.EnumPopup(minigameEnum.enumeration);
        minigameEnum.HideString = minigameEnum.enumeration.ToString();
        minigameEnum.HideInt = (int)minigameEnum.enumeration.GetType().GetField(minigameEnum.HideString).GetValue(minigameEnum.enumeration);

        EditorUtility.SetDirty(minigameEnum);
    }
}