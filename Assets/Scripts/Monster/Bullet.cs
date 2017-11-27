using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    Vector3 direction;
    float speed;
    bool isLaunched;
    float bulletDistance;
    float timerDist;
    public void Init()
    {
        isLaunched = false;
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
        if (playerCollision != null)
        {
            playerCollision.DamagePlayer(other.GetComponent<Player>());
            playerCollision.ExpulsePlayer(other.ClosestPoint(transform.position), other.GetComponent<Rigidbody>(), 50);
        }
        Destroy(this.gameObject);
    }
}
