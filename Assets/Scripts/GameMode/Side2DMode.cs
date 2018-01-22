using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
        Debug.Assert(cameraReferences.Length == 1, "Wrong Assignation of camera in playerStart");
        CinemachineTargetGroup targetGroup = cameraReferences[0].transform.parent.GetComponentInChildren<CinemachineTargetGroup>();
        targetGroup.m_Targets = new CinemachineTargetGroup.Target[activePlayersAtStart];
        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject playerGo = playersReference[i];
            playerGo.GetComponent<Player>().cameraReference = cameraReferences[0];
            targetGroup.m_Targets[i].target = playerGo.transform;
            targetGroup.m_Targets[i].weight = 1;
            targetGroup.m_Targets[i].radius = 10;
        }

    }
}
