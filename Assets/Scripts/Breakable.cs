using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {

    public int minFragments = 2;
    public int maxFragments = 5;
    public int minCollectableDropOnBreak = 1;
    public int maxCollectableDropOnBreak = 5;

    // Index in database to know if it has already been broken. -1 if no persistence.
    public int persistenceIndex = -1;

    private void Start()
    {
        if (transform.gameObject.layer != LayerMask.NameToLayer("Breakable"))
            transform.gameObject.layer = LayerMask.NameToLayer("Breakable");
    }

    public bool DropCollectables()
    {
        return minCollectableDropOnBreak > 0 && maxCollectableDropOnBreak > 0;
    }

    public void HandleCollision(PlayerCharacterHub _playerCharacterHub, PlayerControllerHub _playerControllerHub)
    {
        if (_playerCharacterHub != null && (
                _playerCharacterHub.PlayerState == _playerCharacterHub.dashState
                || _playerCharacterHub.PlayerState == _playerCharacterHub.downDashState
                )
           )
        {
            // Specific case wall breakable
            if (GetComponent<Animation>() && !_playerCharacterHub.GetComponent<EvolutionStrength>())
                return;
            // deep impact

            // TODO: may externalize this behaviour to avoid duplication
            Vector3 playerToTarget = transform.position - _playerCharacterHub.transform.position;
            Vector3 playerCenterToTargetCenter = (transform.position + Vector3.up * 0.5f) - (_playerCharacterHub.transform.position + Vector3.up * 0.5f);
            GameObject go = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(transform, transform.position + Vector3.up * 0.5f + playerCenterToTargetCenter / 2.0f, Quaternion.LookRotation(playerToTarget, Vector3.up), true, true, (int)HitParticles.BigHitStar);
            go.transform.localScale = Vector3.one;
            go.GetComponent<ParticleSystem>().Play();

            //Set vibrations
            UWPAndXInput.GamePad.VibrateForSeconds(_playerControllerHub.playerIndex, 0.8f, 0.8f, .1f);

            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);


            foreach (Renderer mr in GetComponentsInChildren<Renderer>())
                if(mr.GetComponent<ParticleSystem>() == null)
                mr.enabled = false;
            foreach (Collider col in GetComponentsInChildren<Collider>())
                col.enabled = false;

            // pool de morceaux cassés
            int nbFragments = Random.Range(minFragments, maxFragments);
            for (int i = 0; i < nbFragments; i++)
                ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.BreakablePieces).GetItem(transform, Vector3.up * 0.5f, Quaternion.identity, true);

            if (persistenceIndex != -1 && !DatabaseManager.Db.alreadyBrokenBreakables[persistenceIndex])
            {
                DropCollectableOnGround();
                DatabaseManager.Db.alreadyBrokenBreakables[persistenceIndex] = true;
            }

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