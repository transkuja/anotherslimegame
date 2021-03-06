﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleporterToMinigame : MonoBehaviour {

    public bool isTeleporterActive = false;

    string minigameSceneToTeleportTo = "";
    MeshRenderer meshRenderer;
    int minigameVersion = 0;

    Vector3 originPosition;
    Vector3 endPosition;

    GameObject[] refCanvas = new GameObject[2];
    GameObject[] BButtonShown = new GameObject[2];

    public void CreateButtonFeedback(int _playerIndex)
    {
        refCanvas[_playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabCanvasWithUiCameraAdapter, transform);
        refCanvas[_playerIndex].GetComponent<UICameraApdater>().PlayerIndex = _playerIndex;
        refCanvas[_playerIndex].transform.localPosition += Vector3.up * 5.0f;
        refCanvas[_playerIndex].transform.localScale = Vector3.one * 1.5f;
        BButtonShown[_playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabBButton, refCanvas[_playerIndex].transform);
        BButtonShown[_playerIndex].SetActive(true);
        if (_playerIndex == Controls.keyboardIndex)
            BButtonShown[_playerIndex].GetComponentInChildren<Image>().sprite = ResourceUtils.Instance.spriteUtils.keyboardE;

        refCanvas[_playerIndex].layer = LayerMask.NameToLayer((_playerIndex == 0) ? "CameraP1" : "CameraP2");
    }

    public void DestroyButtonFeedback(int _playerIndex)
    {
        Destroy(BButtonShown[_playerIndex]);
        Destroy(refCanvas[_playerIndex]);
    }

    public void TeleportToMinigame(string sceneName, int _minigameVersion)
    {
        minigameSceneToTeleportTo = sceneName;
        minigameVersion = _minigameVersion;
    }

    public void LoadMinigame()
    {
        //Just in case
        ReturnToNormalState();

        List<GameObject> players = GameManager.Instance.PlayerStart.PlayersReference;
        for (int i = 0; i < players.Count; i++)
        {
            Player currentPlayer = players[i].GetComponent<Player>();
            GameManager.Instance.playerEvolutionTutoShown[i] = currentPlayer.evolutionTutoShown;
        }

        GameManager.Instance.savedPositionInHub = transform.parent.position + transform.parent.forward * 5.0f;
        GameManager.Instance.savedRotationInHub = Quaternion.LookRotation(transform.parent.forward);

        GameManager.Instance.DataContainer.minigameVersion = minigameVersion;
        GameManager.Instance.DataContainer.minigameType = DatabaseManager.Db.GetMinigameTypeBySceneAndVersion(minigameSceneToTeleportTo, minigameVersion);
        DatabaseManager.instance.SaveData();

        LevelLoader.LoadLevelWithFadeOut(minigameSceneToTeleportTo);
    }

    public void ReturnToNormalState()
    {
        GameManager.ChangeState(GameState.Normal);
    }
}
