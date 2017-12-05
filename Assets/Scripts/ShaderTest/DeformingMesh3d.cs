using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformingMesh3d : MonoBehaviour {

    Material mat;
    //[SerializeField ] Shader DeformingShader;
	// Use this for initialization

    class DeformingPoint
    {
        public Vector3 pos;
        public float force;
        public float timer;
        public Vector3 normal;
        public DeformingPoint(Vector3 _pos,float _force,Vector3 _normal)
        {
            pos = _pos;
            force = _force;
            timer = 0;
            normal = _normal;
        }
    }
    //DeformingPoint deformingPoint;
    List<DeformingPoint> deformingPointList;

    void Start () {
        this.mat = GetComponent<MeshRenderer>().material;
        deformingPointList = new List<DeformingPoint>();

    }
    void AddForceAtPoint(Vector3 point, float force,Vector3 normal)
    {
        DeformingPoint deformingPoint = new DeformingPoint(point, force,normal);
        deformingPointList.Add(deformingPoint);
        if (deformingPointList.Count > 5)
            deformingPointList.Remove(deformingPointList[0]);
    }
    void UpdateShader()
    {
        Vector4[] posTab = new Vector4[5];
        float[] forceTab = new float[5];
        float[] timerTab = new float[5];
        Vector4[] normalTab = new Vector4[5];
        for (int i = 0;i < deformingPointList.Count;i++)
        {
            DeformingPoint deformingPoint = deformingPointList[i];
            posTab[i] = deformingPoint.pos;
            forceTab[i] = deformingPoint.force;
            timerTab[i] = deformingPoint.timer;
            normalTab[i] = deformingPoint.normal;
            deformingPoint.timer += Time.deltaTime;
        }
        mat.SetVectorArray("_ImpactPos", posTab);
        mat.SetFloatArray("_ImpactForce", forceTab);
        mat.SetFloatArray("_ImpactTimer", timerTab);
        mat.SetVectorArray("_ImpactNormal", normalTab);
    }
      
	
	// Update is called once per frame
	void Update () {

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit) && Input.GetMouseButtonDown(0))
        {
            DeformingMesh3d deformer = hit.collider.GetComponent<DeformingMesh3d>();
            deformer.AddForceAtPoint(hit.point, 1,hit.normal);
        }
        UpdateShader();
    }
}
