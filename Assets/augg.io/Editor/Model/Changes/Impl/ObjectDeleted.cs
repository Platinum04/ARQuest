#if UNITY_EDITOR
using System;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.SDK.Model;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes.Impl
{
    internal class ObjectDeleted : StringValueChange<AuggioObjectTracker>
    {
      
        internal ObjectDeleted(string experienceId, string objectId, AuggioObjectTracker sceneComponent, string objectName, string oldValue, string newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
        }

        internal override string UIStringName()
        {
            return "Object removed";
        }

        protected override AuggioObjectTracker DiscardChange(AuggioObjectTracker tracker, Scene scene, Experience experience, AuggioObject oldData)
        {
            AuggioExperience existingExperience = AuggioSceneManager.GetExperienceFromSceneById(scene, experience.ID);
            if (existingExperience == null)
            {
                throw new ArgumentException("Missing augg.io experience in scene.");
            }
            return AuggioSceneImporter.ImportAuggioObject(scene, oldData, experience, existingExperience.transform);
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            cachedExperience.Objects.RemoveAll(o => o.AuggioId.Equals(objectId));
        }
    }
}
#endif
