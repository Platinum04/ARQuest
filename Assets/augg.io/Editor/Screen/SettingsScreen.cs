#if UNITY_EDITOR
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.Screen.Controller;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Screen
{
    internal class SettingsScreen : EditorScreen<SettingsScreenController>
    {

        internal SettingsScreen(AuggioEditorPlugin rootWindow, SettingsScreenController screenController, bool drawBackButton, bool useFocusLogic) : base(rootWindow, screenController, drawBackButton, useFocusLogic)
        {
        }

        protected override void Initialize()
        {
        }

        internal override void OnBecameActive()
        {
        }

        internal override void DrawOnLoadingScreen()
        {
        }

        internal override void DrawScreen()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Settings", AuggioEditorStyles.Instance.H1);
            GUIUtils.DrawLineSeparator();
            DrawMeshMaterialSelector();
            DrawVisualizeMeshCheckbox();
            GUILayout.EndVertical();
        }

        private void DrawMeshMaterialSelector()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Default Mesh Material on Import", AuggioEditorStyles.Instance.NormalText, GUILayout.Width(250));
            GUI.SetNextControlName("MaterialField");
            
            // Draw the material field
            Object selectedObject = EditorGUILayout.ObjectField(rootWindow.DefaultImportMaterial, typeof(Material), false);

            if (selectedObject == null)
            {
                rootWindow.SetDefaultImportMaterial();
            }
            else
            {
                if (selectedObject.GetType() != typeof(Material))
                {
                    rootWindow.SetDefaultImportMaterial();
                }
                else
                {
                    Material material = (Material) selectedObject;
                    if (IsBuiltInMaterial(material))
                    {
                        Debug.LogWarning("Built in materials are not supported. Setting default material instead.");
                        rootWindow.SetDefaultImportMaterial();
                    }
                    else
                    {
                        rootWindow.DefaultImportMaterial = (Material)selectedObject;
                    }
                }
            }
            
            GUILayout.EndHorizontal();
        }

        private void DrawVisualizeMeshCheckbox()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Show mesh on experience import", AuggioEditorStyles.Instance.NormalText, GUILayout.Width(250));
            rootWindow.VisualizeMeshOnImport = EditorGUILayout.Toggle(rootWindow.VisualizeMeshOnImport);
            GUILayout.EndHorizontal();
        }
        
        private bool IsBuiltInMaterial(Material material)
        {
            // Check if the material is built-in by comparing its asset path
            string path = AssetDatabase.GetAssetPath(material);
            return path.StartsWith("Assets/") == false;
        }


       
    }
}
#endif
