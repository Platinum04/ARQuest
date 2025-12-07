using System.Collections;
using System.Collections.Generic;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils.DetachStrategy;
using UnityEditor;
using UnityEngine;

namespace Auggio.Plugin.Editor.Utils
{
    [InitializeOnLoad]
    public class DisableTransformGizmoGlobal
    {
        static DisableTransformGizmoGlobal()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged()
        {
            if (Selection.activeTransform != null && ShouldBeHidden(Selection.activeTransform))
            {
                Selection.activeTransform.hideFlags = HideFlags.NotEditable;
                Tools.hidden = true;
            }
            else
            {
                Tools.hidden = false;
            }
            
            if (Selection.activeTransform != null && Selection.activeTransform.GetComponent<AuggioPlaneFinder>() != null)
            {
                Selection.activeTransform.GetComponent<AuggioPlaneFinder>().transform.hideFlags = HideFlags.None;
                Selection.activeTransform.GetComponent<BoxCollider>().hideFlags =
                    HideFlags.NotEditable;
            }
        }

        private static bool ShouldBeHidden(Transform transform)
        {
            if (transform.GetComponent<VisualizationHierarchy>() != null)
            {
                return true;
            }

            if (transform.GetComponent<AuggioLocation>() != null)
            {
                return true;
            }

            if (transform.GetComponent<AuggioAnchor>() != null)
            {
                return true;
            }

            if (transform.GetComponentInParent<AuggioAnchor>() != null)
            {
                return true;
            }

            return false;
        }
    }
}
