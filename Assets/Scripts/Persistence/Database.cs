using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum MinigameType { caca, MinigameAutre }

[CreateAssetMenu(fileName = "Data", menuName = "Test/Database", order = 1)]
public class Database : ScriptableObject
{
    [SerializeField]
    public List<ColorData> colors;

    [SerializeField]
    public List<FaceData> faces;

    [SerializeField]
    public List<MinigameData> minigames;

    public void UnlockedAll()
    {
        foreach ( Unlockable a in colors)
            a.isUnlocked = false;
        foreach (Unlockable a in faces)
            a.isUnlocked = false;
        foreach (Unlockable a in minigames)
            a.isUnlocked = false;
    }
    public void SetUnlock<T>(string _id, bool isUnlocked) where T : Unlockable
    {
        if (typeof(T) == typeof(ColorData))
        {
            if (colors.Find(a => a.Id == _id) != null)
                colors.Find(a => a.Id == _id).isUnlocked = isUnlocked;
        }
        else if (typeof(T) == typeof(FaceData))
        {
            if (faces.Find(a => a.Id == _id) != null)
                faces.Find(a => a.Id == _id).isUnlocked = isUnlocked;
        }
        else if (typeof(T) == typeof(MinigameData))
        {
            if (minigames.Find(a => a.Id == _id) != null)
                minigames.Find(a => a.Id == _id).isUnlocked = isUnlocked;
        }
    }
    public bool IsUnlock<T>(string _id) where T : Unlockable
    {
        if (typeof(T) == typeof(ColorData))
        {
            if (colors.Find(a => a.Id == _id) != null)
                return colors.Find(a => a.Id == _id).isUnlocked;
        } 
        else if (typeof(T) == typeof(FaceData))
        {
            if (faces.Find(a => a.Id == _id) != null)
                return faces.Find(a => a.Id == _id).isUnlocked;
        }
        else if (typeof(T) == typeof(MinigameData))
        {
            if (minigames.Find(a => a.Id == _id) != null)
                return minigames.Find(a => a.Id == _id).isUnlocked;
        }
        return false;
    }
    public bool IsUnlock<T>(int _id) where T : Unlockable
    {
        if (typeof(T) == typeof(ColorData)) {
            if (_id < colors.Count)
                return colors[_id].isUnlocked;
        }
        else if (typeof(T) == typeof(FaceData)) { 
            if (_id < faces.Count)
                return faces[_id].isUnlocked;
        }
        else if (typeof(T) == typeof(MinigameData)) { 
            if (_id < minigames.Count)
                return minigames[_id].isUnlocked;
        }
        return false;
    }


}


//[CustomEditor(typeof(Database))]
//public class DatabaseEditor : Editor
//{

//    Database comp;

//    public void OnEnable()
//    {
//        comp = (Database)target;
//    }

//    public override void OnInspectorGUI()
//    {
//        EditorGUI.BeginChangeCheck();
//        //EditorGUI.PropertyField(comp, comp.minigame, GUIContent.none);
//        if (EditorGUI.EndChangeCheck())
//        {
//            Debug.Log("test");
//        }
//    }
//}


[System.Serializable]
public class MinigameData : Unlockable
{ 

    [SerializeField]
    public string spriteImage;

    // Add anything you want
    [SerializeField]
    public MinigameType test;
  
}

[System.Serializable]
public class ColorData : Unlockable
{
    [SerializeField]
    public Color color;
}

[System.Serializable]
public class FaceData : Unlockable
{
    [SerializeField]
    public int indiceForShader;

}

[System.Serializable]
public class Unlockable
{
    //[HideInInspector]
    [SerializeField]
    private string id;

    [SerializeField]
    public bool isUnlocked = false;

    public virtual string Id
    {
        get { return id; }
        set { id = value; }
    }

}