using System.Collections.Generic;
using UnityEngine;

using DatabaseClass;
using System;

public enum MinigameType { Floor, Runner, Fruit, Kart, Clash, Food, Size};

namespace DatabaseClass
{
    [System.Serializable]
    public class MinigameData : Unlockable
    {
        [SerializeField]
        public string spriteImage;

        [SerializeField]
        public string videoPreview;

        [SerializeField]
        public int nbRunesToUnlock = -1;

        [SerializeField]
        public MinigameType type;

        [SerializeField]
        public int version = 0;

    }

    [System.Serializable]
    public class RuneData : Unlockable
    {
        [SerializeField]
        public bool unlockedInMinigame = false;
        [SerializeField]
        public MinigameType associatedMinigame;
        [SerializeField]
        public int associatedMinigameVersion;
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
    public class ModelData : Unlockable
    {
        [SerializeField]
        public string model;
    }

    [System.Serializable]
    public class EarsData : ModelData
    {
    }

    [System.Serializable]
    public class MustacheData : ModelData
    {
    }

    [System.Serializable]
    public class AccessoryData : ModelData
    {
    }

    [System.Serializable]
    public class ChinData : ModelData
    {
    }

    [System.Serializable]
    public class SkinData : Unlockable
    {
        [SerializeField]
        public string texture;
    }

    [System.Serializable]
    public class ForeheadData : ModelData
    {
    }

    [System.Serializable]
    public class HatData : ModelData
    {
        public bool shouldHideEars;
    }

    [System.Serializable]
    public class Unlockable
    {
        //[HideInInspector]
        [SerializeField]
        private string id;

        [SerializeField]
        public bool isUnlocked = false;

        [SerializeField]
        public int costToUnlock = -1;

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
        public List<EarsData> ears;

        [SerializeField]
        public List<MustacheData> mustaches;

        [SerializeField]
        public List<HatData> hats;

        [SerializeField]
        public List<AccessoryData> accessories;

        [SerializeField]
        public List<ChinData> chins;

        [SerializeField]
        public List<SkinData> skins;

        [SerializeField]
        public List<ForeheadData> foreheads;

        [SerializeField]
        public int SneakyChiefProgress;
        [SerializeField]
        public int JokerProgress;

        [SerializeField]
        public bool[] alreadyBrokenBreakables;

        [SerializeField]
        public bool[] alreadyCollectedCollectables;

        public int NbRunes
        {
            get
            {
                if (runes.FindAll(a => a.isUnlocked == true) != null)
                    return runes.FindAll(a => a.isUnlocked == true).Count;
                return 0;
            }
        }

        public void SetUnlockByCustomType(CustomizableType _customType, string _id, bool isUnlocked, int _version = 0)
        {
            switch(_customType)
            {
                case CustomizableType.Accessory:
                    SetUnlock<AccessoryData>(_id, isUnlocked, _version);
                    break;
                case CustomizableType.Chin:
                    SetUnlock<ChinData>(_id, isUnlocked, _version);
                    break;
                case CustomizableType.Color:
                    SetUnlock<ColorData>(_id, isUnlocked, _version);
                    break;
                case CustomizableType.Ears:
                    SetUnlock<EarsData>(_id, isUnlocked, _version);
                    break;
                case CustomizableType.Face:
                    SetUnlock<FaceData>(_id, isUnlocked, _version);
                    break;
                case CustomizableType.Forehead:
                    SetUnlock<ForeheadData>(_id, isUnlocked, _version);
                    break;
                case CustomizableType.Hat:
                    SetUnlock<HatData>(_id, isUnlocked, _version);
                    break;
                case CustomizableType.Mustache:
                    SetUnlock<MustacheData>(_id, isUnlocked, _version);
                    break;
                case CustomizableType.Skin:
                    SetUnlock<SkinData>(_id, isUnlocked, _version);
                    break;
                default:
                    return;
            }
        }

        public void SetUnlock<T>(string _id, bool isUnlocked, int _version = 0) where T : Unlockable
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
                if (minigames.Find(a => a.Id == _id && a.version == _version) != null)
                    minigames.Find(a => a.Id == _id && a.version == _version).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(RuneData))
            {
                if (runes.Find(a => a.Id == _id) != null)
                    runes.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(EarsData))
            {
                if (ears.Find(a => a.Id == _id) != null)
                    ears.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(MustacheData))
            {
                if (mustaches.Find(a => a.Id == _id) != null)
                    mustaches.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(HatData))
            {
                if (hats.Find(a => a.Id == _id) != null)
                    hats.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(AccessoryData))
            {
                if (accessories.Find(a => a.Id == _id) != null)
                    accessories.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(ChinData))
            {
                if (chins.Find(a => a.Id == _id) != null)
                    chins.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(SkinData))
            {
                if (skins.Find(a => a.Id == _id) != null)
                    skins.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
            else if (typeof(T) == typeof(ForeheadData))
            {
                if (foreheads.Find(a => a.Id == _id) != null)
                    foreheads.Find(a => a.Id == _id).isUnlocked = isUnlocked;
            }
        }

