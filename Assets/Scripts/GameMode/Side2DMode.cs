using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Side2DMode : GameMode {


    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        for (int i = 0; i < playerReferences.Count;i++)
        {
            playerReferences[i].GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionZ;
            //playerReferences[i].GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotationY;
        }
    }
    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = playersReference[i];
            go.GetComponent<Player>().cameraReference = cameraReferences[0];
        }

    }
}
