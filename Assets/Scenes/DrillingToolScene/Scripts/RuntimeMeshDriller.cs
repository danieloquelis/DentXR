using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class RuntimeMeshDriller : MonoBehaviour
{
    public Transform drillTool;
    public float drillRadius = 0.3f;
    public float drillStrength = 0.1f;
    public AnimationCurve falloffCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Mesh deformingMesh;
    private Vector3[] displacedVertices;
    private MeshCollider meshCollider;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        deformingMesh = Instantiate(meshFilter.mesh); // Clone mesh for runtime modification
        meshFilter.mesh = deformingMesh;

        displacedVertices = deformingMesh.vertices;

        meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = deformingMesh;
    }

    void Update()
    {
        if (!drillTool) return;

        Vector3 drillLocal = transform.InverseTransformPoint(drillTool.position);

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            float distance = Vector3.Distance(displacedVertices[i], drillLocal);
            if (distance < drillRadius)
            {
                float falloff = falloffCurve.Evaluate(distance / drillRadius);

                // Instead of using normals, push toward the drill center
                Vector3 directionToTool = (drillLocal - displacedVertices[i]).normalized;
                Vector3 push = directionToTool * drillStrength * falloff;

                displacedVertices[i] += push;
            }
        }

        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
        deformingMesh.RecalculateBounds();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = deformingMesh;
    }
}