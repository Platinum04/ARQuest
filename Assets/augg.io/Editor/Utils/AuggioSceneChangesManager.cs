#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Auggio.Plugin.Editor.Model.Changes;
using Auggio.Plugin.Editor.Model.Changes.Impl;
using Auggio.Plugin.Editor.SDK;
using Auggio.Plugin.Editor.Validation;
using Auggio.Plugin.Editor.Validation.Validators;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;
using UnityEngine.SceneManagement;
using SerializedVector3 = Auggio.Utils.Serialization.Model.SerializedVector3;

namespace Auggio.Plugin.Editor.Utils
{
    internal static class AuggioSceneChangesManager
    {
        internal static void DiscardChange(Scene scene, Experience experience,
            AuggionObjectChangeBase auggioObjectChange)
        {
            ExperienceFileCache experienceFileCache = new ExperienceFileCache(experience.OrganizationId, experience.ID);
            Experience cachedExperience = experienceFileCache.Load();

            try
            {
                AuggioObject oldData = cachedExperience.FindObjectByObjectId(auggioObjectChange.ObjectId);
                auggioObjectChange.DiscardChange(scene, experience, oldData);
            }
            catch (ArgumentException)
            {
                //object might be new
                auggioObjectChange.DiscardChange(scene, experience, null);
            }
        }

        internal static List<AuggionObjectChangeBase> GetLocalChanges(Scene scene, Experience experience)
        {
            AuggioExperience sceneExperience = AuggioSceneManager.GetExperienceFromSceneById(scene, experience.ID);
            if (sceneExperience == null)
            {
                return null;
            }

            ExperienceFileCache experienceFileCache = new ExperienceFileCache(experience.OrganizationId, experience.ID);
            List<AuggionObjectChangeBase> localChanges = new List<AuggionObjectChangeBase>();
            List<AuggioObjectTracker> objectTrackers =
                AuggioSceneManager.GetAuggioObjectTrackersByExperienceId(scene, experience.ID);
            Experience cachedExperience = experienceFileCache.Load();

            foreach (AuggioObjectTracker objectTracker in objectTrackers)
            {
                if (cachedExperience.Objects.All(obj => obj.AuggioId != objectTracker.ObjectId))
                {
                    localChanges.Add(new ObjectCreated(experience.ID,
                        objectTracker.ObjectId,
                        objectTracker,
                        objectTracker.gameObject.name,
                        "",
                        "Created")
                    );
                    continue;
                }

                AuggioObject objectData = cachedExperience.FindObjectByObjectId(objectTracker.ObjectId);
                localChanges.AddRange(GetBasicChanges(experience, objectTracker, objectData));
                localChanges.AddRange(GetPlaceholdersLocalChanges(experience.ID, objectTracker, objectData));
                localChanges.AddRange(GetPlaceholderRemovalChanges(experience.ID, objectTracker, objectData));
                localChanges.AddRange(GetTransformChanges(scene, experience.ID, objectTracker, objectData));
                
            }

            localChanges.AddRange(GetObjectRemovalChanges(experience, cachedExperience, objectTrackers));

            RemoveChangesWithErrors(scene, localChanges);

            return localChanges;
        }

        private static void RemoveChangesWithErrors(Scene scene, List<AuggionObjectChangeBase> localChanges)
        {
            localChanges.RemoveAll(change =>
            {
                if (change is ObjectDeleted || change is PlaceholderDeleted)
                {
                    return false;
                }

                GameObject gameObject = change.SceneGameObject;
                AuggioObjectTracker tracker = gameObject.GetComponent<AuggioObjectTracker>();
                AuggioObjectPlaceholderModel placeholderModel = gameObject.GetComponent<AuggioObjectPlaceholderModel>();
                
                if (tracker != null)
                {
                    AuggioObjectTrackerValidator objectTrackerValidator = new AuggioObjectTrackerValidator();
                    var errors = objectTrackerValidator.Validate(tracker);
                    return errors.Count > 0;
                }

                if (placeholderModel != null)
                {
                    AuggioObjectPlaceholderValidator placeholderValidator = new AuggioObjectPlaceholderValidator();
                    var errors = placeholderValidator.Validate(placeholderModel);
                    return errors.Count > 0;
                }

                return false;
            });
        }

