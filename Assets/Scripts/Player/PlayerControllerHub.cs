using UWPAndXInput;
using UnityEngine;

// Gère les input selon l'input appelle des action codée dans une playerState.
public class PlayerControllerHub : PlayerController
{
    #region Controller

    // Plateformist
    bool rightTriggerHasBeenPressed = false;
    float timerRightTriggerPressed = 0.0f;

    public bool DEBUG_hasBeenSpawnedFromTool = false;

    // Aggregations
    public PlayerCollisionCenter collisionCenter;
    public PlayerCharacterHub playerCharacterHub;


    public bool forceCameraRecenter = false;


#if UNITY_EDITOR
    public bool tryByPassJumpStop;
#endif
    #endregion



    private void GhostController()
    {
        if (GetComponent<EvolutionGhost>())
        {
            if(playerCharacterHub.IsGrounded)
                GetComponent<EvolutionGhost>().HandleTrail(state);
        }
    }
    private void PlatformistController()
    {
        // /!\ WARNING: code conflictuel si on combine les évolutions
        EvolutionPlatformist platformistComponent = GetComponent<EvolutionPlatformist>();
        if (platformistComponent != null)
        {
            if (platformistComponent.Charges == 0)
                return;

            if (prevState.Triggers.Right < 0.1f && state.Triggers.Right > 0.1f)
                rightTriggerHasBeenPressed = true;

            if (rightTriggerHasBeenPressed && state.Triggers.Right > 0.1f)
                timerRightTriggerPressed += Time.deltaTime;

            if (timerRightTriggerPressed > platformistComponent.ChargeTime)
            {
                // Show pattern + buttons to swap
                // Tant qu'on a pas relaché la gachette
                playerCharacterHub.PlayerState = playerCharacterHub.platformistChargedState;
                platformistComponent.IndexSelection(prevState, state);
            }

            if (prevState.Triggers.Right > 0.1f && state.Triggers.Right < 0.1f)
            {
                playerCharacterHub.PlayerState = playerCharacterHub.freeState;

                rightTriggerHasBeenPressed = false;

                if (timerRightTriggerPressed > platformistComponent.ChargeTime)
                {
                    platformistComponent.CreatePlatforms();
                }

                timerRightTriggerPressed = 0.0f;
            }

            platformistComponent.TimerPlatform += Time.deltaTime;
        }
    }

    void Start()
    {
        Player = GetComponent<Player>();
        Rb = GetComponent<Rigidbody>();

        collisionCenter = GetComponent<PlayerCollisionCenter>();
        playerCharacterHub = GetComponent<PlayerCharacterHub>();
    }

    public override void Update()
    {
        if (DEBUG_hasBeenSpawnedFromTool)
            return;
        if (!playerIndexSet)
            return;
        if (!prevState.IsConnected)
        {
            isUsingAController = false;
            for (int i = 0; i < GameManager.Instance.ActivePlayersAtStart; i++)
            {
                GamePadState testState = GamePad.GetState(playerIndex);

                if (testState.IsConnected)
                {
                    playerIndexSet = true;
                    isUsingAController = true;
                    break;
                }
            }
        }

        if (isUsingAController)
        {
            base.Update();

            if (GameManager.CurrentState == GameState.Normal)
            {
                HandleJumpWithController();
                HandleMovementWithController();
                HandleDashWithController();
                PlatformistController();
                GhostController();
            }
        }
    }

    public virtual void HandleMovementWithController()
    {
        Vector3 initialVelocity = playerCharacterHub.PlayerState.HandleSpeedWithController();

        playerCharacterHub.PlayerState.Move(initialVelocity);

        // TMP Animation
        playerCharacterHub.PlayerState.HandleControllerAnim();
    }
    private void HandleJumpWithController()
    {
        if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed && playerCharacterHub.PlayerState != playerCharacterHub.dashState)
        {
#if UNITY_EDITOR
            tryByPassJumpStop = true;
#endif
            playerCharacterHub.PlayerState.OnJumpPressed();
        }
#if UNITY_EDITOR
        else
            tryByPassJumpStop = false;
#endif

    }
    public virtual void HandleDashWithController()
    {
        if ((PrevState.Buttons.X == ButtonState.Released && State.Buttons.X == ButtonState.Pressed)
            // Keyboard input
            || Input.GetMouseButtonDown(0)
            )
            playerCharacterHub.PlayerState.OnDashPressed();

        if (GetComponent<EvolutionStrength>() != null)
            if (PrevState.Buttons.Y == ButtonState.Released && State.Buttons.Y == ButtonState.Pressed
                // Keyboard input
                || Input.GetMouseButtonDown(1)
                )
                playerCharacterHub.PlayerState.OnDownDashPressed();
    }

}
