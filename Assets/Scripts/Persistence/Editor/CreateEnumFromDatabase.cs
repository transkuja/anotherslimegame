using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using DatabaseClass;

[ExecuteInEditMode]
public class CreateEnumFromDatabase : MonoBehaviour {

    [SerializeField, HideInInspector]
    public int HideInt = 0;

    [SerializeField, HideInInspector]
    public List<string> enumFromList;

    [SerializeField, HideInInspector]
    public Database db;
}

[CustomEditor(typeof(CreateEnumFromDatabase))]
public class DatabaseEnumEditor : Editor
{

    CreateEnumFromDatabase dynamicChoice;
    DatabaseManager instance;


    private void Awake()
    {
        dynamicChoice = (CreateEnumFromDatabase)target;
    }

    public void OnEnable()
    {
        DatabaseManager.LoadDb();
        dynamicChoice.db = DatabaseManager.Db;

        bool isARune = dynamicChoice.GetComponent<Collectable>() || (dynamicChoice.GetComponent<CostArea>() && dynamicChoice.GetComponent<CostArea>().costAreaType == CostAreaType.PayAndGetItem && dynamicChoice.GetComponent<CostArea>() && dynamicChoice.GetComponent<CostArea>().rewardType == CollectableType.Rune);
        bool isAMinigame = dynamicChoice.GetComponent<CostArea>() && dynamicChoice.GetComponent<CostArea>().costAreaType == CostAreaType.PayAndUnlockMiniGame;
        bool isAColor = dynamicChoice.GetComponent<Collectable>() || (dynamicChoice.GetComponent<CostArea>() && dynamicChoice.GetComponent<CostArea>().costAreaType == CostAreaType.PayAndGetItem && dynamicChoice.GetComponent<CostArea>() && dynamicChoice.GetComponent<CostArea>().rewardType == CollectableType.Color);

        if (isARune)
        {
            dynamicChoice.enumFromList = new List<string>();
            foreach (RuneData s in dynamicChoice.db.runes)
            {
                dynamicChoice.enumFromList.Add(s.Id);
            }
        }
        else if (isAMinigame)
        {
            dynamicChoice.enumFromList = new List<string>();
            foreach (MinigameData s in dynamicChoice.db.minigames)
            {
                dynamicChoice.enumFromList.Add(s.Id);
            }
        }
        else if (isAColor)
        {
            dynamicChoice.enumFromList = new List<string>();
            foreach (ColorData s in dynamicChoice.db.colors)
            {
                dynamicChoice.enumFromList.Add(s.Id);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
     
        if (dynamicChoice.enumFromList.Count > 0)
            dynamicChoice.HideInt = EditorGUILayout.Popup(dynamicChoice.HideInt, dynamicChoice.enumFromList.ToArray());
        if (GUI.changed)
        {
            EditorUtility.SetDirty(dynamicChoice);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
   
    }
}