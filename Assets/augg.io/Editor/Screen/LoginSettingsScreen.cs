#if UNITY_EDITOR
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.Screen.Controller;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Screen
{
    internal class LoginSettingsScreen : EditorScreen<LoginSettingsScreenController>
    {

        internal LoginSettingsScreen(AuggioEditorPlugin rootWindow, LoginSettingsScreenController screenController, bool drawBackButton, bool useFocusLogic) : base(rootWindow, screenController, drawBackButton, useFocusLogic)
        {
        }

        protected override void Initialize()
        {
        }

        internal override void OnBecameActive()
        {
        }

        internal override void DrawOnLoadingScreen()
        {
        }

        internal override void DrawScreen()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Settings", AuggioEditorStyles.Instance.H1);
            GUIUtils.DrawLineSeparator();
            DrawServerUrlField();
            GUILayout.EndVertical();
        }

        private void DrawServerUrlField()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Server URL", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleLeft), GUILayout.Width(250));
            rootWindow.ServerUrl = GUILayout.TextField(rootWindow.ServerUrl, AuggioEditorStyles.Instance.TextField);
            if (GUILayout.Button("Restore default", GUILayout.Width(150)))
            {
                rootWindow.ServerUrl = AuggioEditorPlugin.SERVER_URL;
            };
            GUILayout.EndHorizontal();
        }



       
    }
}
#endif
