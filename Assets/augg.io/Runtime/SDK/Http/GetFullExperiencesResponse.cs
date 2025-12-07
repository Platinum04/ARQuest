using System.Collections.Generic;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;

namespace Auggio.Plugin.SDK.Http {
    public class GetFullExperiencesResponse
    {
        [SerializeField] private List<Experience> experiences;
        

        public List<Experience> Experiences {
            get => experiences;
            set => experiences = value;
        }
    }
}
