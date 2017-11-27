using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    Vector3 direction;
    float speed;
    bool isLaunched;
    float bulletDistance;
    float timerDist;
    public void Init(GameObject launcher)
    {
        isLaunched = false;
        Physics.IgnoreCollision(launcher.GetComponent<Collider>(), this.GetComponent<Collider>());
    }
    public void Lauch(Vector3 _direction,float _speed,float _bulletDistance)
    {
        isLaunched = true;
        direction = _direction;
        speed = _speed;
        bulletDistance = _bulletDistance;
        timerDist = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (isLaunched)
        {
            transform.position += direction * speed * Time.deltaTime;
            timerDist += speed * Time.deltaTime;
            if (timerDist> bulletDistance)
            {
                Destroy(this.gameObject);
            }
        }
	}
    private void OnTriggerEnter(Collider other)
    {
        PlayerCollisionCenter playerCollision = other.GetComponent<PlayerCollisionCenter>();
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerCollision != null)
        {
            if (playerCollision.GetComponent<EvolutionStrength>()!=null && playerController.PlayerState == playerController.dashState)
            {
                direction = (transform.position - other.ClosestPointOnBounds(transform.position)).normalized;
                if (direction == Vector3.zero)
                    direction = other.transform.forward;
                speed *= 2;
                timerDist = 0;
                Physics.IgnoreCollision(other, this.GetComponent<Collider>());
            }
            else
            {
                playerCollision.DamagePlayer(other.GetComponent<Player>());
                playerCollision.ExpulsePlayer(other.ClosestPoint(transform.position), other.GetComponent<Rigidbody>(), 50);
                Destroy(this.gameObject);
            }
        }
        
    }
    private Vector3 Bounce(Vector3 objectPosition, Vector3 impactPoint)
    {
        Vector3 velocity = objectPosition- impactPoint;
        return velocity.normalized;
    }
}
