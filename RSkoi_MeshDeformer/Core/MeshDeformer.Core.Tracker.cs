using RSkoi_MeshDeformer.Component;
using System.Collections.Generic;
using UnityEngine;

namespace RSkoi_MeshDeformer
{
    public partial class MeshDeformer
    {
        private readonly HashSet<GameObject> _trackedTargets = [];
        private readonly HashSet<GameObject> _disabledTrackedTargets = [];

        private readonly HashSet<GameObject> _trackedInputs = [];
        private readonly HashSet<GameObject> _disabledTrackedInputs = [];

        public void ClearTracker()
        {
            _trackedTargets.Clear();
            _disabledTrackedTargets.Clear();
            _trackedInputs.Clear();
            _disabledTrackedInputs.Clear();
        }

        #region target
        public bool IsTrackedTarget(GameObject obj)
        {
            return _trackedTargets.Contains(obj);
        }

        public void AddTrackedTarget(GameObject obj)
        {
            _trackedTargets.Add(obj);
        }

        public void RemoveTrackedTarget(GameObject obj)
        { 
            _trackedTargets.Remove(obj);
            _disabledTrackedTargets.Remove(obj);
        }

        public void RemoveTrackedTargets(MeshDeformerTarget[] targets)
        {
            foreach (MeshDeformerTarget target in targets)
                RemoveTrackedTarget(target.gameObject);
        }

        public void DisableTrackedTarget(GameObject obj)
        {
            if (!IsTrackedTarget(obj))
                return;

            _disabledTrackedTargets.Add(obj);
            _trackedTargets.Remove(obj);
        }

        public void DisableTrackedTargets(MeshDeformerTarget[] targets)
        {
            foreach (MeshDeformerTarget target in targets)
                DisableTrackedTarget(target.gameObject);
        }

        public void EnableTrackedTarget(GameObject obj)
        {
            if (!_disabledTrackedTargets.Contains(obj))
                return;

            _trackedTargets.Add(obj);
            _disabledTrackedTargets.Remove(obj);
        }

        public void EnableTrackedTargets(MeshDeformerTarget[] targets)
        {
            foreach (MeshDeformerTarget target in targets)
                EnableTrackedTarget(target.gameObject);
        }
        #endregion

        #region input
        public bool IsTrackedInput(GameObject obj)
        {
            return _trackedInputs.Contains(obj);
        }

        public void AddTrackedInput(GameObject obj)
        {
            _trackedInputs.Add(obj);
        }

        public void RemoveTrackedInput(GameObject obj)
        {
            _trackedInputs.Remove(obj);
            _disabledTrackedInputs.Remove(obj);
        }

        public void RemoveTrackedInputs(MeshDeformerInput[] inputs)
        {
            foreach (MeshDeformerInput input in inputs)
                RemoveTrackedInput(input.gameObject);
        }

        public void DisableTrackedInput(GameObject obj)
        {
            if (!IsTrackedTarget(obj))
                return;

            _disabledTrackedInputs.Add(obj);
            _trackedInputs.Remove(obj);
        }

        public void DisableTrackedInputs(MeshDeformerInput[] inputs)
        {
            foreach (MeshDeformerInput input in inputs)
                DisableTrackedInput(input.gameObject);
        }

        public void EnableTrackedInput(GameObject obj)
        {
            if (!_disabledTrackedInputs.Contains(obj))
                return;

            _trackedInputs.Add(obj);
            _disabledTrackedInputs.Remove(obj);
        }

        public void EnableTrackedInputs(MeshDeformerInput[] inputs)
        {
            foreach (MeshDeformerInput input in inputs)
                EnableTrackedInput(input.gameObject);
        }
        #endregion
    }
}
