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

            if (GameManager.Instance.IsInHub())
                pController.GetComponent<PlayerCollisionCenter>().DamagePlayerHub();
            else
                pController.GetComponent<PlayerCollisionCenter>().DamagePlayer(pController.GetComponent<Player>(), damageOn);

            pController.GetComponent<PlayerCollisionCenter>().ExpulsePlayer(collision.collider.ClosestPoint(transform.position), pController.GetComponent<Rigidbody>(), 15);
        }
    }
}
 