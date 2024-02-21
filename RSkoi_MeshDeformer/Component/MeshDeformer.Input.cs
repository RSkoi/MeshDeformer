using UnityEngine;

using static RSkoi_MeshDeformer.Scene.MeshDeformerSerializableObjects;

namespace RSkoi_MeshDeformer.Component
{
    public class MeshDeformerInput : MonoBehaviour
    {
        public Collision collision;
        public readonly MeshDeformerInputOptions options = new();

        public void SetOptions(MeshDeformerInputOptions options)
        {
            this.options.force = options.force;
            this.options.forceOffset = options.forceOffset;
            this.options.distance = options.distance;
        }

        public void OnCollisionStay(Collision collision)
        {
            GameObject otherGameObject = collision.contacts[0].otherCollider.gameObject;
            if (!MeshDeformer._instance.IsTrackedTarget(otherGameObject))
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
                MeshDeformerTarget deformer = MeshDeformer._instance.GetTrackedTargetData(otherGameObject).target;

                Vector3 point = contact.point;
                point += contact.normal * options.forceOffset;
                deformer.AddDeformingForce(point, transform.position, options.force, options.distance);
            }
        }
    }
}
