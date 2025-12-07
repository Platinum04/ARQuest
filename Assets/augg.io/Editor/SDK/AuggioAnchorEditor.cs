#if UNITY_EDITOR
using System;
using System.IO;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization;
using Auggio.Utils.Serialization.Model;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.SDK
{
    [CustomEditor(typeof(AuggioAnchor))]
    internal class AuggioAnchorEditor : UnityEditor.Editor
    {
        private SerializedProperty visualizeMeshProp;
        private SerializedProperty materialProp;

        private void OnEnable()
        {
            visualizeMeshProp = serializedObject.FindProperty("visualizeMesh");
            materialProp = serializedObject.FindProperty("meshMaterial");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            AuggioAnchor auggioAnchor = (AuggioAnchor) target;
         
            
            EditorGUILayout.PropertyField(materialProp);
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                if (auggioAnchor.VisualizeMesh)
                {
                    Renderer[] renderers = auggioAnchor.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.material = auggioAnchor.MeshMaterial;
                    }
                }
            }

            // Check if visualizeMesh has changed
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(visualizeMeshProp);
            if (EditorGUI.EndChangeCheck())
            {
                // Handle changes to visualizeMesh here
                if (visualizeMeshProp.boolValue)
                {
                    if (!VisualizeAnchorMesh(auggioAnchor))
                    {
                        auggioAnchor.VisualizeMesh = false;
                        visualizeMeshProp.boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                else
                {
                    HideAnchorMesh(auggioAnchor);
                }
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            GUILayout.Space(20);
            GUILayout.Label("Actions");
            if (GUILayout.Button("Add new augg.io object tied to this anchor"))
            {
                CreateAuggioObjectWindow window = CreateAuggioObjectWindow.GetNewWindowInstance();
                window.SetExperienceId(auggioAnchor.ExperienceId);
                window.SetAnchorId(auggioAnchor.AnchorId);
            }
            
            GUILayout.Space(20);
            GUILayout.Label("Additional tools");
            if (GUILayout.Button("Find anchor mesh data in Project Structure"))
            {
                FindInProjectStructure(auggioAnchor);
            }
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(20);
            GUILayout.Label("Anchor ID: " + auggioAnchor.AnchorId);
            serializedObject.ApplyModifiedProperties();
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Pickable)]
        static void OnDrawGizmo(AuggioAnchor auggioAnchor, GizmoType gizmoType)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(auggioAnchor.transform.position, 0.05f);

            Vector3 start = auggioAnchor.transform.position;
            Vector3 endZ = start + auggioAnchor.transform.forward * 0.5f;
            Vector3 endX = start + auggioAnchor.transform.right * 0.5f;
            Vector3 endY = start + auggioAnchor.transform.up * 0.5f;
            Handles.DrawBezier(start,endZ,start,endZ, Color.blue,null,5);
            Handles.DrawBezier(start,endX,start,endX, Color.red,null,5);
            Handles.DrawBezier(start,endY,start,endY, Color.green,null,5);
        }
        
        public static bool VisualizeAnchorMesh(AuggioAnchor anchor)
        {
            anchor.VisualizeMesh = true;
            string path = Path.Combine(AuggioUtils.GetMeshDataPath(), anchor.OrganizationId, anchor.LocationId, anchor.AnchorId, anchor.MeshHash);
            if (!File.Exists(path))
            {
                anchor.VisualizeMesh = false;
                Debug.LogError("Cannot visualize mesh because mesh data are missing. Maybe you forgot to 'Download Meshes' in augg.io editor plugin?");
                return false;
            }
            
            byte[] meshData = File.ReadAllBytes(path);
                        
            BinaryObjectSerializer<MeshWithAnchor> binaryObjectSerializer =
                new BinaryObjectSerializer<MeshWithAnchor>();
                
            MeshWithAnchor meshWithAnchor = new MeshWithAnchor();
            binaryObjectSerializer.Deserialize(meshData, meshWithAnchor);
            ScanDataImporter scanDataImporter = new ScanDataImporter();
            scanDataImporter.Import(meshWithAnchor, anchor.transform, anchor.MeshMaterial);
            return true;
        }
        
        public static void HideAnchorMesh(AuggioAnchor anchor)
        {
            for (int i = anchor.transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = anchor.transform.GetChild(i).gameObject;
                DestroyImmediate(child);
            }
            anchor.VisualizeMesh = false;
        }
        
        public static bool IsValid(AuggioAnchor anchor)
        {
            return !string.IsNullOrEmpty(anchor.OrganizationId) &&
                   !string.IsNullOrEmpty(anchor.ExperienceId) &&
                   !string.IsNullOrEmpty(anchor.LocationId) &&
                   !string.IsNullOrEmpty(anchor.AnchorId) &&
                   !string.IsNullOrEmpty(anchor.MeshHash);
        }

        private void FindInProjectStructure(AuggioAnchor auggioAnchor)
        {
            string path = Path.Combine(AuggioUtils.GetMeshDataPath(true), auggioAnchor.OrganizationId,
                auggioAnchor.LocationId, auggioAnchor.AnchorId, auggioAnchor.MeshHash);
            if (!File.Exists(path))
            {
                throw new Exception("Cannot find anchor mesh data in Project Structure");
            }
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

    }
}
#endif