using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraBehaviour { SmallArea, VSmallArea, ShowAbovePlatforms, Default }
public class CameraTrigger : MonoBehaviour {
    public CameraBehaviour behaviour;
    public Transform vSmallAreaStandbyTransform;

}
