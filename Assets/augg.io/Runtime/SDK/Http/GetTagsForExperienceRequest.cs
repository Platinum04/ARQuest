using UnityEngine;

namespace Auggio.Runtime.SDK.Http
{
    public class GetTagsForExperienceRequest
    {
        
        [SerializeField] private string experienceId;


        public GetTagsForExperienceRequest(string experienceId)
        {
            this.experienceId = experienceId;
        }

        public string ExperienceId
        {
            get => experienceId;
            set => experienceId = value;
        }
    }
}
