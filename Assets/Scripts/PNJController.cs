using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PNJController : MonoBehaviour
{
    // Aggregations
    private PlayerCharacterHub playerCharacterHub;
    private Player player;

    private Vector3 originalPos;
    private Quaternion originalRot;
    private float timer;
    [Range(5, 20)]
    public float timerToTpBack = 15;

    public AudioSource myAudioSource;

    // Gosse
    public bool isHappy = false;

    void Start()
    {
        player = GetComponent<Player>();
        playerCharacterHub = player.PlayerCharacter as PlayerCharacterHub;

        originalPos = transform.position;
        originalRot = transform.rotation;
    }

    public void Update()
    {
        // retourne à la postion d'origine
        if (playerCharacterHub.DialogState == DialogState.Normal)
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
                    ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up, Quaternion.identity, true, false, (int)HitParticles.BigHit);
                }
            }
            else if (!isHappy && Vector3.Distance(originalPos, transform.position) < 0.5f)
            {
                playerCharacterHub.Rb.velocity = Vector3.zero;
                playerCharacterHub.transform.rotation = originalRot;
            }
            else
            {
                timer = 0;
            }

            if(isHappy && playerCharacterHub.IsGrounded)
            {
                playerCharacterHub.PlayerState.OnJumpPressed();
            }
        } else
        {
            playerCharacterHub.Rb.velocity = Vector3.zero;
            //transform.LookAt(new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z));
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

    public void UpdateOriginalPosition()
    {
        originalPos = transform.position;
        originalRot = transform.rotation;
    }


}