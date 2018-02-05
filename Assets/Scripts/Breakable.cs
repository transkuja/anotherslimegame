using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {

    public int minFragments = 2;
    public int maxFragments = 5;
    public int minCollectableDropOnBreak = 1;
    public int maxCollectableDropOnBreak = 5;

    private void Start()
    {
        if (transform.gameObject.layer != LayerMask.NameToLayer("Breakable"))
            transform.gameObject.layer = LayerMask.NameToLayer("Breakable");
    }

    public void HandleCollision(PlayerControllerHub _player)
    {
        if (_player != null && (
                _player.PlayerState == _player.dashState
                || _player.PlayerState == _player.downDashState
                )
           )
        {
            // impact
            // TODO: may externalize this behaviour to avoid duplication
            Vector3 playerToTarget = transform.position - _player.transform.position;
            Vector3 playerCenterToTargetCenter = (transform.position + Vector3.up * 0.5f) - (_player.transform.position + Vector3.up * 0.5f);
            GameObject go = Instantiate(ResourceUtils.Instance.particleSystemManager.impactFeedback, transform);

            go.transform.position = transform.position + Vector3.up * 0.5f + playerCenterToTargetCenter / 2.0f;
            go.transform.rotation = Quaternion.LookRotation(playerToTarget, Vector3.up);
            Destroy(go, 10.0f); // TODO: use a pool instead of instantiate/destroy in chain

            //Set vibrations
            UWPAndXInput.GamePad.VibrateForSeconds(_player.playerIndex, 0.8f, 0.8f, .1f);

            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);


            if (GetComponent<MeshRenderer>()) GetComponent<MeshRenderer>().enabled = false;
            else if (GetComponentInChildren<MeshRenderer>()) GetComponentInChildren<MeshRenderer>().enabled = false;
            if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
            else if (GetComponentInChildren<Collider>()) GetComponentInChildren<Collider>().enabled = false;

            // pool de morceaux cassés
            int nbFragments = Random.Range(minFragments, maxFragments);
            for (int i = 0; i < nbFragments; i++)
                ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.BreakablePieces).GetItem(transform, Vector3.up * 0.5f, Quaternion.identity, true);

            DropCollectableOnGround();
            if (AudioManager.Instance != null && AudioManager.Instance.breakFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.breakFx);
        }
    }

    void DropCollectableOnGround()
    {
        int numberOfCollectablesToDrop = Random.Range(minCollectableDropOnBreak, maxCollectableDropOnBreak);
        for (int i = 0; i < numberOfCollectablesToDrop; i++)
        {
            if (GameManager.Instance.IsInHub())
            {
                GameObject go = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Money).GetItem(null, transform.position + Vector3.up * 0.5f, Quaternion.identity, true);

                go.GetComponent<Collectable>().Disperse(i);
            }
            else
            {
                GameObject go = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.CollectablePoints).GetItem(null, transform.position + Vector3.up * 0.5f, Quaternion.identity, true);

                go.GetComponent<Collectable>().Disperse(i);
            }


        }
    }
}        