        private static List<AuggionObjectChangeBase> GetObjectRemovalChanges(Experience experience,
            Experience cachedExperience, List<AuggioObjectTracker> objectTrackers)
        {
            List<AuggionObjectChangeBase> localChanges = new List<AuggionObjectChangeBase>();
            if (objectTrackers != null)
            {
                foreach (AuggioObject auggioObject in cachedExperience.Objects)
                {
                    if (!objectTrackers.Any(tracker =>
                        tracker.ObjectId != null && tracker.ObjectId.Equals(auggioObject.AuggioId)))
                    {
                        localChanges.Add(new ObjectDeleted(
                            experience.ID,
                            auggioObject.AuggioId,
                            null,
                            AuggioUtils.GetGameObjectName(auggioObject.Name),
                            "Present",
                            "Removed")
                        );
                    }
                }
            }

            return localChanges;
        }

        private static List<AuggionObjectChangeBase> GetBasicChanges(Experience experience,
            AuggioObjectTracker objectTracker, AuggioObject cachedObjectData)
        {
            List<AuggionObjectChangeBase> localChanges = new List<AuggionObjectChangeBase>();
            if (!objectTracker.ObjectName.Equals(cachedObjectData.Name))
            {
                localChanges.Add(new NameChange(
                    experience.ID,
                    objectTracker.ObjectId,
                    objectTracker,
                    objectTracker.gameObject.name,
                    cachedObjectData.Name,
                    objectTracker.ObjectName)
                );
            }

            if (!string.IsNullOrEmpty(objectTracker.AnchorId) &&
                !objectTracker.AnchorId.Equals(cachedObjectData.AssignedAnchor))
            {
                localChanges.Add(new AssignedAnchorChange(experience,
                    objectTracker.ObjectId,
                    objectTracker,
                    objectTracker.gameObject.name,
                    cachedObjectData.AssignedAnchor,
                    objectTracker.AnchorId)
                );
            }

            return localChanges;
        }

        private static List<AuggionObjectChangeBase> GetTransformChanges(Scene scene, string experienceId,
            AuggioObjectTracker objectTracker, AuggioObject cachedObjectData)
        {
            if (string.IsNullOrEmpty(objectTracker.AnchorId))
            {
                //following changes needs to have anchor assigned
                return new List<AuggionObjectChangeBase>();
            }

            List<AuggionObjectChangeBase> localChanges = new List<AuggionObjectChangeBase>();
            Transform relativeToAnchor =
                AuggioSceneManager.GetAnchorTransformByAnchorId(scene, objectTracker.AnchorId);
            Vector3 calculatedPosition =
                RelativePositionCalculator.InversedObjectPositionRelativeToAnchor(relativeToAnchor,
                    objectTracker.transform.position);
            Vector3 calculatedRotation = RelativePositionCalculator
                .InversedObjectRotationRelativeToAnchor(relativeToAnchor, objectTracker.transform.rotation)
                .eulerAngles;
            Vector3 calculatedScale =
                RelativePositionCalculator.InversedObjectScaleRelativeToAnchor(relativeToAnchor,
                    objectTracker.transform.localScale);

            if (Vector3.Distance(calculatedPosition, cachedObjectData.Position.Deserialize()) > AuggioUtils.Epsilon)
            {
                localChanges.Add(new PositionChange(
                    experienceId,
                    objectTracker.ObjectId,
                    objectTracker,
                    objectTracker.gameObject.name,
                    cachedObjectData.Position,
                    new SerializedVector3(calculatedPosition))
                );
            }

            if (Vector3.Distance(calculatedRotation, cachedObjectData.Rotation.Deserialize()) > AuggioUtils.Epsilon)
            {
                localChanges.Add(new RotationChange(
                    experienceId,
                    objectTracker.ObjectId,
                    objectTracker,
                    objectTracker.gameObject.name,
                    cachedObjectData.Rotation,
                    new SerializedVector3(calculatedRotation))
                );
            }

            if (Vector3.Distance(calculatedScale, cachedObjectData.Scale.Deserialize()) > AuggioUtils.Epsilon)
            {
                localChanges.Add(new ScaleChange(experienceId,
                    objectTracker.ObjectId,
                    objectTracker,
                    objectTracker.gameObject.name,
                    cachedObjectData.Scale,
                    new SerializedVector3(calculatedScale))
                );
            }

            return localChanges;
        }

