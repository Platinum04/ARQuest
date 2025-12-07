#if UNITY_EDITOR
using System;
using System.Linq;
using Auggio.Plugin.Editor.SDK;
using Auggio.Plugin.SDK.Model;
using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization.Plugin;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auggio.Plugin.Editor.Utils
{
    internal static class AuggioSceneUpdater
    {
        internal static void UpdateExistingExperience(Scene scene, Experience currentExperience,
            Experience newExperience, Material meshMaterial, bool visualizeMeshOnImport)
        {
            AuggioExperience experienceTransform =
                AuggioSceneManager.GetExperienceFromSceneById(scene, newExperience.ID);

            experienceTransform.gameObject.name = AuggioUtils.GetExperienceGameObjectName(newExperience.Name);

            VisualizationHierarchy visualizationHierarchy =
                experienceTransform.GetComponentInChildren<VisualizationHierarchy>(true);

            if (visualizationHierarchy == null)
            {
                visualizationHierarchy = AuggioSceneImporter.ImportVisualizationHierarchy(experienceTransform.transform,
                    newExperience,
                    meshMaterial, visualizeMeshOnImport);
            }
            else
            {
                UpdateVisualizationHierarchy(visualizationHierarchy, scene, currentExperience, newExperience,
                    meshMaterial, visualizeMeshOnImport);
            }

            UpdateAuggioObjects(experienceTransform, scene, currentExperience, newExperience);
            DeleteMissingAuggioObjects(scene, currentExperience, newExperience);
            EditorUtility.SetDirty(experienceTransform);
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("Experience " + newExperience.Name + " in scene was sucessfully updated");
        }

        private static void UpdateAuggioObjects(AuggioExperience auggioExperience, Scene scene,
            Experience currentExperience, Experience newExperience)
        {
            //update objects
            foreach (AuggioObject newAuggioObject in newExperience.Objects)
            {
                if (!currentExperience.Objects.Any(obj => obj.AuggioId.Equals(newAuggioObject.AuggioId)))
                {
                    //object does not exist - create new
                    AuggioSceneImporter.ImportAuggioObject(scene, newAuggioObject, newExperience,
                        auggioExperience.transform);
                    continue;
                }

                AuggioObject currentObject = currentExperience.FindObjectByObjectId(newAuggioObject.AuggioId);

                AuggioObjectTracker tracker =
                    AuggioSceneManager.GetAuggioObjectTrackerFromScene(scene, newExperience.ID,
                        newAuggioObject.AuggioId);
                if (tracker == null)
                {
                    AuggioSceneImporter.ImportAuggioObject(scene, newAuggioObject, newExperience,
                        auggioExperience.transform);
                    //Debug.LogError("Missing tracker for auggio object with id: " + newAuggioObject.AuggioId);
                    continue;
                }

                //update object values
                tracker.gameObject.name = AuggioUtils.GetGameObjectName(newAuggioObject.Name);
                tracker.ObjectName = newAuggioObject.Name;
                tracker.AnchorId = newAuggioObject.AssignedAnchor;

                UpdateAuggioObjectPlaceholders(newExperience.ID, tracker, scene, newAuggioObject);
                DeleteMissingAuggioObjectPlaceholders(tracker, currentObject, newAuggioObject);

                if (string.IsNullOrEmpty(newAuggioObject.AssignedAnchor))
                {
                    //Object without anchor
                    continue;
                }

                Transform relativeTo =
                    AuggioSceneManager.GetAnchorTransformByAnchorId(scene, newAuggioObject.AssignedAnchor);
                tracker.transform.position =
                    RelativePositionCalculator.ObjectPositionRelativeToAnchor(relativeTo, newAuggioObject);
                tracker.transform.rotation =
                    RelativePositionCalculator.ObjectRotationRelativeToAnchor(relativeTo, newAuggioObject);
                tracker.transform.localScale =
                    RelativePositionCalculator.ObjectScaleRelativeToAnchor(relativeTo, newAuggioObject);
            }
        }

        private static void UpdateAuggioObjectPlaceholders(string experienceId, AuggioObjectTracker tracker, Scene scene,
            AuggioObject newObject)
        {
            foreach (AuggioObjectPlaceholder objectPlaceholder in newObject.PlaceholderModels)
            {
                AuggioObjectPlaceholderModel objectPlaceholderModel =
                    AuggioSceneManager.GetAuggioPlaceholderModelFromScene(scene, experienceId, objectPlaceholder.ID);
                if (objectPlaceholderModel == null)
                {
                    AuggioSceneImporter.ImportAuggioObjectPlaceholder(scene, experienceId, tracker, objectPlaceholder,
                        tracker.transform);
                    continue;
                }

                objectPlaceholderModel.ModelId = objectPlaceholder.ModelId;
                objectPlaceholderModel.PlaceholderName = objectPlaceholder.Name;
                objectPlaceholderModel.name = AuggioUtils.GetGameObjectPlaceholderName(objectPlaceholder.Name);
                objectPlaceholderModel.transform.localPosition = objectPlaceholder.Position.Deserialize();
                objectPlaceholderModel.transform.localRotation =
                    Quaternion.Euler(objectPlaceholder.Rotation.Deserialize());
                objectPlaceholderModel.transform.localScale = objectPlaceholder.Scale.Deserialize();
            }
        }

        private static void DeleteMissingAuggioObjectPlaceholders(AuggioObjectTracker tracker,
            AuggioObject currentObject, AuggioObject newObject)
        {
            foreach (AuggioObjectPlaceholder objectPlaceholder in currentObject.PlaceholderModels)
            {
                if (newObject.PlaceholderModels.Any(placeholder => placeholder.ID.Equals(objectPlaceholder.ID)))
                {
                    continue;
                }

                //object does not exist in new experience - got deleted
                AuggioObjectPlaceholderModel model = AuggioObjectTrackerEditor.FindPlaceholderById(tracker, objectPlaceholder.ID);
                tracker.Models.Remove(model);
                model.AuggioObjectTracker = null;
                GameObject.DestroyImmediate(model.gameObject);
            }
        }

        private static void DeleteMissingAuggioObjects(Scene scene, Experience currentExperience,
            Experience newExperience)
        {
            foreach (AuggioObject auggioObject in currentExperience.Objects)
            {
                if (newExperience.Objects.Any(obj => obj.AuggioId.Equals(auggioObject.AuggioId)))
                {
                    continue;
                }
                //object does not exist in new experience - got deleted

                AuggioObjectTracker tracker =
                    AuggioSceneManager.GetAuggioObjectTrackerFromScene(scene, currentExperience.ID,
                        auggioObject.AuggioId);
                GameObject.DestroyImmediate(tracker.gameObject);
            }
        }

        private static void UpdateVisualizationHierarchy(VisualizationHierarchy visualizationHierarchy, Scene scene,
            Experience currentExperience, Experience newExperience, Material meshMaterial, bool visualizeMeshOnImport)
        {
            ClearInvalidLocations(visualizationHierarchy);
            UpdateLocations(visualizationHierarchy, scene, currentExperience, newExperience, meshMaterial,
                visualizeMeshOnImport);
            DeleteMissingLocations(scene, currentExperience, newExperience);
        }

        private static void ClearInvalidLocations(VisualizationHierarchy visualizationHierarchy)
        {
            foreach (AuggioLocation location in visualizationHierarchy.GetComponentsInChildren<AuggioLocation>(true))
            {
                if (!AuggioLocationEditor.IsValid(location))
                {
                    GameObject.DestroyImmediate(location.gameObject);
                }
            }
        }

        private static void UpdateLocations(VisualizationHierarchy visualizationHierarchy, Scene scene,
            Experience currentExperience, Experience newExperience, Material meshMaterial, bool visualizeMeshOnImport)
        {
            foreach (Location newLocation in newExperience.AssignedLocations)
            {
                Location currentLocation = null;
                try
                {
                    currentLocation = currentExperience.FindLocationById(newLocation.ID);
                }
                catch (ArgumentException)
                {
                    //location does not exist yet - create one
                    AuggioSceneImporter.ImportLocation(newExperience, newLocation, visualizationHierarchy.transform,
                        meshMaterial,
                        visualizeMeshOnImport);
                    continue;
                }

                UpdateLocation(visualizationHierarchy, scene, currentLocation, newLocation, currentExperience,
                    newExperience, meshMaterial, visualizeMeshOnImport);
            }
        }

        private static void UpdateLocation(VisualizationHierarchy visualizationHierarchy, Scene scene,
            Location currentLocation, Location newLocation, Experience currentExperience, Experience newExperience,
            Material meshMaterial, bool visualizeMeshOnImport)
        {
            AuggioLocation auggioLocation =
                AuggioSceneManager.GetAuggioLocationFromScene(scene, newExperience.ID, newLocation.ID);
            if (auggioLocation == null)
            {
                auggioLocation = AuggioSceneImporter.ImportLocation(newExperience, newLocation,
                    visualizationHierarchy.transform,
                    meshMaterial, visualizeMeshOnImport);
            }

            auggioLocation.gameObject.name = AuggioUtils.GetLocationGameObjectName(newLocation.Name);

            ClearInvalidLocationAnchors(auggioLocation);
            UpdateLocationAnchors(scene, auggioLocation.transform, currentLocation, newLocation, currentExperience,
                newExperience, meshMaterial);
            DeleteMissingAnchors(scene, currentLocation, newLocation);
        }

        private static void ClearInvalidLocationAnchors(AuggioLocation auggioLocation)
        {
            foreach (AuggioAnchor anchor in auggioLocation.GetComponentsInChildren<AuggioAnchor>(true))
            {
                if (!AuggioAnchorEditor.IsValid(anchor))
                {
                    GameObject.DestroyImmediate(anchor.gameObject);
                }
            }
        }

        private static void DeleteMissingLocations(Scene scene, Experience currentExperience, Experience newExperience)
        {
            foreach (Location currentLocation in currentExperience.AssignedLocations)
            {
                if (newExperience.AssignedLocations.Any(loc => loc.ID.Equals(currentLocation.ID)))
                {
                    continue;
                }

                //location does not exists in new experience -> remove it
                AuggioLocation auggioLocation =
                    AuggioSceneManager.GetAuggioLocationFromScene(scene, currentExperience.ID, currentLocation.ID);
                GameObject.DestroyImmediate(auggioLocation.gameObject);
            }
        }

        private static void UpdateLocationAnchors(Scene scene, Transform locationTransform, Location currentLocation,
            Location newLocation, Experience currentExperience, Experience newExperience, Material meshMaterial)
        {
            //anchors
            foreach (SingleAnchor newAnchor in newLocation.SingleAnchorList)
            {
                if (!currentLocation.SingleAnchorList.Any(singleAnchor =>
                    singleAnchor.AuggioId.Equals(newAnchor.AuggioId)))
                {
                    //anchor does not exists -> create
                    AuggioSceneImporter.ImportAnchor(newAnchor, newExperience, currentLocation, locationTransform,
                        meshMaterial);
                    continue;
                }

                UpdateLocationAnchor(scene, newAnchor, currentLocation, locationTransform, newExperience, meshMaterial);
            }
        }

        private static void UpdateLocationAnchor(Scene scene, SingleAnchor newAnchor, Location location,
            Transform locationTransform, Experience experience, Material meshMaterial)
        {
            AuggioAnchor auggioAnchor = null;
            try
            {
                auggioAnchor = AuggioSceneManager.GetAnchorTransformByAnchorId(scene, newAnchor.AuggioId)
                    .GetComponent<AuggioAnchor>();
            }
            catch (ArgumentException)
            {
                auggioAnchor =
                    AuggioSceneImporter.ImportAnchor(newAnchor, experience, location, locationTransform, meshMaterial);
            }

            auggioAnchor.MeshHash = newAnchor.MeshHash;
            auggioAnchor.MeshMaterial = meshMaterial;
            auggioAnchor.gameObject.name = AuggioUtils.GetAnchorGameObjectName(newAnchor.Name);
            auggioAnchor.transform.localPosition = newAnchor.RelativeOriginPosition.Deserialize();
            auggioAnchor.transform.localRotation = Quaternion.Euler(newAnchor.RelativeOriginRotation.Deserialize());
        }

        private static void DeleteMissingAnchors(Scene scene, Location currentLocation, Location newLocation)
        {
            foreach (SingleAnchor currentAnchor in currentLocation.SingleAnchorList)
            {
                if (newLocation.SingleAnchorList.Any(anchor => anchor.AuggioId.Equals(currentAnchor.AuggioId)))
                {
                    continue;
                }

                //anchor does not exists in new experience - delete
                Transform auggioAnchor = AuggioSceneManager.GetAnchorTransformByAnchorId(scene, currentAnchor.AuggioId);
                GameObject.DestroyImmediate(auggioAnchor.gameObject);
            }
        }
    }
}
#endif