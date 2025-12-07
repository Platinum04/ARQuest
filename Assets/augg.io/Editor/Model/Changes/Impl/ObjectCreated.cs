#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Auggio.Plugin.Editor.Utils;
using Auggio.Plugin.SDK.Model;
using Auggio.Utils.Serialization.Model;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Model.Changes.Impl
{
    internal class ObjectCreated : StringValueChange<AuggioObjectTracker>
    {
      
        internal ObjectCreated(string experienceId, string objectId, AuggioObjectTracker sceneComponent, string objectName, string oldValue, string newValue) : base(experienceId, objectId, sceneComponent, objectName, oldValue, newValue)
        {
        }

        internal override string UIStringName()
        {
            return "Object created";
        }

        protected override AuggioObjectTracker DiscardChange(AuggioObjectTracker tracker, Scene scene, Experience experience, AuggioObject oldData)
        {
            GameObject.DestroyImmediate(tracker.gameObject);
            return null;
        }

        internal override void ApplyToExperience(Experience cachedExperience)
        {
            Transform relativeToAnchor =
                AuggioSceneManager.GetAnchorTransformByAnchorId(sceneComponent.gameObject.scene,
                    sceneComponent.AnchorId);

            AuggioObject newObjectData = new AuggioObject();
            newObjectData.AuggioId = sceneComponent.ObjectId;
            newObjectData.IsNewObject = true;
            newObjectData.Name = sceneComponent.ObjectName;
            newObjectData.AssignedAnchor = sceneComponent.AnchorId;
            newObjectData.PlaceholderModels = new List<AuggioObjectPlaceholder>();

            if (sceneComponent.Models != null)
            {
                foreach (AuggioObjectPlaceholderModel model in sceneComponent.Models)
                {
                    AuggioObjectPlaceholder objectPlaceholder = new AuggioObjectPlaceholder();
                    objectPlaceholder.Position = new SerializedVector3(model.transform.localPosition);
                    objectPlaceholder.Rotation = new SerializedVector3(model.transform.localRotation.eulerAngles);
                    objectPlaceholder.Scale = new SerializedVector3(model.transform.localScale);
                    objectPlaceholder.ModelId = model.ModelId;
                    objectPlaceholder.Name = model.PlaceholderName;
                    objectPlaceholder.ID = Guid.NewGuid().ToString();
                    objectPlaceholder.IsNewPlaceholder = true;
                    model.PlaceholderId = objectPlaceholder.ID;
                    newObjectData.PlaceholderModels.Add(objectPlaceholder);
                }
            }
            
            newObjectData.Position =
                new SerializedVector3(
                    RelativePositionCalculator.InversedObjectPositionRelativeToAnchor(relativeToAnchor,
                        sceneComponent.transform.position));
            newObjectData.Rotation =
                new SerializedVector3(
                    RelativePositionCalculator.InversedObjectRotationRelativeToAnchor(relativeToAnchor,
                        sceneComponent.transform.rotation).eulerAngles);
            newObjectData.Scale =
                new SerializedVector3(
                    RelativePositionCalculator.InversedObjectScaleRelativeToAnchor(relativeToAnchor,
                        sceneComponent.transform.localScale));
            
            cachedExperience.Objects.Add(newObjectData);

        }
    }
}
#endif
