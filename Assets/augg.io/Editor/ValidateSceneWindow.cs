#if UNITY_EDITOR
using System.Collections.Generic;
using Auggio.Plugin.Editor.Utils;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor
{
    public class ValidateSceneWindow : EditorWindow
    {

        private Experience experience;
        private List<GameObject> objectsWithErrors;
        private Vector2 _errorsListScrollPosition;

        public static void ShowWindow(Experience experience)
        {
            ValidateSceneWindow window = GetWindow<ValidateSceneWindow>("augg.io Scene Validation");
            window.experience = experience;
            window.objectsWithErrors = AuggioSceneManager.GetGameObjectsWithErrors(experience);
        }

        private void OnGUI()
        {
            if (experience == null || objectsWithErrors == null)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Missing data. Please re-open the window.");
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            else
            {
                DrawErrorsTable();
            }
        }
        
        private void DrawErrorsTable()
        {
            if (experience == null || objectsWithErrors == null)
            {
                return;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Errors in "+experience.Name, AuggioEditorStyles.Instance.H2);
            GUILayout.FlexibleSpace();
            
            GUIContent refreshButtonContent = EditorGUIUtility.IconContent("TreeEditor.Refresh");
            refreshButtonContent.text = "Force Refresh";
            if (GUILayout.Button(refreshButtonContent, AuggioEditorStyles.Instance.PrimaryButton))
            {
                objectsWithErrors = AuggioSceneManager.GetGameObjectsWithErrors(experience);
            }
           
            GUILayout.EndHorizontal();

            DrawErrorsHeader();

            if (objectsWithErrors.Count > 0)
            {
                _errorsListScrollPosition = GUILayout.BeginScrollView(_errorsListScrollPosition,
                    GUILayout.ExpandHeight(true));
                bool hasNullValues = false;
                foreach (GameObject gameObjectWithError in objectsWithErrors)
                {
                    if (gameObjectWithError == null)
                    {
                        hasNullValues = true;
                        continue;
                    }
                    DrawErrorRow(gameObjectWithError);
                }

                if (hasNullValues)
                {
                    objectsWithErrors.RemoveAll(o => o == null);
                    hasNullValues = false;
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No errors found", AuggioEditorStyles.WithMargin(
                    AuggioEditorStyles.WithTextAlignment(
                        AuggioEditorStyles.Instance.H3,
                        TextAnchor.MiddleCenter)
                    , new RectOffset(0,0,16,16)));
            }
        }
        
        private void DrawErrorsHeader()
        {
            //TODO refactor styles?
            GUILayout.Space(1);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(AuggioEditorStyles.Instance.TableHeaderStyle);
            GUILayout.Label("Game object", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter), GUILayout.Width(position.width*0.45f), GUILayout.Height(48));
            GUILayout.Label("Actions", AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.BoldText, TextAnchor.MiddleCenter),GUILayout.Width(position.width*0.45f), GUILayout.Height(48));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(1);
        }
        
         private void DrawErrorRow(GameObject objectWithError)
        {
            
            //TODO refactor styles?
            GUILayout.Space(1);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(AuggioEditorStyles.Instance.TableRowStyle);
            GUILayout.Label(objectWithError.name, AuggioEditorStyles.WithTextAlignment(AuggioEditorStyles.Instance.NormalText, TextAnchor.MiddleCenter), GUILayout.Width(position.width*0.45f), GUILayout.Height(48));
            GUILayout.BeginHorizontal(GUILayout.Width(position.width*0.49f), GUILayout.Height(48));

            if (GUILayout.Button("Select in Scene",
                AuggioEditorStyles.WithMargin(AuggioEditorStyles.Instance.PrimaryButton, new RectOffset(2, 2, 8, 8)),
                GUILayout.Height(48)))
            {
                EditorGUIUtility.PingObject(objectWithError);
                Selection.activeGameObject = objectWithError;
            }
           
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(1);
        }
       
    }
}
#endif