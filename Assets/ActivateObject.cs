using UnityEngine;


public class ActivateObject : MonoBehaviour
{
    public GameObject cartToMove;
    public Cinemachine.CinemachineVirtualCameraBase switchToCam;
    public Cinemachine.CinemachinePathBase.PositionUnits positionUnits = Cinemachine.CinemachinePathBase.PositionUnits.Distance;
    public float speed = 5f;

    private Cinemachine.CinemachineDollyCart dCartComp;
    void Start()
    {
        if (cartToMove)
        {
            dCartComp = cartToMove.GetComponent<Cinemachine.CinemachineDollyCart>();
            dCartComp.m_Speed = 0f;    
        }
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && switchToCam && cartToMove)
        {
            if (Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera.Name != switchToCam.Name)
            {
                dCartComp.m_PositionUnits = positionUnits;
                dCartComp.m_Speed = speed;

                switchToCam.VirtualCameraGameObject.SetActive(false);
                switchToCam.VirtualCameraGameObject.SetActive(true);

                switchToCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = col.transform;


            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player") && switchToCam != null && cartToMove)
        {
            dCartComp.m_Speed = 0f;
            switchToCam.VirtualCameraGameObject.SetActive(false);
        }
    }
}
