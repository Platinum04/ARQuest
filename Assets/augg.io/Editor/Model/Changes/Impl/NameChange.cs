#if UNITY_EDITOR
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes.Impl
{
    internal class NameChange : StringValueChange<AuggioObjectTracker>
    {
        internal NameChange(string experienceId, string objectId, AuggioObjectTracker sceneComponent, string objectName, string oldValue, string newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
        }

        internal override string UIStringName()
        {
            return "Object renamed";
        }

        protected override AuggioObjectTracker DiscardChange(AuggioObjectTracker tracker, Scene scene, Experience experience, AuggioObject oldData)
        {
            tracker.ObjectName = oldData.Name;
            tracker.gameObject.name = AuggioUtils.GetGameObjectName(oldData.Name);
            return tracker;
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            AuggioObject objectData = cachedExperience.FindObjectByObjectId(objectId);
            objectData.Name = newValue;
        }
    }
}
#endif
