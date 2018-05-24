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
        timerSprite.transform.position = Camera.main.WorldToScreenPoint(bombToFollow.transform.position + Vector3.up + Vector3.forward);
        StartCoroutine(BombTimerProcess());
	}
	
	void Update () {
        timerSprite.transform.position = Camera.main.WorldToScreenPoint(bombToFollow.transform.position + Vector3.up + Vector3.forward);
	}

    IEnumerator BombTimerProcess()
    {
        yield return new WaitForSeconds(1.0f);
        timerSprite.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber3;
        yield return new WaitForSeconds(1.0f);
        timerSprite.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber2;
        yield return new WaitForSeconds(1.0f);
        timerSprite.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber1;
        yield return new WaitForSeconds(1.0f);
        bombToFollow.GetComponent<TheBombPickup>().Explode();
        Destroy(gameObject);
    }
}
