#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization.Model;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor
{
    internal class CreateAuggioObjectPlaceholderWindow : EditorWindow
    {

        public const string WINDOW_NAME = "Create new object placeholder";
        
        private List<AuggioExperience> availableExperiences;
        private List<AuggioObjectTracker> availableObjectTrackers;
        private List<string> availableModelIds;

        private int selectedModelOption = 0;
        private int selectedExperienceOption = -1;
        private int selectedObjectOption = -1;

        private string placeholderName;

        private string presetExperienceId;
        private string presetObjectId;

        [MenuItem("GameObject/augg.io/Add New Placeholder Object")]
        public static CreateAuggioObjectPlaceholderWindow GetNewWindowInstance()
        {
            CreateAuggioObjectPlaceholderWindow existingWindow = GetWindow<CreateAuggioObjectPlaceholderWindow>();
            if (existingWindow != null)
            {
                // Close the existing window
                existingWindow.Close();
            }
            return GetWindow<CreateAuggioObjectPlaceholderWindow>(true, WINDOW_NAME);
        }
        
        public void SetExperienceId(string experienceId)
        {
            presetExperienceId = experienceId;
        }
        
        public void SetObject(string objectId)
        {
            presetObjectId = objectId;
        }

        private void OnEnable()
        {
            availableModelIds = PrimitiveTypeHelper.GetIds();
            availableExperiences = AuggioSceneManager.GetAllExperiencesInScene(SceneManager.GetActiveScene());
        }

        private void OnGUI()
        {
            ProcessPresetData();
            
            if (availableExperiences.Count == 0)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                GUILayout.Label("No experiences found in scene.", AuggioEditorStyles.WithTextAlignment(EditorStyles.boldLabel, TextAnchor.MiddleCenter));
                GUILayout.Label("Please import one via 'augg.io -> Unity Editor Plugin'",  AuggioEditorStyles.WithTextAlignment(EditorStyles.boldLabel, TextAnchor.MiddleCenter));
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                return;
            }
            GUILayout.Label("Placeholder settings", EditorStyles.boldLabel);

            // Create a text field
            placeholderName = EditorGUILayout.TextField("Placeholder name", placeholderName);
            selectedModelOption = EditorGUILayout.Popup("Model representation:", selectedModelOption, availableModelIds.Select(modelId => PrimitiveTypeHelper.GetPrimitiveTypeByModelId(modelId).ToString()).ToArray());
            
            // Create a dropdown list
            EditorGUI.BeginDisabledGroup(presetExperienceId != null);
            EditorGUI.BeginChangeCheck();
            selectedExperienceOption = EditorGUILayout.Popup("Experience:", selectedExperienceOption, availableExperiences.Select(experience => experience.ExperienceName).ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                AuggioExperience selectedSceneExperience = availableExperiences[selectedExperienceOption];
                BuildAuggioObjectOptions(selectedSceneExperience);
            }
            EditorGUI.EndDisabledGroup();

            if (availableObjectTrackers != null)
            {
                EditorGUI.BeginDisabledGroup(presetObjectId != null);
                selectedObjectOption =
                    EditorGUILayout.Popup("Auggio object:", selectedObjectOption, availableObjectTrackers.Select(tracker => tracker.ObjectName).ToArray());
                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.BeginDisabledGroup(availableObjectTrackers != null && string.IsNullOrEmpty(placeholderName) || selectedModelOption == -1 || selectedExperienceOption == -1 || selectedObjectOption == -1);
            // Add a button to perform an action
            if (GUILayout.Button("Create"))
            {
                AuggioObjectTracker tracker = availableObjectTrackers[selectedObjectOption];
                if (tracker == null)
                {
                    throw new Exception("Cannot find selected augg.io object in scene. Please check if it is present. If yes try to close and open the window again.");
                }
                AuggioObjectPlaceholder objectPlaceholder = new AuggioObjectPlaceholder();
                objectPlaceholder.ID = Guid.NewGuid().ToString();
                objectPlaceholder.Position = new SerializedVector3(Vector3.zero);
                objectPlaceholder.Rotation = new SerializedVector3(Vector3.zero);
                objectPlaceholder.Scale = new SerializedVector3(Vector3.one);
                objectPlaceholder.ModelId = availableModelIds[selectedModelOption];
                objectPlaceholder.Name = placeholderName;

                AuggioSceneImporter.ImportAuggioObjectPlaceholder(SceneManager.GetActiveScene(), availableExperiences[selectedExperienceOption].ExperienceId, tracker, objectPlaceholder, tracker.transform);
                Close();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void BuildAuggioObjectOptions(AuggioExperience selectedSceneExperience)
        {
            availableObjectTrackers = AuggioSceneManager.GetAuggioObjectTrackersByExperienceId(SceneManager.GetActiveScene(), selectedSceneExperience.ExperienceId);
        }

        private void ProcessPresetData()
        {
            if(selectedExperienceOption == -1 && availableExperiences != null && presetExperienceId != null)
            {
                int index = 0;
                foreach (AuggioExperience experience in availableExperiences)
                {
                    if (experience.ExperienceId.Equals(presetExperienceId))
                    {
                        selectedExperienceOption = index;
                        AuggioExperience selectedSceneExperience = availableExperiences[selectedExperienceOption];
                        BuildAuggioObjectOptions(selectedSceneExperience);
                        ProcessPresetObject();
                        break;
                    }
                    index++;
                }
            }
        }

        private void ProcessPresetObject()
        {
            if (selectedObjectOption == -1 && availableObjectTrackers != null && presetObjectId != null)
            {
                int index = 0;
                foreach (AuggioObjectTracker tracker in availableObjectTrackers)
                {
                    if (tracker.ObjectId.Equals(presetObjectId))
                    {
                        selectedObjectOption = index;
                        break;
                        
                    }
                    index++;
                }
            }
        }
        
       
    }
}
#endif