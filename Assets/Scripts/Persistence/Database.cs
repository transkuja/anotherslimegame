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
        public string videoPreview;

        [SerializeField]
        public int nbRunesToUnlock = -1;

        [SerializeField]
        public MinigameType type;

        [SerializeField]
        public int version = 0;

        public int difficulty = 1;
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
        [SerializeField]
        public RuneObjective objective = RuneObjective.Points;
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
        [SerializeField]
        public SkinType skinType;
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
    [System.Serializable]
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

        [SerializeField]
        public bool[] alreadyUnlockButtons;

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

            DatabaseManager.instance.SaveData();
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

            DatabaseManager.instance.SaveData();
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

        public RuneData GetRuneFromMinigame(GameMode minigameGameMode, int minigameVersion)
        {
            if (minigameGameMode is Runner3DGameMode)
                return GetRuneFromMinigame(MinigameType.Runner, minigameVersion);

            if (minigameGameMode is PushGameMode || minigameGameMode is BreakingGameMode)
                return GetRuneFromMinigame(MinigameType.Clash, minigameVersion);

            if (minigameGameMode is FoodGameMode)
                return GetRuneFromMinigame(MinigameType.Food, minigameVersion);

            if (minigameGameMode is KartGameMode)
                return GetRuneFromMinigame(MinigameType.Kart, minigameVersion);

            if (minigameGameMode is ColorFloorGameMode)
                return GetRuneFromMinigame(MinigameType.Floor, minigameVersion);

            if (minigameGameMode is FruitGameMode)
                return GetRuneFromMinigame(MinigameType.Fruit, minigameVersion);

            return null;
        }

        public RuneData GetRuneFromMinigame(MinigameType minigameType, int minigameVersion)
        {
            return runes.Find(rune => rune.associatedMinigame == minigameType && rune.associatedMinigameVersion == minigameVersion);
        }

        public MinigameType GetMinigameTypeBySceneAndVersion(string _sceneName, int _minigameVersion)
        {
            return minigames.Find(x => x.Id == _sceneName && x.version == _minigameVersion).type;
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
            string[] strMinigame = { "MinigameAntho", "MinigameAntho", "MinigameAntho", "MinigameAntho", "MinigameKart 3", "MinigamePush", "MinigamePush 1",
                    "Minigame3dRunner", "MiniGameFruits", "MinigameKart 2", "MinigameKart", "Minigame3dRunner 1", "MiniGameFruits2", "MinigameAnthourte",
                    "MinigameAnthourte", "MinigameAnthourte", "MinigameAnthourte", "MinigameFood", "MinigameFood", "MinigameFood", "MinigameFood",
                    "MinigameAnthourloupe", "MinigameAnthourloupe", "MinigameAnthourloupe" };
            minigames.Add(new MinigameData { Id = strMinigame[idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameAntho", isUnlocked = true, type = MinigameType.Floor, version = 0, difficulty = 1 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 1, spriteImage = "screenshotMinigameAntho", isUnlocked = false, type = MinigameType.Floor, version = 2, difficulty = 3 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameAntho", isUnlocked = true, type = MinigameType.Floor, version = 4, difficulty = 2 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 0, spriteImage = "screenshotMinigameAntho", isUnlocked = true, type = MinigameType.Floor, version = 6, difficulty = 4 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 1, spriteImage = "screenshotEasyKart", isUnlocked = true, type = MinigameType.Kart, version = 2, difficulty = 1 });

            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigamePush", isUnlocked = true, type = MinigameType.Clash, version = 0, difficulty = 3 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigamePush2", isUnlocked = true, type = MinigameType.Clash, version = 1, difficulty = 1 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 2, spriteImage = "screenshotMinigameRunner", videoPreview = "VideoRunner3D",  isUnlocked = false, type = MinigameType.Runner, version = 0, difficulty = 2 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 5, spriteImage = "screenshotMinigameFruits", isUnlocked = false, type = MinigameType.Fruit, version = 0, difficulty = 1 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 7, spriteImage = "screenshotSnowKart", isUnlocked = false, type = MinigameType.Kart, version = 1, difficulty = 3 });

            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 19, spriteImage = "screenshotMinigameKart", isUnlocked = false, type = MinigameType.Kart, version = 0, difficulty = 5 });

            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 19, spriteImage = "screenshotMinigameSuperRunner", isUnlocked = false, type = MinigameType.Runner, version = 1, difficulty = 5 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameFruits2", isUnlocked = true, type = MinigameType.Fruit, version = 1, difficulty = 1 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameAntho2", isUnlocked = true, type = MinigameType.Floor, version = 1, difficulty = 1 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 11, spriteImage = "screenshotMinigameAntho2", isUnlocked = false, type = MinigameType.Floor, version = 3, difficulty = 3 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigameAntho2", isUnlocked = true, type = MinigameType.Floor, version = 5, difficulty = 2 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 7, spriteImage = "screenshotMinigameAntho2", isUnlocked = false, type = MinigameType.Floor, version = 7, difficulty = 5 });

            // TODO: needs a screenshot
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 5, spriteImage = "screenshotMinigameFood", isUnlocked = false, type = MinigameType.Food, difficulty = 1 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 15, spriteImage = "screenshotMinigameFood", isUnlocked = false, type = MinigameType.Food, version = 1, difficulty = 2 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 11, spriteImage = "screenshotMinigameFood", isUnlocked = false, type = MinigameType.Food, version = 2, difficulty = 2 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = 24, spriteImage = "screenshotMinigameFood", isUnlocked = false, type = MinigameType.Food, version = 3, difficulty = 3 });

            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigamePush", isUnlocked = true, type = MinigameType.Clash, version = 2, difficulty = 1 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigamePush", isUnlocked = true, type = MinigameType.Clash, version = 3, difficulty = 3 });
            minigames.Add(new MinigameData { Id = strMinigame[++idMinigames], costToUnlock = -1, nbRunesToUnlock = -1, spriteImage = "screenshotMinigamePush", isUnlocked = true, type = MinigameType.Clash, version = 4, difficulty = 4 });


            // Adding costArea
            int idRune = 0;
            string[] strRune = { "Rune1Hub1", "Rune2Hub1", "Rune3Hub1", "Rune4Hub1", "Rune5Hub1",
                "Rune6Hub1", "Rune7Hub1", "Rune8Hub1", "Rune9Hub1", "Rune10Hub1", "Rune11Hub1", "Rune12Hub1",
                "Rune13Hub1", "Rune14Hub1", "Rune15Hub1",
                "RuneSneaky1", "RuneJoker1", "RuneSneaky2",
                "RuneKart1", "RuneFC1", "RuneRunner", "RuneFC2", "RuneFood",
                "RuneKart2", "RuneFC3", "RuneFood", "RuneFC4", "RuneFood",
                "RuneRunner2", "RuneKart3", "RuneFood4",
                "RuneBreaking1", "RuneBreaking2", "RuneBreaking3" };
            runes.Add(new RuneData { Id = strRune[idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });

            // Goal
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });

            // VincentTuto
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });

            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });

            // PNJeez
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false });

            // Minigames runes
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Kart, associatedMinigameVersion = 2, objective = RuneObjective.Time });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Floor, associatedMinigameVersion = 6 });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Runner, associatedMinigameVersion = 0 });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Floor, associatedMinigameVersion = 2 });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Food, associatedMinigameVersion = 0 });

            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Kart, associatedMinigameVersion = 1, objective = RuneObjective.Time });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Floor, associatedMinigameVersion = 7 });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Food, associatedMinigameVersion = 2 });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Floor, associatedMinigameVersion = 3 });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Food, associatedMinigameVersion = 1 });

            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Runner, associatedMinigameVersion = 1 });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Kart, associatedMinigameVersion = 0, objective = RuneObjective.Time });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Food, associatedMinigameVersion = 3 });

            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Clash, associatedMinigameVersion = 2 });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Clash, associatedMinigameVersion = 3 });
            runes.Add(new RuneData { Id = strRune[++idRune], isUnlocked = false, unlockedInMinigame = true, associatedMinigame = MinigameType.Clash, associatedMinigameVersion = 4 });

            // Adding mustaches
            int idMustache = 0;
            string[] strMustache = { "Curved", "Monopoly Guy", "Short", "Chinese" };
            mustaches.Add(new MustacheData { Id = strMustache[idMustache], model = "Mustaches/CurvedMustache", isUnlocked = false, costToUnlock = 400 });
            mustaches.Add(new MustacheData { Id = strMustache[++idMustache], model = "Mustaches/SecondMustache", isUnlocked = false, costToUnlock = 400 });
            mustaches.Add(new MustacheData { Id = strMustache[++idMustache], model = "Mustaches/ThirdMustache", isUnlocked = false, costToUnlock = 400 });
            mustaches.Add(new MustacheData { Id = strMustache[++idMustache], model = "Mustaches/ChineseMustache", isUnlocked = false, costToUnlock = 400 });

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
            string[] strForeheads = { "Unicorn", "Diamond", "Heart", "3rd Eye", "Bobble" };
            foreheads.Add(new ForeheadData { Id = strForeheads[idForeheads], model = "Foreheads/Unicorn", isUnlocked = false, costToUnlock = 300 });
            foreheads.Add(new ForeheadData { Id = strForeheads[++idForeheads], model = "Foreheads/Diamond", isUnlocked = false, costToUnlock = 300 });
            foreheads.Add(new ForeheadData { Id = strForeheads[++idForeheads], model = "Foreheads/Heart", isUnlocked = false, costToUnlock = 300 });
            foreheads.Add(new ForeheadData { Id = strForeheads[++idForeheads], model = "Foreheads/3rdEye", isUnlocked = false, costToUnlock = 300 });
            foreheads.Add(new ForeheadData { Id = strForeheads[++idForeheads], model = "Foreheads/Bobble", isUnlocked = false, costToUnlock = 300 });
            // Adding chins
            int idChins = 0;
            string[] strChins = { "Bow Tie", "Small Bow Tie", "Goatee", "Bell" };
            chins.Add(new ChinData { Id = strChins[idChins], model = "Chins/BowTie", isUnlocked = false, costToUnlock = 300 });
            chins.Add(new ChinData { Id = strChins[++idChins], model = "Chins/SmallBowTie", isUnlocked = false, costToUnlock = 300 });
            chins.Add(new ChinData { Id = strChins[++idChins], model = "Chins/Goatee", isUnlocked = false, costToUnlock = 300 });
            chins.Add(new ChinData { Id = strChins[++idChins], model = "Chins/Bell", isUnlocked = false, costToUnlock = 300 });

            // Adding accessories
            int idAccessories = 0;
            string[] strAccessories = { "Glasses", "Long Glasses", "Nose Glasses", "Boss Glasses" };
            accessories.Add(new AccessoryData { Id = strAccessories[idAccessories], model = "Accessories/Glasses", isUnlocked = false, costToUnlock = 200 });
            accessories.Add(new AccessoryData { Id = strAccessories[++idAccessories], model = "Accessories/LongGlasses", isUnlocked = false, costToUnlock = 200 });
            accessories.Add(new AccessoryData { Id = strAccessories[++idAccessories], model = "Accessories/NoseGlasses", isUnlocked = false, costToUnlock = 200 });
            accessories.Add(new AccessoryData { Id = strAccessories[++idAccessories], model = "Accessories/BossGlasses", isUnlocked = false, costToUnlock = 200 });

            // Adding Skins
            int idSkins = 0;
            string[] strSkins = { "Tatoo1", "Tatoo2", "Catch1", "Catch2", "Catch3", "Panda", "Special1", "Special2"};
            skins.Add(new SkinData { Id = strSkins[idSkins], texture = "Skins/Base1", skinType = SkinType.Mixed, isUnlocked = true });
            skins.Add(new SkinData { Id = strSkins[++idSkins], texture = "Skins/Base2", skinType = SkinType.Mixed, isUnlocked = true });
            skins.Add(new SkinData { Id = strSkins[++idSkins], texture = "Skins/Catch1", skinType = SkinType.Texture, isUnlocked = false, costToUnlock = 600 });
            skins.Add(new SkinData { Id = strSkins[++idSkins], texture = "Skins/Catch2", skinType = SkinType.Texture, isUnlocked = false, costToUnlock = 600 });
            skins.Add(new SkinData { Id = strSkins[++idSkins], texture = "Skins/Catch3", skinType = SkinType.Texture, isUnlocked = false, costToUnlock = 600 });
            skins.Add(new SkinData { Id = strSkins[++idSkins], texture = "Skins/Panda", skinType = SkinType.Texture, isUnlocked = false, costToUnlock = 600 });
            skins.Add(new SkinData { Id = strSkins[++idSkins], texture = "Skins/Spe1", skinType = SkinType.Texture, isUnlocked = false, costToUnlock = 600 });
            skins.Add(new SkinData { Id = strSkins[++idSkins], texture = "Skins/Spe2", skinType = SkinType.Texture, isUnlocked = false, costToUnlock = 600 });

            SneakyChiefProgress = 0;
            JokerProgress = 0;

            if (alreadyBrokenBreakables.Length > 0)
                alreadyBrokenBreakables = new bool[alreadyBrokenBreakables.Length];
            if(alreadyCollectedCollectables.Length > 0)
                alreadyCollectedCollectables = new bool[alreadyCollectedCollectables.Length];
            if (alreadyUnlockButtons.Length > 0)
                alreadyUnlockButtons = new bool[alreadyUnlockButtons.Length];
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

            DatabaseManager.instance.SaveData();
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

            if (alreadyBrokenBreakables.Length != breakables.Length - offset)
                alreadyBrokenBreakables = new bool[breakables.Length - offset];
        }

        public void ResetCollectablesState()
        {
            Collectable[] collectables = FindObjectsOfType<Collectable>();

            if (alreadyCollectedCollectables.Length != collectables.Length)
                alreadyCollectedCollectables = new bool[collectables.Length];

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

        public void ResetLockState()
        {
            ButtonTrigger[] button = FindObjectsOfType<ButtonTrigger>();
            int count = 0;
            foreach (ButtonTrigger btn in button)
            {
                if (btn.hasToLockAfterActivation)
                {
                    btn.persistenceIndex = count;
                    count++;
                }

            }

            if (alreadyUnlockButtons.Length != count)
            {
                alreadyUnlockButtons = new bool[count];
            }

            for (int i = 0; i < button.Length; ++i)
            {
                if (button[i].persistenceIndex != -1 && alreadyUnlockButtons[button[i].persistenceIndex])
                {
                    button[i].hasToMoveButton = true;
                }
            }
        }

        public void UnlockAllMinigamesAndAlmostAllCustomizables()
        {
            ResetAll();

            foreach (Unlockable a in minigames)
                a.isUnlocked = true;
            foreach (Unlockable a in ears)
                a.isUnlocked = true;
            foreach (Unlockable a in mustaches)
                a.isUnlocked = true;
            foreach (Unlockable a in skins)
                a.isUnlocked = true;
            foreach (Unlockable a in chins)
                a.isUnlocked = true;
            foreach (Unlockable a in foreheads)
                a.isUnlocked = true;


            foreach (Unlockable a in hats)
                a.isUnlocked = true;
            foreach (Unlockable a in accessories)
                a.isUnlocked = true;


            SetUnlock<HatData>("Cowboy", false);
            SetUnlock<HatData>("Police", false);

            for (int i = 0; i < 3; i++)
            {
                int index = UnityEngine.Random.Range(0, hats.Count);
                while (!hats[index].isUnlocked)
                    index = UnityEngine.Random.Range(0, hats.Count);
                hats[index].isUnlocked = false;
            }
            for (int i = 0; i < 2; i++)
            {
                int index = UnityEngine.Random.Range(0, accessories.Count);
                while (!accessories[index].isUnlocked)
                    index = UnityEngine.Random.Range(0, accessories.Count);
                accessories[index].isUnlocked = false;
            }

            Money = 500;
        }

    }

}