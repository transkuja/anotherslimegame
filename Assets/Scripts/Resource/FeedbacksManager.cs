using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbacksManager : MonoBehaviour
{
    [Header("Replay")]
    public GameObject prefabReplayScreenHub;
    public GameObject prefabCanvasWithUiCameraAdapter;
    public GameObject prefabMessage;
    public GameObject prefabBButton;
    public GameObject prefabFadeInAndOut;

    [Header("Unlocks")]
    public GameObject prefabUnlocks;

    [Header("Minigame Feedbacks")]
    public GameObject prefabRunnerFeedback;
    public GameObject prefabJumpFeedback;
    public GameObject prefabKartFeedback;
    public GameObject prefabPushFeedback;

    [Header("Cost Area Feedbacks")]
    public GameObject prefabCostAreaPlatformistFeedback;
    public GameObject prefabCostAreaStrengthFeedback;
    public GameObject prefabCostAreaAgilityFeedback;
    public GameObject prefabCostAreaGhostFeedback;
    public GameObject prefabCostAreaKeyFeedback;
    public GameObject prefabCostAreaTrophyFeedback;
    public GameObject prefabCostAreaWaterFeedback;

    [Header("Pickup previews")]
    public GameObject ruleScreenShortPrefab;
    public GameObject bombPreview;
    public GameObject missilePreview;
    public GameObject speedUpPreview;
    public GameObject colorAroundPreview;
    public GameObject colorArrowPreview;
    public GameObject colorFloorScorePreview;
    public GameObject colorFloorBadInputPreview;
    public GameObject changerFruitPreview;
    public GameObject aspiratorFruitPreview;
    public GameObject giantFruitPreview;
    public GameObject ballPickupPreview;

    [Header("Feedbacks UI")]
    public GameObject scorePointsPrefab;
    public GameObject feedbackCooldownEvolution;

    public GameObject game;

    public GameObject GetPickupPreview(PickUpType _type)
    {
        GameObject result = null;
        switch (_type)
        {
            case PickUpType.Bomb:
                result = bombPreview;
                break;
            case PickUpType.ColorAround:
                result = colorAroundPreview;
                break;
            case PickUpType.ColorArrow:
                result = colorArrowPreview;
                break;
            case PickUpType.Missile:
                result = missilePreview;
                break;
            case PickUpType.Score:
                result = colorFloorScorePreview;
                break;
            case PickUpType.BadOne:
                result = colorFloorBadInputPreview;
                break;
            case PickUpType.SpeedUp:
                result = speedUpPreview;
                break;
            case PickUpType.Changer:
                result = changerFruitPreview;
                break;
            case PickUpType.Aspirator:
                result = aspiratorFruitPreview;
                break;
            case PickUpType.GiantFruit:
                result = giantFruitPreview;
                break;
            case PickUpType.BallPickup:
                result = ballPickupPreview;
                break;
            default:
                return null;
        }

        if (result == null)
            Debug.LogWarning("No specified preview prefab for pickup type " + _type);

        return result;
    }
}
