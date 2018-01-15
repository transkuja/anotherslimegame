using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class HubMode : GameMode
{

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        int activePlayersAtStart = playerReferences.Count;
        if (GameManager.Instance.playerCollectables == null)
        {
            GameManager.Instance.playerCollectables = new int[activePlayersAtStart][];
            GameManager.Instance.playerEvolutionTutoShown = new bool[activePlayersAtStart][];
            GameManager.Instance.playerCostAreaTutoShown = new bool[activePlayersAtStart];
        }
        else
        {
            for (int i = 0; i < activePlayersAtStart; i++)
            {
                Player currentPlayer = playerReferences[i].GetComponent<Player>();
                currentPlayer.Collectables = GameManager.Instance.playerCollectables[i];
                currentPlayer.evolutionTutoShown = GameManager.Instance.playerEvolutionTutoShown[i];
                currentPlayer.costAreaTutoShown = GameManager.Instance.playerCostAreaTutoShown[i];
            }
        }
    }

    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
        base.AttributeCamera(activePlayersAtStart, cameraReferences, playersReference);
        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = playersReference[i];
           
            cameraReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = go.transform.GetChild((int)PlayerChildren.CameraTarget);
            cameraReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().Follow = go.transform;
            cameraReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().playerIndex = (PlayerIndex)i;
            cameraReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().associatedPlayerController = go.GetComponent<PlayerController>();

            go.GetComponent<Player>().cameraReference = cameraReferences[i];
            cameraReferences[i].SetActive(true);

        }
    }
}
