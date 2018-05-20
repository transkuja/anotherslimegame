using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFloor : MonoBehaviour {

    [SerializeField]
    GameObject warningFeedback;

    Vector3 startPosition;
    bool isFalling = false;
    bool isAscending = false;
    float lerpValue = 0.0f;
    bool isTheLast;

    float speed;

    public void WarnPlayerSmthgBadIsComing(float _reactionTime = 1.0f, bool _isTheLast = false)
    {
        isTheLast = _isTheLast;
        warningFeedback.GetComponentInChildren<WarningFeedback>().reactionTime = _reactionTime;
        warningFeedback.SetActive(true);
    }

    public int GetFloorIndex()
    {
        return transform.GetSiblingIndex() + transform.parent.GetSiblingIndex() * 8;
    }

    public void Fall(float _reactionTime)
    {
        startPosition = transform.position;
        isFalling = true;
        lerpValue = 0.0f;
        speed = _reactionTime;
    }

    public void Ascend(float _reactionTime)
    {
        isAscending = true;
        lerpValue = 0.0f;
        speed = _reactionTime/2.0f;
    }

    private void Update()
    {
        if (isFalling)
        {
            lerpValue += Time.deltaTime/speed;
            transform.position = Vector3.Lerp(startPosition, startPosition + Vector3.down * 10, lerpValue);
            if (lerpValue > 1.0f)
            {
                isFalling = false;
                if (isTheLast)
                    ((BreakingGameMode)(GameManager.Instance.CurrentGameMode)).boardReference.GetComponent<BreakingGameSpawner>().isReady = true;
            }
        }

        if (isAscending)
        {
            lerpValue += Time.deltaTime/speed;
            if (lerpValue > 1.0f)
            {
                lerpValue = 1.0f;
                isAscending = false;
            }
            transform.position = Vector3.Lerp(startPosition + Vector3.down * 10, startPosition, lerpValue);
        }
    }

}
