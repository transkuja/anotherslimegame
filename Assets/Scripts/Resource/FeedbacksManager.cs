using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbacksManager : MonoBehaviour
{

    [Header("Shelter Feedbacks")]
    public GameObject prefabShelterPlatformistFeedback;
    public GameObject prefabShelterStrengthFeedback;
    public GameObject prefabShelterAgilityFeedback;
    public GameObject prefabShelterGhostFeedback;

    [Header("Cost Area Feedbacks")]
    public GameObject prefabCostAreaPlatformistFeedback;
    public GameObject prefabCostAreaStrengthFeedback;
    public GameObject prefabCostAreaAgilityFeedback;
    public GameObject prefabCostAreaGhostFeedback;
    public GameObject prefabCostAreaKeyFeedback;
    public GameObject prefabCostAreaTrophyFeedback;
    public GameObject prefabCostAreaWaterFeedback;


    public GameObject GetShelterFeedbackFromEvolutionName(CollectableType evolutionType)
    {
        switch (evolutionType)
        {
            case CollectableType.AgileEvolution1:
                return prefabShelterAgilityFeedback;
            case CollectableType.PlatformistEvolution1:
                return prefabShelterPlatformistFeedback;
            case CollectableType.StrengthEvolution1:
                return prefabShelterStrengthFeedback;
            case CollectableType.GhostEvolution1:
                return prefabShelterGhostFeedback;
            default:
                return null;
        }
    }
}
