using System;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin.Experience
{
    [Serializable]
    public class ExperiencePreview 
    {
        [SerializeField] private String id;
        [SerializeField] private string name;

        private bool presentInScene;
        
        public string ID
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public bool PresentInScene
        {
            get => presentInScene;
            set => presentInScene = value;
        }
    }
}
