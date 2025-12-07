using System;
using System.Collections.Generic;
using Auggio.Utils.Serialization.Model;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin
{
    [Serializable]
    public class AuggioObject
    {
        [SerializeField] private string auggioId;
        [SerializeField] private string name;
        [SerializeField] private SerializedVector3 position;
        [SerializeField] private SerializedVector3 rotation;
        [SerializeField] private SerializedVector3 scale;
        [SerializeField] private List<AuggioObjectPlaceholder> placeholderModels;
        [SerializeField] private string assignedAnchor;
        
        [SerializeField] private List<string> tags;

        private bool isNewObject;

        public string AuggioId
        {
            get => auggioId;
            set => auggioId = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public SerializedVector3 Position
        {
            get => position;
            set => position = value;
        }

        public SerializedVector3 Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        public SerializedVector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        public string AssignedAnchor
        {
            get => assignedAnchor;
            set => assignedAnchor = value;
        }

        public List<string> Tags
        {
            get => tags;
            set => tags = value;
        }

        public bool IsNewObject
        {
            get => isNewObject;
            set => isNewObject = value;
        }

        public List<AuggioObjectPlaceholder> PlaceholderModels
        {
            get => placeholderModels;
            set => placeholderModels = value;
        }

        #if UNITY_EDITOR
        public AuggioObjectPlaceholder FindPlaceholderById(string id)
        {
            foreach (AuggioObjectPlaceholder placeholder in placeholderModels)
            {
                if (placeholder.ID.Equals(id))
                {
                    return placeholder;
                }
            }
            return null;
        }
        #endif
    }
}
