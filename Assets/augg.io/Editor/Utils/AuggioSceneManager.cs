#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Auggio.Plugin.Editor.SDK;
using Auggio.Plugin.Editor.Validation;
using Auggio.Plugin.Editor.Validation.Validators;
using Auggio.Plugin.SDK.Model;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Utils
{
    
    //TODO split into importer, updater and local changes detector?
    internal static class AuggioSceneManager
    {

        internal static void ProcessExperience(Scene scene, Experience experience, Material meshMaterial, bool visualizeMeshOnImport)
        {
            ExperienceFileCache experienceFileCache = new ExperienceFileCache(experience.OrganizationId, experience.ID);
            AuggioExperience existingExperience = GetExperienceFromSceneById(scene, experience.ID);
            
            if (existingExperience == null)
            {
                experienceFileCache.Save(experience);
                AuggioSceneImporter.ImportExperience(scene, experience, meshMaterial, visualizeMeshOnImport);
            }
            else
            {
                Experience oldExperience = experienceFileCache.Load();
                AuggioSceneUpdater.UpdateExistingExperience(scene, oldExperience, experience, meshMaterial, visualizeMeshOnImport);
                experienceFileCache.Save(experience);
            }
        }
        
        #region Getters
        
        internal static AuggioExperience GetExperienceFromSceneById(Scene activeScene, string experienceId)
        {
            foreach (GameObject rootGameObject in activeScene.GetRootGameObjects())
            {
                AuggioExperience[] sceneExperiences =
                    rootGameObject.GetComponentsInChildren<AuggioExperience>(true);
                foreach (AuggioExperience sceneExperience in sceneExperiences)
                {
                    if (sceneExperience.ExperienceId.Equals(experienceId))
                    {
                        return sceneExperience;
                    }
                }
            }
            return null;
        }
        
        internal static List<AuggioExperience> GetAllExperiencesInScene(Scene activeScene)
        {
            List<AuggioExperience> experiencesInScene = new List<AuggioExperience>();
            foreach (GameObject rootGameObject in activeScene.GetRootGameObjects())
            {
                AuggioExperience[] sceneExperiences =
                    rootGameObject.GetComponentsInChildren<AuggioExperience>(true);
                experiencesInScene.AddRange(sceneExperiences);
            }
            return experiencesInScene;
        }
        
        internal static List<AuggioObjectTracker> GetAuggioObjectTrackersByExperienceId(Scene scene, string experienceId)
        {
            List<AuggioObjectTracker> result = new List<AuggioObjectTracker>();
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                AuggioObjectTracker[] objectTrackers = rootGameObject.GetComponentsInChildren<AuggioObjectTracker>(true);
                foreach (AuggioObjectTracker objectTracker in objectTrackers)
                {
                    if (objectTracker.ExperienceId.Equals(experienceId))
                    {
                        result.Add(objectTracker);
                    }
                }
            }
            return result;
        }
        
        internal static List<AuggioObjectPlaceholderModel> GetAuggioObjectPlaceholderModelsByExperienceId(Scene scene, string experienceId)
        {
            List<AuggioObjectPlaceholderModel> result = new List<AuggioObjectPlaceholderModel>();
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                AuggioObjectPlaceholderModel[] placeholderModels = rootGameObject.GetComponentsInChildren<AuggioObjectPlaceholderModel>(true);
                foreach (AuggioObjectPlaceholderModel placeholderModel in placeholderModels)
                {
                    if (!string.IsNullOrEmpty(placeholderModel.ExperienceId) && placeholderModel.ExperienceId.Equals(experienceId))
                    {
                        result.Add(placeholderModel);
                    }
                }
            }
            return result;
        }
        
        internal static List<AuggioObjectPlaceholderModel> GetUnassignedAuggioObjectPlaceholderModels(Scene scene)
        {
            List<AuggioObjectPlaceholderModel> result = new List<AuggioObjectPlaceholderModel>();
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                AuggioObjectPlaceholderModel[] placeholderModels = rootGameObject.GetComponentsInChildren<AuggioObjectPlaceholderModel>(true);
                foreach (AuggioObjectPlaceholderModel placeholderModel in placeholderModels)
                {
                    if (string.IsNullOrEmpty(placeholderModel.ExperienceId))
                    {
                        result.Add(placeholderModel);
                    }
                }
            }
            return result;
        }
        
        internal static Transform GetAnchorTransformByAnchorId(Scene scene, string anchorId)
        {
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                foreach (AuggioAnchor auggioAnchor in rootGameObject.GetComponentsInChildren<AuggioAnchor>(true))
                {
                    if (auggioAnchor.AnchorId.Equals(anchorId))
                    {
                        return auggioAnchor.transform;
                    }
                }
            }
            throw new ArgumentException("Cannot find augg.io Anchor with id " + anchorId +
                                        " in visualization hierarchy");
        }
        
        internal static AuggioObjectTracker GetAuggioObjectTrackerFromScene(Scene scene, string experienceId,
            string objectId)
        {
            List<AuggioObjectTracker> trackers = GetAuggioObjectTrackersByExperienceId(scene, experienceId);
            foreach (AuggioObjectTracker tracker in trackers)
            {
                if (tracker.ObjectId.Equals(objectId))
                {
                    return tracker;
                }
            }
            return null;
        }
        
        internal static AuggioLocation GetAuggioLocationFromScene(Scene scene, string experienceId,
            string locationId)
        {
            AuggioExperience auggioExperience = GetExperienceFromSceneById(scene, experienceId);
            AuggioLocation[] auggioLocations = auggioExperience.GetComponentsInChildren<AuggioLocation>(true);

            foreach (AuggioLocation auggioLocation in auggioLocations)
            {
                if (auggioLocation.LocationId.Equals(locationId))
                {
                    return auggioLocation;
                }
            }

            return null;
        }
        
        internal static AuggioObjectPlaceholderModel GetAuggioPlaceholderModelFromScene(Scene scene, string experienceId, string placeholderId)
        {
            List<AuggioObjectPlaceholderModel> models = GetAuggioObjectPlaceholderModelsByExperienceId(scene, experienceId);
            foreach (AuggioObjectPlaceholderModel model in models)
            {
                if (model.PlaceholderId == null)
                {
                    continue;
                }
                if (model.PlaceholderId.Equals(placeholderId))
                {
                    return model;
                }
            }
            return null;
        }
        
        internal static List<GameObject> GetGameObjectsWithErrors(Experience experience)
        {
            List<AuggioObjectTracker> allObjectTrackers = AuggioSceneManager.GetAuggioObjectTrackersByExperienceId(SceneManager.GetActiveScene(), experience.ID);
            List<AuggioObjectPlaceholderModel> allPlaceholderModels = AuggioSceneManager.GetAuggioObjectPlaceholderModelsByExperienceId(SceneManager.GetActiveScene(), experience.ID);
            List<AuggioObjectPlaceholderModel> uassignedPlaceholderModels =
                GetUnassignedAuggioObjectPlaceholderModels(SceneManager.GetActiveScene());
            List<GameObject> gameObjectsWithErrors = new List<GameObject>();

            AuggioObjectTrackerValidator objectTrackerValidator = new AuggioObjectTrackerValidator();
            foreach (AuggioObjectTracker auggioObjectTracker in allObjectTrackers)
            {
                var errors = objectTrackerValidator.Validate(auggioObjectTracker);
                if (errors.Count > 0)
                {
                    gameObjectsWithErrors.Add(auggioObjectTracker.gameObject);
                }
            }

            AuggioObjectPlaceholderValidator placeholderValidator = new AuggioObjectPlaceholderValidator();
            foreach (AuggioObjectPlaceholderModel placeholder in allPlaceholderModels)
            {
                var errors = placeholderValidator.Validate(placeholder);
                if (errors.Count > 0)
                {
                    gameObjectsWithErrors.Add(placeholder.gameObject);
                }
            }
            
            foreach (AuggioObjectPlaceholderModel uassignedPlaceholder in uassignedPlaceholderModels)
            {
                var errors = placeholderValidator.Validate(uassignedPlaceholder);
                if (errors.Count > 0)
                {
                    gameObjectsWithErrors.Add(uassignedPlaceholder.gameObject);
                }
            }

            return gameObjectsWithErrors;
        }

        #endregion

     

       

       

       
    
      
    }
}
#endif
