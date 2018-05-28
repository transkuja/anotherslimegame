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
        if (GameManager.Instance.playerEvolutionTutoShown == null)
        {
            GameManager.Instance.playerEvolutionTutoShown = new bool[activePlayersAtStart][];
        }
        else
        {
            for (int i = 0; i < activePlayersAtStart; i++)
            {
                Player currentPlayer = playerReferences[i].GetComponent<Player>();
                currentPlayer.evolutionTutoShown = GameManager.Instance.playerEvolutionTutoShown[i];
            }
        }
    }

    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
        base.AttributeCamera(activePlayersAtStart, cameraReferences, playersReference);
        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject playerGo = playersReference[i];
           
            cameraReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = playerGo.transform.GetChild((int)PlayerChildren.CameraTarget);
            cameraReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().Follow = playerGo.transform;
            cameraReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().playerIndex = (PlayerIndex)i;
            cameraReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().associatedPlayerController = playerGo.GetComponent<PlayerControllerHub>();
            cameraReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().associatedPlayerCharacter = playerGo.GetComponent<PlayerCharacterHub>();

            playerGo.GetComponent<Player>().cameraReference = cameraReferences[i];
            cameraReferences[i].SetActive(true);
        }
    }

}
