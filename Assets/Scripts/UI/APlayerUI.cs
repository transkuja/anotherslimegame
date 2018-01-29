using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class APlayerUI : MonoBehaviour {

    public GameObject UIref;

    public virtual void Init()
    {
        for (int i = 0; i < UIref.transform.childCount; i++)
        {
            if (GameManager.Instance.PlayerStart.PlayersReference[i] != null)
                UIref.transform.GetChild(i).gameObject.SetActive(true);
            else
                UIref.transform.GetChild(i).gameObject.SetActive(false);
        }
        foreach (GameObject p in GameManager.Instance.PlayerStart.PlayersReference)
            p.GetComponent<Player>().OnValuesChange = new UIfct[(int)CollectableType.Size];

        foreach (TextChange obj in GameObject.FindObjectsOfType<TextChange>())
        {
            obj.Init();
        }

    }

    public virtual void OnValueChange(TextChange text)
    {
        // feedback 
    }

    public void HandleFeedbackMoney(TextChange ptsText)
    {
        if (ptsText.GetComponent<AnimTextCantPay>())
            return;

        ptsText.GetComponent<Outline>().effectColor = Color.red;
        ptsText.GetComponent<Text>().fontSize += 20;
        ptsText.gameObject.AddComponent<AnimTextCantPay>();

        StartCoroutine(ReturnToNormalState(ptsText));
    }

    public IEnumerator ReturnToNormalState(TextChange txt)
    {
        yield return new WaitForSeconds(1f);

        txt.GetComponent<Outline>().effectColor = txt.originalState.GetComponent<Outline>().effectColor;
        txt.GetComponent<Text>().fontSize = txt.originalState.GetComponent<Text>().fontSize;
        if (txt.GetComponent<AnimTextCantPay>())
            Destroy(txt.GetComponent<AnimTextCantPay>());


        txt.transform.localScale = txt.originalState.transform.localScale;
    }
}
