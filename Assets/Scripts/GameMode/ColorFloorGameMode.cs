using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class ColorFloorGameMode : GameMode {

    [Header("ColorFloor settings")]
    public float timer;
    public bool freeMovement = true;
    public float restrainedMovementTick;
    [SerializeField]
    List<GameObject> restrainedMovementStarters;

    public bool squareToScoreMode = false;

    public GameObject boardReference;
    public bool withBadSpawns = false;
    public RuntimeAnimatorController RestrainedAnimatorController;

    private void Start()
    {
        if (boardReference == null)
            Debug.LogError("ColorFloorGameMode: Board reference is not linked to gamemode!");
    }

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
        rules = new MinigameRules(this, minigameVersion);

        for (int i = 0; i < playerReferences.Count; i++)
        {
            Destroy(playerReferences[i].GetComponent<PlayerControllerHub>());

            if (!freeMovement)
            {
                PlayerController pc = playerReferences[i].AddComponent<PlayerController>();
                pc.playerIndex = (PlayerIndex)i;
                pc.PlayerIndexSet = true;
                playerReferences[i].transform.position = restrainedMovementStarters[i].transform.position;
                playerReferences[i].GetComponent<Rigidbody>().useGravity = true;
                playerReferences[i].GetComponent<Player>().NbPoints = 0;
                playerReferences[i].GetComponent<PlayerCharacter>().Anim.runtimeAnimatorController = RestrainedAnimatorController;
            }
            else
            {
                PlayerControllerFloorColor pc = playerReferences[i].AddComponent<PlayerControllerFloorColor>();
                pc.playerIndex = (PlayerIndex)i;
                pc.PlayerIndexSet = true;
            }
        }


        boardReference.GetComponent<ColorFloorPickupHandler>().DEBUG_forceBadSpawns = withBadSpawns;

        LaunchTimer();
        ColorFloorHandler.Init(GameManager.Instance.ActivePlayersAtStart, boardReference);
    }

    public override void ExtractVersionData(int _minigameVersion)
    {
        if (_minigameVersion >= 4)
        {
            freeMovement = false;
            _minigameVersion -= 4;
        }
        else
        {
            freeMovement = true;
        }

        if (_minigameVersion >= 2)
        {
            withBadSpawns = true;
            _minigameVersion -= 2;
        }
        else
        {
            withBadSpawns = false;
        }
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
