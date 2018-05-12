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

    public void SetMinigame(DatabaseClass.MinigameData _minigameData)
    {
        minigameData = _minigameData;
        if (_minigameData.isUnlocked)
        {
            transform.GetChild(3).gameObject.SetActive(false);
            GetComponentsInChildren<Image>()[2].enabled = true;
            GetComponentsInChildren<Image>()[3].enabled = true;

            // Load video preview
            if (_minigameData.videoPreview != "")
            {
                if (GetComponentInChildren<VideoPlayer>())
                    GetComponentInChildren<VideoPlayer>().clip = Resources.Load<VideoClip>(_minigameData.videoPreview) as VideoClip;
            }

            GetComponentsInChildren<Image>()[3].sprite = Resources.Load<Sprite>(_minigameData.spriteImage) as Sprite;
            GetComponentInChildren<Text>().text = MinigameDataUtils.GetTitle(_minigameData.Id, _minigameData.version);
        }
        else
        {
            GetComponentsInChildren<Image>()[2].enabled = false;
            GetComponentsInChildren<Image>()[3].enabled = false;
            GetComponentInChildren<Text>().text = "?";
            transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(3).GetComponentInChildren<Text>().text = DatabaseManager.Db.NbRunes.ToString() + " / " + minigameData.costToUnlock;
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
}
