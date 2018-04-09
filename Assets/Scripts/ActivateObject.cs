using UnityEngine;


public class ActivateObject : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCameraBase switchToCam;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && switchToCam)
        {

            col.GetComponent<Player>().cameraReference.transform.GetChild(1).gameObject.SetActive(false);
            switchToCam.VirtualCameraGameObject.SetActive(true);

            switchToCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = col.transform;
            //switchToCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = col.transform;

        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player") && switchToCam != null)
        {
            switchToCam.VirtualCameraGameObject.SetActive(false);
            col.GetComponent<Player>().cameraReference.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
