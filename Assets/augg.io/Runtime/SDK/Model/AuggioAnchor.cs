using Auggio.Plugin.SDK.Utils;
using Auggio.Utils.Serialization;
using Auggio.Utils.Serialization.Model;
using UnityEngine;
using System.IO;

namespace Auggio.Plugin.SDK.Model
{
    public class AuggioAnchor : MonoBehaviour
    {
        [HideInInspector][SerializeField] private string organizationId;
        [HideInInspector][SerializeField] private string experienceId;
        [HideInInspector][SerializeField] private string locationId;
        [HideInInspector][SerializeField] private string anchorId;
        [HideInInspector][SerializeField] private string meshHash;

        [Header("Visualization settings")]
        [HideInInspector][SerializeField] private Material meshMaterial;
        [HideInInspector][SerializeField] private bool visualizeMesh;

        
        public string OrganizationId
        { 
            set => organizationId = value;
            get => organizationId;
        }

        public string ExperienceId
        {
            set => experienceId = value;
            get => experienceId;
        }

        public string LocationId
        {
            set => locationId = value;
            get => locationId;
        }

        public string AnchorId
        {
            get => anchorId;
            set => anchorId = value;
        }

        public string MeshHash
        {
            set => meshHash = value;
            get => meshHash;
        }

        public bool VisualizeMesh
        {
            set => visualizeMesh = value;
            get => visualizeMesh;
        }

        public Material MeshMaterial
        {
            set => meshMaterial = value;
            get => meshMaterial;
        }

    }
}
