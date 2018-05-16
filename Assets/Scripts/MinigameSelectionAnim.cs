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
    bool isMinigameTypeOnly = false;

    private void Start()
    {
        if (isMinigameTypeOnly)
        {
            // Title
            transform.GetChild(1).GetComponentInChildren<Text>().text = Utils.MinigameTypeStr[transform.GetSiblingIndex()];

            // Visual
            transform.GetChild(2).GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>(Utils.MinigameTypeSprite[transform.GetSiblingIndex()]) as Sprite;
        }
    }

    public void IsSelected(bool _isSelected, bool _moveToTheLeft)
    {
        isSelected = _isSelected;

        if (isSelected)
        {
            if (_moveToTheLeft)
            {

            }
            else
            {

            }
        }
        else
        {
            if (_moveToTheLeft)
            {

            }
            else
            {

            }
        }
    }

    public void ChangeMinigameVersionSelected()
    {
        //GetComponentsInChildren<Image>()[3].sprite = Resources.Load<Sprite>(_minigameData.spriteImage) as Sprite;

    }

    public void SetMinigame(DatabaseClass.MinigameData _minigameData)
    {
        minigameData = _minigameData;
        transform.GetChild(1).gameObject.SetActive(!_minigameData.isUnlocked);

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

            //GetComponentsInChildren<Image>()[3].sprite = Resources.Load<Sprite>(_minigameData.spriteImage) as Sprite;
            GetComponentInChildren<Text>().text = MinigameDataUtils.GetTitle(_minigameData.Id, _minigameData.version);
        }
        else
        {
            GetComponentInChildren<Text>().text = "";
            transform.GetChild(1).GetComponentInChildren<Text>().text = DatabaseManager.Db.NbRunes.ToString() + " / " + minigameData.nbRunesToUnlock;
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

    public bool IsUnlocked()
    {
        return minigameData.isUnlocked;
    }

    public void IsSelected(bool _isSelected)
    {
        if (_isSelected)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = feedbackMinigameIsSelected;
            hasBeenSelected = true;
            hasBeenDeselected = false;
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().sprite = initialBackground;
            hasBeenDeselected = true;
            hasBeenSelected = false;
        }
    }

    private void Update()
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

    public void Hide()
    {
        isHiding = true;
        lerpValue = 1.0f;
        hasBeenSelected = false;
        hasBeenDeselected = false;
    }

    public void Show()
    {
        isShowing = true;
        lerpValue = 0.0f;
        hasBeenSelected = false;
        hasBeenDeselected = false;
    }
}
