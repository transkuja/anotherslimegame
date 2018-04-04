using UWPAndXInput;
using UnityEngine;

// Gère les input selon l'input appelle des action codée dans une playerState.
public class PlayerControllerHub : PlayerController
{
    // Aggregations
    private PlayerCharacterHub playerCharacterHub;
    float timeBeforeTeleportation;
    float timeBeforeTeleportationReset = 1.5f;
    bool canTeleportAgain = true;

    #region Controller

    // Plateformist
    bool rightTriggerHasBeenPressed = false;
    float timerRightTriggerPressed = 0.0f;

    public bool DEBUG_hasBeenSpawnedFromTool = false;

   
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

        playerCharacterHub = Player.PlayerCharacter as PlayerCharacterHub;
        Rb = playerCharacterHub.Rb;

        timeBeforeTeleportation = timeBeforeTeleportationReset;
        canTeleportAgain = true;
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
                TeleportToOtherPlayer();
                HandlePNJWithController((int)PlayerIndex);
            }
        }
    }
    public Vector3 GetVelocity3DThirdPerson(Vector3 initialVelocity, float airControlFactor)
    {
        Vector3 camVectorForward = new Vector3(Player.cameraReference.transform.GetChild(0).forward.x, 0.0f, Player.cameraReference.transform.GetChild(0).forward.z);
        camVectorForward.Normalize();

        Vector3 velocityVec = initialVelocity.z * camVectorForward;
        if (!playerCharacterHub.IsGrounded)
            velocityVec += initialVelocity.x * Player.cameraReference.transform.GetChild(0).right * airControlFactor;
        else
            velocityVec += initialVelocity.x * Player.cameraReference.transform.GetChild(0).right;

        return velocityVec;
    }

    public virtual void HandleMovementWithController()
    {
        Vector3 initialVelocity = playerCharacterHub.PlayerState.HandleSpeed(State.ThumbSticks.Left.X, State.ThumbSticks.Left.Y);

        Vector3 velocityVec = Vector3.zero;
        if(GameManager.Instance.CurrentGameMode.ViewMode == ViewMode.thirdPerson3d)
            velocityVec = GetVelocity3DThirdPerson(initialVelocity, Player.airControlFactor);
        
        playerCharacterHub.PlayerState.Move(velocityVec, Player.airControlFactor, State.ThumbSticks.Left.X, State.ThumbSticks.Left.Y, forceCameraRecenter);

        // TMP Animation
        playerCharacterHub.PlayerState.HandleControllerAnim(State.ThumbSticks.Left.X, State.ThumbSticks.Left.Y);
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
        if (PrevState.Buttons.X == ButtonState.Released && State.Buttons.X == ButtonState.Pressed)
            playerCharacterHub.PlayerState.OnDashPressed();

        if (GetComponent<EvolutionStrength>() != null)
            if (PrevState.Buttons.Y == ButtonState.Released && State.Buttons.Y == ButtonState.Pressed)
                playerCharacterHub.PlayerState.OnDownDashPressed();
    }

    private void TeleportToOtherPlayer()
    {
        if (GameManager.Instance.IsInHub() && GameManager.Instance.PlayerStart.ActivePlayersAtStart == 2 && playerCharacterHub.IsGrounded)
        {
            if (state.Buttons.B == ButtonState.Pressed && state.Buttons.Y == ButtonState.Pressed)
            {
                if (!canTeleportAgain)
                    return;

                if (playerCharacterHub.PlayerState != playerCharacterHub.teleportState)
                {
                    if (playerCharacterHub.PlayerState == playerCharacterHub.freeState)
                    {
                        playerCharacterHub.PlayerState = playerCharacterHub.teleportState;
                    }
                    else
                        return;
                }

                if (timeBeforeTeleportation > 0.0f)
                    timeBeforeTeleportation -= Time.deltaTime;

                if (timeBeforeTeleportation < 0.0f)
                {
                    // TeleportToOtherPlayerProcess
                    int otherPlayerIndex = ((int)playerIndex + 1) % 2;
                    if (GameManager.Instance.PlayerStart.PlayersReference[otherPlayerIndex].GetComponent<PlayerCharacterHub>().IsGrounded)
                    {
                        transform.position = GameManager.Instance.PlayerStart.PlayersReference[otherPlayerIndex].transform.position + Vector3.up * 2.0f;
                        canTeleportAgain = false;
                        timeBeforeTeleportation = timeBeforeTeleportationReset;
                        playerCharacterHub.PlayerState = playerCharacterHub.freeState;
                    }
                }
            }
            else
            {
                if (playerCharacterHub.PlayerState == playerCharacterHub.teleportState)
                    playerCharacterHub.PlayerState = playerCharacterHub.freeState;

                canTeleportAgain = true;
                timeBeforeTeleportation = timeBeforeTeleportationReset;
            }
        }
    }

    public void HandlePNJWithController(int indexPlayer)
    {
        if( Player.refHubMinigameHandler != null)
            if (PrevState.Buttons.B == ButtonState.Released && State.Buttons.B == ButtonState.Pressed)
                Player.refHubMinigameHandler.DisplayMessage(indexPlayer);
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PNJTrigger" && isUsingAController)
            if (GameManager.CurrentState == GameState.Normal)
            {
                Player.refHubMinigameHandler = other.GetComponentInParent<HubMinigameHandler>();
                other.GetComponentInParent<HubMinigameHandler>().CreateUIHubMinigame((int)PlayerIndex);
            }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "PNJTrigger" && isUsingAController)
            if (GameManager.CurrentState == GameState.Normal)
            {
                Player.refHubMinigameHandler = null;
                other.GetComponentInParent<HubMinigameHandler>().DestroyUIMinigame((int)PlayerIndex);
            }
    }

}
