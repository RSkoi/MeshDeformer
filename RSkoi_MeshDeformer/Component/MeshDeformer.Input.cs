using Studio;
using UnityEngine;

namespace RSkoi_MeshDeformer.Component
{
    public class MeshDeformerInput : MonoBehaviour
    {
        public float force = 0.02f;
        public float forceOffset = 0.2f;
        public float distance = 0.03f;
        public Collision collision;
        //public LineRenderer line;

        public void Start()
        {
            /*line = gameObject.AddComponent<LineRenderer>();
            line.startWidth = 0.015f;
            line.endWidth = 0.015f;
            line.useWorldSpace = false;*/
        }

        public void OnCollisionStay(Collision collision)
        {
            if (!MeshDeformer._instance.IsTrackedTarget(collision.contacts[0].otherCollider.gameObject))
                return;
            
            // this is only relevant if the input is disabled
            if (!MeshDeformer._instance.IsTrackedInput(gameObject))
                return;

            /*ContactPoint contactPoint = collision.contacts[0];
            Collider collider = contactPoint.otherCollider;
            Vector3 center = collider.bounds.center;
            Vector3 size = collider.bounds.size;
            Vector3 min = collider.bounds.min;
            Vector3 max = collider.bounds.max;
            MeshDeformer.logger.LogMessage($"collider info: center {center}, size {size}, min {min}, max {max}");*/

            //MeshDeformer.logger.LogMessage("[" + collision.contacts.Length.ToString() + "] collision with " + collision.contacts[0].otherCollider.gameObject.name);

            foreach (ContactPoint contact in collision.contacts)
            {
                //line.SetPositions([contact.point, contact.normal * contact.separation]);

                MeshDeformerTarget deformer = contact.otherCollider.GetComponent<MeshDeformerTarget>();
                if (deformer)
                {
                    Vector3 point = contact.point;
                    point += contact.normal * forceOffset;
                    deformer.AddDeformingForce(point, transform.position, force, distance);
                }
            }
        }
    }
}
