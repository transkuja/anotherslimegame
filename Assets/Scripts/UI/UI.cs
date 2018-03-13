    using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
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
        }
    }


    void DestroyOnMenuScreen(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == 0) //could compare Scene.name instead
        {
            // test
            GameManager.UiReference = null;
            Destroy(this); //change as appropriate
            Destroy(this.gameObject); //change as appropriate
      
        }
    }

    private void Start()
    {
        timerText = transform.GetChild(1).GetComponent<Text>();
        UIref = transform.GetChild(0);
        ptsText = UIref.GetChild(0).GetComponentInChildren<Text>().transform;
        runeText = UIref.GetChild(1).GetComponentInChildren<Text>().transform;
        RuleScreen = transform.GetChild(2);

        // Merde copy de reference ..
        ptsTextOriginalState = ptsText;
        runeTextOriginalState = runeText;

        if (!GameManager.Instance.IsInHub())
        {
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
        else
        {
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if( level != 0)
        {
            if (!GameManager.Instance.IsInHub())
            {
                GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                GetComponent<Canvas>().worldCamera = Camera.main;
            }
            else
            {
                GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }
      
    }

    public void TimerNeedUpdate(float _currentGameFinalTimer)
    {
        if (!timerText.gameObject.activeInHierarchy)
        {
            // Init timer visual
            timerText.color = Color.white;
            timerText.GetComponent<AnimText>().enabled = false;
            timerText.gameObject.SetActive(true);
        }

        // Set timer red with feedback under 10 seconds
        if (_currentGameFinalTimer < 10.0f && !timerText.GetComponent<AnimText>().isActiveAndEnabled)
        {
            timerText.color = Color.red;
            timerText.GetComponent<AnimText>().enabled = true;
        }

        // DEBUG, only to have fancy debug, reset timer settings if we increase the time left
        if (timerText.GetComponent<AnimText>().isActiveAndEnabled && _currentGameFinalTimer > 10.0f)
        {
            timerText.color = Color.white;
            timerText.GetComponent<AnimText>().enabled = false;
        }
        
        int minutes = Mathf.FloorToInt(_currentGameFinalTimer / 60);
        int seconds = (int)_currentGameFinalTimer % 60;
        String timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.text = timeStr;
    }

    public void HandleFeedback(CollectableType type)
    {
        if (type == CollectableType.Rune)
            HandleFeedback(runeText, runeTextOriginalState);
        if (type == CollectableType.Money)
            HandleFeedback(ptsText, ptsTextOriginalState);
    }

    public void HandleFeedback(Transform txtToChange, Transform originalState)
    {
        TooglePersistenceUI(true);
        txtToChange.GetComponent<Text>().fontSize += 20;
        StartCoroutine(ReturnToNormalState(txtToChange, originalState));
    }

    public void HandleFeedbackCantPay(CollectableType type)
    {
        if (type == CollectableType.Rune)
            HandleFeedbackCantPay(runeText, runeTextOriginalState);
        if( type == CollectableType.Money)
            HandleFeedbackCantPay(ptsText, ptsTextOriginalState);
    }

    public void HandleFeedbackCantPay(Transform txtToChange, Transform originalState)
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
        if (txtToChange.GetComponent<AnimTextCantPay>())
            Destroy(txtToChange.GetComponent<AnimTextCantPay>());

        txtToChange.GetComponent<Outline>().effectColor = Color.blue;
        txtToChange.GetComponent<Text>().fontSize = originalState.GetComponent<Text>().fontSize;
    
        txtToChange.transform.localScale = originalState.transform.localScale;
        TooglePersistenceUI(false);
    }


    public void TooglePersistenceUI(bool active)
    {
        if (active)
        {
            UpdateGlobalMoney();
            UpdateRunes();
        }
        UIref.gameObject.SetActive(active);
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
