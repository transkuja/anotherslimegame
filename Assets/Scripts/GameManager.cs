
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }

        private set { }
    }

    public static float JumpUnit
    {
        get
        {
            return Instance.jumpUnit;
        }

        private set { }
    }

    public static float MaxMovementSpeed
    {
        get
        {
            return Instance.maxMovementSpeed;
        }

        set
        {
            Instance.maxMovementSpeed = value;
        }
    }

    public static UI UiReference
    {
        get
        {
            return Instance.uiReference;
        }

        set
        {
            Instance.uiReference = value;
        }
    }

    public static int GameplayType
    {
        get
        {
            return Instance.gameplayType;
        }

        set
        {
            Instance.gameplayType = value;
        }
    }

    [Header("Jump Settings")]
    [Tooltip("Jump unit is used to calibrate full charge jump")]
    [SerializeField] float jumpUnit = 250.0f;
    [SerializeField] float maxMovementSpeed = 40.0f;


    [SerializeField]
    [Range(1, 2)]
    private int gameplayType = 1;

    UI uiReference;
}
