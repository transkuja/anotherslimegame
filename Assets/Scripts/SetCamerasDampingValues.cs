using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// Script à attacher à l'objet "Cameras" parent de "Player1", "Player2"... Eux-mêmes parents d'une caméra et d'un composant cinémachine.
// Les modifications se font dès ajout du script. Il faut donc le retirer de suite après (Ou le laisser pour ne plus avoir à faire la manip mais il se lancera souvent pour rien)

[ExecuteInEditMode]
public class SetCamerasDampingValues : MonoBehaviour {
    CinemachineFreeLook[] cameras;
    CinemachineCollider[] colliders;
    [SerializeField]
    bool ReApply = true;
	void Start () {
        ReApply = true;
    }

    private void Update()
    {
        if(ReApply)
        {
            Sync();
            ReApply = false;
        }
    }

    void Sync()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            cameras = transform.GetChild(i).GetComponentsInChildren<CinemachineFreeLook>();
            foreach (CinemachineFreeLook cam in cameras)
            {
                CinemachineTransposer tr;
                tr = ((CinemachineTransposer)(cam.GetRig(0).GetCinemachineComponent(CinemachineCore.Stage.Body)));
                tr.m_XDamping = .2f;
                tr.m_YDamping = .2f;
                tr.m_ZDamping = .2f;

                tr = ((CinemachineTransposer)(cam.GetRig(1).GetCinemachineComponent(CinemachineCore.Stage.Body)));
                tr.m_XDamping = .2f;
                tr.m_YDamping = .2f;
                tr.m_ZDamping = .2f;

                tr = ((CinemachineTransposer)(cam.GetRig(2).GetCinemachineComponent(CinemachineCore.Stage.Body)));
                tr.m_XDamping = .2f;
                tr.m_YDamping = .2f;
                tr.m_ZDamping = .2f;
            }
            colliders = transform.GetChild(i).GetComponentsInChildren<CinemachineCollider>();
            foreach (CinemachineCollider col in colliders)
            {
                col.m_Damping = .2f;
            }
        }
        Debug.Log("Camera damping values set");
    }

}
