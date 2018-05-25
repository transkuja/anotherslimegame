using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakableForBreakingMinigame : Breakable {

    [SerializeField]
    bool gameModeCalled = false;

    [SerializeField]
    GameObject rabbit;

    BreakingGameMode gameModeRef;

    private void OnEnable()
    {
        gameModeCalled = false;
        foreach (Renderer mr in GetComponentsInChildren<Renderer>())
            if (mr.GetComponent<ParticleSystem>() == null)
                mr.enabled = true;
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = true;
    }

    public override bool OverrideDestruction(PlayerCharacterHub _playerCharacterHub)
    {
        if (_playerCharacterHub.GetComponent<EnnemyController>())
            return true;
        return false;
    }

    public override void PostCollisionProcess(PlayerCharacterHub _playerCharacterHub, PlayerControllerHub _playerControllerHub)
    {
        if (!gameModeCalled)
        {
            gameModeRef = ((BreakingGameMode)GameManager.Instance.CurrentGameMode);
            gameModeRef.ActivePots--;

            gameModeCalled = true;
            _playerCharacterHub.GetComponent<Player>().UpdateCollectableValue(CollectableType.Points, 1);

            // Handle feedback
            GameObject feedback = Instantiate(ResourceUtils.Instance.feedbacksManager.scorePointsPrefab, null);
            feedback.GetComponentInChildren<Outline>().effectColor = Color.green;
            feedback.GetComponentInChildren<Text>().text = "+ 1";
            feedback.transform.GetChild(0).position = Camera.main.WorldToScreenPoint(transform.position);
            feedback.GetComponentInChildren<Text>().enabled = true;

            if (gameModeRef.withTrappedPots && Random.Range(0, 100) < gameModeRef.boardReference.GetComponent<BreakingGameSpawner>().trapFrequency)
                Invoke("DelayedSummonRabbit", 0.3f);
            else
                Invoke("DelayedReturnToPool", 1.0f);
        }
    }

    void DelayedReturnToPool()
    {
        GetComponent<PoolChild>().ReturnToPool();

    }

    void DelayedSummonRabbit()
    {
        if (gameModeRef.activeRabbits != gameModeRef.maxRabbits)
        {
            Instantiate(rabbit, transform.position + Vector3.up, Quaternion.identity);
            gameModeRef.activeRabbits++;
        }
        GetComponent<PoolChild>().ReturnToPool();
    }
}
