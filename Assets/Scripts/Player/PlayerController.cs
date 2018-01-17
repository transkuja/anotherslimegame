using UnityEngine;
using UWPAndXInput;


public class PlayerController : MonoBehaviour {

    protected PlayerState<PlayerController> playerState;

    protected Rigidbody rb;
    protected Player player;


    public PlayerIndex playerIndex;

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

    #endregion


    protected virtual void Awake()
    {

    }

}
