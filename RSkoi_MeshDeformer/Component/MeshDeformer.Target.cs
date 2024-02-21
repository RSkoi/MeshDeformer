using UnityEngine;

using static RSkoi_MeshDeformer.Scene.MeshDeformerSerializableObjects;

namespace RSkoi_MeshDeformer.Component
{
    public class MeshDeformerTarget : MonoBehaviour
    {
        public Mesh deformingMesh;
        public Vector3[] originalVertices, displacedVertices;
        public Vector3[] vertexVelocities;
        public Collider collider;
        public readonly MeshDeformerTargetOptions options = new();

        public void Start()
        {
            if (GetComponent<MeshFilter>() != null)
                deformingMesh = GetComponent<MeshFilter>().mesh;
            else
                deformingMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            originalVertices = deformingMesh.vertices;
            displacedVertices = new Vector3[originalVertices.Length];
            for (int i = 0; i < originalVertices.Length; i++)
                displacedVertices[i] = originalVertices[i];
            vertexVelocities = new Vector3[originalVertices.Length];
            collider = GetComponent<Collider>();
        }

        public void SetOptions(MeshDeformerTargetOptions options)
        {
            this.options.springForce = options.springForce;
            this.options.damping = options.damping;
            this.options.uniformScale = options.uniformScale;
        }

        public void FixedUpdate()
        {
            options.uniformScale = transform.localScale.x;

            for (int i = 0; i < displacedVertices.Length; i++)
                UpdateVertex(i);
            deformingMesh.vertices = displacedVertices;
            deformingMesh.RecalculateNormals();
        }

        public void UpdateVertex(int i)
        {
            Vector3 velocity = vertexVelocities[i];
            Vector3 displacement = displacedVertices[i] - originalVertices[i];
            displacement *= options.uniformScale;
            velocity -= displacement * options.springForce * Time.deltaTime;
            velocity *= 1f - options.damping * Time.deltaTime;
            vertexVelocities[i] = velocity;
            displacedVertices[i] += velocity * (Time.deltaTime / options.uniformScale);
        }

        public void AddDeformingForce(Vector3 point, Vector3 origin, float force, float distance)
        {
            point = transform.InverseTransformPoint(point);

            for (int i = 0; i < displacedVertices.Length; i++)
                AddForceToVertex(i, point, origin, force, distance);
        }

        public void AddForceToVertex(int i, Vector3 point, Vector3 origin, float force, float distance)
        {
            if (Vector3.Distance(transform.TransformPoint(originalVertices[i]), origin) <= distance)
            {
                Vector3 pointToVertex = displacedVertices[i] - point;
                pointToVertex *= options.uniformScale;
                float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
                float velocity = attenuatedForce * Time.deltaTime;
                vertexVelocities[i] += pointToVertex.normalized * velocity;
            }
        }
    }
}
