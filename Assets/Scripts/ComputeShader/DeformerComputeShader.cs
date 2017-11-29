using System.Collections;
using UnityEngine;

public class DeformerComputeShader : MonoBehaviour {
    public ComputeShader shader;
    public float springForce = 20f;
    public float damping = 5f;

    private Mesh m;

    int kernel;
    Vector3 vertex;
    Vector3[] vertices;
    ComputeBuffer buffer;
    ComputeBuffer buffer2;
    ComputeBuffer buffer3;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;
    private Vector3[] vertexVelocities;

    float originalMediumHeight;
    float uniformScale = 1f;

    void RunShader()
    {
        buffer = new ComputeBuffer(originalVertices.Length, 12);
        buffer.SetData(originalVertices);

        buffer2 = new ComputeBuffer(originalVertices.Length, 12);
        buffer2.SetData(displacedVertices);

        buffer3 = new ComputeBuffer(originalVertices.Length, 12);
        buffer3.SetData(vertexVelocities);


        uniformScale = transform.localScale.x;

        shader.SetFloat("time", Time.time);

        shader.SetFloat("uniformScale", uniformScale);
        shader.SetFloat("springForce", springForce);
        shader.SetFloat("damping", damping);

        shader.SetInt("size", originalVertices.Length);

        kernel = shader.FindKernel("Deformer");
        shader.SetBuffer(kernel, "originalVertices", buffer);
        shader.SetBuffer(kernel, "displacedVertices", buffer2);
        shader.SetBuffer(kernel, "vertexVelocities", buffer3);


        // Toute la puissance est la 603 thread envoyé sur le GPU
        shader.Dispatch(kernel, originalVertices.Length, 1, 1);
        buffer2.GetData(displacedVertices);
        buffer3.GetData(vertexVelocities);

        UpdateMeshHeight();


        vertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            vertex = originalVertices[i];
            vertex.y += displacedVertices[i].y;
            vertex.x += displacedVertices[i].x;
            vertices[i] = vertex;
        }
        //UpdateMeshHeight();
        m.vertices = vertices;
        m.RecalculateNormals();


        buffer.Dispose();
        buffer = null;
        buffer2.Dispose();
        buffer2 = null;
        buffer3.Dispose();
        buffer3 = null;
    }

    // Use this for initialization
    void Start()
    {
        m = GetComponent<MeshFilter>().mesh;
        originalVertices = m.vertices;
        vertexVelocities = new Vector3[originalVertices.Length];
        displacedVertices = new Vector3[originalVertices.Length];

        originalMediumHeight = 0f;
        for (int i = 0; i < originalVertices.Length; i++)
        {
            //displacedVertices[i] = originalVertices[i];
            originalMediumHeight += originalVertices[i].y;
        }
        originalMediumHeight /= originalVertices.Length;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (vertexVelocities[i].magnitude > 0.01f)
            RunShader();
    }

    public void AddDeformingForce(Vector3 point, float force)
    {
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }

    void UpdateMeshHeight()
    {
        float mediumHeight = 0f;
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            mediumHeight += displacedVertices[i].y;
        }
        mediumHeight /= displacedVertices.Length;
        float heightDisp = originalMediumHeight - mediumHeight;
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            displacedVertices[i].y = displacedVertices[i].y + heightDisp;
        }
    }

}
