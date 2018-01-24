using UnityEngine;
using UWPAndXInput;


public class PlayerController : MonoBehaviour {


    protected Rigidbody rb;
    protected Player player;

    public PlayerIndex playerIndex;

    protected bool playerIndexSet = false;

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

    #endregion
}
