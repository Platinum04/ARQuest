#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using augg.io.Serialization.Plugin.Workspace;
using Auggio.Plugin.Editor;
using Auggio.Plugin.Editor.Http;
using Auggio.Plugin.Editor.Screen.Controller;
using Auggio.Plugin.Editor.Utils;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Screen.Controller
{
    internal class ExperienceScreenController : AbstractScreenController
    {
    
        private List<OrganizationPreview> organizations;
        private List<WorkspacePreview> workspaces;
        private int selectedOrganizationIndex = 0;
        private string selectedWorkspaceId;
        private UserPreview user;
        private SelectOrganizationResponse selectOrganizationResponse;
        private PluginExperiencesResponse pluginExperiencesResponse;

        public ExperienceScreenController(EditorWindowController windowController) : base(windowController)
        {
        }
        
        internal override void OnScreenBecomeActive()
        {
            if (selectOrganizationResponse != null && selectedWorkspaceId != null) //make sure its not called twice when logging in
            {
                FetchExperiences(selectedWorkspaceId);
            }
        }

        internal override void OnScreenFocus()
        {
            CheckIfExperiencesAreInActiveScene();
        }

        internal override void OnSceneOpened(Scene scene)
        {
            CheckIfExperiencesAreInActiveScene();
        }

        internal void SelectOrganization()
        {
            windowController.SetLoading(true);
            WebRequestUtility.WebRequestPlugin(windowController.RootWindow, HttpMethod.Post, "/plugin/selectOrganization", JsonUtility.ToJson(new OrganizationIdRequest()
            {
                OrganizationId = organizations[selectedOrganizationIndex].Id
            }), windowController.RootWindow.AuggioAPIKey, (response) =>
            {
                OnOrganizationSelected(organizations[selectedOrganizationIndex].Id, response);
            }, OnError);
        }

        internal void SelectWorkspace(int selectedWorkspaceIndex)
        {
            windowController.SetLoading(true);
            selectedWorkspaceId = workspaces[selectedWorkspaceIndex].ID;
            FetchExperiences(selectedWorkspaceId);
            
        }

        internal void SelectExperience(string experienceId)
        {
            windowController.SetLoading(true);
            windowController.RootWindow.Errors.Clear();
            WebRequestUtility.WebRequestPlugin(windowController.RootWindow, HttpMethod.Get, "/plugin/experience/"+experienceId, null, windowController.RootWindow.AuggioAPIKey, OnExperienceFetched, OnError);
        }

        private void OnExperienceFetched(string response)
        {
            Experience experience = JsonUtility.FromJson<Experience>(response);
            
            experience.AssignedLocations.Sort((location, otherLocation) => string.Compare(location.Name, otherLocation.Name, StringComparison.Ordinal));
            
            windowController.ExperienceDetailScreen.ScreenController.Experience = experience;
            windowController.SetCurrentScreen(windowController.ExperienceDetailScreen);
            windowController.SetLoading(false);
        }

        private void OnOrganizationSelected(string organizationId, string response)
        {
            selectOrganizationResponse =
                JsonUtility.FromJson<SelectOrganizationResponse>(response);
            windowController.RootWindow.SelectedOrganizationId = organizationId;
            FetchWorkspaces();
        }

        private void FetchWorkspaces()
        {
            windowController.SetLoading(true);
            WebRequestUtility.WebRequestPlugin(windowController.RootWindow, HttpMethod.Post, "/plugin/workspaces", JsonUtility.ToJson(new OrganizationIdRequest()
            {
                OrganizationId = organizations[selectedOrganizationIndex].Id
            }), windowController.RootWindow.AuggioAPIKey, OnWorkspacesFetched, OnError);
        }

        private void FetchExperiences(string workspaceId)
        {
            windowController.SetLoading(true);
            WebRequestUtility.WebRequestPlugin(windowController.RootWindow, HttpMethod.Post, "/plugin/experiences", JsonUtility.ToJson(new PluginExperiencesRequest()
            {
                OrganizationId = organizations[selectedOrganizationIndex].Id,
                WorkspaceId = workspaceId
            }), windowController.RootWindow.AuggioAPIKey, OnExperiencesFetched, OnError);
        }
    
        private void OnExperiencesFetched(string response)
        {
            pluginExperiencesResponse = JsonUtility.FromJson<PluginExperiencesResponse>(response);
            CheckIfExperiencesAreInActiveScene();
            windowController.SetLoading(false);
        }
        
        private void OnWorkspacesFetched(string response)
        {
            workspaces = JsonUtility.FromJson<PluginWorkspacesResponse>(response).Workspaces;
            if (selectedWorkspaceId == null)
            {
                SelectWorkspace(0);
            }
            else
            {
                windowController.SetLoading(false);
            }
        }

        private void CheckIfExperiencesAreInActiveScene()
        {
            if (pluginExperiencesResponse == null)
            {
                return;
            }
            foreach (ExperiencePreview experiencePlugin in pluginExperiencesResponse.Experiences)
            {
                experiencePlugin.PresentInScene = IsInActiveScene(experiencePlugin.ID);
            }
        }

        internal void ShowSettings()
        {
            windowController.SetCurrentScreen(windowController.SettingsScreen);
        }
    
        internal void Logout()
        {
            windowController.RootWindow.AuggioAPIKey = null;
            windowController.RootWindow.SelectedOrganizationId = null;
            pluginExperiencesResponse = null;
            selectOrganizationResponse = null;
            selectedOrganizationIndex = 0;
            user = null;
            organizations = null;
            workspaces = null;
            selectedWorkspaceId = null;
            windowController.SetCurrentScreen(windowController.LoginScreen);
        }

        private void OnError(string error)
        {
            windowController.RootWindow.Errors.Add(error);
            windowController.SetLoading(false);
        }
        
        private bool IsInActiveScene(string experienceID)
        {
            return (AuggioSceneManager.GetExperienceFromSceneById(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), experienceID) != null);
        }
        
        internal List<OrganizationPreview> Organizations
        {
            get => organizations;
            set => organizations = value;
        }
        
        internal List<WorkspacePreview> Workspaces
        {
            get => workspaces;
            set => workspaces = value;
        }


        internal UserPreview User
        {
            get => user;
            set => user = value;
        }

        internal SelectOrganizationResponse SelectOrganizationResponse
        {
            get => selectOrganizationResponse;
            set => selectOrganizationResponse = value;
        }

        internal PluginExperiencesResponse PluginExperiencesResponse
        {
            get => pluginExperiencesResponse;
            set => pluginExperiencesResponse = value;
        }

        internal string SelectedWorkspaceId
        {
            get => selectedWorkspaceId;
            set => selectedWorkspaceId = value;
        }
     
    }
}

#endif
