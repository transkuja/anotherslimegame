using System;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
using Runner3D;
public class Runner3DGameMode : GameMode {

    enum Mode
    {
        SoloInfinite,
        LastRemaining
    }
    private Mode mode = Mode.LastRemaining;

    int nbRemainingPlayer;
    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this);
        nbRemainingPlayer = playerReferences.Count;
        for (int i = 0; i < playerReferences.Count; i++)
        {
            PlayerControllerHub playerControllerHub = playerReferences[i].GetComponent<PlayerControllerHub>();
            if (playerControllerHub != null)
                playerControllerHub.OnDeathEvent += OnPlayerDeath;
        }
    }

    // identique au hub bad copie collé
    public override void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
        base.AttributeCamera(activePlayersAtStart, cameraReferences, playersReference);
        for (int i = 0; i < activePlayersAtStart; i++)
        {
            GameObject go = playersReference[i];

            cameraReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().LookAt = go.transform.GetChild((int)PlayerChildren.CameraTarget);
            cameraReferences[i].transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().Follow = go.transform;
            cameraReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().playerIndex = (PlayerIndex)i;
            cameraReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().associatedPlayerController = go.GetComponent<PlayerControllerHub>();

            go.GetComponent<Player>().cameraReference = cameraReferences[i];
            cameraReferences[i].SetActive(true);
        }
    }
    public override void PlayerHasFinished(Player _player)
    {
        _player.HasFinishedTheRun = true;
        GameManager.Instance.ScoreScreenReference.RefreshScores(_player);

    }
    public void OnPlayerDeath(int id)
    {
        GameManager.Instance.PlayerStart.PlayersReference[id].gameObject.SetActive(false);
        switch (mode)
        {
            case Mode.SoloInfinite:
                throw new NotImplementedException();
            case Mode.LastRemaining:
                nbRemainingPlayer--;
                if (nbRemainingPlayer == 1)
                    Debug.Log("EndGame");
                break;
            default:
                break;
        }

        
    }
}
