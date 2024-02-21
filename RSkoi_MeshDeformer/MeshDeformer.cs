using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using Studio;
using KKAPI.Studio;
using KKAPI.Utilities;

using RSkoi_MeshDeformer.UI;
using KKAPI.Studio.SaveLoad;
using RSkoi_MeshDeformer.Scene;

namespace RSkoi_MeshDeformer
{
    [BepInProcess("CharaStudio.exe")]
    [BepInDependency(KKAPI.KoikatuAPI.GUID, KKAPI.KoikatuAPI.VersionConst)]
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public partial class MeshDeformer : BaseUnityPlugin
    {
        internal const string PLUGIN_GUID = "RSkoi_MeshDeformer";
        internal const string PLUGIN_NAME = "RSkoi_MeshDeformer";
        internal const string PLUGIN_VERSION = "1.0.0";

        internal static MeshDeformer _instance;

        internal static ConfigEntry<float> UiScale { get; private set; }
        internal static ConfigEntry<bool> CheckForVisibility { get; private set; }
        internal static ConfigEntry<KeyboardShortcut> ToggleUI { get; private set; }
        internal static ConfigEntry<KeyboardShortcut> AddTargetComponentToItems { get; private set; }
        internal static ConfigEntry<KeyboardShortcut> AddInputComponentToItems { get; private set; }

        public static ManualLogSource logger;

        private void Awake()
        {
            _instance = this;

            logger = Logger;

            SetupConfig();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += new UnityAction<UnityEngine.SceneManagement.Scene, LoadSceneMode>(LoadedEvent);
            StudioSaveLoadApi.RegisterExtraBehaviour<MeshDeformerSceneBehaviour>(PLUGIN_GUID);
        }

        private void Update()
        {
            if (ToggleUI.Value.IsDown())
                MeshDeformerUI.ToggleWindow();
            else if (AddTargetComponentToItems.Value.IsDown())
                SetupSelectedForDeformation(true);
            else if (AddInputComponentToItems.Value.IsDown())
                SetupSelectedForDeformation(false);
        }

        private void SetupConfig()
        {
            UiScale = Config.Bind(
                "Config",
                "UI scale",
                1.0f,
                new ConfigDescription("Scales the UI to given factor. Open and reopen MeshDeformer window to see the change.",
                null,
                new ConfigurationManagerAttributes { Order = 1 }));

            CheckForVisibility = Config.Bind(
                "Config",
                "Check for item visibility",
                false,
                new ConfigDescription("Whether MeshDeformer should check for workspace item visibility changes and disable/enable input " +
                    "and target objects when the item is turned off/on. Disabled by default for performance reasons.",
                null,
                new ConfigurationManagerAttributes { Order = 2 }));

            ToggleUI = Config.Bind(
                "Keyboard Shortcuts",
                "Toggle UI",
                new KeyboardShortcut(KeyCode.M, KeyCode.RightControl),
                new ConfigDescription("Toggle the UI of MeshDeformer.",
                null,
                new ConfigurationManagerAttributes { Order = 3 }));

            AddTargetComponentToItems = Config.Bind(
                "Keyboard Shortcuts",
                "Enable mesh deformer target on items",
                new KeyboardShortcut(KeyCode.T, KeyCode.RightControl),
                new ConfigDescription("Adds the mesh deformer target component to the selected items' meshes.",
                null,
                new ConfigurationManagerAttributes { Order = 4 }));

            AddInputComponentToItems = Config.Bind(
                "Keyboard Shortcuts",
                "Enable mesh deformer input on items",
                new KeyboardShortcut(KeyCode.I, KeyCode.RightControl),
                new ConfigDescription("Adds the mesh deformer input component to the selected items' meshes.",
                null,
                new ConfigurationManagerAttributes { Order = 5 }));
        }

        private void SetupSelectedForDeformation(bool target)
        {
            var selectedItems = StudioAPI.GetSelectedObjects();
            foreach (ObjectCtrlInfo obj in selectedItems)
            {
                GameObject objGO = obj.guideObject.transformTarget.gameObject;
                if (target)
                    SetupTargetObjectForDeformation(objGO, obj);
                else
                    SetupInputObjectForDeformation(objGO, obj);
            }
        }

        private void LoadedEvent(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadMode)
        {
            if (scene.buildIndex != 1)
                return;

            MeshDeformerUI.Init();
        }
    }
}
