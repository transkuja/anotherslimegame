using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour, IDoor {

    public GameObject doorToOpen;
    public float delayBeforeActivating = 0.2f;

    public PlatformGameplay pg;
    private float timer = 0;

    public void OpenDoor()
    {
        throw new System.NotImplementedException();
    }

    public void Start()
    {
        timer = 0.0f;
    }

    public void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.GetComponentInParent<Player>() && doorToOpen.activeSelf)
        {
            timer += Time.deltaTime;
            if (timer > delayBeforeActivating)
                pg.isRotating = true;
        }

        if (pg.transform.localEulerAngles.x >= 359)
        {
            doorToOpen.SetActive(false);
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        pg.isRotating = false;
        timer = 0.0f;
    }
}
