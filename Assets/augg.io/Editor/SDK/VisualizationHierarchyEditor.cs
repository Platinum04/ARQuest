#if UNITY_EDITOR
using System.Linq;
using Auggio.Plugin.SDK.Model;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.SDK
{
    [CustomEditor(typeof(VisualizationHierarchy))]
    internal class VisualizationHierarchyEditor : UnityEditor.Editor
    {
        private SerializedProperty visualizeMeshProp;
        private SerializedProperty materialProp;

        private void OnEnable()
        {
            visualizeMeshProp = serializedObject.FindProperty("visualizeMesh");
            materialProp = serializedObject.FindProperty("material");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            VisualizationHierarchy visualizationHierarchy = (VisualizationHierarchy) target;
            
            if (visualizationHierarchy.GetComponentsInChildren<AuggioLocation>(true)
                .All(location => location.VisualizeMesh))
            {
                visualizationHierarchy.VisualizeMesh = true;
            }
            else
            {
                visualizationHierarchy.VisualizeMesh = false;
            }
            
            EditorGUILayout.PropertyField(materialProp);
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                AuggioAnchor[] auggioAnchors = visualizationHierarchy.GetComponentsInChildren<AuggioAnchor>();
                foreach (AuggioAnchor anchor in auggioAnchors)
                {
                    anchor.MeshMaterial = visualizationHierarchy.Material;
                    if (anchor.VisualizeMesh)
                    {
                        Renderer[] renderers = anchor.GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderer in renderers)
                        {
                            renderer.material = visualizationHierarchy.Material;
                        }
                    }
                }
            }


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(visualizeMeshProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (visualizeMeshProp.boolValue)
                {
                    if (!VisualizeAllMeshes(visualizationHierarchy))
                    {
                        visualizationHierarchy.VisualizeMesh = false;
                        visualizeMeshProp.boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                else
                {
                    HideAllMeshes(visualizationHierarchy);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        private bool VisualizeAllMeshes(VisualizationHierarchy visualizationHierarchy)
        {
            visualizationHierarchy.VisualizeMesh = true;
            bool visualizeResult = true;
            foreach (AuggioLocation location in visualizationHierarchy.transform.GetComponentsInChildren<AuggioLocation>(true))
            {
                visualizeResult = visualizeResult && AuggioLocationEditor.VisualizeLocationMeshes(location);
            }

            if (!visualizeResult)
            {
                //some of the locations are cannot be visualized
                visualizationHierarchy.VisualizeMesh = false;
                return false;
            }
            return true;
        }
        
        private void HideAllMeshes(VisualizationHierarchy visualizationHierarchy)
        {
            visualizationHierarchy.VisualizeMesh = false;
            foreach (AuggioLocation location in visualizationHierarchy.transform.GetComponentsInChildren<AuggioLocation>(true))
            {
                AuggioLocationEditor.HideLocationMeshes(location);
            }
        }
    }
    
   
}

#endif