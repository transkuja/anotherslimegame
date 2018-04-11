using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakyChiefBehavior : MonoBehaviour {

    int currentStep = 0; // TODO: save in DB
    // Reward list

    // Next transform
    [SerializeField]
    Transform[] nextTransforms;

    // next transform need something to be broken
    bool[] nextIsABreakable = { false, false, true };

}
