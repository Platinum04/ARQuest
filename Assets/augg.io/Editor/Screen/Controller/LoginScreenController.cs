#if UNITY_EDITOR
using System;
using System.Net.Http;
using Auggio.Plugin.Editor;
using Auggio.Plugin.Editor.Http;
using Auggio.Plugin.Editor.Screen.Controller;
using Auggio.Plugin.Editor.Utils;
using Auggio.Utils.Serialization;
using UnityEditor;
using UnityEngine;
using Version = Auggio.Plugin.SDK.Utils.Version;

namespace Auggio.Plugin.Screen.Controller
{
    internal class LoginScreenController : AbstractScreenController
    {
      
        internal LoginScreenController(EditorWindowController windowController) : base(windowController)
        {
        }
        
        internal override void OnScreenBecomeActive()
        {
            Login();
        }

        internal void Login()
        {
            windowController.SetLoading(true);
            windowController.RootWindow.Errors.Clear();
            if (string.IsNullOrEmpty(windowController.RootWindow.AuggioAPIKey))
            {
                windowController.RootWindow.Errors.Add("Please enter the augg.io API key!");
                windowController.SetLoading(false);
                return;
            }
            WebRequestUtility.WebRequestPlugin(windowController.RootWindow, HttpMethod.Post, "/plugin/login", null, windowController.RootWindow.AuggioAPIKey, OnLoggedIn, OnError);
        }
    
        private void OnLoggedIn(string response)
        {
            PluginLoginResponse loginResponse = JsonUtility.FromJson<PluginLoginResponse>(response);
            windowController.ExperienceScreen.ScreenController.Organizations = loginResponse.AllOrganizations;
            windowController.ExperienceScreen.ScreenController.User = loginResponse.User;
            windowController.SetCurrentScreen(windowController.ExperienceScreen);
            windowController.ExperienceScreen.ScreenController.SelectOrganization();
            APIKeyManager.SetActiveKey(windowController.RootWindow.AuggioAPIKey);
            AssetDatabase.Refresh();
            windowController.SetLoading(false);
        }
    
        private void OnError(string error)
        {
            try
            {
                ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(error);
                if (errorResponse.ErrorCode == ErrorPopupCode.AG010)
                {
                    if (EditorUtility.DisplayDialog("Version Mismatch",
                        errorResponse.Message + " Your version is [" + Version.pluginVersion +
                        "]. You'll need to download proper version in order to use augg.io plugin.",
                        "Take me to downloading page", "I'll download it myself"))
                    {
                        Application.OpenURL("https:/augg.io/download-sdk");
                    }

                    ;
                    windowController.RootWindow.Errors.Add("Can't login due to version mismatch");
                }
                else
                {
                    windowController.RootWindow.Errors.Add(errorResponse.Message);
                }

                windowController.SetLoading(false);
            }
            catch (Exception)
            {
                windowController.RootWindow.Errors.Add(error);
                windowController.SetLoading(false);
            }
        }


        internal void ShowSettings()
        {
            windowController.SetCurrentScreen(windowController.LoginSettingsScreen);
        }
    }
}

#endif
