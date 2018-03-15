using UnityEngine;
using UWPAndXInput;


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
    }

    void UsePickupControls()
    {
        if (PrevState.Buttons.B == ButtonState.Released && State.Buttons.B == ButtonState.Pressed)
        {
            if (player.currentStoredPickup != null)
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
