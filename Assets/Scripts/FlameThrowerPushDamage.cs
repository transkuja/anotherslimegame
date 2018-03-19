using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerPushDamage : MonoBehaviour {
    [SerializeField]
    GameObject hitParticles;
    [SerializeField]
    PlayerUIStat damageOn = PlayerUIStat.Points;
    Vector3 direction;
    private void OnTriggerEnter(Collider other)
    {
        PlayerCollisionCenter playerCollision = other.GetComponent<PlayerCollisionCenter>();
        PlayerControllerHub playerController = other.GetComponent<PlayerControllerHub>();
        if (playerCollision != null)
        {
            if (playerCollision.GetComponent<EvolutionStrength>() != null && playerController.PlayerState == playerController.dashState)
            {
                direction = (transform.position - other.ClosestPointOnBounds(transform.position)).normalized;
                if (direction == Vector3.zero)
                    direction = other.transform.forward;
                Physics.IgnoreCollision(other, this.GetComponent<Collider>(), true);
            }
            else
            {
                Vector3 centerToTargetCenter = other.transform.position + Vector3.up * 0.5f - transform.position;
                GameObject go = Instantiate(hitParticles);
                go.transform.position = transform.position + Vector3.up * 0.5f + centerToTargetCenter / 2.0f;
                go.transform.rotation = Quaternion.LookRotation(centerToTargetCenter, Vector3.up);
                Destroy(go, 10.0f);
                if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
                UWPAndXInput.GamePad.VibrateForSeconds(playerController.playerIndex, 0.9f, 0.9f, .2f);

                if (GameManager.Instance.CurrentGameMode.TakesDamageFromTraps)
                {
                    if (GameManager.Instance.IsInHub())
                        playerCollision.DamagePlayerHub();
                    else
                        playerCollision.DamagePlayer(other.GetComponent<Player>(), damageOn);
                }

                playerCollision.ExpulsePlayer(other.ClosestPoint(transform.position), other.GetComponent<Rigidbody>(), 50);
            }
        }

    }
}
