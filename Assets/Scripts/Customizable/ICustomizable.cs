using UnityEngine;

public interface ICustomizable {
	void Init(PlayerCharacter _player);
    void Init(Rigidbody _rbToAttachTo);
}
