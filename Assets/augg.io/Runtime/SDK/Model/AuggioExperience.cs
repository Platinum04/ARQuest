using UnityEngine;

namespace Auggio.Plugin.SDK.Model
{
    public class AuggioExperience : MonoBehaviour
    {
        [HideInInspector][SerializeField] private string organizationId;
        [HideInInspector][SerializeField] private string experienceId;
        [HideInInspector] [SerializeField] private string experienceName;

        public string OrganizationId
        {
            get => organizationId;
            set => organizationId = value;
        }

        public string ExperienceId
        {
            get => experienceId;
            set => experienceId = value;
        }

        public string ExperienceName
        {
            get => experienceName;
            set => experienceName = value;
        }
    }
}
