#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Utils
{
    internal static class GUIUtils
    {
        internal static void DrawLineSeparator() { 
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        internal static void DrawBackButton(EditorWindowController windowController)
        {
            if (GUILayout.Button("<", AuggioEditorStyles.Instance.PrimaryButton, GUILayout.Width(32), GUILayout.Height(32)))
            {
                windowController.GoBack();
            }
            GUILayout.Space(32);
        }
    
    }
}
#endif
