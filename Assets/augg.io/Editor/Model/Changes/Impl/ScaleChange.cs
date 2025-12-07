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
    internal class ScaleChange : Vector3ValueChange<AuggioObjectTracker>
    {
      
        internal ScaleChange(string experienceId, string objectId, AuggioObjectTracker sceneComponent, string objectName, SerializedVector3 oldValue, SerializedVector3 newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
        }

        internal override string UIStringName()
        {
            return "Object Scale changed";
        }

        protected override AuggioObjectTracker DiscardChange(AuggioObjectTracker tracker, Scene scene, Experience experience, AuggioObject oldData)
        {
            Transform relativeToAnchor = AuggioSceneManager.GetAnchorTransformByAnchorId(scene, oldData.AssignedAnchor);
            tracker.transform.localScale = RelativePositionCalculator.ObjectScaleRelativeToAnchor(relativeToAnchor, oldData);
            return tracker;
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            AuggioObject objectData = cachedExperience.FindObjectByObjectId(objectId);
            objectData.Scale = newValue;
        }
    }
}
#endif
