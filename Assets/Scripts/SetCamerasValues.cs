using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// Script à attacher à l'objet "Cameras" parent de "Player1", "Player2"... Eux-mêmes parents d'une caméra et d'un composant cinémachine.
// Les modifications se font dès ajout du script. Il faut donc le retirer de suite après (Ou le laisser pour ne plus avoir à faire la manip mais il se lancera souvent pour rien)

[ExecuteInEditMode]
public class SetCamerasValues : MonoBehaviour {
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
            SetCamerasValues();
            ReApply = false;
        }
    }

    public void SetCamerasValues()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            cameras = transform.GetChild(i).GetComponentsInChildren<CinemachineFreeLook>();

            foreach (CinemachineFreeLook cam in cameras)
            {
                //Axis Control
                cam.m_YAxis.m_MaxSpeed = 30;
                cam.m_YAxis.m_AccelTime = 3;
                cam.m_YAxis.m_DecelTime = .5f;

                cam.m_XAxis.m_MaxSpeed = 500;
                cam.m_XAxis.m_AccelTime = .15f;
                cam.m_XAxis.m_DecelTime = .15f;

                //Orbits
                cam.m_Orbits[0].m_Height = 4.5f;
                cam.m_Orbits[0].m_Radius = 6.0f;

                cam.m_Orbits[1].m_Height = 2.5f;
                cam.m_Orbits[1].m_Radius = 7.0f;

                cam.m_Orbits[2].m_Height = 0.4f;
                cam.m_Orbits[2].m_Radius = 6.0f;

                //Lens
                LensSettings lens = cam.GetRig(0).m_Lens;
                lens.FieldOfView = 85f;

                lens = cam.GetRig(1).m_Lens;
                lens.FieldOfView = 85f;

                lens = cam.GetRig(2).m_Lens;
                lens.FieldOfView = 80f;

                //Aim
                CinemachineComposer cp;
                cp = ((CinemachineComposer)(cam.GetRig(0).GetCinemachineComponent(CinemachineCore.Stage.Aim)));
                cp.m_TrackedObjectOffset = new Vector3(0f, .5f, 0f);
                cp.m_ScreenX = .5f;
                cp.m_ScreenY = .55f;

                cp = ((CinemachineComposer)(cam.GetRig(1).GetCinemachineComponent(CinemachineCore.Stage.Aim)));
                cp.m_TrackedObjectOffset = new Vector3(0f, .5f, 0f);
                cp.m_ScreenX = .5f;
                cp.m_ScreenY = .55f;

                cp = ((CinemachineComposer)(cam.GetRig(2).GetCinemachineComponent(CinemachineCore.Stage.Aim)));
                cp.m_TrackedObjectOffset = new Vector3(0f, .5f, 0f);
                cp.m_ScreenX = .5f;
                cp.m_ScreenY = .6f;

                //Body
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
                col.m_MinimumDistanceFromTarget = .91f;
                col.m_Strategy = CinemachineCollider.ResolutionStrategy.PullCameraForward;
                col.m_Damping = .4f;
            }
        }
        Debug.Log("Camera damping values set");
    }

}
