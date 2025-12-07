#if UNITY_EDITOR
using System;
using System.IO;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.SDK
{
    [CustomEditor(typeof(AuggioExperience))]
    internal class AuggioExperienceEditor : UnityEditor.Editor
    {
       
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AuggioExperience auggioExperience = (AuggioExperience) target;
      
            GUILayout.Space(20);
            
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            GUILayout.Label("Actions");
            if (GUILayout.Button("Add new augg.io object"))
            {
                // Check if any instance of CreateAuggioObjectWindow is already open
                CreateAuggioObjectWindow window = CreateAuggioObjectWindow.GetNewWindowInstance();
                window.SetExperienceId(auggioExperience.ExperienceId);
            }
            
            GUILayout.Space(20);
            GUILayout.Label("Additional Tools");
            if (GUILayout.Button("Find experience data in Project Structure"))
            {
                FindInProjectStructure(auggioExperience);
            }
            
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(20);
            
            GUILayout.Label("Experience ID: " + auggioExperience.ExperienceId);
        }

        private void FindInProjectStructure(AuggioExperience auggioExperience)
        {
            string path =
                AuggioUtils.GetExperienceFilePath(auggioExperience.OrganizationId, auggioExperience.ExperienceId, true);
       
            if (!File.Exists(path))
            {
                throw new Exception("Cannot find experience data in Project Structure");
            }

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }
    }
}
#endif