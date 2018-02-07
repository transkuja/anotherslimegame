using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehaviour : MonoBehaviour {

    bool isInitialized = false;
    int owner = -1;

    public void Init(int _ownerIndex)
    {
        isInitialized = true;
        owner = _ownerIndex;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isInitialized)
            return;

        if (collision.transform.GetComponent<Player>() != null && (int)collision.transform.GetComponent<Player>().PlayerController.playerIndex != owner)
        {
            // TODO: missile behaviour

            Destroy(gameObject);
        }

    }
}
