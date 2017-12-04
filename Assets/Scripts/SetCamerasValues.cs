using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// Script à attacher à l'objet "Cameras" parent de "Player1", "Player2"... Eux-mêmes parents d'une caméra et d'un composant cinémachine.
// Les modifications se font dès ajout du script. Il faut donc le retirer de suite après (Ou le laisser pour ne plus avoir à faire la manip mais il se lancera souvent pour rien)

[ExecuteInEditMode]
public class SetCamerasValues : MonoBehaviour {
#if UNITY_EDITOR

    CinemachineFreeLook[] cameras;
    CinemachineCollider[] colliders;
    [SerializeField]
    bool ReApply = true;
    void Start()
    {
        ReApply = true;
    }

    private void Update()
    {
        if (ReApply)
        {
            SetValues();
            ReApply = false;
        }
    }

    public void SetValues()
    {
        for (int i = 0; i < transform.childCount; i++)
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
                cam.m_Orbits[0].m_Height = 9.0f;
                cam.m_Orbits[0].m_Radius = 5.0f;

                cam.m_Orbits[1].m_Height = 5.0f;
                cam.m_Orbits[1].m_Radius = 6.5f;

                cam.m_Orbits[2].m_Height = 1.0f;
                cam.m_Orbits[2].m_Radius = 5.0f;

                //Lens
                cam.GetRig(0).m_Lens.FieldOfView = 95.0f;

                cam.GetRig(1).m_Lens.FieldOfView = 87.5f;

                cam.GetRig(2).m_Lens.FieldOfView = 80.0f;

                //Aim
                CinemachineComposer cp;
                cp = ((cam.GetRig(0).GetCinemachineComponent<CinemachineComposer>()));
                cp.m_TrackedObjectOffset = new Vector3(0f, -2.0f, 0f);
                cp.m_ScreenX = .5f;
                cp.m_ScreenY = .55f;

                cp = ((cam.GetRig(1).GetCinemachineComponent<CinemachineComposer>()));
                cp.m_TrackedObjectOffset = new Vector3(0f, -1.0f, 0f);
                cp.m_ScreenX = .5f;
                cp.m_ScreenY = .55f;

                cp = ((cam.GetRig(2).GetCinemachineComponent<CinemachineComposer>()));
                cp.m_TrackedObjectOffset = new Vector3(0f, -0.15f, 0f);
                cp.m_ScreenX = .5f;
                cp.m_ScreenY = .6f;

                //Body
                CinemachineTransposer tr;
                tr = ((cam.GetRig(0).GetCinemachineComponent<CinemachineTransposer>()));
                tr.m_XDamping = 0.0f;
                tr.m_YDamping = 0.0f;
                tr.m_ZDamping = 0.0f;

                tr = ((cam.GetRig(1).GetCinemachineComponent<CinemachineTransposer>()));
                tr.m_XDamping = 0.0f;
                tr.m_YDamping = 0.0f;
                tr.m_ZDamping = 0.0f;

                tr = ((cam.GetRig(2).GetCinemachineComponent<CinemachineTransposer>()));
                tr.m_XDamping = 0.0f;
                tr.m_YDamping = 0.0f;
                tr.m_ZDamping = 0.0f;
            }
            colliders = transform.GetChild(i).GetComponentsInChildren<CinemachineCollider>();
            foreach (CinemachineCollider col in colliders)
            {
                col.m_MinimumDistanceFromTarget = .91f;
                col.m_Strategy = CinemachineCollider.ResolutionStrategy.PullCameraForward;
                col.m_Damping = .4f;
            }
        }
        Debug.Log("Camera values set");
    }

#endif
}
