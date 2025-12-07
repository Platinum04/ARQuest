using System;
using UnityEngine;

namespace Auggio.Runtime.serialization.Plugin.Experience
{
    
    [Serializable]
    public class TagPreview
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        
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
    }
}
