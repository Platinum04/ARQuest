using System;
using System.Collections.Generic;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;

namespace Auggio.Plugin.Editor.Http
{
    [Serializable]
    internal class PluginExperiencesResponse
    {
        [SerializeField] private List<ExperiencePreview> experiences;

        public List<ExperiencePreview> Experiences
        {
            get => experiences;
            set => experiences = value;
        }
    }
}
