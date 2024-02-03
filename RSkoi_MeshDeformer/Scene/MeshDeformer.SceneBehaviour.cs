using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using RSkoi_MeshDeformer.UI;
using MessagePack;
using Studio;
using System.Collections.Generic;
using ExtensibleSaveFormat;
using UnityEngine;

using RSkoi_MeshDeformer.Component;

namespace RSkoi_MeshDeformer.Scene
{
    internal class MeshDeformerSceneBehaviour : SceneCustomFunctionController
    {
        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            if (operation == SceneOperationKind.Clear || operation == SceneOperationKind.Load)
                MeshDeformer._instance.ClearTracker();

            var data = GetExtendedData();
            if (data == null)
                return;
            if (operation == SceneOperationKind.Clear)
                return;

            // TODO: get targets and inputs
        }

        protected override void OnSceneSave()
        {
            //var data = new PluginData();

            // TODO: save targets and inputs and their properties

            //SetExtendedData(data);
        }

        protected override void OnObjectsCopied(ReadOnlyDictionary<int, ObjectCtrlInfo> copiedItems)
        {
            // TODO: find out whether this needs to be implemented
            // shared mesh deformation could make this irrelevant, as changes to the mesh persist across items/scenes
        }

        protected override void OnObjectDeleted(ObjectCtrlInfo objectCtrlInfo)
        {
            GameObject obj = objectCtrlInfo.guideObject.transformTarget.gameObject;

            MeshDeformerTarget[] targets = obj.GetComponentsInChildren<MeshDeformerTarget>();
            MeshDeformerInput[] inputs = obj.GetComponentsInChildren<MeshDeformerInput>();

            MeshDeformer._instance.RemoveTrackedTargets(targets);
            MeshDeformer._instance.RemoveTrackedInputs(inputs);

            base.OnObjectDeleted(objectCtrlInfo);
        }

        protected override void OnObjectVisibilityToggled(ObjectCtrlInfo objectCtrlInfo, bool visible)
        {
            base.OnObjectVisibilityToggled(objectCtrlInfo, visible);

            if (!MeshDeformer.CheckForVisibility.Value)
                return;

            GameObject obj = objectCtrlInfo.guideObject.transformTarget.gameObject;

            MeshDeformerTarget[] targets = obj.GetComponentsInChildren<MeshDeformerTarget>();
            MeshDeformerInput[] inputs = obj.GetComponentsInChildren<MeshDeformerInput>();

            if (visible)
            {
                MeshDeformer._instance.DisableTrackedTargets(targets);
                MeshDeformer._instance.DisableTrackedInputs(inputs);
            }
            else
            {
                MeshDeformer._instance.EnableTrackedTargets(targets);
                MeshDeformer._instance.EnableTrackedInputs(inputs);
            }
        }

        protected override void OnObjectsSelected(List<ObjectCtrlInfo> objectCtrlInfo)
        {
            if (MeshDeformerUI.uiCanvas.activeSelf)
                MeshDeformerUI.Repopulate();

            base.OnObjectsSelected(objectCtrlInfo);
        }
    }
}
