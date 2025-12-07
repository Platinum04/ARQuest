#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.Editor.Validation;
using Auggio.Plugin.Editor.Validation.Validators;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using Auggio.Plugin.SDK.Utils.DetachStrategy;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Auggio.Plugin.Editor.SDK
{
    [CustomEditor(typeof(AuggioObjectTracker))]
    internal class AuggioObjectTrackerEditor : UnityEditor.Editor
    {
        private List<string> availableAnchorIds = new();
        private int selectedAnchorOptionIndex = -1;

        private AuggioObjectTrackerValidator validator = new();
        private Dictionary<ErrorCode, string> errors = new();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AuggioObjectTracker tracker = (AuggioObjectTracker)target;

            //validation
            errors = validator.Validate(tracker);
            Experience experience = GetExperienceFromFile(tracker);
            if (experience == null || string.IsNullOrEmpty(experience.ID))
            {
                errors.Add(ErrorCode.AUGGIO_TRACKER_MISSING_OR_CORRUPTED_EXPERIENCE,
                    "Experience data are missing or corrupted.");
            }

            if (errors.Count > 0)
            {
                DrawErrors(tracker);
                if (errors.Any(error => error.Key == ErrorCode.AUGGIO_TRACKER_MISSING_OR_CORRUPTED_EXPERIENCE))
                {
                    return;
                }
            }


            //draw inspector
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            DrawObjectNameInspector(tracker);
            DrawAnchorSelectInspector(experience, tracker);
            DrawActions(tracker);

            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);
            GUILayout.Label("Object ID: " + tracker.ObjectId);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawErrors(AuggioObjectTracker tracker)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Encountered " + errors.Count + " Error(s)");

            foreach (var error in errors)
            {
                if (error.Key == ErrorCode.AUGGIO_TRACKER_MISSING_EXPERIENCE_ID ||
                    error.Key == ErrorCode.AUGGIO_TRACKER_MISSING_ORGANIZATION_ID)
                {
                    AuggioEditorStyles.DrawHelpBoxWithButtons(error.Value, MessageType.Error, new List<ErrorButton>()
                    {
                        new("Try to fix", () => { TryToFixObjectTracker(tracker); })
                    });
                }
                else if (error.Key == ErrorCode.AUGGIO_TRACKER_NOT_UNIQUE)
                {
                    AuggioEditorStyles.DrawHelpBoxWithButtons(error.Value, MessageType.Error, new List<ErrorButton>()
                    {
                        new("Create new from this object", () => { CreateNewObjectFromExisting(tracker); })
                    });
                }
                else
                {
                    EditorGUILayout.HelpBox(error.Value, MessageType.Error);
                }
            }

            GUILayout.Space(20);
            EditorGUILayout.EndVertical();
        }

        private void CreateNewObjectFromExisting(AuggioObjectTracker tracker)
        {
            tracker.ObjectName = "New Object";
            tracker.gameObject.name = AuggioUtils.GetGameObjectName("New Object");
            tracker.ObjectId = Guid.NewGuid().ToString();

            foreach (AuggioObjectPlaceholderModel model in tracker.Models)
            {
                model.PlaceholderId = Guid.NewGuid().ToString();
                model.PlaceholderName = "New Placeholder Model";
                model.gameObject.name =
                    AuggioUtils.GetGameObjectPlaceholderName("New Placeholder Model");
                EditorUtility.SetDirty(model);
            }

            EditorUtility.SetDirty(tracker);
            EditorSceneManager.MarkSceneDirty(tracker.gameObject.scene);
        }


        private void DrawActions(AuggioObjectTracker tracker)
        {
            GUILayout.Space(20);
            GUILayout.Label("Actions");
            if (GUILayout.Button("Add placeholder"))
            {
                // Check if any instance of CreateAuggioObjectWindow is already open
                CreateAuggioObjectPlaceholderWindow existingWindow =
                    EditorWindow.GetWindow<CreateAuggioObjectPlaceholderWindow>();
                if (existingWindow != null)
                {
                    // Close the existing window
                    existingWindow.Close();
                }

                CreateAuggioObjectPlaceholderWindow auggioPlaceholderWindow =
                    CreateAuggioObjectPlaceholderWindow.GetNewWindowInstance();
                auggioPlaceholderWindow.SetExperienceId(tracker.ExperienceId);
                auggioPlaceholderWindow.SetObject(tracker.ObjectId);
            }

            if (GUILayout.Button("Add detach strategy"))
            {
                ShowAddDetachStrategyMenu();
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        internal static void OnDrawGizmosSelected(AuggioObjectTracker tracker, GizmoType gizmoType)
        {
            if (tracker.transform.parent == null || tracker.transform.GetComponentInParent<AuggioExperience>() == null)
            {
                Debug.LogError("AuggioObjectTracker should be under GameObject with AuggioExperience attached.");
                return;
            }

            if (string.IsNullOrEmpty(tracker.AnchorId))
            {
                //Debug.LogError("AuggioObjectTracker is missing attached anchor information.");
                return;
            }

            Transform experienceTransform = tracker.transform.parent;
            Transform parentAnchorTransform = null;

            foreach (AuggioAnchor anchor in experienceTransform.GetComponentsInChildren<AuggioAnchor>(true))
            {
                if (tracker.AnchorId.Equals(anchor.AnchorId))
                {
                    parentAnchorTransform = anchor.transform;
                    break;
                }
            }

            if (parentAnchorTransform == null)
            {
                return;
            }

            Handles.color = Color.white;
            Handles.DrawDottedLine(tracker.transform.position, parentAnchorTransform.position, 5f);
            Handles.color = new Color(0f, 1f, 1f, 1f);

            if (tracker.Models != null)
            {
                foreach (AuggioObjectPlaceholderModel placeholderModel in tracker.Models)
                {
                    Handles.DrawLine(tracker.transform.position, placeholderModel.transform.position, 1f);
                }
            }
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Active | GizmoType.Pickable)]
        internal static void OnDrawGizmos(AuggioObjectTracker tracker, GizmoType gizmoType)
        {
            if (!tracker.VisualizeMesh)
            {
                return;
            }

            if (string.IsNullOrEmpty(tracker.AnchorId))
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawIcon(tracker.transform.position, "console.warnicon", false);
            }
            else
            {
                Gizmos.color = Selection.activeGameObject == tracker.gameObject
                    ? new Color(0f, 1f, 1f, 0.7f)
                    : new Color(1, 1, 1, 0.3f);
            }

            if (tracker.Models != null)
            {
                tracker.Models.RemoveAll(model => model == null);
            }

            if (tracker.Models == null || tracker.Models.Count == 0)
            {
                Gizmos.color = new Color(1, 1, 1, 0.5f);
                Gizmos.DrawIcon(tracker.transform.position, "d_PreMatCube@2x", false);
            }
            else
            {
                foreach (AuggioObjectPlaceholderModel placeholderModel in tracker.Models)
                {
                    if (placeholderModel.Mesh == null)
                    {
                        continue;
                    }

                    // Store the current Gizmos matrix
                    Matrix4x4 oldMatrix = Gizmos.matrix;
                    Gizmos.matrix = Matrix4x4.TRS(placeholderModel.transform.position,
                        placeholderModel.transform.rotation,
                        Vector3.Scale(tracker.transform.localScale, placeholderModel.transform.localScale));
                    Gizmos.DrawMesh(placeholderModel.Mesh);
                    // Restore the old matrix to prevent affecting other Gizmos drawing
                    Gizmos.matrix = oldMatrix;
                }
            }
        }

        internal static Experience GetExperienceFromFile(AuggioObjectTracker tracker)
        {
            try
            {
                string path = AuggioUtils.GetExperienceFilePath(tracker.OrganizationId, tracker.ExperienceId);
                if (!File.Exists(path))
                {
                    return null;
                }

                byte[] bytes = File.ReadAllBytes(path);
                string json = Encoding.UTF8.GetString(bytes);
                Experience experience = JsonUtility.FromJson<Experience>(json);

                return experience;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void TryToFixObjectTracker(AuggioObjectTracker tracker)
        {
            if (tracker.transform.parent == null ||
                tracker.transform.GetComponentInParent<AuggioExperience>() == null)
            {
                Debug.LogError(
                    "Cannot fix. AuggioObjectTracker should be under GameObject with AuggioExperience attached.");
                return;
            }

            AuggioExperience parentExperience = tracker.transform.GetComponentInParent<AuggioExperience>();
            if (string.IsNullOrEmpty(parentExperience.OrganizationId) ||
                string.IsNullOrEmpty(parentExperience.ExperienceId))
            {
                Debug.LogError("Cannot fix. Parent experience is missing information (invalid)");
                return;
            }

            tracker.ExperienceId = parentExperience.ExperienceId;
            tracker.OrganizationId = parentExperience.OrganizationId;

            tracker.ObjectName = tracker.gameObject.name.Replace(AuggioUtils.OBJECT_NAME_PREFIX, "");
            tracker.gameObject.name = AuggioUtils.GetGameObjectName(tracker.ObjectName);

            EditorSceneManager.MarkSceneDirty(tracker.gameObject.scene);

            if (string.IsNullOrEmpty(tracker.AnchorId))
            {
                AuggioAnchor[] availableAnchors = parentExperience.GetComponentsInChildren<AuggioAnchor>(true);
                if (availableAnchors.Length == 0)
                {
                    Debug.LogError("Cannot fix anchor. There are no anchors in experience");
                    return;
                }

                tracker.AnchorId = availableAnchors[0].AnchorId;
            }

            EditorUtility.SetDirty(this);
        }


        private void DrawObjectNameInspector(AuggioObjectTracker tracker)
        {
            //object name
            GUI.SetNextControlName("ObjectNameField");
            EditorGUI.BeginChangeCheck();
            tracker.ObjectName = EditorGUILayout.DelayedTextField("Object Name", tracker.ObjectName);
            if (EditorGUI.EndChangeCheck())
            {
                tracker.gameObject.name = AuggioUtils.GetGameObjectName(tracker.ObjectName);
                EditorSceneManager.MarkSceneDirty(tracker.gameObject.scene);
                EditorUtility.SetDirty(this);
                serializedObject.ApplyModifiedProperties();
            }

            if (GUI.GetNameOfFocusedControl() == "ObjectNameField")
            {
                GUILayout.Label("Changes apply on Enter or when you leave the field", EditorStyles.miniLabel);
            }
        }

        private void DrawAnchorSelectInspector(Experience experience, AuggioObjectTracker tracker)
        {
            List<string> options = new List<string>();
            int index = 0;
            foreach (Location location in experience.AssignedLocations)
            {
                foreach (SingleAnchor anchor in location.SingleAnchorList)
                {
                    options.Add("[" + location.Name + "] " + anchor.Name);
                    availableAnchorIds.Add(anchor.AuggioId);
                    if (anchor.AuggioId.Equals(tracker.AnchorId))
                    {
                        selectedAnchorOptionIndex = index;
                    }

                    index++;
                }
            }

            EditorGUI.BeginChangeCheck();
            selectedAnchorOptionIndex =
                EditorGUILayout.Popup("Parent Anchor", selectedAnchorOptionIndex, options.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                tracker.AnchorId = availableAnchorIds[selectedAnchorOptionIndex];
                EditorSceneManager.MarkSceneDirty(tracker.gameObject.scene);
                EditorUtility.SetDirty(this);
                serializedObject.ApplyModifiedProperties();
            }
        }

        internal static AuggioObjectPlaceholderModel FindPlaceholderById(AuggioObjectTracker tracker, string id)
        {
            foreach (AuggioObjectPlaceholderModel placeholder in tracker.Models)
            {
                if (placeholder.PlaceholderId.Equals(id))
                {
                    return placeholder;
                }
            }

            return null;
        }

        void ShowAddDetachStrategyMenu()
        {
            Assembly sdkAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "cz.augg.io.sdk");
            if (sdkAssembly == null)
            {
                throw new Exception("augg.io sdk assembly not found");
            }

            var types = sdkAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(AbstractDetachStrategy)));

            if (!types.Any())
            {
                Debug.Log("No detach strategy scripts found.");
                return;
            }

            GenericMenu menu = new GenericMenu();

            foreach (var type in types)
            {
                menu.AddItem(new GUIContent(type.Name), false,
                    () => { Selection.activeGameObject.AddComponent(type); });
            }

            menu.ShowAsContext();
        }
    }
}
#endif