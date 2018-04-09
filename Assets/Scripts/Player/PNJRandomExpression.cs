using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNJRandomExpression : MonoBehaviour {

    PlayerCosmetics cosmeticsComp;
    [SerializeField]
    bool hidingPNJSpecialBehaviourInColorFloorSceneButIWasTooLazyToDoAnotherScript = false;


    Vector3 lerpOrigin;
    Vector3 lerpDest;
    float lerpParam = 0.0f;
    bool isLerping = false;

    IEnumerator Start()
    {
        cosmeticsComp = GetComponentInChildren<PlayerCosmetics>();
        if (hidingPNJSpecialBehaviourInColorFloorSceneButIWasTooLazyToDoAnotherScript)
            StartCoroutine(Hide());

        while (true)
        {
            yield return new WaitForSeconds(6.0f);
            cosmeticsComp.FaceEmotion = (FaceEmotion)Random.Range(1, 4);
            yield return new WaitForSeconds(2.0f);
            cosmeticsComp.FaceEmotion = FaceEmotion.Neutral;
        }
    }

    IEnumerator Hide()
    {
        while (true)
        {
            yield return new WaitForSeconds(8.0f);
            InitLerp(transform.up * 1.5f);
            yield return new WaitForSeconds(4.0f);
            InitLerp(-transform.up * 1.5f);

        }
    }

    void InitLerp(Vector3 _lerpDir)
    {
        lerpOrigin = transform.position;
        lerpParam = 0.0f;
        lerpDest = lerpOrigin + _lerpDir;
        isLerping = true;
    }

    void Update()
    {
       if (isLerping)
       {
            lerpParam += Time.deltaTime;
            transform.position = Vector3.Lerp(lerpOrigin, lerpDest, Mathf.Clamp(lerpParam, 0, 1.0f));
            if (lerpParam > 1.0f)
                isLerping = false;
       }
    }
}
