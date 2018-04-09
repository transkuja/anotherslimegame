using System;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
using Runner3D;
public class Runner3DGameMode : GameMode {

    public enum EMode
    {
        SoloInfinite,
        LastRemaining
    }

    [SerializeField]private EMode mode = EMode.SoloInfinite;
    public RunnerLevelGenerator levelGenerator;
    int nbDeadPlayers;

    [SerializeField]
    bool usePlatformist = false;

    public EMode Mode
    {
        get
        {
            return mode;
        }
    }

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this, minigameVersion);
        nbDeadPlayers = 0;
        for (int i = 0; i < playerReferences.Count; i++)
        {
            Player player = playerReferences[i].GetComponent<Player>();
            if (player != null)
            {
                player.OnDeathEvent += OnPlayerDeath;
                if(usePlatformist)
                {
                    if (player.GetComponent<EvolutionPlatformist>() == null)
                        GameManager.EvolutionManager.AddEvolutionComponent(playerReferences[i], GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Platformist));
                }
            }
        }
        if (curNbPlayers == 1)
            mode = EMode.SoloInfinite;
        else
            mode = EMode.LastRemaining;
    }
    public override void OnReadySetGoBegin()
    {
        base.OnReadySetGoBegin();
        levelGenerator.LevelBegin();
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
            cameraReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().associatedPlayerCharacter = go.GetComponent<PlayerCharacterHub>();

            go.GetComponent<Player>().cameraReference = cameraReferences[i];
            cameraReferences[i].SetActive(true);
        }
    }
  
    public void OnPlayerDeath(Player player)
    {
        player.gameObject.SetActive(false);
        player.NbPoints = Mathf.CeilToInt(player.transform.position.z);
        nbDeadPlayers++;
        CheckVictory();
    }
    public void CheckVictory()
    {
        switch (Mode)
        {
            case EMode.SoloInfinite:
                EndGame();
                break;
            case EMode.LastRemaining:
                if (nbDeadPlayers == curNbPlayers)
                    EndGame();
                break;


        }
    }
    public void EndGame()
    {
        GameObject[] playerTab = GameManager.Instance.PlayerStart.PlayersReference.ToArray();
        Array.Sort(playerTab, 
            (GameObject player, GameObject other) => {
                return Mathf.FloorToInt( other.transform.position.z- player.transform.position.z);
            });
        foreach (GameObject playerObj in playerTab)
        {
            Player player = playerObj.GetComponent<Player>();
            player.HasFinishedTheRun = true;
            GameManager.Instance.ScoreScreenReference.RefreshScores(player);
        }
    }
}
