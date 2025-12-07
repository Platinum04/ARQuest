#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Auggio.Plugin.Editor.Model;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;

namespace Auggio.Plugin.Editor.Http
{
    [Serializable]
    internal class UploadExperienceChangesResponse
    {
        [SerializeField] private Experience experience;
        [SerializeField] private List<ObjectIdMapping> idMapping;
        [SerializeField] private List<ObjectIdMapping> placeholderIdMapping;

        public Experience Experience
        {
            get => experience;
            set => experience = value;
        }

        public List<ObjectIdMapping> IDMapping
        {
            get => idMapping;
            set => idMapping = value;
        }

        public List<ObjectIdMapping> PlaceholderIdMapping
        {
            get => placeholderIdMapping;
            set => placeholderIdMapping = value;
        }
    }
}
#endif