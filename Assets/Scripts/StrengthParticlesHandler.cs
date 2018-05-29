using UnityEngine;

public class StrengthParticlesHandler : MonoBehaviour {
    PlayerCharacterHub pc;
    ParticleSystem ps;
	void Init () {
        pc = GetComponentInParent<PlayerCharacterHub>();
        ps = GetComponent<ParticleSystem>();
	}
	
	void Update () {
        if (!pc.IsGrounded)
            ps.Stop();
        else if (!ps.isEmitting)
            ps.Play();
	}

    private void OnEnable()
    {
        if (!ps)
            Init();
        ps.Play();
    }

    private void OnDisable()
    {
        ps.Stop();
    }
}
