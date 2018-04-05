using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorPlayerId : MonoBehaviour {

    public void Init()
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.ActivePlayersAtStart; ++i)
        {

        }
    }

    //public void Start()
    //{
    //    //if (!GameManager.Instance.IsInHub())
    //    {
    //        //Color tmpColor = GameManager.Instance.PlayerStart.colorPlayer[(int)GetComponentInParent<PlayerController>().playerIndex];
    //        //Color playerColor = new Color(tmpColor.r, tmpColor.g, tmpColor.b, 1.0f);
    //        //transform.GetChild(0).GetComponentInChildren<Image>().color = playerColor;

    //        //while (true)
    //        //{
    //        transform.GetChild(0).gameObject.SetActive(true);

    //        yield return new WaitForSeconds(122.5f);
    //            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);
    //        //}
    //    }
    //}

}
