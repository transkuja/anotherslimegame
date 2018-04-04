using UnityEngine;

public class UICameraApdater : MonoBehaviour {

    public int PlayerIndex = -1 ;
    private Transform refCamera;

    public float speed = 12.0f;
	
	// Update is called once per frame
	void Update () {
        if (refCamera != null)
        {
            Quaternion newRotation = Quaternion.LookRotation(-(refCamera.transform.position - transform.position), Vector3.up);

            // Bon on est pas obligé de faire Slerp
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newRotation.eulerAngles.x, newRotation.eulerAngles.y, 0), Time.deltaTime * speed);
        }
        else if (PlayerIndex != -1)
        {
            refCamera = GameManager.Instance.PlayerStart.PlayersReference[PlayerIndex].GetComponent<Player>().cameraReference.transform.GetChild(0).transform;
        }

    }
}
