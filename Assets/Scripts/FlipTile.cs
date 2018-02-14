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

    bool isUp = false;
    [SerializeField]
    bool goDownImmediately = true;
    bool isMoving = false;
	// Use this for initialization
	void Start () {
        startLocalRotation = transform.localRotation;
	}
	
	// Update is called once per frame
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
        isMoving = true;
        float flipTimer = 0.0f;

        if (moveDuration == 0.0f)
            moveDuration = 0.01f;
        while (flipTimer < moveDuration)
        {
            yield return null;
            flipTimer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(startLocalRotation, Quaternion.Euler(startLocalRotation.eulerAngles + new Vector3(-85.0f, 0.0f, 0.0f)), flipTimer/moveDuration);
        }
        isMoving = false;
        if (goDownImmediately)
        {
            isUp = false;
            StartCoroutine(GoDown());
        }
    }

    IEnumerator GoDown()
    {
        isMoving = true;
        float downTimer = 0.0f;
        if (moveDuration == 0.0f)
            moveDuration = 0.01f;
        while (downTimer < moveDuration)
        {
            yield return null;
            downTimer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startLocalRotation.eulerAngles + new Vector3(-85.0f, 0.0f, 0.0f)), startLocalRotation, downTimer / moveDuration);
        }
        isMoving = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isMoving && collision.rigidbody && collision.rigidbody.GetComponent<PlayerControllerKart>())
        {
            collision.rigidbody.GetComponent<PlayerControllerKart>().CurrentState = PlayerControllerKart.KartPlayerState.Hit;
            collision.rigidbody.AddForce(-transform.forward * 200.0f + transform.up * 200.0f, ForceMode.Impulse);
        }
    }
}
