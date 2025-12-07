#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.SDK
{
    [CustomEditor(typeof(AuggioLocation))]
    internal class AuggioLocationEditor : UnityEditor.Editor
    {
        private SerializedProperty visualizeMeshProp;

        private void OnEnable()
        {
            visualizeMeshProp = serializedObject.FindProperty("visualizeMesh");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            AuggioLocation auggioLocation = (AuggioLocation) target;
            
            if (auggioLocation.GetComponentsInChildren<AuggioAnchor>(true).All(anchor => anchor.VisualizeMesh))
            {
                auggioLocation.VisualizeMesh = true;
            }
            else
            {
                auggioLocation.VisualizeMesh = false;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(visualizeMeshProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (visualizeMeshProp.boolValue)
                {
                    if (!VisualizeLocationMeshes(auggioLocation))
                    {
                        auggioLocation.VisualizeMesh = false;
                        visualizeMeshProp.boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                else
                {
                    HideLocationMeshes(auggioLocation);
                }
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            GUILayout.Space(20);
            GUILayout.Label("Additional tools");
            if (GUILayout.Button("Find location mesh folder in Project Structure"))
            {
               FindInProjectStructure(auggioLocation);
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);
            GUILayout.Label("Location ID: " + auggioLocation.LocationId);
            serializedObject.ApplyModifiedProperties();
        }
        
        public static bool VisualizeLocationMeshes(AuggioLocation location)
        {
            location.VisualizeMesh = true;
            bool visualizationFound = true;
            foreach (AuggioAnchor anchor in location.transform.GetComponentsInChildren<AuggioAnchor>(true))
            {
                anchor.VisualizeMesh = true;
                visualizationFound = visualizationFound && AuggioAnchorEditor.VisualizeAnchorMesh(anchor);
            }

            if (!visualizationFound)
            {
                //some of the anchors were not found and cannot be visualized
                location.VisualizeMesh = false;
                return false;
            }
            return true;
        }
        
        public static void HideLocationMeshes(AuggioLocation location)
        {
            foreach (AuggioAnchor anchor in location.transform.GetComponentsInChildren<AuggioAnchor>(true))
            {
                anchor.VisualizeMesh = false;
                AuggioAnchorEditor.HideAnchorMesh(anchor);
            }
            location.VisualizeMesh = false;
        }
        
        public static bool IsValid(AuggioLocation location)
        {
            return !string.IsNullOrEmpty(location.LocationId) && !string.IsNullOrEmpty(location.OrganizationId) &&
                   !string.IsNullOrEmpty(location.ExperienceId);
        }

        private void FindInProjectStructure(AuggioLocation auggioLocation)
        {
            string path = Path.Combine(AuggioUtils.GetMeshDataPath(true), auggioLocation.OrganizationId,
                auggioLocation.LocationId);
            if (!Directory.Exists(path))
            {
                throw new Exception("Cannot find mesh folder for location in Project Structure");
            }
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }
    }
}
#endif