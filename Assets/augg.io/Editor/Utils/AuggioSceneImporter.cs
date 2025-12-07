#if UNITY_EDITOR
using System.Collections.Generic;
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
    internal static class AuggioSceneImporter
    {
        internal static void ImportExperience(Scene scene, Experience experience, Material meshMaterial,
            bool visualizeMeshOnImport)
        {
            GameObject experienceGameObject = new GameObject(AuggioUtils.GetExperienceGameObjectName(experience.Name));
            AuggioExperience e = experienceGameObject.AddComponent<AuggioExperience>();
            e.ExperienceId = experience.ID;
            e.OrganizationId = experience.OrganizationId;
            e.ExperienceName = experience.Name;

            ImportVisualizationHierarchy(experienceGameObject.transform, experience, meshMaterial,
                visualizeMeshOnImport);
            ImportAuggioObjects(scene, experience, experienceGameObject.transform);
            EditorUtility.SetDirty(e);
            EditorSceneManager.MarkSceneDirty(scene);
        }

        internal static VisualizationHierarchy ImportVisualizationHierarchy(Transform parentTransform,
            Experience experience, Material meshMaterial, bool visualizeMeshOnImport)
        {
            GameObject visualizationHierarchy = new GameObject("VisualizationHierarchy")
            {
                transform = {parent = parentTransform}
            };
            VisualizationHierarchy vhScript = visualizationHierarchy.AddComponent<VisualizationHierarchy>();
            vhScript.ExperienceId = experience.ID;
            vhScript.Material = meshMaterial;
            vhScript.VisualizeMesh = visualizeMeshOnImport;
            vhScript.tag = "EditorOnly";

            foreach (Location location in experience.AssignedLocations)
            {
                ImportLocation(experience, location, visualizationHierarchy.transform, meshMaterial,
                    visualizeMeshOnImport);
            }
            SceneVisibilityManager.instance.DisablePicking(vhScript.gameObject, true);

            return visualizationHierarchy.GetComponent<VisualizationHierarchy>();
        }

        internal static AuggioLocation ImportLocation(Experience exp, Location location, Transform parent,
            Material meshMaterial, bool visualizeMeshOnImport)
        {
            GameObject locationGO = new GameObject(AuggioUtils.GetLocationGameObjectName(location.Name))
            {
                transform = {parent = parent}
            };
            AuggioLocation locationScript = locationGO.AddComponent<AuggioLocation>();
            locationScript.OrganizationId = exp.OrganizationId;
            locationScript.ExperienceId = exp.ID;
            locationScript.LocationId = location.ID;
            locationScript.tag = "EditorOnly";

            foreach (SingleAnchor anchor in location.SingleAnchorList)
            {
                ImportAnchor(anchor, exp, location, locationGO.transform, meshMaterial);
            }

            if (visualizeMeshOnImport)
            {
                AuggioLocationEditor.VisualizeLocationMeshes(locationScript);
            }

            return locationScript;
        }

        internal static AuggioAnchor ImportAnchor(SingleAnchor anchor, Experience exp, Location location,
            Transform parent, Material meshMaterial)
        {
            GameObject anchorGO = new GameObject(AuggioUtils.GetAnchorGameObjectName(anchor.Name))
            {
                transform =
                {
                    parent = parent,
                    localPosition = anchor.RelativeOriginPosition.Deserialize(),
                    localRotation = Quaternion.Euler(anchor.RelativeOriginRotation.Deserialize())
                }
            };
            AuggioAnchor anchorScript = anchorGO.AddComponent<AuggioAnchor>();
            anchorScript.OrganizationId = exp.OrganizationId;
            anchorScript.ExperienceId = exp.ID;
            anchorScript.LocationId = location.ID;
            anchorScript.MeshMaterial = meshMaterial;
            anchorScript.AnchorId = anchor.AuggioId;
            anchorScript.MeshHash = anchor.MeshHash;
            anchorScript.tag = "EditorOnly";

            return anchorScript;
        }

        private static void ImportAuggioObjects(Scene scene, Experience exp, Transform parent)
        {
            foreach (AuggioObject auggioObject in exp.Objects)
            {
                ImportAuggioObject(scene, auggioObject, exp, parent);
            }
        }

        internal static AuggioObjectTracker ImportAuggioObject(Scene scene, AuggioObject auggioObject, Experience exp,
            Transform parent)
        {
            GameObject auggioObjectGO = new GameObject(AuggioUtils.GetGameObjectName(auggioObject.Name))
            {
                transform = {parent = parent}
            };
            AuggioObjectTracker auggioObjectTracker = auggioObjectGO.AddComponent<AuggioObjectTracker>();

            //Bind data
            auggioObjectTracker.ExperienceId = exp.ID;
            auggioObjectTracker.OrganizationId = exp.OrganizationId;
            auggioObjectTracker.AnchorId = auggioObject.AssignedAnchor;
            auggioObjectTracker.ObjectId = auggioObject.AuggioId;
            auggioObjectTracker.gameObject.name = AuggioUtils.GetGameObjectName(auggioObject.Name);
            auggioObjectTracker.ObjectName = auggioObject.Name;

            if (string.IsNullOrEmpty(auggioObjectTracker.AnchorId))
            {
                //Object without assigned anchor
                //TODO
                parent.localPosition = Vector3.zero;
                parent.localRotation = Quaternion.identity;
                parent.localScale = Vector3.one;
                return auggioObjectTracker;
            }

            //Find anchor transform
            Transform relativeToAnchor =
                AuggioSceneManager.GetAnchorTransformByAnchorId(scene, auggioObject.AssignedAnchor);

            //position/rotate/scale object relative to anchor transform
            auggioObjectGO.transform.position =
                RelativePositionCalculator.ObjectPositionRelativeToAnchor(relativeToAnchor, auggioObject);
            auggioObjectGO.transform.rotation =
                RelativePositionCalculator.ObjectRotationRelativeToAnchor(relativeToAnchor, auggioObject);
            auggioObjectGO.transform.localScale =
                RelativePositionCalculator.ObjectScaleRelativeToAnchor(relativeToAnchor, auggioObject);

            foreach (AuggioObjectPlaceholder objectPlaceholder in auggioObject.PlaceholderModels)
            {
                ImportAuggioObjectPlaceholder(scene, exp.ID, auggioObjectTracker, objectPlaceholder,
                    auggioObjectTracker.transform);
            }

            return auggioObjectTracker;
        }

        internal static AuggioObjectPlaceholderModel ImportAuggioObjectPlaceholder(Scene scene, string experienceId, 
            AuggioObjectTracker auggioObjectTracker, AuggioObjectPlaceholder objectPlaceholder, Transform parent)
        {
            GameObject placeholderGO = new GameObject(AuggioUtils.GetGameObjectPlaceholderName(objectPlaceholder.Name))
            {
                transform =
                {
                    parent = parent,
                    localPosition = objectPlaceholder.Position.Deserialize(),
                    localRotation = Quaternion.Euler(objectPlaceholder.Rotation.Deserialize()),
                    localScale = objectPlaceholder.Scale.Deserialize()
                }
            };

            AuggioObjectPlaceholderModel auggioObjectPlaceholderModel =
                placeholderGO.AddComponent<AuggioObjectPlaceholderModel>();
            auggioObjectPlaceholderModel.ExperienceId = experienceId;
            auggioObjectPlaceholderModel.PlaceholderId = objectPlaceholder.ID;
            auggioObjectPlaceholderModel.ModelId = objectPlaceholder.ModelId;
            auggioObjectPlaceholderModel.PlaceholderName = objectPlaceholder.Name;
            auggioObjectPlaceholderModel.gameObject.name = AuggioUtils.GetGameObjectPlaceholderName(objectPlaceholder.Name);

            if (auggioObjectTracker.Models == null)
            {
                auggioObjectTracker.Models = new List<AuggioObjectPlaceholderModel>();
            }

            auggioObjectTracker.Models.Add(auggioObjectPlaceholderModel);
            auggioObjectPlaceholderModel.AuggioObjectTracker = auggioObjectTracker;
            return auggioObjectPlaceholderModel;
        }
    }
}
#endif