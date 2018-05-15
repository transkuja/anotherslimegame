using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardCamera : MonoBehaviour {

    [SerializeField]
    int indexPlayer;
    Transform cameraRef;

    private void Start()
    {
        cameraRef = GameManager.Instance.PlayerStart.PlayersReference[indexPlayer].GetComponent<Player>().cameraReference.GetComponentInChildren<Camera>().transform;
    }

    void Update () {
        transform.rotation = Quaternion.LookRotation(cameraRef.forward);
	}
}
