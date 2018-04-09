using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PNJController : MonoBehaviour
{

    // Aggregations
    private PlayerCharacterHub playerCharacterHub;
    private Player player;
    //private Rigidbody rb;

    private Vector3 originalPos;
    private float timer;
    [Range(5, 20)]
    public float timerToTpBack = 20;

    public bool isHappy = false;


    void Start()
    {
        player = GetComponent<Player>();

        playerCharacterHub = player.PlayerCharacter as PlayerCharacterHub;
        //rb = playerCharacterHub.Rb;
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

            if(isHappy && playerCharacterHub.IsGrounded)
            {
                playerCharacterHub.PlayerState.OnJumpPressed();
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


}