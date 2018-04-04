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
            PlayerCharacterHub pCharacter = p.GetComponent<PlayerCharacterHub>();
            PlayerCollisionCenter pCollisionCenter = p.GetComponent<PlayerCollisionCenter>();

            if (pCollisionCenter.canBeHit)
            {
                pCollisionCenter.canBeHit = false;
                if (GameManager.Instance.IsInHub() && pCharacter.GetComponent<PlayerController>())
                    pCollisionCenter.DamagePlayerHub();
                else
                    pCollisionCenter.DamagePlayer(p, damageOn);

                pCollisionCenter.ExpulsePlayer(collision.collider.ClosestPoint(transform.position), pCharacter.Rb, 15);

            }

        }
    }
}
 