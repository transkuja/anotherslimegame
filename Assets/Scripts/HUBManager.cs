using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tampax
{
    [SerializeField]
    public CollectableType evolutionType;
    [SerializeField]
    public GameObject gameplayRoomStarter;
}

public class HUBManager : MonoBehaviour
{

    public GameObject CostAreaMiniGamePush;
    public SpawnManager spawnManagerRef;

    public WaterState WaterState = WaterState.Clear;

    #region GameplayRoom
    [Tooltip("The transform from which all instantiates will be done. Instantiated platforms will be this transform's children.")]
    public Transform referenceTransform;

    // Pardon je m'intercale ici par ce que sa
    [Header("Rune shelters")]
    public GameObject ghostRuneShelter;
    public GameObject platformistRuneShelter;
    public GameObject agileRuneShelter;
    public GameObject strengthRuneShelter;

    public static HUBManager instance;

    // Gameplay room starters
    public List<Tampax> gameplayRoomStarters = new List<Tampax>();

    public void Awake()
    {
        instance = this;
    }
    #endregion

    // HUB events
    // Water

    public void StartIncreasing()
    {
        WaterState = WaterState.WaterIsMovingTop;
    }
}
