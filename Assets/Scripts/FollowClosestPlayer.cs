using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowClosestPlayer : MonoBehaviour {

    Transform target;

    Vector3 targetLookAt;
    Vector3 startForward;
    
    [SerializeField]
    float maxRange = 25.0f;

    [SerializeField]
    float maxAngle = 75f;

    [SerializeField]
    float speed = 10.0f;

    [SerializeField]
    float heightOffset = 4.0f;

    [SerializeField]
    bool speedRelativeToAngle = false;
    private void Start()
    {
        targetLookAt = transform.forward;
        startForward = transform.forward;
    }

    void Update () {
        if (target)
            GetLookAtFromTarget();
        if (target == null)
            FindNewTarget();

        float curSpeed = speed;
        if (speedRelativeToAngle)
            curSpeed = speed + Mathf.Abs(Vector3.Angle(transform.forward, targetLookAt)/90.0f * speed * 2.0f);

        if (targetLookAt == startForward)
            curSpeed /= 10f;

        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, targetLookAt, Time.deltaTime * curSpeed), Vector3.up);

    }

    void GetLookAtFromTarget()
    {
        if (target == null)
        { 
            targetLookAt = startForward;
            return;
        }

        if (Vector3.Distance(transform.position, target.position) > maxRange)
        {
            target = null;
            targetLookAt = startForward;
            return;
        }

        targetLookAt = ((target.position + Vector3.up * heightOffset) - transform.position).normalized;

        if (Vector3.Angle(startForward, targetLookAt) > maxAngle)
        {
            target = null;
            targetLookAt = startForward;
            return;
        }
    }

    void FindNewTarget()
    {
        GameObject ClosestPlayer = null;
        float minDistance = Mathf.Infinity;
        List<GameObject> players = GameManager.Instance.PlayerStart.PlayersReference;
        for (int i = 0; i < GameManager.Instance.PlayerStart.ActivePlayersAtStart; i++)
        {
            float dist = Vector3.Distance(transform.position, players[i].transform.position);
            if (dist < minDistance)
            {
                ClosestPlayer = players[i];
                minDistance = dist;
            }
        }
        if (minDistance < 100.0f)
        {
            target = ClosestPlayer.transform;
            GetLookAtFromTarget();
        }
        else
        {
            target = null;
            targetLookAt = startForward;
        }
    }
}
