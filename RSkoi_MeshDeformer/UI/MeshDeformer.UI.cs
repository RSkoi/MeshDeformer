using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Studio;
using KKAPI.Studio;

namespace RSkoi_MeshDeformer.UI
{
    internal static class MeshDeformerUI
    {
        internal static GameObject _meshDeformerUIPrefab;
        internal static GameObject _meshRendererEntryPrefab;
        internal static List<MeshDeformerEntry> _meshRendererEntryPool = [];

        internal static List<MeshDeformerEntry> _selectedEntries = [];

        public static GameObject uiCanvas;
        public static Transform uiContainer;
        public static ScrollRect meshEntryScrollRect;

        public static void Init()
        {
            LoadUIResources();
            InstantiateUI();
            
            // TODO: add all event handlers for input/target buttons, entry toggles
            // TODO: edit options
                // default value "---" if multiple entries selected with different values, cannot edit field
                // edit options across multiple entries if value is the same
            // TODO: make input/target buttons not interactable if no item selected in workspace
        }

        public static void ToggleWindow()
        {
            if (uiCanvas.activeSelf)
            {
                SetEntriesInactive();
                HideWindow();
            }
            else
            {
                ShowWindow();
                PopulateMenu();
            }
        }

        public static void Repopulate()
        {
            SetEntriesInactive();
            PopulateMenu();
        }

        #region private
        private static void ShowWindow()
        {
            uiCanvas.gameObject.SetActive(true);
            float uiScale = MeshDeformer.UiScale.Value;
            uiContainer.localScale = new(uiScale, uiScale, uiScale);
        }

        private static void HideWindow()
        {
            uiCanvas.gameObject.SetActive(false);
        }

        private static void PopulateMenu()
        {
            List<ObjectCtrlInfo> selectedObjects = StudioAPI.GetSelectedObjects().ToList();
            List<Transform> meshTransforms = [];
            foreach (ObjectCtrlInfo obj in selectedObjects)
                meshTransforms.AddRange(MeshDeformer.GetAllMeshTransforms(obj.guideObject.transformTarget));

            if (meshTransforms.Count > meshEntryScrollRect.content.transform.childCount)
                Populate(meshTransforms.Count - meshEntryScrollRect.content.transform.childCount);

            for (int i = 0; i < meshTransforms.Count; i++)
                ShowEntry(meshTransforms[i], i);
        }

        private static void Populate(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject entryGO = GameObject.Instantiate(_meshRendererEntryPrefab, meshEntryScrollRect.content.transform, false);
                entryGO.transform.SetAsLastSibling();
                Toggle toggle = entryGO.transform.Find("EntryToggle").GetComponent<Toggle>();
                Text label = toggle.transform.Find("EntryLabel").GetComponent<Text>();
                _meshRendererEntryPool.Add(new(entryGO, toggle, label));
            }
        }

        private static void ShowEntry(Transform meshTransform, int poolIndex)
        {
            MeshDeformerEntry entry = _meshRendererEntryPool[poolIndex];
            entry.label.text = meshTransform.name;
            entry.target = meshTransform;
            entry.container.SetActive(true);
        }

        private static void SetEntriesInactive()
        {
            _meshRendererEntryPool.ForEach(entry => entry.container.SetActive(false));
        }

        private static void LoadUIResources()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RSkoi_MeshDeformer.Resources.meshdeformer.unity3d");
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            _meshDeformerUIPrefab = AssetBundle.LoadFromMemory(buffer).LoadAsset<GameObject>("MeshDeformerCanvas");
            _meshRendererEntryPrefab = AssetBundle.LoadFromMemory(buffer).LoadAsset<GameObject>("MeshDeformerRendererEntry");
            stream.Close();
        }

        private static void InstantiateUI()
        {
            uiCanvas = GameObject.Instantiate(_meshDeformerUIPrefab);
            uiCanvas.SetActive(false);
            uiContainer = uiCanvas.transform.Find("MeshDeformerContainer");
            meshEntryScrollRect = uiContainer.Find("MeshEntryScrollView").GetComponent<ScrollRect>();

            MeshDeformerDraggable draggable = 
                uiContainer.Find("LabelDragPanel").gameObject.AddComponent<MeshDeformerDraggable>();
            draggable.target = uiContainer.GetComponent<RectTransform>();
        }
        #endregion

        #region helper
        public static void PrintPool()
        {
            MeshDeformer.logger.LogMessage("MeshDeformer.UI pool ------------");
            foreach (MeshDeformerEntry e in _meshRendererEntryPool)
                MeshDeformer.logger.LogMessage(e.ToString());
            MeshDeformer.logger.LogMessage("MeshDeformer.UI pool ------------");
        }
        #endregion
    }
}
