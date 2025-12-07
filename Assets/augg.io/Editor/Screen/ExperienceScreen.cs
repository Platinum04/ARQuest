#if UNITY_EDITOR
using System.Linq;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.Screen.Controller;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Screen
{
    internal class ExperienceScreen : EditorScreen<ExperienceScreenController>
    {
        private int selectedOrganizationIndex = 0;
        private int selectedWorkspaceIndex = 0;
        private Vector2 _downloadListScrollPosition;


        internal ExperienceScreen(AuggioEditorPlugin rootWindow, ExperienceScreenController screenController, bool drawBackButton, bool useFocusLogic) : base(rootWindow, screenController, drawBackButton, useFocusLogic)
        {
        }

        protected override void Initialize()
        {
        }

        internal override void DrawOnLoadingScreen()
        {
        }

        internal override void DrawScreen()
        {
            GUIUtils.DrawLineSeparator();
            GUILayout.Space(8);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            DrawOrganizationSelect();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            DrawUserInfo();
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
            GUIUtils.DrawLineSeparator();
            DrawExperiences();
            GUILayout.EndVertical();
        }

        private void DrawExperiences()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Experiences", AuggioEditorStyles.Instance.H1);
            GUILayout.FlexibleSpace();
            DrawWorkspaceSelect();
            GUILayout.EndHorizontal();
            GUILayout.Space(16);
            
            if (screenController.PluginExperiencesResponse == null || screenController.PluginExperiencesResponse.Experiences.Count == 0)
            {
                GUILayout.Label("No Experiences available", AuggioEditorStyles.Instance.BoldText);
                return;
            }
            
            _downloadListScrollPosition = GUILayout.BeginScrollView(_downloadListScrollPosition, GUILayout.ExpandHeight(true));
            int index = 0;
            foreach (ExperiencePreview experiencePlugin in screenController.PluginExperiencesResponse.Experiences)
            {
                DrawExperienceButton(experiencePlugin, index);
                GUILayout.Space(8);
                index++;
            }
            GUILayout.EndScrollView();
        }

        private void DrawExperienceButton(ExperiencePreview experiencePreview, int index)
        {
            if (GUILayout.Button(experiencePreview.Name, AuggioEditorStyles.Instance.ExperienceButtonSkinStyle, GUILayout.Height(64)))
            {
                screenController.SelectExperience(experiencePreview.ID);
            }
            if (experiencePreview.PresentInScene)
            {
                float xPos = rootWindow.position.width - 124;
                float yPos = (index * 74) + 28;
                GUI.DrawTexture(new Rect(xPos,yPos,16,16), EditorGUIUtility.FindTexture("d_Scene"));
                GUI.Label(new Rect(xPos + 20, yPos, 72, 16), "(in scene)");
            }
        }

        private void DrawOrganizationSelect()
        {
            if (screenController.Organizations == null || screenController.Organizations.Count == 0)
            {
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.Label("Selected organization: ", AuggioEditorStyles.Instance.NormalText);
            EditorGUI.BeginChangeCheck();
            selectedOrganizationIndex = EditorGUILayout.Popup("", selectedOrganizationIndex,
                screenController.Organizations.Select(preview => preview.Name).ToArray(), AuggioEditorStyles.Instance.DropDown, GUILayout.MinWidth(300), GUILayout.MinHeight(32));
            if (EditorGUI.EndChangeCheck())
            {
                screenController.SelectOrganization();
            }

            GUILayout.EndVertical();
        }
        
        private void DrawWorkspaceSelect()
        {
            if (screenController.Workspaces == null || screenController.Workspaces.Count == 0)
            {
                return;
            }

            selectedWorkspaceIndex =
                screenController.Workspaces.FindIndex(workspace =>
                    workspace.ID == screenController.SelectedWorkspaceId);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Workspace: ", AuggioEditorStyles.Instance.NormalText, GUILayout.MinHeight(32));
            EditorGUI.BeginChangeCheck();
            selectedWorkspaceIndex = EditorGUILayout.Popup("", selectedWorkspaceIndex,
                screenController.Workspaces.Select(preview => preview.Name).ToArray(), AuggioEditorStyles.Instance.DropDown, GUILayout.MinWidth(225), GUILayout.MinHeight(32));
            if (EditorGUI.EndChangeCheck())
            {
                screenController.SelectWorkspace(selectedWorkspaceIndex);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawUserInfo()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (screenController.User != null)
            {
                GUILayout.Label(screenController.User.Email, AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleCenter));
            }

            if (screenController.SelectOrganizationResponse != null)
            {
                GUILayout.Label("(" + screenController.SelectOrganizationResponse.OrganizationRole + ")", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleCenter));
            }
            
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIContent settingsButtonContent = EditorGUIUtility.IconContent("SettingsIcon");
            settingsButtonContent.text = "Settings";
            if (GUILayout.Button(settingsButtonContent, AuggioEditorStyles.Instance.PrimaryButton))
            {
                screenController.ShowSettings();
            }
            if (screenController.User != null)
            {
                GUIContent logoutContent = EditorGUIUtility.IconContent("d_account");
                logoutContent.text = "Logout";
                if (GUILayout.Button(logoutContent, AuggioEditorStyles.Instance.PrimaryButton, GUILayout.MinWidth(150)))
                {
                    screenController.Logout();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

          
        }


       
    }
}
#endif
