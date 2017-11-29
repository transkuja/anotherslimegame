using System.Collections;
using UnityEngine;


public class MeshComputeShader : MonoBehaviour {
    public ComputeShader shader;
    public float scale = 10.0f;
    public float speed = 1.0f;

    private Mesh m;

    int kernel;
    Vector3 vertex;
    Vector3[] vertices;
    ComputeBuffer buffer;
    private Vector3[] output;
    private Vector3[] data;


    void RunShader()
    {
     
        buffer = new ComputeBuffer(data.Length, 12);
        buffer.SetData(data);
        shader.SetFloat("time", Time.time);
        shader.SetFloat("speed", speed);
        shader.SetFloat("scale", scale);
        shader.SetInt("size", data.Length);

        kernel = shader.FindKernel("CSMain");
        shader.SetBuffer(kernel, "dataBuffer", buffer);
        shader.Dispatch(kernel, data.Length, 1, 1);
        buffer.GetData(output);

        vertices = new Vector3[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            vertex = data[i];
            vertex.y += output[i].y;
            vertex.x += output[i].x;
            vertices[i] = vertex;
        }

        m.vertices = vertices;
        m.RecalculateNormals();
        buffer.Dispose();
        buffer = null;
    }

	// Use this for initialization
	void Start () {
        m = GetComponent<MeshFilter>().mesh;
        data = m.vertices;
        output = new Vector3[data.Length];
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        RunShader();
	}
}
