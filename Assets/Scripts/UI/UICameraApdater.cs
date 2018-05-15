using UnityEngine;

public class UICameraApdater : MonoBehaviour {

    public int PlayerIndex = -1 ;
    private Transform refCamera;

    public float speed = 12.0f;
	
	// Update is called once per frame
	void Update () {
        if (refCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(refCamera.transform.forward);
        }
        else if (PlayerIndex != -1)
        {
            refCamera = GameManager.Instance.PlayerStart.PlayersReference[PlayerIndex].GetComponent<Player>().cameraReference.transform.GetChild(0).transform;
        }

    }
}
