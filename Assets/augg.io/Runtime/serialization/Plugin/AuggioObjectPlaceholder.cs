using System;
using System.Collections;
using System.Collections.Generic;
using Auggio.Utils.Serialization.Model;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin
{
    [Serializable]
    public class AuggioObjectPlaceholder
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private SerializedVector3 position;
        [SerializeField] private SerializedVector3 rotation;
        [SerializeField] private SerializedVector3 scale;
        [SerializeField] private string modelId;

        private bool isNewPlaceholder;

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

        public string ModelId
        {
            get => modelId;
            set => modelId = value;
        }

        public bool IsNewPlaceholder
        {
            get => isNewPlaceholder;
            set => isNewPlaceholder = value;
        }
    }
}
