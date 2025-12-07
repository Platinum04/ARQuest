#if UNITY_EDITOR
using System;
using System.Linq.Expressions;
using Auggio.Plugin.SDK.Utils.DetachStrategy;
using Auggio.Utils.SDK.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Auggio.Plugin.Editor.SDK
{
    [CustomEditor(typeof(PlaneSnappingDetachStrategy))]
    internal class PlaneSnappingDetachStrategyEditor : UnityEditor.Editor
    {
    
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            PlaneSnappingDetachStrategy planeSnappingDetachStrategy = (PlaneSnappingDetachStrategy) target;

            ARComponentsProvider arComponentsProvider = GetARComponentsProviderFromScene();
            if (arComponentsProvider == null)
            {
                ProcessMissingARComponentsProvider();
                return;
            }

            if (arComponentsProvider.PlaneManager == null)
            {
                ProcessMissingPlaneManager(arComponentsProvider);
                return;
            }

            if (arComponentsProvider.AnchorManager == null)
            {
                ProcessMissingAnchorManager(arComponentsProvider);
                return;
            }

            GUILayout.Space(10);
            DrawSnapAxisInfo(planeSnappingDetachStrategy);
            
            if (planeSnappingDetachStrategy.PlaneFinder == null)
            {
                ProcessMissingPlaneFinder(planeSnappingDetachStrategy);
            }
            else
            {
                GUILayout.Space(10);
                
                SceneVisibilityManager.instance.DisablePicking(planeSnappingDetachStrategy.PlaneFinder.gameObject, true);

                SerializedObject childTransformObject = new SerializedObject(planeSnappingDetachStrategy.PlaneFinder.transform);
                childTransformObject.Update();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(childTransformObject.FindProperty("m_LocalPosition"), new GUIContent("Detection area position"), true);
                EditorGUILayout.PropertyField(childTransformObject.FindProperty("m_LocalRotation"), new GUIContent("Detection area rotation"), true);
                EditorGUILayout.PropertyField(childTransformObject.FindProperty("m_LocalScale"), new GUIContent("Detection area size"),true);
                if (EditorGUI.EndChangeCheck())
                {
                    childTransformObject.ApplyModifiedProperties();
                }
                if (GUILayout.Button("Select Detection Area Object"))
                {
                    Selection.activeObject = planeSnappingDetachStrategy.PlaneFinder.gameObject;
                }
            }
            
            GUILayout.Space(20);
            DrawPlaneManagerSettings(arComponentsProvider);

            if (arComponentsProvider.PlaneManager.planePrefab == null)
            {
                ProcessMissingPlanePrefab();
                return;
            }

            if (arComponentsProvider.PlaneManager.planePrefab.GetComponent<MeshCollider>() == null)
            {
                ProcessPlanePrefabMissingMeshCollider();
                return;
            }

            if (arComponentsProvider.PlaneManager.planePrefab.GetComponent<MeshCollider>() == null)
            {
                ProcessPlanePrefabMissingMeshCollider();
                return;
            }

            if (arComponentsProvider.PlaneManager.planePrefab.GetComponent<AuggioPlane>() == null)
            {
                ProcessPlanePrefabMissingAuggioPlaneScript();
                return;
            }

            
        }

        private void DrawSnapAxisInfo(PlaneSnappingDetachStrategy planeSnappingDetachStrategy)
        {
            string message = "";
            switch (planeSnappingDetachStrategy.SnapAxis)
            {
                case PlaneSnappingDetachStrategy.Axis.X:
                    message = "red X axis will be pointing away from";
                    break;
                case PlaneSnappingDetachStrategy.Axis.Y:
                    message = "green Y axis will be pointing away from";
                    break;
                case PlaneSnappingDetachStrategy.Axis.Z:
                    message = "blue Z axis will be pointing away from";
                    break;
                case PlaneSnappingDetachStrategy.Axis.X_Negative:
                    message = "red X axis will be pointing into";
                    break;
                case PlaneSnappingDetachStrategy.Axis.Y_Negative:
                    message = "green Y axis will be pointing into";
                    break;
                case PlaneSnappingDetachStrategy.Axis.Z_Negative:
                    message = "blue Z axis will be pointing into";
                    break;
            }

            EditorGUILayout.HelpBox(
                "Plane snapping will align the selected axis perpendicular to the detected plane. In this case, " +
                message + " the detected plane", MessageType.Info);
        }

        private void ProcessMissingPlaneFinder(PlaneSnappingDetachStrategy planeSnappingDetachStrategy)
        {
            EditorGUILayout.HelpBox("Missing plane finder", MessageType.Error);
            if (GUILayout.Button("Try to fix"))
            {
                AuggioPlaneFinder existingChild =
                    planeSnappingDetachStrategy.GetComponentInChildren<AuggioPlaneFinder>();
                if (existingChild != null)
                {
                    planeSnappingDetachStrategy.PlaneFinder = existingChild;
                }
                else
                {
                    string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
                    string spriteFolderPath = scriptPath.Substring(0, scriptPath.LastIndexOf('/') + 1);
                    string spritePath = spriteFolderPath + "AuggioPlaneFinder.prefab";
                    GameObject planeFinder = AssetDatabase.LoadAssetAtPath<GameObject>(spritePath);
                    GameObject planeFinderInstance = Instantiate(planeFinder, planeSnappingDetachStrategy.transform);
                    planeSnappingDetachStrategy.PlaneFinder = planeFinderInstance.GetComponent<AuggioPlaneFinder>();
                }

                EditorUtility.SetDirty(planeSnappingDetachStrategy);
            }
        }

        private ARComponentsProvider GetARComponentsProviderFromScene()
        {
            ARComponentsProvider[] componentsProviders =
                FindObjectsByType<ARComponentsProvider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (componentsProviders.Length == 0)
            {
                return null;
            }

            if (componentsProviders.Length > 1)
            {
                throw new Exception("Ambiguous reference. Multiple ARComponentProviders found in scene.");
            }

            return componentsProviders[0];
        }

        private void ProcessMissingARComponentsProvider()
        {
            EditorGUILayout.HelpBox(
                "Missing ARComponentsProvider in scene. Please add ARComponentsProvider to some GameObject (e.g. AuggioTrackingManager)",
                MessageType.Error);
        }

        private void DrawPlaneManagerSettings(ARComponentsProvider arComponentsProvider)
        {
            GUILayout.Label("AR Plane Manager Settings");
            EditorGUILayout.HelpBox("Following properties change global AR Plane Manager settings",
                MessageType.Warning);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("AR Plane Manager", arComponentsProvider.PlaneManager, typeof(GameObject),
                false);
            EditorGUI.EndDisabledGroup();
            arComponentsProvider.PlaneManager.planePrefab = (GameObject) EditorGUILayout.ObjectField("Plane Prefab",
                arComponentsProvider.PlaneManager.planePrefab, typeof(GameObject), false);
        }

        private void ProcessMissingPlanePrefab()
        {
            EditorGUILayout.HelpBox("AR Plane Manager is missing plane prefab.", MessageType.Error);
        }

        private void ProcessPlanePrefabMissingMeshCollider()
        {
            EditorGUILayout.HelpBox(
                "AR Plane Manager plane prefab is missing a mesh collider. Please set prefab with collider.",
                MessageType.Error);
        }

        private void ProcessPlanePrefabMissingAuggioPlaneScript()
        {
            EditorGUILayout.HelpBox(
                "AR Plane Manager plane prefab is missing a AuggioPlane script. Please add script to prefab or change the plane prefab to one one with this script attached.",
                MessageType.Error);
        }

        private void ProcessMissingPlaneManager(ARComponentsProvider arComponentsProvider)
        {
            EditorGUILayout.HelpBox(
                "AR Components Provider is missing Plane Manager which is required for this strategy to work.",
                MessageType.Error);
            if (GUILayout.Button("Try to fix"))
            {
                ARPlaneManager[] scenePlaneManagers =
                    FindObjectsByType<ARPlaneManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                if (scenePlaneManagers.Length == 0)
                {
                    arComponentsProvider.Origin.gameObject.AddComponent<ARPlaneManager>();
                }

                ARComponentsProvider.ARComponentsProviderEditor.TryToFillReferences(arComponentsProvider);
            }
        }

        private void ProcessMissingAnchorManager(ARComponentsProvider arComponentsProvider)
        {
            EditorGUILayout.HelpBox(
                "AR Components Provider is missing Anchor Manager which is required for this strategy to work.",
                MessageType.Error);
            if (GUILayout.Button("Try to fix"))
            {
                ARAnchorManager[] sceneAnchorManagers =
                    FindObjectsByType<ARAnchorManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                if (sceneAnchorManagers.Length == 0)
                {
                    arComponentsProvider.Origin.gameObject.AddComponent<ARAnchorManager>();
                }

                ARComponentsProvider.ARComponentsProviderEditor.TryToFillReferences(arComponentsProvider);
            }
        }
    }
}
#endif