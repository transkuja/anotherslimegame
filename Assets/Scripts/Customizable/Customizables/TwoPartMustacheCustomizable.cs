using UnityEngine;

public class TwoPartMustacheCustomizable : MonoBehaviour, ICustomizable {

    public void Init(PlayerCharacter _player)
    {
        transform.GetChild(0).GetChild(0).GetComponent<HingeJoint>().connectedBody = _player.Rb;
        transform.GetChild(0).GetChild(1).GetComponent<HingeJoint>().connectedBody = _player.Rb;
    }

    public void Init(Rigidbody _rbToAttachTo)
    {
        transform.GetChild(0).GetChild(0).GetComponent<HingeJoint>().connectedBody = _rbToAttachTo;
        transform.GetChild(0).GetChild(1).GetComponent<HingeJoint>().connectedBody = _rbToAttachTo;
    }
}
