#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.SDK.Model;
using Auggio.Utils.Serialization.Model;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor
{
    [Serializable]
    public class AuggioEditorPlugin : EditorWindow
    {
        internal const string SERVER_URL = "https://api.augg.io";

        [SerializeField] private string defaultImportMaterialGuid;
        [SerializeField] private bool visualizeMeshOnImport;
        [SerializeField] private string serverUrl = SERVER_URL;
        
        private Material defaultImportMaterial;
        private EditorWindowController controller;
        private readonly List<string> errors = new List<string>();
        private string auggioAPIKey;
       
        private string _selectedOrganizationId;

        [MenuItem("augg.io/Editor Plugin")]
        public static void Init()
        {
            AuggioEditorPlugin window =
                (AuggioEditorPlugin) GetWindow(typeof(AuggioEditorPlugin), false, "[augg.io] Unity Editor Plugin");
            window.minSize = new Vector2(800, 800);
            window.Show();
        }
        
        [MenuItem("augg.io/Remove current API key")]
        static void ClearKey()
        {
            APIKeyManager.RemoveCurrentKey();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            AuggioEditorStyles.Instance.Initialize(this); 
        }
        
        private void EditorSceneManagerOnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            AuggioEditorStyles.Instance.Initialize(this);
            controller.CurrentScreen.OnSceneOpened(scene);
        }

        private void OnEnable()
        {
            string data = EditorPrefs.GetString("AuggioEditorPlugin", JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);

            if (string.IsNullOrEmpty(auggioAPIKey) && APIKeyManager.GetActiveKey(out string apiKey))
            {
                auggioAPIKey = apiKey;
            }

            if (defaultImportMaterialGuid != null)
            {
                string path = AssetDatabase.GUIDToAssetPath(defaultImportMaterialGuid);
                defaultImportMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
            }

            if (defaultImportMaterial == null )
            {
                SetDefaultImportMaterial();
            }
            
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorSceneManager.sceneOpened += EditorSceneManagerOnSceneOpened;
            AuggioEditorStyles.Instance.Initialize(this); 
            controller = new EditorWindowController(this);
        }

        private void OnDisable()
        {
            string data = JsonUtility.ToJson(this, false);
            APIKeyManager.SetActiveKey(auggioAPIKey);
            EditorPrefs.SetString("AuggioEditorPlugin", data);
            AssetDatabase.SaveAssets();
        }

        public void SetDefaultImportMaterial()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string materialFolderPath = scriptPath.Substring(0, scriptPath.LastIndexOf('/') + 1);
            defaultImportMaterial = (Material) AssetDatabase.LoadAssetAtPath(materialFolderPath + "DefaultMeshImportMaterial.mat", typeof(Material));
        }

        public static void ShowErrorDialog(string message)
        {
            EditorUtility.DisplayDialog("Error", message, "OK");
        }

        private void OnGUI()
        {
            if (controller == null)
            {
                return;
            }

            if (!AuggioEditorStyles.Instance.Initialized)
            {
                AuggioEditorStyles.Instance.Initialize(this); 
            }
            GUILayout.BeginVertical();
            controller.CurrentScreen.Draw();
            GUILayout.FlexibleSpace();
            DrawErrors();
            GUILayout.EndVertical();
        }

        private void OnFocus()
        {
            if (controller == null)
            {
                return;
            }
            controller.CurrentScreen.OnFocus();
            controller.SetWindowFocused(true);
        }

        private void OnLostFocus()
        {
            if (controller == null)
            {
                return;
            }
            controller.CurrentScreen.OnLostFocus();
            controller.SetWindowFocused(false);
        }

        private void DrawErrors()
        {
            if (errors.Count > 0)
            {
                foreach (string message in errors)
                {
                    EditorGUILayout.HelpBox(message, MessageType.Error);
                }
            }
        }
        
        public string AuggioAPIKey
        {
            get => auggioAPIKey;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    APIKeyManager.RemoveCurrentKey();
                }
                auggioAPIKey = value;
            }
        }

        public string ServerUrl
        {
            get => serverUrl;
            set   {
                if (string.IsNullOrEmpty(value))
                {
                    value = "https://api.augg.io";
                }
                serverUrl = value;
            }
        }

        public Material DefaultImportMaterial
        {
            get => defaultImportMaterial;
            set
            {
                defaultImportMaterialGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                defaultImportMaterial = value;
            }
        }

        public bool VisualizeMeshOnImport
        {
            get => visualizeMeshOnImport;
            set => visualizeMeshOnImport = value;
        }

        public string SelectedOrganizationId
        {
            get => _selectedOrganizationId;
            set => _selectedOrganizationId = value;
        }

        public List<string> Errors => errors;

    }

}


#endif