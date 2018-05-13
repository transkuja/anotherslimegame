using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerKartDamage : MonoBehaviour {
    private void OnTriggerStay(Collider other)
    {

        if (other.GetComponent<PlayerControllerKart>())
        {
            other.GetComponent<PlayerControllerKart>().CurrentState = PlayerControllerKart.KartPlayerState.Hit;
            Physics.IgnoreCollision(other, GetComponent<Collider>(), true);
            StartCoroutine(ReactivateTriggerForPlayerCollider(other, .75f));
        }
        else if (other.GetComponent<EnnemyController>())
        {
            other.GetComponent<EnnemyController>().CurrentState = EnnemyController.RabiteState.Dead;
        }
    }

    IEnumerator ReactivateTriggerForPlayerCollider(Collider col, float waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);
        if (col != null)
        {
            Physics.IgnoreCollision(col, GetComponent<Collider>(), false);
        }
    }
}
