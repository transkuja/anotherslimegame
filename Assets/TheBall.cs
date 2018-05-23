using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBall : MonoBehaviour {

    Rigidbody rb;

	void Start () {
        rb = GetComponent<Rigidbody>();

    }
	
	void Update () {
        rb.AddForce(1000 * Vector3.down * Time.deltaTime, ForceMode.Acceleration);
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerCharacterHub>())
        {
            PlayerCharacterHub pch = collision.gameObject.GetComponent<PlayerCharacterHub>();
            if (pch.PlayerState is DashDownState || pch.PlayerState is DashState)
            {
                OnKick(pch);                
            } else
            {
                OnHit(pch);
            }
        }
    }

    public virtual void OnKick(PlayerCharacterHub _pch)
    {
        Vector3 collisionPoint = _pch.GetComponent<Collider>().ClosestPoint(transform.position);
        Vector3 direction = transform.position - collisionPoint;
        direction.y = 0;

        direction.Normalize();

        direction += (Vector3.up * 0.5f);

        // play hit particles
        ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up + direction * 2, Quaternion.identity, true, false, (int)HitParticles.HitStar);

        rb.AddForce(200 * direction, ForceMode.Impulse);

        //Set vibrations
        UWPAndXInput.GamePad.VibrateForSeconds(_pch.GetComponent<PlayerController>().playerIndex, 0.8f, 0.8f, .1f);


        if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
    }


    public virtual void OnHit(PlayerCharacterHub _pch)
    {
        rb.velocity = Vector3.zero;
        Vector3 collisionPoint = _pch.GetComponent<Collider>().ClosestPoint(transform.position);
        Vector3 direction = transform.position - collisionPoint;
        direction.y = 0;

        direction.Normalize();

        direction += (Vector3.up * 0.5f);

        rb.AddForce(80 * direction, ForceMode.Impulse);
    }
}
