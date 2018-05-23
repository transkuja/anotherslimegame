using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFloorPickUp : MinigamePickUp {

    [SerializeField]
    float speedBoostDuration = 5.0f;

    [SerializeField]
    float colorArrowRotationDelay = 2.0f;

    [SerializeField]
    float missileSpeed = 10.0f;

    IEnumerator Start()
    {
        InitializeDelegates(pickupType);
        if (pickupType == PickUpType.ColorArrow)
        {
            while (true)
            {
                yield return new WaitForSeconds(colorArrowRotationDelay);
                transform.localEulerAngles += Vector3.up * 90;
            }
        }

    }
    bool isDisapearing = false;
    IEnumerator OnEnableCoroutine()
    {
        if (pickupType == PickUpType.BadOne)
        {
            yield return new WaitForSeconds(2.0f);
            // TODO: anim
            if (!cancelBadPickupCoroutine)
                StartCoroutine(PlayEffectAndReturnToPool());
            else
                cancelBadPickupCoroutine = false;
        }
    }

    void OnEnable()
    {
        StartCoroutine(OnEnableCoroutine());
    }

    void ScorePoints(int _playerIndex)
    {
        ColorFloorHandler.ScorePoints(_playerIndex);
    }

    bool cancelBadPickupCoroutine = false;

    void BadEffect(int _playerIndex)
    {
        if (isDisapearing)
            return;
        ColorFloorHandler.LosePoints(_playerIndex);
        ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position, Quaternion.identity, true, true, 4);
        cancelBadPickupCoroutine = true;
        GetComponentInChildren<PoolChild>().ReturnToPool();
    }

    IEnumerator PlayEffectAndReturnToPool()
    {
        isDisapearing = true;
        transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>().Play();
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        GetComponent<Animator>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        isDisapearing = false;
        GetComponent<Animator>().enabled = true;
        GetComponentInChildren<PoolChild>().ReturnToPool();
    }

    void ColorFloorWithPickup(int _playerIndex)
    {
        ColorFloorHandler.ColorFloorWithPickup(this, _playerIndex);
    }

    void UseBomb(int _playerIndex)
    {

    }

    void UseMissile(int _playerIndex)
    {
        GameObject owner = GameManager.Instance.PlayerStart.PlayersReference[_playerIndex];
        GameObject missile = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.ColorFloorPickUps)
            .GetItem(
            null,
            owner.transform.position + owner.transform.forward * 2.0f,
            Quaternion.LookRotation(owner.transform.forward),
            true,
            false,       
            4 // Missile index
       );

        Rigidbody rb = missile.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.drag = 0.0f;
        rb.AddForce(missile.transform.forward * missileSpeed, ForceMode.Acceleration);

        missile.AddComponent<BoxCollider>();
        missile.AddComponent<MissileBehaviour>();
    }

    void UseSpeedUp(int _playerIndex)
    {
        GameManager.EvolutionManager.AddEvolutionComponent(
            GameManager.Instance.PlayerStart.PlayersReference[_playerIndex],
            GameManager.EvolutionManager.GetEvolutionByPowerName(Powers.Agile),
            true,
            speedBoostDuration
        );
    }

    protected override void InitializeDelegates(PickUpType _pickupType)
    {
        switch (_pickupType)
        {
            case PickUpType.Score:
            case PickUpType.ColorArrow:
            case PickUpType.ColorAround:
            case PickUpType.SpeedUp:
            case PickUpType.BadOne:
                collectPickup = InstantUse;
                break;

            default:
                collectPickup = StoreAndUseLater;
                break;
        }

        switch (_pickupType)
        {
            case PickUpType.Score:
                usePickup = ScorePoints;
                break;
            case PickUpType.ColorArrow:
            case PickUpType.ColorAround:
                usePickup = ColorFloorWithPickup;
                break;
            case PickUpType.SpeedUp:
                usePickup = UseSpeedUp;
                break;
            case PickUpType.Missile:
                usePickup = UseMissile;
                break;
            case PickUpType.Bomb:
                usePickup = UseBomb;
                break;
            case PickUpType.BadOne:
                usePickup = BadEffect;
                break;
        }

        if (usePickup == null)
            Debug.LogWarning("No use method defined for pickup " + _pickupType);
    }
}
