using System.Collections.Generic;
using UnityEngine;
using Studio;

using RSkoi_MeshDeformer.Component;

namespace RSkoi_MeshDeformer
{
    public partial class MeshDeformer
    {
        public readonly Dictionary<GameObject, TrackerData> trackedTargets = [];
        public readonly Dictionary<GameObject, TrackerData> disabledTrackedTargets = [];

        public readonly Dictionary<GameObject, TrackerData> trackedInputs = [];
        public readonly Dictionary<GameObject, TrackerData> disabledTrackedInputs = [];

        public void ClearTracker()
        {
            trackedTargets.Clear();
            disabledTrackedTargets.Clear();
            trackedInputs.Clear();
            disabledTrackedInputs.Clear();
        }

        #region target
        public TrackerData GetTrackedTargetData(GameObject key)
        {
            return trackedTargets[key];
        }

        public bool IsTrackedTarget(GameObject obj)
        {
            return trackedTargets.ContainsKey(obj);
        }

        public void AddTrackedTarget(TrackerData data)
        {
            trackedTargets.Add(data.obj, data);
        }

        public void RemoveTrackedTarget(GameObject obj)
        { 
            trackedTargets.Remove(obj);
            disabledTrackedTargets.Remove(obj);
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

            TrackerData data = trackedTargets[obj];
            disabledTrackedTargets.Add(data.obj, data);
            trackedTargets.Remove(obj);
        }

        public void DisableTrackedTargets(MeshDeformerTarget[] targets)
        {
            foreach (MeshDeformerTarget target in targets)
                DisableTrackedTarget(target.gameObject);
        }

        public void EnableTrackedTarget(GameObject obj)
        {
            if (!disabledTrackedTargets.ContainsKey(obj))
                return;

            TrackerData data = disabledTrackedTargets[obj];
            trackedTargets.Add(data.obj, data);
            disabledTrackedTargets.Remove(obj);
        }

        public void EnableTrackedTargets(MeshDeformerTarget[] targets)
        {
            foreach (MeshDeformerTarget target in targets)
                EnableTrackedTarget(target.gameObject);
        }
        #endregion

        #region input
        public TrackerData GetTrackedInputData(GameObject key)
        {
            return trackedInputs[key];
        }

        public bool IsTrackedInput(GameObject obj)
        {
            return trackedInputs.ContainsKey(obj);
        }

        public void AddTrackedInput(TrackerData data)
        {
            trackedInputs.Add(data.obj, data);
        }

        public void RemoveTrackedInput(GameObject obj)
        {
            trackedInputs.Remove(obj);
            disabledTrackedInputs.Remove(obj);
        }

        public void RemoveTrackedInputs(MeshDeformerInput[] inputs)
        {
            foreach (MeshDeformerInput input in inputs)
                RemoveTrackedInput(input.gameObject);
        }

        public void DisableTrackedInput(GameObject obj)
        {
            if (!IsTrackedInput(obj))
                return;

            TrackerData data = trackedInputs[obj];
            disabledTrackedInputs.Add(data.obj, data);
            trackedInputs.Remove(obj);
        }

        public void DisableTrackedInputs(MeshDeformerInput[] inputs)
        {
            foreach (MeshDeformerInput input in inputs)
                DisableTrackedInput(input.gameObject);
        }

        public void EnableTrackedInput(GameObject obj)
        {
            if (!disabledTrackedInputs.ContainsKey(obj))
                return;

            TrackerData data = disabledTrackedInputs[obj];
            trackedInputs.Add(data.obj, data);
            disabledTrackedInputs.Remove(obj);
        }

        public void EnableTrackedInputs(MeshDeformerInput[] inputs)
        {
            foreach (MeshDeformerInput input in inputs)
                EnableTrackedInput(input.gameObject);
        }
        #endregion

        public class TrackerData
        {
            public GameObject obj;
            public ObjectCtrlInfo objInfo;
            public MeshDeformerInput input;
            public MeshDeformerTarget target;

            public TrackerData(GameObject obj, ObjectCtrlInfo objInfo, MeshDeformerInput input)
            {
                this.obj = obj;
                this.objInfo = objInfo;
                this.input = input;
            }

            public TrackerData(GameObject obj, ObjectCtrlInfo objInfo, MeshDeformerTarget target)
            {
                this.obj = obj;
                this.objInfo = objInfo;
                this.target = target;
            }
        }
    }
}
