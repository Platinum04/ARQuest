#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Auggio.Plugin.SDK.Model;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Auggio
{
    public class AnchorCountBuildPreprocessor : IPreprocessBuildWithReport
    {
        private const int MAX_ANCHORS = 40;
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            // Get the scenes in the build
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            foreach (var scene in scenes)
            {
                if (scene.enabled)
                {
                    // Load the scene
                    string scenePath = scene.path;
                    Scene loadedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                    // Find all AuggioAnchor components in the scene
                    AuggioObjectTracker[] objectTrackers = Object.FindObjectsByType<AuggioObjectTracker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    
                    // Collect unique parentAnchor references
                    HashSet<string> uniqueParentAnchorIds = new HashSet<string>();

                    foreach (AuggioObjectTracker tracker in objectTrackers)
                    {
                        if (tracker.AnchorId != null)
                        {
                            uniqueParentAnchorIds.Add(tracker.AnchorId);
                        }
                    }

                    // Check the count
                    if (uniqueParentAnchorIds.Count > MAX_ANCHORS)
                    {
                        bool cancelBuild = EditorUtility.DisplayDialog("Warning: Too many anchors might be resolved at the same time",
                            $"Scene '{loadedScene.name}' contains augg.io objects parented to {uniqueParentAnchorIds.Count} unique anchors.\n\nHowever augg.io can resolve a maximum of {MAX_ANCHORS} anchors simultaneously. " +
                            $"\n\nIf you are confident that no more than {MAX_ANCHORS} anchors will be resolved at any given time, you may proceed with the build.",
                            "Cancel Build", "Continue Anyway");

                        if (cancelBuild)
                        {
                            throw new BuildFailedException("Build cancelled due to too many anchors in scene.");
                        }
                    }
                }
            }
        }
    }
}
#endif