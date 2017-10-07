
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

    [Header("Jump Settings")]
    [Tooltip("Jump unit is used to calibrate full charge jump")]
    [SerializeField] float jumpUnit = 250.0f;
}