        private static List<AuggionObjectChangeBase> GetPlaceholdersLocalChanges(string experienceId,
            AuggioObjectTracker objectTracker, AuggioObject cachedObjectData)
        {
            List<AuggionObjectChangeBase> localChanges = new List<AuggionObjectChangeBase>();
            if(objectTracker.Models != null) {
                foreach (AuggioObjectPlaceholderModel scenePlaceholder in objectTracker.Models)
            {
                if (cachedObjectData.PlaceholderModels.All(placeholder => placeholder.ID != scenePlaceholder.PlaceholderId))
                {
                    localChanges.Add(new PlaceholderCreated(experienceId,
                        objectTracker.ObjectId,
                        scenePlaceholder,
                        objectTracker.gameObject.name,
                        "",
                        "Created")
                    );
                    continue;
                }
                
                foreach (AuggioObjectPlaceholder dataPlaceholderModel in cachedObjectData.PlaceholderModels)
                {
                    if (!scenePlaceholder.PlaceholderId.Equals(dataPlaceholderModel.ID))
                    {
                        continue;
                    }

                    if (scenePlaceholder.ModelId != dataPlaceholderModel.ModelId)
                    {
                        localChanges.Add(new ModelIdChange(
                            experienceId,
                            objectTracker.ObjectId,
                            scenePlaceholder.PlaceholderId,
                            scenePlaceholder,
                            objectTracker.gameObject.name,
                            dataPlaceholderModel.ModelId,
                            scenePlaceholder.ModelId)
                        );
                    }

                    if (scenePlaceholder.PlaceholderName != dataPlaceholderModel.Name)
                    {
                        if (!string.IsNullOrEmpty(scenePlaceholder.PlaceholderName) ||
                            !string.IsNullOrEmpty(dataPlaceholderModel.Name))
                        {
                            localChanges.Add(new PlaceholderNameChange(
                                experienceId,
                                objectTracker.ObjectId,
                                scenePlaceholder.PlaceholderId,
                                scenePlaceholder,
                                objectTracker.gameObject.name,
                                dataPlaceholderModel.Name,
                                scenePlaceholder.PlaceholderName)
                            );
                        }
                    }

                    if (Vector3.Distance(scenePlaceholder.transform.localPosition,
                        dataPlaceholderModel.Position.Deserialize()) > AuggioUtils.Epsilon)
                    {
                        localChanges.Add(new PlaceholderPositionChange(
                                experienceId,
                                objectTracker.ObjectId,
                                scenePlaceholder.PlaceholderId,
                                scenePlaceholder,
                                objectTracker.gameObject.name,
                                dataPlaceholderModel.Position,
                                new SerializedVector3(scenePlaceholder.transform.localPosition)
                            )
                        );
                    }

                    if (Vector3.Distance(scenePlaceholder.transform.localRotation.eulerAngles,
                        dataPlaceholderModel.Rotation.Deserialize()) > AuggioUtils.Epsilon)
                    {
                        localChanges.Add(new PlaceholderRotationChange(
                                experienceId,
                                objectTracker.ObjectId,
                                scenePlaceholder.PlaceholderId,
                                scenePlaceholder,
                                objectTracker.gameObject.name,
                                dataPlaceholderModel.Position,
                                new SerializedVector3(scenePlaceholder.transform.localRotation.eulerAngles)
                            )
                        );
                    }

                    if (Vector3.Distance(scenePlaceholder.transform.localScale,
                        dataPlaceholderModel.Scale.Deserialize()) > AuggioUtils.Epsilon)
                    {
                        localChanges.Add(new PlaceholderScaleChange(
                                experienceId,
                                objectTracker.ObjectId,
                                scenePlaceholder.PlaceholderId,
                                scenePlaceholder,
                                objectTracker.gameObject.name,
                                dataPlaceholderModel.Position,
                                new SerializedVector3(scenePlaceholder.transform.localScale)
                            )
                        );
                    }
                }
            }
            }
            return localChanges;
        }

        private static List<AuggionObjectChangeBase> GetPlaceholderRemovalChanges(string experienceId,
            AuggioObjectTracker objectTracker,
            AuggioObject cachedObjectData)
        {
            List<AuggionObjectChangeBase> localChanges = new List<AuggionObjectChangeBase>();
            if (cachedObjectData.PlaceholderModels != null && objectTracker.Models != null)
            {
                foreach (AuggioObjectPlaceholder cachedPlaceholder in cachedObjectData.PlaceholderModels)
                {
                    if (!objectTracker.Models.Any(model =>
                        model.PlaceholderId != null && model.PlaceholderId.Equals(cachedPlaceholder.ID)))
                    {
                        localChanges.Add(new PlaceholderDeleted(
                            experienceId,
                            cachedObjectData.AuggioId,
                            cachedPlaceholder.ID,
                            null,
                            AuggioUtils.GetGameObjectName(cachedObjectData.Name),
                            "Present",
                            "Removed")
                        );
                    }
                }
            }

            return localChanges;
        }
    }
}
#endif