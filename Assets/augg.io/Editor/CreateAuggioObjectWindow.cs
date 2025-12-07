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
    internal class CreateAuggioObjectWindow : EditorWindow
    {

        public const string WINDOW_NAME = "Create new augg.io object";
        
        private List<string> anchorOptions;
        private List<string> modelOptions;

        private List<AuggioExperience> availableExperiences;
        private List<string> availableAnchorIds;
        private List<string> availableModelIds;

        private int selectedModelOption = 0;
        private int selectedExperienceOption = -1;
        private int selectedAnchorOption = -1;

        private string objectName;
        private Experience selectedExperienceData;

        private string presetExperienceId;
        private string presetAnchorId;

        [MenuItem("GameObject/augg.io/Add New Object")]
        public static CreateAuggioObjectWindow GetNewWindowInstance()
        {
            CreateAuggioObjectWindow existingWindow = GetWindow<CreateAuggioObjectWindow>();
            if (existingWindow != null)
            {
                // Close the existing window
                existingWindow.Close();
            }
            return GetWindow<CreateAuggioObjectWindow>(true, WINDOW_NAME);
        }
        
        public void SetExperienceId(string experienceId)
        {
            presetExperienceId = experienceId;
        }
        
        public void SetAnchorId(string anchorId)
        {
            presetAnchorId = anchorId;
        }

        private void OnEnable()
        {
            availableModelIds = PrimitiveTypeHelper.GetIds();
            modelOptions = new List<string>();
            modelOptions.Add(PrimitiveTypeHelper.NO_PLACEHOLDER);
            modelOptions.AddRange(availableModelIds
                .Select(modelId => PrimitiveTypeHelper.GetPrimitiveTypeByModelId(modelId).ToString()).ToList());
            
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
            GUILayout.Label("Object settings", EditorStyles.boldLabel);

            // Create a text field
            objectName = EditorGUILayout.TextField("Object name", objectName);
            selectedModelOption = EditorGUILayout.Popup("Model representation:", selectedModelOption, modelOptions.ToArray());
            
            // Create a dropdown list
            EditorGUI.BeginDisabledGroup(presetExperienceId != null);
            EditorGUI.BeginChangeCheck();
            selectedExperienceOption = EditorGUILayout.Popup("Experience:", selectedExperienceOption, availableExperiences.Select(experience => experience.ExperienceName).ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                AuggioExperience selectedSceneExperience = availableExperiences[selectedExperienceOption];
                BuildAnchorOptions(selectedSceneExperience);
            }
            EditorGUI.EndDisabledGroup();

            if (anchorOptions != null)
            {
                EditorGUI.BeginDisabledGroup(presetAnchorId != null);
                selectedAnchorOption =
                    EditorGUILayout.Popup("Anchor:", selectedAnchorOption, anchorOptions.ToArray());
                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(objectName) || selectedModelOption == -1 || selectedExperienceOption == -1 || selectedAnchorOption == -1);
            // Add a button to perform an action
            if (GUILayout.Button("Create"))
            {
                AuggioExperience selectedExperience = availableExperiences[selectedExperienceOption];
                if (selectedExperience == null)
                {
                    throw new Exception("Cannot find selected experience in scene. Please check if it is present. If yes try to close and open the window again.");
                }
                AuggioObject auggioObject = new AuggioObject();
                auggioObject.AuggioId = Guid.NewGuid().ToString();
                auggioObject.Name = objectName;
                auggioObject.Position = new SerializedVector3(Vector3.zero);
                auggioObject.Rotation = new SerializedVector3(Vector3.zero);
                auggioObject.Scale = new SerializedVector3(Vector3.one);
                auggioObject.AssignedAnchor = availableAnchorIds[selectedAnchorOption];

                if (modelOptions[selectedModelOption] != PrimitiveTypeHelper.NO_PLACEHOLDER)
                {
                    AuggioObjectPlaceholder objectPlaceholder = new AuggioObjectPlaceholder();
                    objectPlaceholder.ID = Guid.NewGuid().ToString();
                    objectPlaceholder.Position = new SerializedVector3(Vector3.zero);
                    objectPlaceholder.Rotation = new SerializedVector3(Vector3.zero);
                    objectPlaceholder.Scale = new SerializedVector3(Vector3.one);
                    objectPlaceholder.ModelId = availableModelIds[selectedModelOption-1]; 
                    objectPlaceholder.Name = availableModelIds[selectedModelOption-1];
                    auggioObject.PlaceholderModels = new List<AuggioObjectPlaceholder>() {objectPlaceholder};
                }
                else
                {
                    auggioObject.PlaceholderModels = new List<AuggioObjectPlaceholder>();
                }

                AuggioSceneImporter.ImportAuggioObject(SceneManager.GetActiveScene(), auggioObject,
                    selectedExperienceData, selectedExperience.transform);
                Close();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void BuildAnchorOptions(AuggioExperience selectedSceneExperience)
        {
            string path =
                AuggioUtils.GetExperienceFilePath(selectedSceneExperience.OrganizationId, selectedSceneExperience.ExperienceId);
            anchorOptions = new List<string>();
            availableAnchorIds = new List<string>();
            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                string json = Encoding.UTF8.GetString(bytes);
                selectedExperienceData = JsonUtility.FromJson<Experience>(json);

                foreach (Location location in selectedExperienceData.AssignedLocations)
                {
                    foreach (SingleAnchor anchor in location.SingleAnchorList)
                    {
                        anchorOptions.Add("[" + location.Name + "] " + anchor.Name);
                        availableAnchorIds.Add(anchor.AuggioId);
                    }
                }
            }
            else
            {
                Debug.LogError("Cannot build anchor options because experience data is missing or corrupted.");
            }
        }

        private void ProcessPresetData()
        {
            if (selectedExperienceOption == -1 && availableExperiences != null && presetExperienceId != null)
            {
                int index = 0;
                foreach (AuggioExperience experience in availableExperiences)
                {
                    if (experience.ExperienceId.Equals(presetExperienceId))
                    {
                        selectedExperienceOption = index;
                        AuggioExperience selectedSceneExperience = availableExperiences[selectedExperienceOption];
                        BuildAnchorOptions(selectedSceneExperience);
                        ProcessPresetAnchor();
                        break;
                    }
                    index++;
                }
            }
        }
        
        private void ProcessPresetAnchor()
        {
            if (selectedAnchorOption == -1 && availableAnchorIds != null && presetAnchorId != null)
            {
                int index = 0;
                foreach (string anchorId in availableAnchorIds)
                {
                    if (anchorId.Equals(presetAnchorId))
                    {
                        selectedAnchorOption = index;
                        break;
                    }
                    index++;
                }
            }
        }
    }
}
#endif