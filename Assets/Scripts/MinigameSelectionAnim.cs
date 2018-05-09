using UnityEngine;
using UnityEngine.UI;

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
            GetComponentsInChildren<Image>()[3].sprite = Resources.Load<Sprite>(_minigameData.spriteImage) as Sprite;
        else
            GetComponentsInChildren<Image>()[3].sprite = Resources.Load<Sprite>("LockedMinigamePreview") as Sprite;
        GetComponentInChildren<Text>().text = MinigameDataUtils.GetTitle(_minigameData.Id, _minigameData.version);
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
