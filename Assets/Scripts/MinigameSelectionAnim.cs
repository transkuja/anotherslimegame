using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MinigameSelectionAnim : MonoBehaviour {

    [SerializeField]
    bool isSelected = true;

    [SerializeField]
    Vector2 notSelectedScale;

    // Anim lerp
    Vector2 lerpDestination;
    Vector2 lerpOrigin;
    Vector2 lerpScaleGoal;
    Vector2 lerpOriginScale;

    DatabaseClass.MinigameData minigameData;
    [SerializeField]
    Sprite feedbackMinigameIsSelected;
    [SerializeField]
    Sprite initialBackground;

    bool hasBeenSelected = false;
    bool hasBeenDeselected = false;
    float lerpValue = 0.0f;
    bool isHiding = false;
    bool isShowing = false;

    [SerializeField]
    bool isMinigamePanel = false;

    bool isEnlarging = false;
    bool isReducing = false;

    Vector3 initialPosition;

    [SerializeField]
    GameObject costToUnlockChild;

    private void OnEnable()
    {
        if (isMinigamePanel)
        {
            MinigameGenericInformationLoad();
        }
        initialPosition = transform.localPosition;
    }

    public void MinigameGenericInformationLoad()
    {
        if (DatabaseManager.Db.GetUnlockedMinigamesOfType((MinigameType)transform.GetSiblingIndex()) == null ||
            DatabaseManager.Db.GetUnlockedMinigamesOfType((MinigameType)transform.GetSiblingIndex()).Count == 0)
        {
            costToUnlockChild.SetActive(true);
            GetComponentInChildren<Text>().text = "Locked";
            costToUnlockChild.GetComponentInChildren<Text>().text = "";
            costToUnlockChild.transform.GetChild(0).GetComponent<Image>().enabled = false;
            transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            costToUnlockChild.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);

            // Title
            GetComponentInChildren<Text>().text = Utils.MinigameTypeStr[transform.GetSiblingIndex()];

            // Visual
            transform.GetComponentsInChildren<Image>()[3].sprite = Resources.Load<Sprite>(Utils.MinigameTypeSprite[transform.GetSiblingIndex()]) as Sprite;
        }
    }

    public void SetMinigame(DatabaseClass.MinigameData _minigameData)
    {
        minigameData = _minigameData;
        costToUnlockChild.SetActive(!_minigameData.isUnlocked);

        // Update minigame version prefab (on the right)
        if (!isMinigamePanel)
        {
            if (_minigameData.isUnlocked)
            {
                GetComponentInChildren<Text>().text = MinigameDataUtils.GetTitle(_minigameData.Id, _minigameData.version);
            }
            else
            {
                GetComponentInChildren<Text>().text = "";
                transform.GetChild(1).GetComponentInChildren<Text>().text = DatabaseManager.Db.NbRunes.ToString() + " / " + minigameData.nbRunesToUnlock;
            }
        }
        // Update the huge panel (left side)
        else
        {
            if (_minigameData.isUnlocked)
            {
                //transform.GetChild(3).gameObject.SetActive(false);
                //GetComponentsInChildren<Image>()[2].enabled = true;
                //GetComponentsInChildren<Image>()[3].enabled = true;

                //// Load video preview
                //if (_minigameData.videoPreview != "")
                //{
                //    if (GetComponentInChildren<VideoPlayer>())
                //        GetComponentInChildren<VideoPlayer>().clip = Resources.Load<VideoClip>(_minigameData.videoPreview) as VideoClip;
                //}

                GetComponentInChildren<Text>().text = MinigameDataUtils.GetTitle(_minigameData.Id, _minigameData.version);
                transform.GetChild(2).gameObject.SetActive(true);
                GetComponentsInChildren<Image>()[3].sprite = Resources.Load<Sprite>(_minigameData.spriteImage) as Sprite;
            }
            else
            {
                GetComponentInChildren<Text>().text = "Find more runes!";
                costToUnlockChild.GetComponentInChildren<Text>().text = DatabaseManager.Db.NbRunes.ToString() + " / " + minigameData.nbRunesToUnlock;
                costToUnlockChild.transform.GetChild(0).GetComponent<Image>().enabled = true;
                transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    public string GetMinigameId()
    {
        return minigameData.Id;
    }

    public int GetMinigameVersion()
    {
        return minigameData.version;
    }

    public MinigameType GetMinigameType()
    {
        return minigameData.type;
    }

    public bool IsUnlocked()
    {
        return minigameData.isUnlocked;
    }

    public void IsSelected(bool _isSelected)
    {
        if (_isSelected)
        {
            transform.GetComponentInChildren<Image>().sprite = feedbackMinigameIsSelected;
            hasBeenSelected = true;
            hasBeenDeselected = false;
        }
        else
        {
            transform.GetComponentInChildren<Image>().sprite = initialBackground;
            hasBeenDeselected = true;
            hasBeenSelected = false;
        }
  
    }

    private void Update()
    {
        if (!isEnlarging && !isReducing)
        {
            if (!isHiding && !isShowing)
            {
                if (hasBeenSelected || hasBeenDeselected)
                {
                    lerpValue = lerpValue + ((hasBeenSelected) ? Time.deltaTime : -Time.deltaTime) * 2.5f;
                    transform.localScale = Vector3.one * Mathf.Lerp(1.0f, 1.25f, lerpValue);

                    if (hasBeenSelected && lerpValue > 1.0f)
                    {
                        hasBeenSelected = false;
                        lerpValue = 1.0f;
                    }
                    if (hasBeenDeselected && lerpValue < 0.0f)
                    {
                        hasBeenDeselected = false;
                        lerpValue = 0.0f;
                    }
                }
            }
            else
            {
                //if (lerpValue < 0.0f || lerpValue > 1.0f)
                //{
                lerpValue = lerpValue + ((isShowing) ? Time.deltaTime : -Time.deltaTime) * 3.0f;
                transform.localScale = Vector3.one * Mathf.Lerp(0.0f, 1.0f, lerpValue);

                if (isShowing && lerpValue > 1.0f)
                {
                    isShowing = false;
                }
                if (isHiding && lerpValue < 0.0f)
                {
                    isHiding = false;
                }
                //}
            }
        }
        else
        {
            lerpValue = lerpValue + ((isEnlarging) ? Time.deltaTime : -Time.deltaTime) * 2.0f;
            transform.localScale = Vector3.right * Mathf.Lerp(1.25f, 2.25f, lerpValue) + Vector3.up * Mathf.Lerp(1.25f, 2.0f, lerpValue);
            transform.localPosition = Vector3.Lerp(initialPosition, new Vector3(-150.0f, 0.0f, 0.0f), lerpValue);

            if (isEnlarging && lerpValue > 1.0f)
            {
                isEnlarging = false;
            }
            if (isReducing && lerpValue < 0.0f)
            {
                isReducing = false;
            }
        }
    }

    public void Hide()
    {
        isHiding = true;
        isShowing = false;
        lerpValue = 1.0f;
        hasBeenSelected = false;
        hasBeenDeselected = false;
    }

    public void Show()
    {
        isShowing = true;
        isHiding = false;
        lerpValue = 0.0f;
        hasBeenSelected = false;
        hasBeenDeselected = false;
    }

    public void EnlargeYourUI()
    {
        isEnlarging = true;
        lerpValue = 0.0f;
        isReducing = false;
        hasBeenSelected = false;
        hasBeenDeselected = false;
        isShowing = false;
        isHiding = false;
    }

    public void ReduceYourUI()
    {
        isEnlarging = false;
        isReducing = true;
        lerpValue = 1.0f;
        hasBeenSelected = false;
        hasBeenDeselected = false;
        MinigameGenericInformationLoad();
        isShowing = false;
        isHiding = false;
    }

}
