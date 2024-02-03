﻿using System.Collections.Generic;
using UnityEngine;

using RSkoi_MeshDeformer.Component;

namespace RSkoi_MeshDeformer
{
    public partial class MeshDeformer
    {
        public void SetupTargetObjectForDeformation(GameObject go)
        {
            List<Transform> targetTransforms = GetAllMeshTransforms(go.transform);
            foreach (Transform t in targetTransforms)
            {
                GameObject targetGO = t.gameObject;
                if (targetGO == null)
                    continue;

                if (GameObjectIsTargetInput(targetGO))
                    continue;

                SetupCollider(targetGO);

                targetGO.AddComponent<MeshDeformerTarget>();

                AddTrackedTarget(targetGO);

                logger.LogMessage($"Target object {t.name} setup complete");
            }
        }

        public void SetupInputObjectForDeformation(GameObject go)
        {
            List<Transform> targetTransforms = GetAllMeshTransforms(go.transform);
            foreach (Transform t in targetTransforms)
            {
                GameObject targetGO = t.gameObject;
                if (targetGO == null)
                    continue;

                if (GameObjectIsTargetInput(targetGO))
                    continue;

                SetupCollider(targetGO, true);

                if (targetGO.GetComponent<Rigidbody>() == null)
                {
                    Rigidbody targetRB = targetGO.AddComponent<Rigidbody>();
                    targetRB.mass = 5.0f;
                    targetRB.useGravity = false;
                    targetRB.constraints = RigidbodyConstraints.FreezeAll;
                }

                targetGO.AddComponent<MeshDeformerInput>();

                AddTrackedInput(targetGO);

                logger.LogMessage($"Input object {t.name} setup complete");
            }
        }

        public static List<Transform> GetAllMeshTransforms(Transform root)
        {
            List<Transform> result = [];

            // GetComponentsInChildren also gets parent/root
            var childrenMeshes = root.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in childrenMeshes)
                result.Add(mesh.gameObject.transform);

            var childrenSkinnedMeshes = root.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer mesh in childrenSkinnedMeshes)
                result.Add(mesh.gameObject.transform);

            return result;
        }

        public static bool GameObjectIsTargetInput(GameObject target)
        {
            return target.GetComponent<MeshDeformerTarget>() != null
                || target.GetComponent<MeshDeformerInput>() != null;
        }

        private static void SetupCollider(GameObject targetGO, bool isInput = false)
        {
            Mesh mesh = null;
            MeshRenderer meshRenderer = targetGO.GetComponent<MeshRenderer>();
            SkinnedMeshRenderer meshSkinnedRenderer = targetGO.GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                MeshFilter meshFilter = targetGO.GetComponent<MeshFilter>() ?? targetGO.AddComponent<MeshFilter>();
                mesh = meshFilter.sharedMesh;
            }
            else if (meshSkinnedRenderer != null)
                mesh = meshSkinnedRenderer.sharedMesh;

            Collider collider = targetGO.GetComponent<Collider>();
            MeshCollider mCollider = null;
            if (collider == null)
                mCollider = targetGO.AddComponent<MeshCollider>();
            else if (collider is MeshCollider meshCollider)
                mCollider = meshCollider;

            mCollider.enabled = false;
            if (isInput)
            {
                mCollider.inflateMesh = true;
                mCollider.skinWidth = 0.05f;
                mCollider.convex = true;
            }
            mCollider.sharedMesh = mesh;
            mCollider.enabled = true;
        }
    }
}