using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FruitGameMode : GameMode {

    public float timer;
    public FruitsSpawner spawner;
    public BonusSpawner bonus;
    bool spawnerInitialized = false;

    public override void StartGame(List<GameObject> playerReferences)
    {
        base.StartGame(playerReferences);
        rules = new MinigameRules(this);

        checkRuneObjective = CheckRuneObjectiveForFruits;

        foreach (GameObject player in playerReferences)
            player.GetComponent<Player>().associateFruit = (Fruit)(player.GetComponent<Player>().PlayerController.playerIndex);
        LaunchTimer();
    }

    public void LaunchTimer()
    {
        GameManager.Instance.GameFinalTimer = timer;
        GameManager.Instance.LaunchFinalTimer();
    }

    protected override void Update()
    {
        base.Update();
        if (GameManager.CurrentState != GameState.Normal || spawnerInitialized == true)
            return;

        StartCoroutine(spawner.Spawner());
        StartCoroutine(bonus.SpawnBonus(BonusSpawner.BonusType.ChangeFruit, bonus.changeFruitSpawnDelay));
        StartCoroutine(bonus.SpawnBonus(BonusSpawner.BonusType.Aspirator, bonus.aspiratorFruitSpawnDelay));
        spawnerInitialized = true;
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


    bool CheckRuneObjectiveForFruits()
    {
        int pointsObjectiveFruit = 0;
        int curScoreFruit = 0;
        foreach (GameObject go in GameManager.Instance.PlayerStart.PlayersReference)
        {
            curScoreFruit += go.GetComponent<Player>().NbPoints;
            pointsObjectiveFruit += necessaryPointsForRune;
        }
        currentScore = curScoreFruit;
        return curScoreFruit >= pointsObjectiveFruit;
    }

}
