using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformingMesh : MonoBehaviour {

    Material mat;
    //[SerializeField ] Shader DeformingShader;
	// Use this for initialization

    class DeformingPoint
    {
        public Vector3 pos;
        public float force;
        public float timer;
        public DeformingPoint(Vector3 _pos,float _force)
        {
            pos = _pos;
            force = _force;
            timer = 0;
        }
    }
    //DeformingPoint deformingPoint;
    List<DeformingPoint> deformingPointList;

    void Start () {
        this.mat = GetComponent<MeshRenderer>().material;
        deformingPointList = new List<DeformingPoint>();

    }
    void AddForceAtPoint(Vector3 point, float force)
    {
        DeformingPoint deformingPoint = new DeformingPoint(point, force);
        deformingPointList.Add(deformingPoint);
        if (deformingPointList.Count > 5)
            deformingPointList.Remove(deformingPointList[0]);
    }
    void UpdateShader()
    {
        Vector4[] posTab = new Vector4[5];
        float[] forceTab = new float[5];
        float[] timerTab = new float[5];
        for (int i = 0;i < deformingPointList.Count;i++)
        {
            DeformingPoint deformingPoint = deformingPointList[i];
            posTab[i] = deformingPoint.pos;
            forceTab[i] = deformingPoint.force;
            timerTab[i] = deformingPoint.timer;
            deformingPoint.timer += Time.deltaTime;
        }
        mat.SetVectorArray("_ImpactPos", posTab);
        mat.SetFloatArray("_ImpactForce", forceTab);
        mat.SetFloatArray("_ImpactTimer", timerTab);
    }
      
	
	// Update is called once per frame
	void Update () {

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit) && Input.GetMouseButtonDown(0))
        {
            DeformingMesh deformer = hit.collider.GetComponent<DeformingMesh>();
            deformer.AddForceAtPoint(hit.point, 1);
        }
        UpdateShader();
    }
}
