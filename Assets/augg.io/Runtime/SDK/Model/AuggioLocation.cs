using UnityEngine;

namespace Auggio.Plugin.SDK.Model
{
    public class AuggioLocation : MonoBehaviour
    {
        [HideInInspector][SerializeField] private string organizationId;
        [HideInInspector][SerializeField] private string experienceId;
        [HideInInspector][SerializeField] private string locationId;
        
        [HideInInspector] 
        [SerializeField] private bool visualizeMesh;
    
        public string ExperienceId
        {
            get => experienceId;
            set => experienceId = value;
        }

        public string LocationId
        {
            get => locationId;
            set => locationId = value;
        }

        public string OrganizationId
        {
            get => organizationId;
            set => organizationId = value;
        }

        public bool VisualizeMesh
        {
            get => visualizeMesh;
            set => visualizeMesh = value;
        }
    }
}
