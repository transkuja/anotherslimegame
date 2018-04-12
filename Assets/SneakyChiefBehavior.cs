using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakyChiefBehavior : MonoBehaviour {

    public string[] messages;

    [SerializeField]
    GameObject[] sneakyChiefBreakablePrefabs;

    int currentStep = 0; // TODO: save in DB
    // Reward list

    // Next transform
    [SerializeField]
    Transform[] nextTransforms;

    // next transform need something to be broken
    bool[] nextIsABreakable = { false, false, true };

    private void Start()
    {
        // TODO: Load current step from database    
        currentStep = 0;
        transform.position = nextTransforms[currentStep].position;
        transform.rotation = nextTransforms[currentStep].rotation;
        GetComponent<PNJController>().UpdateOriginalPosition();
    }

    public void InitNextStep()
    {
        ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up, Quaternion.identity, true, false, (int)HitParticles.BigHit);
        currentStep++;
        if (nextIsABreakable[currentStep])
        {
            GameObject pot = Instantiate(sneakyChiefBreakablePrefabs[Random.Range(0, sneakyChiefBreakablePrefabs.Length)], nextTransforms[currentStep].position, nextTransforms[currentStep].rotation);
            pot.GetComponent<SneakyChiefPot>().Init(gameObject);
            transform.position = nextTransforms[currentStep].position;
            transform.rotation = nextTransforms[currentStep].rotation;
            gameObject.SetActive(false);
        }
        else
        {
            transform.position = nextTransforms[currentStep].position;
            transform.rotation = nextTransforms[currentStep].rotation;
        }
        GetComponent<PNJController>().UpdateOriginalPosition();

    }

}
