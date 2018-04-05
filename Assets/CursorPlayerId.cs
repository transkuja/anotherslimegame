using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorPlayerId : MonoBehaviour {

	
    IEnumerator Start()
    {
        Color tmpColor = GameManager.Instance.PlayerStart.colorPlayer[(int)GetComponentInParent<PlayerController>().playerIndex];
        Color playerColor = new Color(tmpColor.r, tmpColor.g, tmpColor.b, 1.0f);
        transform.GetChild(0).GetComponentInChildren<Image>().color = playerColor;

        while (true)
        {
            yield return new WaitForSeconds(2.5f);
            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);
        }
    }

}
