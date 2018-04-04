using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

public class PNJController : MonoBehaviour
{

    // Aggregations
    private PlayerCharacterHub playerCharacterHub;
    private Player player;
    private Rigidbody rb;

    private Vector3 originalPos;
    private float timer;
    [Range(5, 20)]
    public float timerToTpBack = 20;

    float indexStored;
    // gamePad
    protected GamePadState state;
    protected GamePadState prevState;

    void Start()
    {
        player = GetComponent<Player>();

        playerCharacterHub = player.PlayerCharacter as PlayerCharacterHub;
        rb = playerCharacterHub.Rb;
        originalPos = transform.position;
    }

    public void Update()
    {
        // Oui bob est con
        if (GameManager.CurrentState == GameState.Normal)
        {
            if (Vector3.Distance(originalPos, transform.position) > 1f)
            {
                transform.LookAt(originalPos);
                HandleMovement(0, 1);

                timer += Time.deltaTime;
                if (timer > timerToTpBack)
                {
                    transform.position = originalPos;
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
        }
        else if (GameManager.CurrentState == GameState.ForcedPauseMGRules)
        {
            prevState = state;
            state = GamePad.GetState((PlayerIndex)indexStored);

            if (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed)
            {
                GameManager.ChangeState(GameState.Normal);
                GameManager.UiReference.RuleScreen.gameObject.SetActive(false);
            }
        }

    }

    public virtual void HandleMovement(float x, float y)
    {
        Vector3 initialVelocity = playerCharacterHub.PlayerState.HandleSpeed(x, y);
        Vector3 velocityVec = initialVelocity.z * transform.forward;
        if (!playerCharacterHub.IsGrounded)
            velocityVec += initialVelocity.x * transform.right * player.airControlFactor;
        else
            velocityVec += initialVelocity.x * transform.right;


        playerCharacterHub.PlayerState.Move(velocityVec, player.airControlFactor, x, y);

        // TMP Animation
        playerCharacterHub.PlayerState.HandleControllerAnim(initialVelocity.x, initialVelocity.y);
    }


    // PNJ Character 
    public void MessageTest(float indexPlayer)
    {
        GameManager.ChangeState(GameState.ForcedPauseMGRules);
        //GameObject go = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);
        //go.transform.GetChild(1).GetComponentInChildren<Text>().text = "BIIIITE";
        indexStored = indexPlayer;
    }
}