using System.Collections.Generic;
using UnityEngine;
using Studio;
using MessagePack;
using ExtensibleSaveFormat;
using KKAPI.Utilities;
using KKAPI.Studio.SaveLoad;

using RSkoi_MeshDeformer.UI;
using RSkoi_MeshDeformer.Component;
using static RSkoi_MeshDeformer.MeshDeformer;
using static RSkoi_MeshDeformer.Scene.MeshDeformerSerializableObjects;

namespace RSkoi_MeshDeformer.Scene
{
    internal class MeshDeformerSceneBehaviour : SceneCustomFunctionController
    {
        private const string TARGET_DICT_NAME = "RSkoi.MeshDeformer.trackedTargets";
        private const string INPUT_DICT_NAME = "RSkoi.MeshDeformer.trackedInputs";

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            if (operation == SceneOperationKind.Clear || operation == SceneOperationKind.Load)
                _instance.ClearTracker();

            var data = GetExtendedData();
            if (data == null)
                return;
            if (operation == SceneOperationKind.Clear)
                return;

            LoadTargets(data, loadedItems);
            LoadInputs(data, loadedItems);
        }

        protected override void OnSceneSave()
        {
            var data = new PluginData();

            SaveTargets(data);
            SaveInputs(data);

            SetExtendedData(data);
        }

        protected override void OnObjectsCopied(ReadOnlyDictionary<int, ObjectCtrlInfo> copiedItems)
        {
            // TODO: find out whether this needs to be implemented for targets, yes for inputs
            // shared mesh deformation could make this irrelevant, as changes to the mesh persist across items/scenes
        }

        protected override void OnObjectDeleted(ObjectCtrlInfo objectCtrlInfo)
        {
            GameObject obj = objectCtrlInfo.guideObject.transformTarget.gameObject;

            MeshDeformerTarget[] targets = obj.GetComponentsInChildren<MeshDeformerTarget>();
            MeshDeformerInput[] inputs = obj.GetComponentsInChildren<MeshDeformerInput>();

            _instance.RemoveTrackedTargets(targets);
            _instance.RemoveTrackedInputs(inputs);

            base.OnObjectDeleted(objectCtrlInfo);
        }

        protected override void OnObjectVisibilityToggled(ObjectCtrlInfo objectCtrlInfo, bool visible)
        {
            base.OnObjectVisibilityToggled(objectCtrlInfo, visible);

            if (!CheckForVisibility.Value)
                return;

            GameObject obj = objectCtrlInfo.guideObject.transformTarget.gameObject;

            MeshDeformerTarget[] targets = obj.GetComponentsInChildren<MeshDeformerTarget>();
            MeshDeformerInput[] inputs = obj.GetComponentsInChildren<MeshDeformerInput>();

            if (visible)
            {
                _instance.EnableTrackedTargets(targets);
                _instance.EnableTrackedInputs(inputs);
            }
            else
            {
                _instance.DisableTrackedTargets(targets);
                _instance.DisableTrackedInputs(inputs);
            }
        }

        protected override void OnObjectsSelected(List<ObjectCtrlInfo> objectCtrlInfo)
        {
            if (MeshDeformerUI.uiCanvas.activeSelf)
                MeshDeformerUI.Repopulate();

            base.OnObjectsSelected(objectCtrlInfo);
        }

        private void SaveTargets(PluginData data)
        {
            Dictionary<GameObject, TrackerData> dict = [];
            foreach (var entry in _instance.trackedTargets)
                dict.Add(entry.Key, entry.Value);
            foreach (var entry in _instance.disabledTrackedTargets)
                dict.Add(entry.Key, entry.Value);

            SaveDict(data, TARGET_DICT_NAME, _instance.trackedTargets);
        }

        private void SaveInputs(PluginData data)
        {
            Dictionary<GameObject, TrackerData> dict = [];
            foreach (var entry in _instance.trackedInputs)
                dict.Add(entry.Key, entry.Value);
            foreach (var entry in _instance.disabledTrackedInputs)
                dict.Add(entry.Key, entry.Value);

            SaveDict(data, INPUT_DICT_NAME, _instance.trackedInputs);
        }

        private void SaveDict(PluginData data, string name, Dictionary<GameObject, TrackerData> dict)
        {
            bool isTarget = name.Equals(TARGET_DICT_NAME);
            SortedDictionary<int, List<TrackerDataContainer>> savedDict = [];
            foreach (var entry in dict)
            {
                int key = entry.Value.objInfo.objectInfo.dicKey;
                TrackerDataContainer container = isTarget ? new(
                        key,
                        entry.Key.transform.name,
                        SaveItemIsDisabled(name, entry.Key),
                        entry.Value.target.options) : new(
                        key,
                        entry.Key.transform.name,
                        SaveItemIsDisabled(name, entry.Key),
                        entry.Value.input.options);

                if (savedDict.ContainsKey(key))
                    savedDict[key].Add(container);
                else
                    savedDict.Add(key, [container]);
            }
            data.data.Add(name, MessagePackSerializer.Serialize(savedDict));
        }

        private bool SaveItemIsDisabled(string name, GameObject go)
        {
            if (name.Equals(TARGET_DICT_NAME))
                return _instance.disabledTrackedTargets.ContainsKey(go);
            else if (name.Equals(INPUT_DICT_NAME))
                return _instance.disabledTrackedInputs.ContainsKey(go);

            return false;
        }

        private void LoadTargets(PluginData data, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            LoadDict(data, loadedItems, TARGET_DICT_NAME);
        }

        private void LoadInputs(PluginData data, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            LoadDict(data, loadedItems, INPUT_DICT_NAME);
        }

        private void LoadDict(PluginData data, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems, string name)
        {
            if (data.data.TryGetValue(name, out var dict) && dict != null)
            {
                bool isTarget = name.Equals(TARGET_DICT_NAME);
                SortedDictionary<int, List<TrackerDataContainer>> deserializedTrackerDataDict
                    = MessagePackSerializer.Deserialize<SortedDictionary<int, List<TrackerDataContainer>>>((byte[])dict);

                foreach (var item in loadedItems)
                {
                    if (!deserializedTrackerDataDict.ContainsKey(item.Key))
                        continue;

                    GameObject rootItemGO = item.Value.guideObject.transformTarget.gameObject;
                    if (isTarget)
                        _instance.SetupTargetObjectForDeformation(rootItemGO, item.Value);
                    else
                        _instance.SetupInputObjectForDeformation(rootItemGO, item.Value);

                    foreach (TrackerDataContainer trackerData in deserializedTrackerDataDict[item.Key])
                    {
                        // TODO: options are not loaded in, something in here is broken

                        Transform transform = rootItemGO.transform.Find(trackerData.rendererTransformName);
                        if (transform == null)
                            continue;

                        if (isTarget)
                        {
                            transform.GetComponent<MeshDeformerTarget>().SetOptions(trackerData.targetOptions);
                            if (trackerData.isDisabled)
                                _instance.DisableTrackedTarget(transform.gameObject);
                        }
                        else
                        {
                            transform.GetComponent<MeshDeformerInput>().SetOptions(trackerData.inputOptions);

                            if (trackerData.isDisabled)
                                _instance.DisableTrackedInput(transform.gameObject);
                        }
                    }
                }    
            }
        }
    }
}
