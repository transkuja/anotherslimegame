using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTile : MonoBehaviour {
    [SerializeField]
    float frequency = 2.0f;

    [SerializeField]
    float offsetTime = 0.0f;

    [SerializeField]
    float moveDuration = .25f;

    float timer = 0.0f;
    
    Quaternion startLocalRotation;
    
    bool isUp = false; // is needed if we don't want the tile to go down immediately

    [SerializeField]
    bool goDownImmediately = true;

    bool isMovingUp = false;


    // I need to disable the collider when down to prevent some weird physics bugs
    Collider Collider;
    
	void Start () {
        startLocalRotation = transform.localRotation;
        timer = offsetTime;
        Collider = GetComponentInChildren<Collider>();
        Collider.enabled = false;
	}
	
	void Update () {
        timer += Time.deltaTime;
        if (timer >= frequency)
        {
            isUp = !isUp;
            if (isUp)
            {
                StopCoroutine(GoDown());
                StartCoroutine(Flip());
            }
            else
            {
                StopCoroutine(Flip());
                StartCoroutine(GoDown());
            }

            timer = 0.0f;
        }
    }

    IEnumerator Flip()
    {
        Collider.enabled = true;
        isMovingUp = true;
        float flipTimer = 0.0f;

        if (moveDuration == 0.0f)
            moveDuration = 0.01f;
        while (flipTimer < moveDuration)
        {
            yield return null;
            flipTimer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(startLocalRotation, Quaternion.Euler(startLocalRotation.eulerAngles + new Vector3(-85.0f, 0.0f, 0.0f)), flipTimer/moveDuration);
        }
        isMovingUp = false;
        if (goDownImmediately)
        {
            isUp = false;
            StartCoroutine(GoDown());
        }
    }

    IEnumerator GoDown()
    {
        float downTimer = 0.0f;
        if (moveDuration == 0.0f)
            moveDuration = 0.01f;
        while (downTimer < moveDuration)
        {
            yield return null;
            downTimer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startLocalRotation.eulerAngles + new Vector3(-85.0f, 0.0f, 0.0f)), startLocalRotation, downTimer / moveDuration);
        }
        Collider.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isMovingUp && collision.rigidbody)
        {
            if (collision.rigidbody.GetComponent<PlayerControllerKart>())
            {
                collision.rigidbody.GetComponent<PlayerControllerKart>().CurrentState = PlayerControllerKart.KartPlayerState.Hit;
                collision.rigidbody.AddForce(-transform.forward * 200.0f + transform.up * 200.0f, ForceMode.Impulse);
                Physics.IgnoreCollision(collision.collider, GetComponentInChildren<Collider>(), true);
                StartCoroutine(ReactivateColliders(collision.collider, collision.rigidbody.GetComponent<PlayerControllerKart>().HitRecoveryTime));
            }
            else if(collision.rigidbody.GetComponent<AIRabite>())
            {
                collision.rigidbody.GetComponent<AIRabite>().CurrentState = AIRabite.RabiteState.Dead;
            }
        }
    }

    IEnumerator ReactivateColliders(Collider col, float waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);
        if (col != null)
        {
            Physics.IgnoreCollision(col.GetComponent<Collider>(), GetComponentInChildren<Collider>(), false);
        }
    }
}
