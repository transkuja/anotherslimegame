    using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI : MonoBehaviour {

    Text timerText;
    Transform ptsText;
    Transform ptsTextOriginalState;

    Transform runeText;
    Transform runeTextOriginalState;

    Transform UIref;
    [HideInInspector]
    public Transform RuleScreen;

    public void Awake()
    {
        if (GameManager.UiReference == null)
        {
            GameManager.UiReference = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        timerText = transform.GetChild(1).GetComponent<Text>();
        UIref = transform.GetChild(0);
        ptsText = UIref.GetChild(0).GetComponentInChildren<Text>().transform;
        runeText = UIref.GetChild(1).GetComponentInChildren<Text>().transform;
        RuleScreen = transform.GetComponentInChildren<RuleScreenHandler>().transform;

        ptsTextOriginalState = ptsText;
        runeTextOriginalState = runeText;

    }

    public void TimerNeedUpdate(float _currentGameFinalTimer)
    {
        if (!timerText.gameObject.activeInHierarchy) timerText.gameObject.SetActive(true);

        int minutes = Mathf.FloorToInt(_currentGameFinalTimer / 60);
        int seconds = (int)_currentGameFinalTimer % 60;
        String timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.text = timeStr;
    }

    public void HandleFeedback(CollectableType type)
    {
        if (type == CollectableType.Rune)
            HandleFeedback(runeText, runeTextOriginalState);
        if( type == CollectableType.Money)
            HandleFeedback(ptsText, ptsTextOriginalState);
    }

    public void HandleFeedback(Transform txtToChange, Transform originalState)
    {
        TooglePersistenceUI(true);
        if (txtToChange.GetComponent<AnimTextCantPay>())
            return;

        txtToChange.GetComponent<Outline>().effectColor = Color.red;
        txtToChange.GetComponent<Text>().fontSize += 20;
        txtToChange.gameObject.AddComponent<AnimTextCantPay>();

        StartCoroutine(ReturnToNormalState(txtToChange, originalState));
    }

    public IEnumerator ReturnToNormalState(Transform txtToChange, Transform originalState)
    {
        yield return new WaitForSeconds(2f);

        TooglePersistenceUI(false);
        txtToChange.GetComponent<Outline>().effectColor = originalState.GetComponent<Outline>().effectColor;
        txtToChange.GetComponent<Text>().fontSize = originalState.GetComponent<Text>().fontSize;
        if (txtToChange.GetComponent<AnimTextCantPay>())
            Destroy(txtToChange.GetComponent<AnimTextCantPay>());


        txtToChange.transform.localScale = originalState.transform.localScale;
    }


    public void TooglePersistenceUI(bool active)
    {
        UIref.gameObject.SetActive(active);

        if (active)
        {
            UpdateGlobalMoney();
            UpdateRunes();
        }
    }


    public void UpdateGlobalMoney()
    {
        ptsText.GetComponent<Text>().text = GameManager.Instance.GlobalMoney.ToString();
    }

    public void UpdateRunes()
    {
        runeText.GetComponent<Text>().text = GameManager.Instance.Runes.ToString();
    }
}
