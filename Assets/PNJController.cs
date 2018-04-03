using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNJController : MonoBehaviour {

    // Aggregations
    private PlayerCharacterHub playerCharacterHub;
    private Player player;
    private Rigidbody rb;

    public GameObject waypoints;
    public int curWayPoint = 0;

    void Start()
    {
        player = GetComponent<Player>();

        playerCharacterHub = player.PlayerCharacter as PlayerCharacterHub;
        rb = playerCharacterHub.Rb;

    }

    public void Update()
    {
        // Oui bob est con
        if (GameManager.CurrentState == GameState.Normal)
        {
            //float xFactor = Mathf.Clamp((transform.worldToLocalMatrix * (waypoints.transform.GetChild(curWayPoint).transform.position)).x - transform.position.x, -1f, 1f);
            //float yFactor = Mathf.Clamp((transform.worldToLocalMatrix * (waypoints.transform.GetChild(curWayPoint).transform.position)).z - transform.position.z, -1f, 1f);
            
           

            HandleMovement(xFactor, yFactor);

            //if(Vector3.Distance(waypoints.transform.GetChild(curWayPoint).transform.position, transform.position) < 0.2f)
            //{
            //    curWayPoint = (curWayPoint == 0) ? 1 : 0;
            //}
        }
    }

    public virtual void HandleMovement(float x, float y)
    {
        Vector3 initialVelocity = playerCharacterHub.PlayerState.HandleSpeed(x, y);
        Vector3 velocityVec = initialVelocity.z * transform.forward;
        if (!playerCharacterHub.IsGrounded)
            velocityVec += initialVelocity.x * transform.right * player.airControlFactor;
        else
            velocityVec += initialVelocity.x * transform.right;
     

        playerCharacterHub.PlayerState.Move(velocityVec, player.airControlFactor, x, y);

        // TMP Animation
        playerCharacterHub.PlayerState.HandleControllerAnim(x, y);
    }

 
}
