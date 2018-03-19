using UnityEngine;
using UnityEngine.UI;

public class MinigameSelectionAnim : MonoBehaviour {

    [SerializeField]
    bool isSelected = true;

    [SerializeField]
    Vector2 notSelectedScale;

    // Anim lerp
    bool isLerpActive = false;
    float lerpValue = 0.0f;
    Vector2 lerpDestination;
    Vector2 lerpOrigin;
    Vector2 lerpScaleGoal;
    Vector2 lerpOriginScale;

    DatabaseClass.MinigameData minigameData;

    public void IsSelected(bool _isSelected, bool _moveToTheLeft)
    {
        isSelected = _isSelected;
        isLerpActive = true;
        lerpValue = 0.0f;

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
        GetComponentsInChildren<Image>()[2].sprite = Resources.Load<Sprite>(_minigameData.spriteImage) as Sprite;
        GetComponentInChildren<Text>().text = MinigameDataUtils.GetTitle(_minigameData.Id);
    }

}
