#if UNITY_EDITOR
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.SDK.Model;

using Auggio.Utils.Serialization.Model;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes.Impl
{
    internal class PositionChange : Vector3ValueChange<AuggioObjectTracker>
    {
      
        internal PositionChange(string experienceId, string objectId, AuggioObjectTracker sceneComponent, string objectName, SerializedVector3 oldValue, SerializedVector3 newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
        }

        internal override string UIStringName()
        {
            return "Object Position changed";
        }

        protected override AuggioObjectTracker DiscardChange(AuggioObjectTracker tracker, Scene scene, Experience experience, AuggioObject oldData)
        {
            if (string.IsNullOrEmpty(oldData.AssignedAnchor))
            {
                string error =
                    "Cannot discard change because old data are missing anchor which the old value is relative to.";
                AuggioEditorPlugin.ShowErrorDialog(error);
                Debug.LogError(error);
                return tracker;
            }
            Transform relativeToAnchor = AuggioSceneManager.GetAnchorTransformByAnchorId(scene, oldData.AssignedAnchor);
            tracker.transform.position = RelativePositionCalculator.ObjectPositionRelativeToAnchor(relativeToAnchor, oldData);
            return tracker;
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            AuggioObject objectData = cachedExperience.FindObjectByObjectId(objectId);
            objectData.Position = newValue;
        }
    }
}
#endif
