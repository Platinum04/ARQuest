using System;
using System.Collections.Generic;
using UnityEngine;

namespace Auggio.Plugin.SDK.Http {
    
    [Serializable]
    public class GetFullExperiencesByIdRequest {
        
        [SerializeField] private List<string> experienceIds;

        public GetFullExperiencesByIdRequest(List<string> experienceIds) {
            this.experienceIds = experienceIds;
        }

        public List<string> ExperienceIds {
            get => experienceIds;
            set => experienceIds = value;
        }
    }
}
