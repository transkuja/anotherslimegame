using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour {

	public float springForce = 20f;
	public float damping = 5f;

	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices;
	Vector3[] vertexVelocities;
    float originalMediumHeight;
    float uniformScale = 1f;

	void Start () {
		deformingMesh = GetComponent<MeshFilter>().mesh;
		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
        originalMediumHeight = 0f;
        for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices[i] = originalVertices[i];
            originalMediumHeight += originalVertices[i].y;
	        }
        originalMediumHeight /= originalVertices.Length;
        //originalMediumHeight = (originalHighestHeight + originalLowestHeight) / 2.0f;
		vertexVelocities = new Vector3[originalVertices.Length];
	}
	
	void FixedUpdate () {
		uniformScale = transform.localScale.x;
		for (int i = 0; i < displacedVertices.Length; i++) {
            if(vertexVelocities[i].magnitude > 0.01f)
			    UpdateVertex(i);
		}
        UpdateMeshHeight();
        deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
	}

	void UpdateVertex (int i) {
		Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        displacement *= uniformScale;
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
		velocity = Vector3.ClampMagnitude(velocity, 20.0f);
        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
	}
	
	public void AddDeformingForce (Vector3 point, float force) {
		point = transform.InverseTransformPoint(point);
		for (int i = 0; i < displacedVertices.Length; i++) {
			AddForceToVertex(i, point, force);
		}
	}

	void AddForceToVertex (int i, Vector3 point, float force) {
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