using System;
using System.Collections.Generic;
using Auggio.Utils.Serialization.Plugin.Experience;
using UnityEngine;

namespace Auggio.Plugin.Editor.Http
{
    [Serializable]
    internal class EditExperienceRequest
    {
        [SerializeField] private Experience experience;
        [SerializeField] private List<string> newObjects;
        [SerializeField] private List<string> newPlaceholders;

        public EditExperienceRequest()
        {
        }

        public EditExperienceRequest(Experience experience, List<string> newObjects, List<string> newPlaceholders)
        {
            this.experience = experience;
            this.newObjects = newObjects;
            this.newPlaceholders = newPlaceholders;
        }

        public Experience Experience
        {
            get => experience;
            set => experience = value;
        }

        public List<string> NewObjects
        {
            get => newObjects;
            set => newObjects = value;
        }

        public List<string> NewPlaceholders
        {
            get => newPlaceholders;
            set => newPlaceholders = value;
        }
    }
}
