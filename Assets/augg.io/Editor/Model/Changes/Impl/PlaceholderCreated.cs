#if UNITY_EDITOR
using System;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.SDK.Model;
using Auggio.Utils.Serialization.Model;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes.Impl
{
    internal class PlaceholderCreated : StringValueChange<AuggioObjectPlaceholderModel>
    {
      
        internal PlaceholderCreated(string experienceId, string objectId, AuggioObjectPlaceholderModel sceneComponent, string objectName, string oldValue, string newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
        }

        internal override string UIStringName()
        {
            return "New object placeholder model \n[" + (string.IsNullOrEmpty(sceneComponent.PlaceholderName) ? "empty name" : sceneComponent.PlaceholderName) + "]";
        }

        protected override AuggioObjectPlaceholderModel DiscardChange(AuggioObjectPlaceholderModel model, Scene scene, Experience experience, AuggioObject oldData)
        {
            AuggioObjectTracker tracker = AuggioSceneManager.GetAuggioObjectTrackerFromScene(scene, experienceId, objectId);
            tracker.Models.RemoveAll(placeholderModel => placeholderModel.Equals(model));
            GameObject.DestroyImmediate(model.gameObject);
            return null;
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            AuggioObjectPlaceholder newPlaceholderData = new AuggioObjectPlaceholder();
            newPlaceholderData.ID = sceneComponent.PlaceholderId;
            newPlaceholderData.Name = sceneComponent.PlaceholderName;
            newPlaceholderData.Position = new SerializedVector3(sceneComponent.transform.localPosition);
            newPlaceholderData.Rotation = new SerializedVector3(sceneComponent.transform.localRotation.eulerAngles);
            newPlaceholderData.Scale = new SerializedVector3(sceneComponent.transform.localScale);
            newPlaceholderData.ModelId = sceneComponent.ModelId;
            newPlaceholderData.IsNewPlaceholder = true;
            sceneComponent.PlaceholderId = newPlaceholderData.ID;

            cachedExperience.FindObjectByObjectId(objectId).PlaceholderModels.Add(newPlaceholderData);
        }
    }
}
#endif
