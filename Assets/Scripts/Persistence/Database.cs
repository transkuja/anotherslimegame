using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using DatabaseClass;
using System;

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
        public int costToUnlock = -1;

    }

    [System.Serializable]
    public class CostAreaData : Unlockable
    {
        // Empty
    }


    [System.Serializable]
    public class RuneData : Unlockable
    {
        // Empty
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

        [SerializeField]
        public List<CostAreaData> costAreas;

        [SerializeField]
        public List<RuneData> runes;

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
            else if (typeof(T) == typeof(CostAreaData))
            {
                if (costAreas.Find(a => a.Id == _id) != null)
                    costAreas.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(RuneData))
            {
                if (runes.Find(a => a.Id == _id) != null)
                    runes.Find(a => a.Id == _id).isUnlocked = isUnlocked;
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
            else if (typeof(T) == typeof(CostAreaData))
            {
                if (costAreas.Find(a => a.Id == _id) != null)
                    return costAreas.Find(a => a.Id == _id).isUnlocked;
            }
            else if (typeof(T) == typeof(RuneData))
            {
                if (runes.Find(a => a.Id == _id) != null)
                    return runes.Find(a => a.Id == _id).isUnlocked;
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
            else if (typeof(T) == typeof(CostAreaData))
            {
                if (_id < minigames.Count)
                    return minigames[_id].isUnlocked;
            }
            else if (typeof(T) == typeof(RuneData))
            {
                if (_id < minigames.Count)
                    return minigames[_id].isUnlocked;
            }
            return false;
        }



        public void UnlockedAll()
        {
            foreach (Unlockable a in colors)
                a.isUnlocked = true;
            foreach (Unlockable a in faces)
                a.isUnlocked = true;
            foreach (Unlockable a in minigames)
                a.isUnlocked = true;
            foreach (Unlockable a in costAreas)
                a.isUnlocked = true;
            foreach (Unlockable a in runes)
                a.isUnlocked = true;
        }
        public void ResetAll() {
            colors = new List<ColorData>();
            faces = new List<FaceData>();
            minigames = new List<MinigameData>();
            costAreas = new List<CostAreaData>();
            runes = new List<RuneData>();

            Money = 0;
            nbRunes = 0;

            // Adding colors
            int idColors = 0;
            string[] strColor = { "Rouge", "Bleu", "Magenta", "Jaune", "Bonbon" };
            Color[] tabColor = { new Color(255, 73, 73, 0) / 255, new Color(21, 255, 243, 0) / 255, new Color(255, 34, 249, 0) / 255, new Color(241, 255, 0, 0) / 255, new Color(255, 167, 220, 0) / 255 };
            colors.Add(new ColorData { Id = strColor[idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = false });

            // Adding faces
            int idFaces = 0;
            string[] strFace = { "Tete de con", "Tete de bite", "Tete de cul", "Tete de gland", "Tete de caca" };
            faces.Add(new FaceData { Id = strFace[idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });

            // Adding minigames
            int idMinigames = 0;
            string[] strMinigame = { "MinigameDantho", "MinigameDeMatthieu" };
            minigames.Add(new MinigameData { Id = strMinigame[idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "", isUnlocked = true });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "", isUnlocked = true });

            // Adding costArea
            int idCostArea = 0;
            string[] strCostArea = { "CostArea1", "CostArea2" };
            costAreas.Add(new CostAreaData { Id = strCostArea[idCostArea], isUnlocked = false });
            costAreas.Add(new CostAreaData { Id = strCostArea[++idCostArea], isUnlocked = false });

            // Adding costArea
            int idRune = 0;
            string[] strRune = { "Rune1", "Rune2" };
            runes.Add(new RuneData { Id = strRune[idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
        }
        public void AllCostToZero()
        {
            foreach (MinigameData a in minigames)
            {
                a.costToUnlock = 0;
                a.nbRunesToUnlock = 0;
            }
        }

        public void TestCost()
        {
            foreach (MinigameData a in minigames)
            {
                a.costToUnlock = 100;
                a.nbRunesToUnlock = -1;
            }
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


