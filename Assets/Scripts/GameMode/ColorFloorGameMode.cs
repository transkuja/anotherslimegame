using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class ColorFloorGameMode : GameMode {

    public float timer;
    public bool freeMovement = true;
    public float restrainedMovementTick;
    [SerializeField]
    List<GameObject> restrainedMovementStarters;

    public List<GameObject> RestrainedMovementStarters
    {
        get
        {
            return restrainedMovementStarters;
        }
    }

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        if (!freeMovement)
        {
            for (int i = 0; i < playerReferences.Count; i++)
            {
                Destroy(playerReferences[i].GetComponent<PlayerControllerHub>());
                PlayerController pc = playerReferences[i].AddComponent<PlayerController>();
                pc.playerIndex = (PlayerIndex)i;
                playerReferences[i].transform.position = restrainedMovementStarters[i].transform.position;
                playerReferences[i].GetComponent<Rigidbody>().useGravity = true;
                playerReferences[i].GetComponent<Player>().NbPoints = 0;
            }
        }

        LaunchTimer();
    }

    public void LaunchTimer()
    {
        GameManager.Instance.GameFinalTimer = timer;
        GameManager.Instance.LaunchFinalTimer();
    }

    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
        base.AttributeCamera(activePlayersAtStart, cameraReferences, playersReference);
        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = playersReference[i];

            go.GetComponent<Player>().cameraReference = cameraReferences[i];
            cameraReferences[i].SetActive(true);

        }
    }

}
