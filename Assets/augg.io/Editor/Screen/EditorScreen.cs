#if UNITY_EDITOR
using Auggio.Plugin.Editor.Screen.Controller;
using Auggio.Plugin.Editor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Screen
{
    internal abstract class EditorScreen<T> : AbstractEditorScreen where T : AbstractScreenController
    {
     
        protected T screenController;
        private bool drawBackButton;
        private bool useFocusLogic;

        private Color colorBackup;

        protected EditorScreen(AuggioEditorPlugin rootWindow, T screenController, bool drawBackButton, bool useFocusLogic) : base(rootWindow)
        {
            this.screenController = screenController;
            this.drawBackButton = drawBackButton;
            this.useFocusLogic = useFocusLogic;
        }

        internal override void Draw()
        {
            GUILayout.BeginArea(new Rect(16, 16, rootWindow.position.width - 32, rootWindow.position.height - 32));
            if (drawBackButton && screenController.WindowController.HasPreviousScreens())
            {
                GUIUtils.DrawBackButton(screenController.WindowController);
            }
            EditorGUI.BeginDisabledGroup(screenController.WindowController.Loading || (useFocusLogic && !screenController.WindowController.Focused) || EditorApplication.isPlaying);
            DrawScreen();
            EditorGUI.EndDisabledGroup();
            GUILayout.EndArea();
            if (EditorApplication.isPlaying)
            {
                colorBackup = GUI.color;
                GUI.color = new Color(1, 0, 0, 0.1f);
                GUI.DrawTexture(new Rect(0, 0, rootWindow.position.width, rootWindow.position.height), EditorGUIUtility.whiteTexture);
                GUI.color = new Color(1, 1, 1);
                EditorGUI.LabelField(new Rect(rootWindow.position.width/2 - 200, rootWindow.position.height/2 - 16, 400, 32), "Please Exit Play mode to enable interaction", AuggioEditorStyles.Instance.LoadingLabelStyle);
                GUI.color = colorBackup;
            }
             else if (screenController.WindowController.Loading)
            {
                colorBackup = GUI.color;
                GUI.color = new Color(0, 0, 0, 0.3f);
                GUI.DrawTexture(new Rect(0, 0, rootWindow.position.width, rootWindow.position.height), EditorGUIUtility.whiteTexture);
                GUI.color = new Color(1, 1, 1);
                EditorGUI.LabelField(new Rect(rootWindow.position.width/2 - 200, rootWindow.position.height/2 - 16, 400, 32), screenController.WindowController.LoadingText ?? "Loading...", AuggioEditorStyles.Instance.LoadingLabelStyle);
                GUI.color = colorBackup;
                DrawOnLoadingScreen();
            }
            else if (useFocusLogic && !screenController.WindowController.Focused)
            {
                colorBackup = GUI.color;
                GUI.color = new Color(0, 0, 0, 0.3f);
                GUI.DrawTexture(new Rect(0, 0, rootWindow.position.width, rootWindow.position.height), EditorGUIUtility.whiteTexture);
                GUI.color = new Color(1, 1, 1);
                EditorGUI.LabelField(new Rect(rootWindow.position.width/2 - 200, rootWindow.position.height/2 - 16, 400, 32), "'Click' to window to enable interaction", AuggioEditorStyles.Instance.LoadingLabelStyle);
                GUI.color = colorBackup;
            }

            
        }

        internal abstract void DrawOnLoadingScreen();

        internal abstract void DrawScreen();

        internal override void OnBecameActive()
        {
            screenController.OnScreenBecomeActive();
        }

        internal override void OnFocus()
        {
            if (!screenController.WindowController.Loading &&  !EditorApplication.isPlaying && !screenController.WindowController.Focused)
            {
                screenController.OnScreenFocus();
            }
        }

        internal override void OnLostFocus()
        {
            if (screenController.WindowController.Focused)
            {
                screenController.OnScreenLostFocus();
            }
        }

        internal override void OnSceneOpened(Scene scene)
        {
            screenController.OnSceneOpened(scene);
        }

        public T ScreenController => screenController;
    }
}
#endif
