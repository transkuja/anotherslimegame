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
        rules = new MinigameRules(this, minigameVersion);

        checkRuneObjective = CheckRuneObjectiveForFruits;
        //Debug.Log(minigameVersion);
        if (minigameVersion == 1)
        {
            foreach (GameObject player in playerReferences)
                player.AddComponent<EvolutionGhost>();
        }
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
    }

    public override void OnReadySetGoBegin()
    {
        base.OnReadySetGoBegin();
        GameManager.ChangeState(GameState.Normal);
        StartCoroutine(spawner.Spawner());
        StartCoroutine(bonus.SpawnBonus(BonusSpawner.BonusType.FruitBonus, bonus.fruitBonusSpawnDelay));
        StartCoroutine(bonus.SpawnBonus(BonusSpawner.BonusType.ChangeFruit, bonus.changeFruitSpawnDelay));
        StartCoroutine(bonus.SpawnBonus(BonusSpawner.BonusType.Aspirator, bonus.aspiratorFruitSpawnDelay));
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
        int pointsObjective = 0;
        int curScore = 0;
        for (int i = 0; i < curNbPlayers; i++)
        {
            curScore += GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().NbPoints;
            pointsObjective += necessaryPointsForRune;
        }
        currentScore = curScore;
        return curScore >= pointsObjective;
    }

}
