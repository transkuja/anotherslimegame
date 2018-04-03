using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpikeTrap : MonoBehaviour {
    [SerializeField]
    PlayerUIStat damageOn = PlayerUIStat.Points;
    [SerializeField]
    int damage;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<Player>())
        {
            Player p = collision.gameObject.GetComponentInParent<Player>();
            PlayerControllerHub pController = p.GetComponent<PlayerControllerHub>();
            PlayerCollisionCenter pCollisionCenter = pController.GetComponent<PlayerCollisionCenter>();

            if (pCollisionCenter.canBeHit)
            {
                pCollisionCenter.canBeHit = false;
                if (GameManager.Instance.IsInHub())
                    pCollisionCenter.DamagePlayerHub();
                else
                    pCollisionCenter.DamagePlayer(p, damageOn);

                pCollisionCenter.ExpulsePlayer(collision.collider.ClosestPoint(transform.position), pController.Rb, 15);

            }

        }
    }
}
 