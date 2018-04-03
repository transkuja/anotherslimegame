using UnityEngine;

public class BasicCustomizable : MonoBehaviour, ICustomizable {

    public void Init(PlayerCharacter _player)
    {}

    public void Init(Rigidbody _rbToAttachTo)
    {}
}
