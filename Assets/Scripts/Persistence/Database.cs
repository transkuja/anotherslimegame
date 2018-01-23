using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using DatabaseClass;

namespace DatabaseClass
{
    [System.Serializable]
    public class MinigameData : Unlockable
    {

        [SerializeField]
        public string spriteImage;


        [SerializeField]
        public int nbRunesToUnlock = -1;

        [SerializeField]
        public int cout = -1;

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
   
    [CreateAssetMenu(fileName = "Database", menuName = "Custom/Database", order = 1)]
    public class Database : ScriptableObject
    {
        [SerializeField]
        public int nbRunes;

        [SerializeField]
        public int Money;

        [SerializeField]
        public List<ColorData> colors;

        [SerializeField]
        public List<FaceData> faces;

        [SerializeField]
        public List<MinigameData> minigames;

        public void UnlockedAll()
        {
            foreach ( Unlockable a in colors)
                a.isUnlocked = true;
            foreach (Unlockable a in faces)
                a.isUnlocked = true;
            foreach (Unlockable a in minigames)
                a.isUnlocked = true;
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

}

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
        {   
            comp.colors = new List<ColorData>();
            comp.faces = new List<FaceData>();
            comp.minigames = new List<MinigameData>();

            comp.Money = 0;
            comp.nbRunes = 0;

            // Adding colors
            int idColors = 0;
            string[] strColor = { "Rouge", "Bleu", "Magenta", "Jaune" };
            Color[] tabColor = { new Color(255, 73, 73, 0) / 255, new Color(21, 255, 243, 0) / 255, new Color(255, 34, 249, 0) / 255, new Color(241, 255, 0, 0) / 255 };
            comp.colors.Add(new ColorData { Id = strColor[idColors], color = tabColor[idColors], isUnlocked = true });
            comp.colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            comp.colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            comp.colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
        
            // Adding faces
            int idFaces = 0;
            string[] strFace = { "Tete de con", "Tete de bite", "Tete de cul", "Tete de gland", "Tete de caca" };
            comp.faces.Add(new FaceData { Id = strFace[idFaces], indiceForShader = idFaces, isUnlocked = true });
            comp.faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            comp.faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            comp.faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            comp.faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });

            // Adding minigames
            int idMinigames = 0;
            string[] strMinigame = { "MinigameDantho", "MinigameDeMatthieu" };
            comp.minigames.Add(new MinigameData { Id = strMinigame[idMinigames], cout = -1, nbRunesToUnlock = -1, spriteImage = "", isUnlocked = true });
            comp.minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], cout = -1, nbRunesToUnlock = -1, spriteImage = "", isUnlocked = true });

        }

        base.OnInspectorGUI();
       
        EditorUtility.SetDirty(comp);
    }

}


