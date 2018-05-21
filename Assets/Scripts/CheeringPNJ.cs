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
        animator = GetComponentInChildren<Animator>();

        if (randomScale)
            transform.localScale = Vector3.one * Random.Range(0.8f, 1.7f);

        bool hasSkin = (Random.Range(0, 2) == 1);

        if (hasSkin)
            cosmetics.Skin = DatabaseManager.Db.skins[Random.Range(0, DatabaseManager.Db.skins.Count)].Id;
        else
            cosmetics.SetUniqueColor(DatabaseManager.Db.colors[Random.Range(0, DatabaseManager.Db.colors.Count)].color);

        cosmetics.FaceType = DatabaseManager.Db.faces[Random.Range(0, DatabaseManager.Db.faces.Count)].indiceForShader;
        bool hasEars = (Random.Range(0, 2) == 1);
        bool hasHat = (Random.Range(0, 2) == 1);
        bool hasMustache = (Random.Range(0, 2) == 1);
        bool hasAccessory = (Random.Range(0, 2) == 1);
        bool hasChin = (Random.Range(0, 2) == 1);
        bool hasForehead = (Random.Range(0, 2) == 1);

        if (!noHatNorEars)
        {
            if (hasHat)
                cosmetics.Hat = DatabaseManager.Db.hats[Random.Range(0, DatabaseManager.Db.hats.Count)].Id;
            else
            {
                if (hasEars)
                    cosmetics.Ears = DatabaseManager.Db.ears[Random.Range(0, DatabaseManager.Db.ears.Count)].Id;
            }
        }
        if (hasMustache)
            cosmetics.Mustache = DatabaseManager.Db.mustaches[Random.Range(0, DatabaseManager.Db.mustaches.Count)].Id;
        if (hasAccessory)
            cosmetics.Accessory = DatabaseManager.Db.accessories[Random.Range(0, DatabaseManager.Db.accessories.Count)].Id;
        if (hasChin)
            cosmetics.Chin = DatabaseManager.Db.chins[Random.Range(0, DatabaseManager.Db.chins.Count)].Id;
        if (hasForehead)
            cosmetics.Forehead = DatabaseManager.Db.foreheads[Random.Range(0, DatabaseManager.Db.foreheads.Count)].Id;

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
