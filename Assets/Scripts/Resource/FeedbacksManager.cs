using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbacksManager : MonoBehaviour
{

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
            case PickUpType.SpeedUp:
                result = speedUpPreview;
                break;
            default:
                return null;
        }

        if (result == null)
            Debug.LogWarning("No specified preview prefab for pickup type " + _type);

        return result;
    }
}
