using UnityEngine;
using UWPAndXInput;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    // gamePad
    protected GamePadState state;
    protected GamePadState prevState;
    protected bool isUsingAController = false;

    protected Rigidbody rb;
    protected Player player;

    public PlayerIndex playerIndex;

    protected bool playerIndexSet = false;

    public CameraTrigger currentCameraTrigger;

    #region getterSetters
    public Rigidbody Rb
    {
        get
        {
            return rb;
        }

        set
        {
            rb = value;
        }
    }
    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
        }
    }
    public PlayerIndex PlayerIndex
    {
        get
        {
            return playerIndex;
        }

        set
        {
            playerIndex = value;
#if UNITY_EDITOR
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.Index, ((int)(playerIndex)).ToString(), (int)(playerIndex));
#endif
        }
    }


    public bool PlayerIndexSet
    {
        get
        {
            return playerIndexSet;
        }
        set
        {
            playerIndexSet = value;
        }
    }

    public bool IsUsingAController
    {
        get
        {
            return isUsingAController;
        }

        set
        {
            isUsingAController = value;
        }
    }

    public GamePadState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    public GamePadState PrevState
    {
        get
        {
            return prevState;
        }

        set
        {
            prevState = value;
        }
    }

    #endregion

    public virtual void Update()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);

        if (GameManager.CurrentState == GameState.Normal)
        {
            UsePickupControls();
        }

        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
            {
                // Only the player who paused the game can remove the pause
                if (GameManager.CurrentState == GameState.Paused)
                {
                    // Check if player index match to remove pause
                    if ((int)playerIndex == GameManager.Instance.playerWhoPausedTheGame)
                    {
                        GameManager.ChangeState(GameState.Normal);
                    }
                }
                else
                {
                    if(!LevelLoader.IsLoading)
                    {
                        GameManager.Instance.playerWhoPausedTheGame = (int)playerIndex;
                        GameManager.ChangeState(GameState.Paused);
                    }
                }
            }
        }
    }

    void UsePickupControls()
    {
        if (PrevState.Buttons.B == ButtonState.Released && State.Buttons.B == ButtonState.Pressed)
        {
            if (player != null && player.currentStoredPickup != null)
            {
                player.currentStoredPickup((int)playerIndex);
                player.currentStoredPickup = null;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CameraTrigger>())
        {
            currentCameraTrigger = other.GetComponent<CameraTrigger>();
            if (player != null && player.cameraReference != null)
                player.cameraReference.GetComponentInChildren<DynamicJoystickCameraController>().ChangeCameraBehaviour(currentCameraTrigger.behaviour);
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CameraTrigger>())
        {
            currentCameraTrigger = null;
            player.cameraReference.GetComponentInChildren<DynamicJoystickCameraController>().ChangeCameraBehaviour(CameraBehaviour.Default);
        }
    }
}
