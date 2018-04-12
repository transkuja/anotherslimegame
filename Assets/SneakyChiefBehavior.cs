using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakyChiefBehavior : MonoBehaviour {

    [SerializeField]
    SneakyChiefMessageContainer[] messageContainer;

    [SerializeField]
    GameObject[] sneakyChiefBreakablePrefabs;

    // Reward list

    // Next transform
    [SerializeField]
    Transform[] nextTransforms;

    // next transform need something to be broken
    bool[] nextIsABreakable = { false, false, true };

    private void Start()
    {
        // TODO: Load current step from database    
        NextStepCommonProcess();
    }

    public void InitNextStep()
    {
        DatabaseManager.Db.SneakyChiefProgress++;

        if (IsEventOver())
            return;

        ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up, Quaternion.identity, true, false, (int)HitParticles.BigHit);
        if (nextIsABreakable[DatabaseManager.Db.SneakyChiefProgress])
        {
            GameObject pot = Instantiate(sneakyChiefBreakablePrefabs[Random.Range(0, sneakyChiefBreakablePrefabs.Length)], nextTransforms[DatabaseManager.Db.SneakyChiefProgress].position, nextTransforms[DatabaseManager.Db.SneakyChiefProgress].rotation);
            pot.GetComponent<SneakyChiefPot>().Init(gameObject);
            gameObject.SetActive(false);
        }

        NextStepCommonProcess();

    }

    void NextStepCommonProcess()
    {
        if (IsEventOver())
            return;

        transform.position = nextTransforms[DatabaseManager.Db.SneakyChiefProgress].position;
        transform.rotation = nextTransforms[DatabaseManager.Db.SneakyChiefProgress].rotation;
        GetComponent<PNJController>().UpdateOriginalPosition();
    }

    public string GetNextMessage(int _messageIndex)
    {
        return messageContainer[DatabaseManager.Db.SneakyChiefProgress].messages[_messageIndex];
    }

    public int GetNextMessagesLength()
    {
        return messageContainer[DatabaseManager.Db.SneakyChiefProgress].messages.Length;
    }

    public bool IsEventOver()
    {
        return DatabaseManager.Db.SneakyChiefProgress == messageContainer.Length;
    }
}

[System.Serializable]
class SneakyChiefMessageContainer
{
    public string[] messages;
}