        public Unlockable GetDataFromId<T>(string _id) where T : Unlockable
        {
            if (typeof(T) == typeof(ColorData))
                return colors.Find(a => a.Id == _id);
            else if (typeof(T) == typeof(FaceData))
                return faces.Find(a => a.Id == _id);
            else if (typeof(T) == typeof(MinigameData))
                return minigames.Find(a => a.Id == _id);
            else if (typeof(T) == typeof(RuneData))
                return runes.Find(a => a.Id == _id);
            else if (typeof(T) == typeof(EarsData))
                return ears.Find(a => a.Id == _id);
            else if (typeof(T) == typeof(MustacheData))
                return mustaches.Find(a => a.Id == _id);
            else if (typeof(T) == typeof(HatData))
                return hats.Find(a => a.Id == _id);
            else if (typeof(T) == typeof(ChinData))
                return chins.Find(a => a.Id == _id);
            else if (typeof(T) == typeof(SkinData))
                return skins.Find(a => a.Id == _id);
            else if (typeof(T) == typeof(AccessoryData))
                return accessories.Find(a => a.Id == _id);
            else if (typeof(T) == typeof(ForeheadData))
                return foreheads.Find(a => a.Id == _id);
            return null;
        }

        public ModelData GetDataFromModel<T>(string _model) where T : ModelData
        {
            if (typeof(T) == typeof(EarsData))
                return ears.Find(a => a.model == _model);
            else if (typeof(T) == typeof(MustacheData))
                return mustaches.Find(a => a.model == _model);
            else if (typeof(T) == typeof(HatData))
                return hats.Find(a => a.model == _model);
            else if (typeof(T) == typeof(ChinData))
                return chins.Find(a => a.model == _model);
            else if (typeof(T) == typeof(AccessoryData))
                return accessories.Find(a => a.model == _model);
            else if (typeof(T) == typeof(ForeheadData))
                return foreheads.Find(a => a.model == _model);
            return null;
        }

