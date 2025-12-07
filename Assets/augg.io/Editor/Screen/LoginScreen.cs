#if UNITY_EDITOR

using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.Screen.Controller;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Screen
{
    internal class LoginScreen : EditorScreen<LoginScreenController>
    {
        private Sprite logo;
  
        internal LoginScreen(AuggioEditorPlugin rootWindow, LoginScreenController screenController, bool drawBackButton, bool useFocusLogic) : base(rootWindow, screenController, drawBackButton, useFocusLogic)
        {
        }
        
        protected override void Initialize()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(rootWindow));
            string spriteFolderPath = scriptPath.Substring(0, scriptPath.LastIndexOf('/') + 1);
            string spritePath = spriteFolderPath + "auggio_logo.png";
            logo = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        }

        internal override void DrawOnLoadingScreen()
        {
        }

        internal override void DrawScreen()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIContent settingsButtonContent = EditorGUIUtility.IconContent("SettingsIcon");
            settingsButtonContent.text = string.Empty;
            if (GUILayout.Button(settingsButtonContent, AuggioEditorStyles.Instance.PrimaryButton))
            {
                screenController.ShowSettings();
            }
            GUILayout.EndHorizontal();
            
                Rect spriteRect = new Rect(
                rootWindow.position.size.x/2 - logo.rect.width/2 - 16,
                rootWindow.position.size.y/2 - logo.rect.height/2 - 150,
                logo.rect.width,
                logo.rect.height
            );
            GUI.DrawTexture(spriteRect, logo.texture, ScaleMode.ScaleToFit);
           
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.BeginVertical();
            
            GUILayout.Label("Unity Editor Plugin", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.H1, TextAnchor.MiddleCenter));
            GUILayout.Space(10);
            GUILayout.Label("augg.io API key", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleCenter));
            rootWindow.AuggioAPIKey = GUILayout.TextField(rootWindow.AuggioAPIKey, AuggioEditorStyles.Instance.TextField);
            GUILayout.Space(10);
            if (GUILayout.Button("Continue", AuggioEditorStyles.Instance.PrimaryButton))
            {
                screenController.Login();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
           
        
        }


        
    }
}
#endif