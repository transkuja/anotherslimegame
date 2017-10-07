
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

    [Header("Jump Settings")]
    [Tooltip("Jump unit is used to calibrate full charge jump")]
    [SerializeField] float jumpUnit = 250.0f;
    [SerializeField] float maxMovementSpeed = 50.0f;


    [SerializeField]
    [Range(1, 2)]
    public static int gameplayType = 1;
}
