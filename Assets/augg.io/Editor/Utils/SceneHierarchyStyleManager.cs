#if UNITY_EDITOR
using Auggio.Plugin.Editor.Validation.Validators;
using Auggio.Plugin.SDK.Model;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Utils
{
    [InitializeOnLoad]
    public class SceneHierarchyStyleManager
    {
        static SceneHierarchyStyleManager()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawColoredName;
        }
        
        static void DrawColoredName(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            DrawAuggioObjectName(obj, selectionRect);
            DrawAuggioObjectPlaceholderName(obj, selectionRect);
        }

        private static void DrawAuggioObjectName(GameObject gameObject, Rect selectionRect)
        {
            AuggioObjectTracker tracker = gameObject.GetComponent<AuggioObjectTracker>();
            if (tracker == null)
            {
                return;
            }

            
            AuggioObjectTrackerValidator objectTrackerValidator = new AuggioObjectTrackerValidator();

            bool errorIconVisible = false;
            var errors = objectTrackerValidator.Validate(tracker);
            if (errors.Count > 0)
            {
                Rect iconRect = new Rect(selectionRect.xMax - 16, selectionRect.y, 16, 16);
                GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("console.erroricon").image);
                errorIconVisible = true;
            }
            
            if (Utils.IsValidUUID(tracker.ObjectId))
            {
                float xOffset = errorIconVisible ? 52 : 28;
                //means new object
                Rect iconRect = new Rect(selectionRect.xMax - xOffset, selectionRect.y + 2, 32, 12);
                GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("d_AS Badge New").image);
            }
        }
        
        private static void DrawAuggioObjectPlaceholderName(GameObject gameObject, Rect selectionRect)
        {
            AuggioObjectPlaceholderModel model = gameObject.GetComponent<AuggioObjectPlaceholderModel>();
            if (model == null)
            {
                return;
            }

            AuggioObjectPlaceholderValidator validator = new AuggioObjectPlaceholderValidator();
            var errors = validator.Validate(model);
            bool errorIconVisible = false;
            if (errors.Count > 0)
            {
                Rect iconRect = new Rect(selectionRect.xMax - 16, selectionRect.y, 16, 16);
                GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("console.erroricon").image);
                errorIconVisible = true;
            }
            
            if (Utils.IsValidUUID(model.PlaceholderId))
            {
                float xOffset = errorIconVisible ? 52 : 28;
                //means new object
                Rect iconRect = new Rect(selectionRect.xMax - xOffset, selectionRect.y + 2, 32, 12);
                GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("d_AS Badge New").image);
            }
        }
        
        
    }
}
#endif
