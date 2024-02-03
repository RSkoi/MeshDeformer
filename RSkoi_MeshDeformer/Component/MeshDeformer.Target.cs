using UnityEngine;
using static UnityEngine.UI.Image;

namespace RSkoi_MeshDeformer.Component
{
    public class MeshDeformerTarget : MonoBehaviour
    {
        public Mesh deformingMesh;
        public Vector3[] originalVertices, displacedVertices;
        public Vector3[] vertexVelocities;
        public float springForce = 20f;
        public float damping = 5f;
        public float uniformScale = 1f;
        public Collider collider;
        //public LineRenderer line;

        private void Start()
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
            //line = gameObject.AddComponent<LineRenderer>();
        }

        public void Update()
        {
            /*if (collider is not MeshCollider meshCollider)
                return;
            // in local space
            //Bounds meshBounds = deformer.deformingMesh.bounds;

            // collider center, extents
            // draw box

            line.startWidth = 0.015f;
            line.endWidth = 0.015f;
            line.useWorldSpace = false;
            //line.SetPositions([contact.point, contact.normal * contact.separation]);
            line.SetPositions(vertices);*/
        }

        public void FixedUpdate()
        {
            uniformScale = transform.localScale.x;

            for (int i = 0; i < displacedVertices.Length; i++)
                UpdateVertex(i);
            deformingMesh.vertices = displacedVertices;
            deformingMesh.RecalculateNormals();
        }

        public void UpdateVertex(int i)
        {
            Vector3 velocity = vertexVelocities[i];
            Vector3 displacement = displacedVertices[i] - originalVertices[i];
            displacement *= uniformScale;
            velocity -= displacement * springForce * Time.deltaTime;
            velocity *= 1f - damping * Time.deltaTime;
            vertexVelocities[i] = velocity;
            displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
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
                pointToVertex *= uniformScale;
                float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
                float velocity = attenuatedForce * Time.deltaTime;
                vertexVelocities[i] += pointToVertex.normalized * velocity;
            }
        }
    }
}
