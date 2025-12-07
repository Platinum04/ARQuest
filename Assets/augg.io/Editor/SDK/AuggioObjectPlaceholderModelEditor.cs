#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.Editor.Validation;
using Auggio.Plugin.Editor.Validation.Validators;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Auggio.Plugin.Editor.SDK
{
    [CustomEditor(typeof(AuggioObjectPlaceholderModel))]
    internal class AuggioObjectPlaceholderModelEditor : UnityEditor.Editor
    {

        private SerializedProperty placeholderNameProp;
        private List<string> availableModelIds = new List<string>();
        private int selectedModelOptionIndex = -1;
        private AuggioObjectPlaceholderValidator validator = new();
        private Dictionary<ErrorCode, string> errors = new();
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            placeholderNameProp = serializedObject.FindProperty("placeholderName");
            base.OnInspectorGUI();
            
            AuggioObjectPlaceholderModel model = (AuggioObjectPlaceholderModel) target;
            
            errors = validator.Validate(model);
            if(errors.Count > 0)
            {
                DrawErrors(model);
            }
            EditorGUI.BeginChangeCheck();
            
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            
            DrawPlaceholderNameField(model);
            DrawModelIdInspector(model);
            
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(20);
            GUILayout.Label("Placeholder ID: " + model.PlaceholderId);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPlaceholderNameField(AuggioObjectPlaceholderModel model)
        {
            GUI.SetNextControlName("PlaceholderNameField");
            EditorGUI.BeginChangeCheck();
            string newName = EditorGUILayout.DelayedTextField("Placeholder Name", placeholderNameProp.stringValue);
            if (newName != placeholderNameProp.stringValue)
            {
                placeholderNameProp.stringValue = newName;
                model.gameObject.name = AuggioUtils.GetGameObjectPlaceholderName(newName);
            }
            if (GUI.GetNameOfFocusedControl() == "PlaceholderNameField")
            {
                GUILayout.Label("Changes apply on Enter or when you leave the field", EditorStyles.miniLabel);
            }
        }

        private void DrawErrors(AuggioObjectPlaceholderModel model)
        {
            if (errors.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                GUILayout.Label("Encountered " + errors.Count + " Error(s)");
            }

            foreach (var error in errors)
            {
                if (error.Key == ErrorCode.AUGGIO_PLACEHOLDER_DUPLICATE)
                {
                    AuggioEditorStyles.DrawHelpBoxWithButtons(error.Value, MessageType.Error, new List<ErrorButton>()
                    {
                        new("Create new from this placeholder", (() =>
                        {
                            model.PlaceholderName = "New placeholder model";
                            model.gameObject.name = AuggioUtils.GetGameObjectPlaceholderName("New placeholder model");
                            model.PlaceholderId = Guid.NewGuid().ToString();
                            AttachModelToTracker(model);
                            serializedObject.ApplyModifiedProperties();
                        }))
                    });
                }
                else if (error.Key == ErrorCode.AUGGIO_PLACEHOLDER_NOT_REGISTERED)
                {
                    AuggioEditorStyles.DrawHelpBoxWithButtons(error.Value, MessageType.Error, new List<ErrorButton>()
                    {
                        new("Try to fix", (() =>
                            {
                                AttachModelToTracker(model);
                            })
                        )
                    });
                } else if (error.Key == ErrorCode.AUGGIO_PLACEHOLDER_DIFFERENT_PARENT)
                {
                    AuggioEditorStyles.DrawHelpBoxWithButtons(error.Value, MessageType.Error, new List<ErrorButton>()
                    {
                        new("Try to fix", (() =>
                            {
                                AttachModelToTracker(model);
                            })
                        ),
                        new ("Revert back to original object", () =>
                        {
                            model.transform.parent = model.AuggioObjectTracker.transform;
                        })
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
      

        private void AttachModelToTracker(AuggioObjectPlaceholderModel model)
        {
            AuggioObjectTracker tracker = model.GetComponentInParent<AuggioObjectTracker>(true);
            if (tracker == null)
            {
                Debug.LogError("Cannot fix. AuggioObjectPlaceholderModel parent object has not AuggioObjectTracker script attached");
                return;
            }

            if (model.AuggioObjectTracker != null)
            {
                //object is already attached to different tracker => remove it from there
                model.AuggioObjectTracker.Models.Remove(model);
                model.AuggioObjectTracker = null;
            }

            if (tracker.Models == null)
            {
                tracker.Models = new List<AuggioObjectPlaceholderModel>();
            }
            tracker.Models.Add(model);
            model.AuggioObjectTracker = tracker;
            model.ExperienceId = tracker.ExperienceId;
            model.PlaceholderId = Guid.NewGuid().ToString();
            
            EditorSceneManager.MarkSceneDirty(tracker.gameObject.scene);
            EditorUtility.SetDirty(tracker);
        }

        private void DrawModelIdInspector(AuggioObjectPlaceholderModel model)
        {
            if (model.ModelId == null)
            {
                model.ModelId = PrimitiveTypeHelper.CUBE_ID;
            }
            //model ids
            availableModelIds = PrimitiveTypeHelper.GetIds();
            int i = 0;
            foreach (string modelId in availableModelIds)
            {
                if (model.ModelId != null && model.ModelId.Equals(modelId))
                {
                    selectedModelOptionIndex = i;
                    break;
                }

                i++;
            }

            EditorGUI.BeginChangeCheck();
            selectedModelOptionIndex =
                EditorGUILayout.Popup("Model representation", selectedModelOptionIndex,
                    availableModelIds
                        .Select(modelId => PrimitiveTypeHelper.GetPrimitiveTypeByModelId(modelId).ToString())
                        .ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                model.ModelId = availableModelIds[selectedModelOptionIndex] == PrimitiveTypeHelper.NO_PLACEHOLDER? null : availableModelIds[selectedModelOptionIndex];
                EditorSceneManager.MarkSceneDirty(model.gameObject.scene);
                EditorUtility.SetDirty(this);
            }
        }
        
    }
}
#endif