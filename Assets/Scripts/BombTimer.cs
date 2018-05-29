using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombTimer : MonoBehaviour {

    GameObject bombToFollow;
    Image timerSprite;

	void Start () {
        bombToFollow = transform.parent.gameObject;
        transform.SetParent(null);
        timerSprite = GetComponentInChildren<Image>();
        if(timerSprite)
            timerSprite.transform.position = Camera.main.WorldToScreenPoint(bombToFollow.transform.position + Vector3.up + Vector3.forward);
        StartCoroutine(BombTimerProcess());
	}
	
	void Update () {
        if(timerSprite)
            timerSprite.transform.position = Camera.main.WorldToScreenPoint(bombToFollow.transform.position + Vector3.up + Vector3.forward);
	}

    IEnumerator BombTimerProcess()
    {
        if (timerSprite)
        {
            yield return new WaitForSeconds(1.0f);
            timerSprite.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber3;
            yield return new WaitForSeconds(1.0f);
            timerSprite.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber2;
            yield return new WaitForSeconds(1.0f);
            timerSprite.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber1;
            yield return new WaitForSeconds(1.0f);
        }
        else
            yield return new WaitForSeconds(4f);

        if (bombToFollow && bombToFollow.GetComponent<TheBombPickup>())
        {
            bombToFollow.GetComponent<TheBombPickup>().Explode();
        }
        if(gameObject)
            Destroy(gameObject);
    }
}
