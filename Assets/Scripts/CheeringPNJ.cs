using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeringPNJ : MonoBehaviour {

    PlayerCosmetics cosmetics;
    Animator animator;

    [SerializeField]
    int maxCustomizables = 5;
    [SerializeField]
    bool noHatNorEars = false;
    [SerializeField]
    bool randomScale = true;
    [SerializeField]
    bool noRoll = false;

    IEnumerator Start () {
        cosmetics = GetComponentInChildren<PlayerCosmetics>();
        cosmetics.RandomSelection();
        animator = GetComponentInChildren<Animator>();

        if (randomScale)
            transform.localScale = Vector3.one * Random.Range(0.8f, 1.7f);

        if (noHatNorEars)
        {
            cosmetics.Hat = "None";
            cosmetics.Ears = "None";
        }

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));

            cosmetics.FaceEmotion = FaceEmotion.Winner;
            int randAnim = Random.Range(0, 3);
            if (randAnim == 0) animator.SetTrigger("Applause");
            else if (randAnim == 1 && !noRoll) animator.SetTrigger("Roll");
            else animator.SetTrigger("Dance");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle");
            cosmetics.FaceEmotion = FaceEmotion.Neutral;
        }
    }

}
