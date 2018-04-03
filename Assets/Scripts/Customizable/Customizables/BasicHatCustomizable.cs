using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHatCustomizable : MonoBehaviour, ICustomizable {

    public void Init(PlayerCharacter _player)
    {
        transform.GetChild(0).GetChild(0).GetComponent<HingeJoint>().connectedBody = _player.Rb;
    }

    public void Init(Rigidbody _rbToAttachTo)
    {
        transform.GetChild(0).GetChild(0).GetComponent<HingeJoint>().connectedBody = _rbToAttachTo;
    }
}
