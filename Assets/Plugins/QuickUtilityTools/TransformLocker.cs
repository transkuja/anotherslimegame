using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class TransformLocker : MonoBehaviour {
    Vector3 Pos;
    Quaternion Rot;
    Vector3 Scale;

	void Start () {
        Pos = transform.position;
        Rot = transform.rotation;
        Scale = transform.localScale;
	}

	void Update () {
        transform.position = Pos;
        transform.rotation = Rot;
        transform.localScale = Scale;
	}
}
