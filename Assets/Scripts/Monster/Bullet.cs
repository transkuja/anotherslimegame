using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    Vector3 direction;
    float speed;
    bool isFired;
    float bulletDistance;
    float timerDist;
    [SerializeField]
    GameObject hitParticles;
    [SerializeField]
    PlayerUIStat damageOn = PlayerUIStat.Points;

    public void Init(GameObject launcher)
    {
        isFired = false;
        Physics.IgnoreCollision(launcher.GetComponent<Collider>(), this.GetComponent<Collider>(), true);
    }
    public void Fire(Vector3 _direction,float _speed,float _bulletDistance)
    {
        isFired = true;
        direction = _direction;
        speed = _speed;
        bulletDistance = _bulletDistance;
        timerDist = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (isFired)
        {
            transform.position += direction * speed * Time.deltaTime;
            timerDist += speed * Time.deltaTime;
            // TODO @Olivier, set timer on poolManager directly
            if (timerDist> bulletDistance)
            {
                GetComponent<PoolChild>().ReturnToPool();
            }
        }
	}
    private void OnTriggerEnter(Collider other)
    {
        PlayerCollisionCenter playerCollision = other.GetComponent<PlayerCollisionCenter>();
        PlayerControllerHub playerController = other.GetComponent<PlayerControllerHub>();
        PlayerCharacterHub playerCharacter = other.GetComponent<PlayerCharacterHub>();
        if (playerCollision != null)
        {
            if (playerCollision.GetComponent<EvolutionStrength>()!=null && playerCharacter.PlayerState == playerCharacter.dashState)
            {
                direction = (transform.position - other.ClosestPointOnBounds(transform.position)).normalized;
                if (direction == Vector3.zero)
                    direction = other.transform.forward;
                speed *= 2;
                timerDist = 0;
                Physics.IgnoreCollision(other, this.GetComponent<Collider>(), true);
            }
            else
            {
                Vector3 centerToTargetCenter = other.transform.position+Vector3.up*0.5f - transform.position;
                GameObject go = Instantiate(hitParticles);
                go.transform.position = transform.position + Vector3.up * 0.5f + centerToTargetCenter / 2.0f;
                go.transform.rotation = Quaternion.LookRotation(centerToTargetCenter, Vector3.up);
                Destroy(go, 10.0f);
                if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
                UWPAndXInput.GamePad.VibrateForSeconds(playerController.playerIndex, 0.9f, 0.9f, .2f);
                
                if (GameManager.Instance.CurrentGameMode.TakesDamageFromTraps)
                {
                    if(GameManager.Instance.IsInHub())
                        playerCollision.DamagePlayerHub();
                    else
                        playerCollision.DamagePlayer(other.GetComponent<Player>(), damageOn);
                }
        
                playerCollision.ExpulsePlayer(other.ClosestPoint(transform.position), other.GetComponent<Rigidbody>(), 50);
                GetComponent<PoolChild>().ReturnToPool();
            }
        } else
        {
            GetComponent<PoolChild>().ReturnToPool();
        }
        
    }
    private Vector3 Bounce(Vector3 objectPosition, Vector3 impactPoint)
    {
        Vector3 velocity = objectPosition- impactPoint;
        return velocity.normalized;
    }
}