        public bool IsUnlock<T>(string _id, int _version = 0) where T : Unlockable
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
                if (minigames.Find(a => a.Id == _id && a.version == _version) != null)
                    return minigames.Find(a => a.Id == _id && a.version == _version).isUnlocked;
            }
            else if (typeof(T) == typeof(RuneData))
            {
                if (runes.Find(a => a.Id == _id) != null)
                    return runes.Find(a => a.Id == _id).isUnlocked;
            }
            else if (typeof(T) == typeof(EarsData))
            {
                if (ears.Find(a => a.Id == _id) != null)
                    return ears.Find(a => a.Id == _id).isUnlocked;
            }
            else if (typeof(T) == typeof(MustacheData))
            {
                if (mustaches.Find(a => a.Id == _id) != null)
                    return mustaches.Find(a => a.Id == _id).isUnlocked;
            }
            else if (typeof(T) == typeof(HatData))
            {
                if (hats.Find(a => a.Id == _id) != null)
                    return hats.Find(a => a.Id == _id).isUnlocked;
            }
            else if (typeof(T) == typeof(ChinData))
            {
                if (chins.Find(a => a.Id == _id) != null)
                    return chins.Find(a => a.Id == _id).isUnlocked;
            }
            else if (typeof(T) == typeof(SkinData))
            {
                if (skins.Find(a => a.Id == _id) != null)
                    return skins.Find(a => a.Id == _id).isUnlocked;
            }
            else if (typeof(T) == typeof(AccessoryData))
            {
                if (accessories.Find(a => a.Id == _id) != null)
                    return accessories.Find(a => a.Id == _id).isUnlocked;
            }
            else if (typeof(T) == typeof(ForeheadData))
            {
                if (foreheads.Find(a => a.Id == _id) != null)
                    return foreheads.Find(a => a.Id == _id).isUnlocked;
            }
            return false;
        }

        public MinigameData GetUnlockedMinigameOfType(MinigameType t, int version = 0)
        {
            List<MinigameData> unlocked = GetUnlockedMinigamesOfType(t);
            if (unlocked != null)
                return unlocked.Find(a => a.version == version);
            else
                return null;
        }

        public List<MinigameData> GetUnlockedMinigamesOfType(MinigameType t)
        {
            return minigames.FindAll(a => a.isUnlocked == true && a.type == t);
        }

        public int GetNbUnlockedMinigamesOfEachType()
        {
            int nb = 0;
            for(int i= 0; i < (int)MinigameType.Size; i++)
            {
                if (GetUnlockedMinigamesOfType((MinigameType)i) != null)
                {
                    nb++;
                }
            }
            return nb;
        }

        public List<MinigameData> GetAllMinigamesOfType(MinigameType t)
        {
            return minigames.FindAll(a => a.type == t);
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
            foreach (Unlockable a in ears)
                a.isUnlocked = true;
            foreach (Unlockable a in mustaches)
                a.isUnlocked = true;
            foreach (Unlockable a in hats)
                a.isUnlocked = true;
            foreach (Unlockable a in accessories)
                a.isUnlocked = true;
            foreach (Unlockable a in skins)
                a.isUnlocked = true;
            foreach (Unlockable a in chins)
                a.isUnlocked = true;
            foreach (Unlockable a in foreheads)
                a.isUnlocked = true;
        }

        public RuneData GetRuneFromMinigame(MinigameType minigameType, int minigameVersion)
        {
            return runes.Find(rune => rune.associatedMinigame == minigameType && rune.associatedMinigameVersion == minigameVersion);
        }

        public void ResetAll() {
            colors = new List<ColorData>();
            faces = new List<FaceData>();
            minigames = new List<MinigameData>();
            runes = new List<RuneData>();
            ears = new List<EarsData>();
            mustaches = new List<MustacheData>();
            hats = new List<HatData>();
            accessories = new List<AccessoryData>();
            skins = new List<SkinData>();
            chins = new List<ChinData>();
            foreheads = new List<ForeheadData>();

            Money = 0;

            // Adding colors
            int idColors = 0;
            string[] strColor = { "Color 1",
                                "Color 2",
                                "Color 3",
                                "Color 4",
                                "Color 5",
                                "Color 6",
                                "Color 7",
                                "Color 8",
                                "Color 9",
                                "Color 10",
                                "Color 11",
                                "Color 12",
                                "Color 13",
                                "Color 14",
                                "Color 15",
                                "Color 16",
                                "Color 17",
                                "Color 18"
            };
            Color[] tabColor = { new Color(255, 197, 41, 255) / 255,
                                new Color(244, 230, 81, 255) / 255,
                                new Color(233, 152, 0, 255) / 255,
                                new Color(144, 27, 27, 255) / 255,
                                new Color(170, 247, 215, 255) / 255,
                                new Color(116, 205, 242, 255) / 255,
                                new Color(0, 147, 210, 255) / 255,
                                new Color(128, 131, 245, 255) / 255,
                                new Color(61, 65, 184, 255) / 255,
                                new Color(230, 181, 246, 255) / 255,
                                new Color(162, 77, 191, 255) / 255,
                                new Color(142, 223, 95, 255) / 255,
                                new Color(143, 216, 0, 255) / 255,
                                new Color(249, 142, 195, 255) / 255,
                                new Color(225, 121, 71, 255) / 255,
                                new Color(244, 59, 151, 255) / 255,
                                new Color(236, 108, 108, 255) / 255,
                                new Color(236, 81, 24, 255) / 255
            };
            colors.Add(new ColorData { Id = strColor[idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });
            colors.Add(new ColorData { Id = strColor[++idColors], color = tabColor[idColors], isUnlocked = true });

            // Adding faces
            int idFaces = 0;
            string[] strFace = { "Happy", "Slimy", "Binety", "Weary", "Cuty", "Doty", "Kitty" };
            faces.Add(new FaceData { Id = strFace[idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });
            faces.Add(new FaceData { Id = strFace[++idFaces], indiceForShader = idFaces, isUnlocked = true });

            // Adding minigames
            int idMinigames = 0;
            string[] strMinigame = { "MinigameAntho", "MinigameAntho", "MinigameAntho", "MinigameAntho", "MinigameKart", "MinigamePush", "MinigamePush 1",
                    "Minigame3dRunner", "MiniGameFruits", "MinigameKart 2", "Minigame3dRunner 1", "MiniGameFruits2", "MinigameAnthourte",
                    "MinigameAnthourte", "MinigameAnthourte", "MinigameAnthourte", "MinigameFood", "MinigameFood", "MinigameFood", "MinigameFood" };
            minigames.Add(new MinigameData { Id = strMinigame[idMinigames], costToUnlock = -1, nbRunesToUnlock = 1, spriteImage = "screenshotMinigameAntho", isUnlocked = true, type = MinigameType.Floor, version = 0 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 3, spriteImage = "screenshotMinigameAntho", isUnlocked = false, type = MinigameType.Floor, version = 2});
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 3, spriteImage = "screenshotMinigameAntho", isUnlocked = false, type = MinigameType.Floor, version = 4 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 3, spriteImage = "screenshotMinigameAntho", isUnlocked = false, type = MinigameType.Floor, version = 6 });

            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 3, spriteImage = "screenshotMinigameKart", isUnlocked = false, type = MinigameType.Kart });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 2, spriteImage = "screenshotMinigamePush", isUnlocked = false, type = MinigameType.Clash, version = 0 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 2, spriteImage = "screenshotMinigamePush", isUnlocked = true, type = MinigameType.Clash, version = 1 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 4, spriteImage = "screenshotMinigameRunner", videoPreview = "VideoRunner3D",  isUnlocked = true, type = MinigameType.Runner });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 5, spriteImage = "screenshotMinigameFruits", isUnlocked = false, type = MinigameType.Fruit });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotSnowKart", isUnlocked = false, type = MinigameType.Kart });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameSuperRunner", isUnlocked = false, type = MinigameType.Runner });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameFruits2", isUnlocked = false, type = MinigameType.Fruit });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameAntho2", isUnlocked = false, type = MinigameType.Floor, version = 1 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameAntho2", isUnlocked = false, type = MinigameType.Floor, version = 3 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameAntho2", isUnlocked = false, type = MinigameType.Floor, version = 5 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameAntho2", isUnlocked = false, type = MinigameType.Floor, version = 7 });

            // TODO: needs a screenshot
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameFood", isUnlocked = true, type = MinigameType.Food });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameFood", isUnlocked = false, type = MinigameType.Food, version = 1 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameFood", isUnlocked = false, type = MinigameType.Food, version = 2 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameFood", isUnlocked = false, type = MinigameType.Food, version = 3 });

            // Adding costArea
            int idRune = 0;
            string[] strRune = { "Rune1Hub1", "Rune2Hub1", "Rune3Hub1", "Rune4Hub1", "RuneColorFloor", "RuneSneaky1", "RuneFood" };
            runes.Add(new RuneData { Id = strRune[idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Floor, associatedMinigameVersion = 2 });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Food, associatedMinigameVersion = 0 });

            // Adding mustaches
            int idMustache = 0;
            string[] strMustache = { "Curved", "Second", "Third" };
            mustaches.Add(new MustacheData { Id = strMustache[idMustache], model = "Mustaches/CurvedMustache", isUnlocked = false, costToUnlock = 400 });
            mustaches.Add(new MustacheData { Id = strMustache[++idMustache], model = "Mustaches/SecondMustache", isUnlocked = false, costToUnlock = 400 });
            mustaches.Add(new MustacheData { Id = strMustache[++idMustache], model = "Mustaches/ThirdMustache", isUnlocked = false, costToUnlock = 400 });

            // Adding hats
            int idHat = 0;
            string[] strHat = { "Cap", "Chief", "Cowboy", "Glitter", "Top Hat", "Flowers", "Chinese", "Cat", "Marine", "Police", "Sombrero", "Crete", "Party", "Magical", "Witch" };
            hats.Add(new HatData { Id = strHat[idHat], model = "Hats/CapHat", shouldHideEars = true, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/ChiefHat", shouldHideEars = true, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/CowboyHat", shouldHideEars = true, isUnlocked = false }); // Bob give it
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/GlitterHat", shouldHideEars = true, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/TopHatHat", shouldHideEars = true, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/FlowerCrown", shouldHideEars = false, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/ChineseHat", shouldHideEars = false, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/CatHat", shouldHideEars = false, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/MarineHat", shouldHideEars = false, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/PoliceHat", shouldHideEars = false, isUnlocked = false });  // Gwen give it
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/SombreroHat", shouldHideEars = false, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/CreteHat", shouldHideEars = false, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/PartyHat", shouldHideEars = false, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/MagicalHat", shouldHideEars = false, isUnlocked = false, costToUnlock = 200 });
            hats.Add(new HatData { Id = strHat[++idHat], model = "Hats/WitchHat", shouldHideEars = false, isUnlocked = false, costToUnlock = 200 });
            // Adding ears
            int idEars = 0;
            string[] strEars = { "Ears1", "Ears2", "Ears3", "Ears4", "Ears5", "Ears6", "Ears7", "Ears8", "Ears9", "Ears10", "Ears11", "Ears12" };
            ears.Add(new EarsData { Id = strEars[idEars], model = "Ears/Ears1", isUnlocked = true, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears2", isUnlocked = false, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears3", isUnlocked = false, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears4", isUnlocked = false, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears5", isUnlocked = false, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears6", isUnlocked = false, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears7", isUnlocked = false, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears8", isUnlocked = false, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears9", isUnlocked = false, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears10", isUnlocked = false, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears11", isUnlocked = false, costToUnlock = 200 });
            ears.Add(new EarsData { Id = strEars[++idEars], model = "Ears/Ears12", isUnlocked = false, costToUnlock = 200 });

            // Adding foreheads
            int idForeheads = 0;
            string[] strForeheads = { "Unicorn" };
            foreheads.Add(new ForeheadData { Id = strForeheads[idForeheads], model = "Foreheads/Unicorn", isUnlocked = false, costToUnlock = 300 });

            // Adding chins
            int idChins = 0;
            string[] strChins = { "Bow Tie", "Goatee" };
            chins.Add(new ChinData { Id = strChins[idChins], model = "Chins/BowTie", isUnlocked = false, costToUnlock = 300 });
            chins.Add(new ChinData { Id = strChins[++idChins], model = "Chins/Goatee", isUnlocked = false, costToUnlock = 300 });

            // Adding accessories
            int idAccessories = 0;
            string[] strAccessories = { "Glasses", "Long Glasses", "Nose Glasses", "Boss Glasses" };
            accessories.Add(new AccessoryData { Id = strAccessories[idAccessories], model = "Accessories/Glasses", isUnlocked = false, costToUnlock = 200 });
            accessories.Add(new AccessoryData { Id = strAccessories[++idAccessories], model = "Accessories/LongGlasses", isUnlocked = false, costToUnlock = 200 });
            accessories.Add(new AccessoryData { Id = strAccessories[++idAccessories], model = "Accessories/NoseGlasses", isUnlocked = false, costToUnlock = 200 });
            accessories.Add(new AccessoryData { Id = strAccessories[++idAccessories], model = "Accessories/BossGlasses", isUnlocked = false, costToUnlock = 200 });

            // Adding Skins
            int idSkins = 0;
            string[] strSkins = { "Catch1", "Catch2", "Special1", "Special2"};
            skins.Add(new SkinData { Id = strSkins[idSkins], texture = "Skins/Catch1", isUnlocked = false });
            skins.Add(new SkinData { Id = strSkins[++idSkins], texture = "Skins/Catch2", isUnlocked = false });
            skins.Add(new SkinData { Id = strSkins[++idSkins], texture = "Skins/Spe1", isUnlocked = false, costToUnlock = 600 });
            skins.Add(new SkinData { Id = strSkins[++idSkins], texture = "Skins/Spe2", isUnlocked = false, costToUnlock = 600 });

            SneakyChiefProgress = 0;
            JokerProgress = 0;

            if (alreadyBrokenBreakables.Length > 0)
                alreadyBrokenBreakables = new bool[alreadyBrokenBreakables.Length];
            if(alreadyCollectedCollectables.Length > 0)
                alreadyCollectedCollectables = new bool[alreadyCollectedCollectables.Length];
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

        public void NewGameSettings()
        {
            ResetAll();
        }

        public void DEBUG_UnlockedAllExceptPNJ()
        {
            ResetAll();
            UnlockedAll();
            SetUnlock<HatData>("Cowboy", false);
            SetUnlock<HatData>("Police", false);
        }

        public void ResetBreakablesState()
        {
            Breakable[] breakables = FindObjectsOfType<Breakable>();

            int offset = 0;
            for (int i = 0; i < breakables.Length; ++i)
            {
                if (!breakables[i].DropCollectables())
                {
                    offset++;
                    continue;
                }
                else
                {
                    breakables[i].persistenceIndex = i - offset;
                }

            }

            if (alreadyBrokenBreakables.Length == 0)
            {
                alreadyBrokenBreakables = new bool[breakables.Length - offset];
            }
        }

        public void ResetCollectablesState()
        {
            Collectable[] collectables = FindObjectsOfType<Collectable>();

            if (alreadyCollectedCollectables.Length == 0)
            {
                alreadyCollectedCollectables = new bool[collectables.Length];
            }

            for (int i = 0; i < collectables.Length; ++i)
            {
                if (alreadyCollectedCollectables[i])
                {
                    Destroy(collectables[i].gameObject);
                }
                else
                    collectables[i].persistenceIndex = i;
            }
        }
    }

}
