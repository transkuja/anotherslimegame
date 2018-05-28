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

    public Transform dialog1;
    public Transform dialog2;
    public Transform dialog3;

    Transform UIref;
    [HideInInspector]
    public Transform RuleScreen;
    private bool isUiShown = false;
    private float showTime = 4.0f;
    private float currentTimer = 0;

    // See gameManager Update
    public Text TimerText
    {
        get
        {
            return timerText;
        }
    }

    public void Awake()
    {
        if (GameManager.UiReference == null)
        {

            GameManager.UiReference = this;
            SceneManager.sceneLoaded += LoadScene;
        }
    }

    public void LoadScene(Scene sceneId, LoadSceneMode mode)
    {
        if (sceneId.buildIndex != 0)
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
        GameManager.Instance.finalTimerInitialized = false;
        GameManager.Instance.isTimeOver = true;

        timerText = transform.GetChild(1).GetComponent<Text>();
        UIref = transform.GetChild(0);
        ptsText = UIref.GetChild(0).GetComponentInChildren<Text>().transform;
        runeText = UIref.GetChild(1).GetComponentInChildren<Text>().transform;

        RuleScreen = transform.GetChild(transform.childCount - 1).transform;
    }

    void DestroyOnMenuScreen(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == 0) //could compare Scene.name instead
        {
            GameManager.UiReference = null;
            Destroy(this); //change as appropriate
            Destroy(this.gameObject); //change as appropriate
        }
    }

    private void Start()
    {
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
        // Special case TODO:Refacto : 
        if (GameManager.Instance.CurrentGameMode is Runner3DGameMode)
        {
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        }

        if( SlimeDataContainer.instance != null && SlimeDataContainer.instance.nbPlayers > 1)
        {
            if (SceneManager.GetActiveScene().name != "Podium")
            {
                UIref.transform.localPosition = new Vector3(-115.0f, 360f);
                UIref.GetChild(0).GetComponent<Image>().enabled = true;
                UIref.GetChild(1).GetComponent<Image>().enabled = true;
            }
        }
    }

    public void Update()
    {
        if (isUiShown)
        {
            currentTimer += Time.deltaTime;
            if( currentTimer > showTime)
            {
                TooglePersistenceUI(false);

                isUiShown = false;
            }
        }
    }

    public void TimerNeedUpdate(float _currentGameFinalTimer)
    {
        if (timerText == null)
            return;

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
            HandleFeedback(type, runeText, runeTextOriginalState);
        if (type == CollectableType.Money)
            HandleFeedback(type, ptsText, ptsTextOriginalState);
    }

    public void HandleFeedback(CollectableType type,Transform txtToChange, Transform originalState)
    {
        TooglePersistenceUI(true, type);
        txtToChange.GetComponent<Text>().fontSize += 20;

        isUiShown = true;
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


    public void TooglePersistenceUI(bool active, CollectableType type = CollectableType.Money, float t =1.0f)
    {
        if (active)
        {
            UpdateGlobalMoney();
            UpdateRunes();
            // Reset timer
            currentTimer = 0.0f;
            if(!UIref.gameObject.activeSelf)
                StartCoroutine(FadeIn(.75f, type));
        }
        else
        {
            if (UIref.gameObject.activeSelf)
                StartCoroutine(FadeOut(t));
        }

    }

    IEnumerator FadeIn(float seconds, CollectableType type)
    {
        UIref.gameObject.SetActive(true);
        if (type == CollectableType.Money)
            UIref.GetChild(0).gameObject.SetActive(true);
        if (type == CollectableType.Rune)
            UIref.GetChild(1).gameObject.SetActive(true);
        Image[] images = UIref.GetComponentsInChildren<Image>();
        Text[] texts = UIref.GetComponentsInChildren<Text>();

        foreach (Image i in images)
            i.color = new Color(i.color.r, i.color.g, i.color.b, 0.0f);

        foreach (Text t in texts)
            t.color = new Color(t.color.r, t.color.g, t.color.b, 0.0f);

        float timer = 0.0f;
        while(timer < seconds)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            foreach(Image i in images)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, Mathf.Lerp(0.0f, 1.0f, timer / seconds));
            }

            foreach (Text t in texts)
            {
                t.color = new Color(t.color.r, t.color.g, t.color.b, Mathf.Lerp(0.0f, 1.0f, timer / seconds));
            }
        }
        
    }

    IEnumerator FadeOut(float seconds)
    {
        UIref.GetChild(0).gameObject.SetActive(false);
        UIref.GetChild(1).gameObject.SetActive(false);
        Image[] images = UIref.GetComponentsInChildren<Image>();
        Text[] texts = UIref.GetComponentsInChildren<Text>();
        float timer = 0.0f;
        while (timer < seconds)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            foreach (Image i in images)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, Mathf.Lerp(1.0f, 0.0f, timer / seconds));
            }

            foreach (Text t in texts)
            {
                t.color = new Color(t.color.r, t.color.g, t.color.b, Mathf.Lerp(1.0f, 0.0f, timer / seconds));
            }
        }
        UIref.gameObject.SetActive(false);
    }

    public void UpdateGlobalMoney()
    {
        ptsText.GetComponent<Text>().text = GameManager.Instance.GlobalMoney.ToString();
    }

    public void UpdateRunes()
    {
        runeText.GetComponent<Text>().text = GameManager.Instance.Runes.ToString();
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= LoadScene;
    }
}
