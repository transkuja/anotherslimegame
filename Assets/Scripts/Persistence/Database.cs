using System.Collections.Generic;
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
    public class MustacheData : Unlockable
    {
        [SerializeField]
        public string model;
    }

    [System.Serializable]
    public class HatData : Unlockable
    {
        [SerializeField]
        public string model;
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
        public int Money;

        [SerializeField]
        public List<ColorData> colors;

        [SerializeField]
        public List<FaceData> faces;

        [SerializeField]
        public List<MinigameData> minigames;

        [SerializeField]
        public List<RuneData> runes;

        [SerializeField]
        public List<MustacheData> mustaches;

        [SerializeField]
        public List<HatData> hats;

        public int NbRunes
        {
            get
            {
                if (runes.FindAll(a => a.isUnlocked == true) != null)
                    return runes.FindAll(a => a.isUnlocked == true).Count;
                return 0;
            }
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
            else if (typeof(T) == typeof(RuneData))
            {
                if (runes.Find(a => a.Id == _id) != null)
                    runes.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(MustacheData))
            {
                if (mustaches.Find(a => a.Id == _id) != null)
                    mustaches.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(HatData))
            {
                if (mustaches.Find(a => a.Id == _id) != null)
                    mustaches.Find(a => a.Id == _id).isUnlocked = isUnlocked;
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
            else if (typeof(T) == typeof(RuneData))
            {
                if (runes.Find(a => a.Id == _id) != null)
                    return runes.Find(a => a.Id == _id).isUnlocked;
            }
            else if (typeof(T) == typeof(MustacheData))
            {
                if (mustaches.Find(a => a.Id == _id) != null)
                    return mustaches.Find(a => a.Id == _id).isUnlocked;
            }
            else if (typeof(T) == typeof(HatData))
            {
                if (mustaches.Find(a => a.Id == _id) != null)
                    return mustaches.Find(a => a.Id == _id).isUnlocked;
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
            foreach (Unlockable a in runes)
                a.isUnlocked = true;
            foreach (Unlockable a in mustaches)
                a.isUnlocked = true;
            foreach (Unlockable a in hats)
                a.isUnlocked = true;
        }

        public void ResetAll() {
            colors = new List<ColorData>();
            faces = new List<FaceData>();
            minigames = new List<MinigameData>();
            runes = new List<RuneData>();
            mustaches = new List<MustacheData>();
            hats = new List<HatData>();
            Money = 0;

            // Adding colors
            int idColors = 0;
            string[] strColor = { "Red", "Blue", "Magenta", "Yellow", "Candy" };
            Color[] tabColor = { new Color(255, 10, 10, 0) / 255, new Color(21, 255, 243, 0) / 255, new Color(255, 34, 249, 0) / 255, new Color(241, 255, 0, 0) / 255, new Color(255, 167, 220, 0) / 255 };
            colors.Add(new ColorData { Id = strColor[idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = false });

            // Adding faces
            int idFaces = 0;
            string[] strFace = { "Happy", "Slimy", "Weary", "Cuty", "Binety" };
            faces.Add(new FaceData { Id = strFace[idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });

            // Adding minigames
            int idMinigames = 0;
            string[] strMinigame = { "MinigameAntho", "MinigameKart", "MinigamePush", "Minigame3dRunner", "MiniGameFruits" };
            minigames.Add(new MinigameData { Id = strMinigame[idMinigames], costToUnlock = -1, nbRunesToUnlock = 1, spriteImage = "screenshotMinigameAntho", isUnlocked = false });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 3, spriteImage = "screenshotMinigameKart", isUnlocked = false });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 2, spriteImage = "screenshotMinigamePush", isUnlocked = false });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 4, spriteImage = "screenshotMinigameRunner2", isUnlocked = false });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 5, spriteImage = "screenshotMinigameFruits", isUnlocked = false });


            // Adding costArea
            int idRune = 0;
            string[] strRune = { "Rune1Hub1", "Rune2Hub1", "Rune3Hub1", "Rune4Hub1", "RuneColorFloor" };
            runes.Add(new RuneData { Id = strRune[idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });

            // Adding mustaches
            int idMustache = 0;
            string[] strMustache = { "Curved", "Second", "Third" };
            mustaches.Add(new MustacheData { Id = strMustache[idMustache], model = "Mustaches/CurvedMustache", isUnlocked = false });
            mustaches.Add(new MustacheData { Id = strMustache[++idMustache], model = "Mustaches/SecondMustache", isUnlocked = false });
            mustaches.Add(new MustacheData { Id = strMustache[++idMustache], model = "Mustaches/ThirdMustache", isUnlocked = false });

            // Adding hats
            int idHat = 0;
            string[] strHat = { "Cap", "Chief", "Cowboy", "Glitter", "Top Hat" };
            hats.Add(new HatData { Id = strHat[idHat], model = "Hats/CapHat", isUnlocked = false });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/ChiefHat", isUnlocked = false });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/CowboyHat", isUnlocked = false });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/GlitterHat", isUnlocked = false });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/TopHatHat", isUnlocked = false });
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
