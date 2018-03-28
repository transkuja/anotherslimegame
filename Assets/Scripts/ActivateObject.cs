using UnityEngine;


public class ActivateObject : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCameraBase switchToCam;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && switchToCam)
        {
            if (Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera.Name != switchToCam.Name)
            {
                //dCartComp.m_Speed = speed;

                switchToCam.VirtualCameraGameObject.SetActive(false);
                switchToCam.VirtualCameraGameObject.SetActive(true);

                switchToCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = col.transform;
                switchToCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = col.transform;


            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player") && switchToCam != null)
        {
            switchToCam.VirtualCameraGameObject.SetActive(false);
        }
    }
}
