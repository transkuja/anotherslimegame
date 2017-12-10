using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour {
    [SerializeField]
    CollectableType damageOn = CollectableType.Points;
    [SerializeField]
    int damage;

    public bool respawnPlayer = false;
    public bool istest = false;

	void OnTriggerEnter(Collider col)
    {
        if (col.GetComponentInParent<Player>())
        {
            Player p = col.GetComponentInParent<Player>();
            p.CanDoubleJump = true;
            p.UpdateCollectableValue(damageOn, -damage);
            p.GetComponent<JumpManager>().Jump(JumpManager.JumpEnum.Basic);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<Player>())
        {
            Player p = collision.gameObject.GetComponentInParent<Player>();
            if (respawnPlayer)
            {
                Respawner.RespawnProcess(p);
                return;
            }

            PlayerController pController = p.GetComponent<PlayerController>();
            if (istest)
            {
                Vector3 direction = transform.forward;
                pController.GetComponent<PlayerCollisionCenter>().ForcedJump(direction, 200, pController.GetComponent<Rigidbody>());
                StartCoroutine(pController.GetComponent<PlayerCollisionCenter>().ReactivateCollider(pController.GetComponent<Rigidbody>().GetComponent<Player>()));
                return;
            }

            p.CanDoubleJump = true;

 
            pController.GetComponent<PlayerCollisionCenter>().DamagePlayer(pController.GetComponent<Player>());
            pController.GetComponent<PlayerCollisionCenter>().ExpulsePlayer(collision.collider.ClosestPoint(transform.position), pController.GetComponent<Rigidbody>(), 900);
        }
    }
}
 