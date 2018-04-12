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

    public void HandleCollision(PlayerCharacterHub _playerCharacterHub, PlayerControllerHub _playerControllerHub)
    {
        if (_playerCharacterHub != null && (
                _playerCharacterHub.PlayerState == _playerCharacterHub.dashState
                || _playerCharacterHub.PlayerState == _playerCharacterHub.downDashState
                )
           )
        {
            //OnBreakEvent
            if (GetComponent<BreakEvent>() != null)
            {
                //if (_player.GetComponent<EvolutionComponent>() != null)
                //{
                //    return;
                //}

                Collectable collectableData = GetComponentInChildren<Collectable>();
                if (collectableData != null)
                {
                    switch (collectableData.type)
                    {
                        case CollectableType.AgileEvolution1:
                            if (_playerCharacterHub.GetComponent<EvolutionAgile>() != null)
                                return;
                            break;
                        case CollectableType.PlatformistEvolution1:
                            if (_playerCharacterHub.GetComponent<EvolutionPlatformist>() != null)
                                return;
                            break;
                        case CollectableType.StrengthEvolution1:
                            if (_playerCharacterHub.GetComponent<EvolutionStrength>() != null)
                                return;
                            break;
                        case CollectableType.GhostEvolution1:
                            if (_playerCharacterHub.GetComponent<EvolutionGhost>() != null)
                                return;
                            break;
                        default:
                            break;

                    }
                }
                GetComponent<BreakEvent>().OnBreakEvent();
            }


            // Specific case wall breakable
            if (GetComponent<Animation>() && !_playerCharacterHub.GetComponent<EvolutionStrength>())
                return;
            // deep impact



            // TODO: may externalize this behaviour to avoid duplication
            Vector3 playerToTarget = transform.position - _playerCharacterHub.transform.position;
            Vector3 playerCenterToTargetCenter = (transform.position + Vector3.up * 0.5f) - (_playerCharacterHub.transform.position + Vector3.up * 0.5f);
            GameObject go = Instantiate(ResourceUtils.Instance.particleSystemManager.impactFeedback, transform);

            go.transform.position = transform.position + Vector3.up * 0.5f + playerCenterToTargetCenter / 2.0f;
            go.transform.rotation = Quaternion.LookRotation(playerToTarget, Vector3.up);
            Destroy(go, 10.0f); // TODO: use a pool instead of instantiate/destroy in chain

            //Set vibrations
            UWPAndXInput.GamePad.VibrateForSeconds(_playerControllerHub.playerIndex, 0.8f, 0.8f, .1f);

            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);


            foreach (Renderer mr in GetComponentsInChildren<Renderer>())
                mr.enabled = false;

            foreach (Collider col in GetComponentsInChildren<Collider>())
                col.enabled = false;
            
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